using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;
using MyTcpClient;

/*
NetManager基于异步Socket实现。异步socket回调函数把收到的消息按顺序存入消息队列msgList中，
Update方法依次读取消息，再根据监听表和协议名，调用相应的处理方法

网络模块分为两个部分，
第一部分是框架部分framework。

framework包含网络管理器NetManager、为提高运行效率使用的ByteArray缓冲区​、以及协议基类MsgBase。

第二部分是协议类​。
它定义了客户端和服务端通信的数据格式，例如移动协议MsgMove、攻击协议MsgAttack会定义在BattleMsg中。

*/
namespace MyTcpClient
{
    public static class NetManager
    {
        #region 私有、公有变量 状态初始化函数
        static Socket socket = null;
        static Queue<ByteArray> writeQueue;

        static ByteArray readBuff;
        static List<MsgBase> msgList = new List<MsgBase>();//消息列表
        static int msgCount = 0;//消息列表长度
        readonly static int MAX_MESSAGE_FIRE = 10;//每一次Update处理的消息个数

        /// <summary>
        /// 是否启用心跳检测机制
        /// </summary>
        public static bool isUsePing = true;
        public static float pingInterval = 30;
        static float lastPingTime = 0;//上一次发送ping的时间
        static float lastPongTime = 0;//上一次收到ping的时间

        private static void InitState()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            readBuff = new ByteArray();
            writeQueue = new Queue<ByteArray>();
            isConnecting = false;
            isClosing = false;
            isClosed = false;

            msgList = new List<MsgBase>();
            msgCount = 0;

            lastPingTime = Time.time;
            lastPongTime = Time.time;

            //监听Pong协议
            if(msgListeners.ContainsKey("MsgPong") == false)
            {
                AddMsgListener("MsgPong", OnMsgPong);
            }
        }
        #endregion

        #region 事件处理
        public enum NetEvent
        {
            ConnectSucc = 1,
            ConnectFail = 2,
            Close = 3
        }

        public delegate void EventListener(string info);
        private static Dictionary<NetEvent, EventListener> eventListeners
            = new Dictionary<NetEvent, EventListener>();
        public static void AddEventListener(NetEvent _event, EventListener listener) 
        {
            if(eventListeners.ContainsKey(_event))
            {
                eventListeners[_event] += listener;
            }
            else
            {
                eventListeners[_event] = listener;
            }
        }

        public static void RemoveEventListener(NetEvent _event, EventListener listener) 
        {
            if (eventListeners.ContainsKey(_event))
            {
                eventListeners[_event] -= listener;

                if (eventListeners[_event] == null)
                {
                    eventListeners.Remove(_event);
                }
            }
        }

        private static void FireEvent(NetEvent _event, string info) 
        {
            if(eventListeners.ContainsKey(_event))
            {
                eventListeners[_event](info);
            }
        }
        #endregion

        #region 连接和断开
        static bool isConnecting = false;
        static bool isClosing = false;
        static bool isClosed = false;
        public static void Connect(string ip, int port)
        {
            if(socket != null && socket.Connected)
            {
                Debug.Log("Connect fail, already connected!");
                return;
            }

            if(isConnecting)
            {
                Debug.Log("Connect fail, is connecting !");
                return;
            }

            InitState();

            socket.NoDelay = true;
            isConnecting = true;
            socket.BeginConnect(ip, port, ConnectCallback, socket);
        }
        
        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket s = (Socket)ar.AsyncState;
                s.EndConnect(ar);
                Debug.Log("Socket connect succesfully");
                isConnecting = false;
                FireEvent(NetEvent.ConnectSucc, "");
                
                s.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain,
                    0, ReceiveCallback, s);
            }
            catch(SocketException ex)
            {
                Debug.LogError("Socket connect fail " + ex.ToString());
                FireEvent(NetEvent.ConnectFail, ex.ToString());
                isConnecting =false;
            }
        }

        public static void Close() 
        {
            Debug.Log($"close {socket} {socket.Connected} {isClosed} {isConnecting} {writeQueue.Count}");

            if(socket == null || socket.Connected == false || isClosed)
            {
                return;
            }

            if (isConnecting)
            {
                return;
            }

            if(writeQueue.Count > 0)
            {
                isClosing = true;
            }
            else
            {
                isClosed = true;
                socket.Close();
                FireEvent(NetEvent.Close, "");
            }
        }
        #endregion

        #region 消息处理、接收
        public delegate void MsgListener(MsgBase msgBase);
        private static Dictionary<string, MsgListener> msgListeners = new Dictionary<string, MsgListener>();

        public static void AddMsgListener(string msgName, MsgListener listener)
        {
            if (msgListeners.ContainsKey(msgName))
            {
                msgListeners[msgName] += listener;
            }
            else
            {
                msgListeners[msgName] = listener;
            }
        }
        public static void RemoveMsgListener(string msgName, MsgListener listener)
        {
            if(msgListeners.ContainsKey(msgName))
            {
                msgListeners[msgName] -= listener;

                if (msgListeners[msgName] == null)
                {
                    msgListeners.Remove(msgName);
                }
            }
        }

        private static void FireMsg(string msgName, MsgBase msg)
        {
            if(msgListeners.ContainsKey(msgName))
            {
                msgListeners[msgName](msg);
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                Socket s = (Socket)ar.AsyncState;
                int count = s.EndReceive(ar);
                //Debug.Log("ReceiveCallback " + count);
                if (count == 0)
                {
                    Close();
                    return;
                }
                readBuff.writeIdx += count;
                OnReceiveData();
                if(readBuff.remain < 8)
                {
                    readBuff.MoveBytes();
                    readBuff.ReSize(readBuff.length * 2);
                }

                s.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain,
                    0, ReceiveCallback, s);
            }
            catch (SocketException ex)
            {
                Debug.Log("Socket Receive fail " + ex.ToString());
            }
        }


        private static void OnReceiveData()
        {
            //Debug.Log("OnReceiveData");
            if (readBuff.length <= 2) return;

            int readIdx = readBuff.readIdx;
            byte[] bytes = readBuff.bytes;
            Int16 bodyLength = (Int16)((bytes[readIdx + 1] << 8) | bytes[readIdx]);
            if (readBuff.length < bodyLength + 2) return;

            readBuff.readIdx += 2;

            int nameLength = 0;
            string protoName = MsgBase.DecodeName(readBuff.bytes, readBuff.readIdx, out nameLength);
            if(protoName == "")
            {
                Debug.Log("OnReceiveData MsgBase.DecodeName fail");
                return;
            }
            
            readBuff.readIdx += nameLength;

            int bodyCount = bodyLength - nameLength;
            //Debug.Log($"OnReceiveData {protoName} {readBuff.readIdx} {bodyCount} {bodyLength} {nameLength}");
            MsgBase msg = MsgBase.Decode(protoName, readBuff.bytes, readBuff.readIdx, bodyCount);
            //Debug.Log($"OnReceiveData {protoName} {msg}");

            readBuff.readIdx += bodyCount;
            readBuff.CheckAndMoveBytes();
            /*
            使用 lock(msgList) 锁住消息列表，因为OnReceiveData在子线程将数据写入消息队列，
            而Update在主线程读取消息队列，为了避免线程冲突，对msgList的操作都需要加锁。 
            */
            lock (msgList)
            {
                msgList.Add(msg);
            }
            msgCount++;
            //是否继续读取消息
            if(readBuff.length > 2)
            {
                OnReceiveData();
            }
        }
        
        private static void MsgUpdate()
        {
            /*
            msgCount可能受到线程冲突的影响。
            比如在某一帧中，msgCount为0，但当程序刚好执行到“if(msgCount==0)”时，
            子线程的ReceiveCallback很可能刚好执行完“msgList.Add(msgBase)”但还没有执行“msgCount++”​，
            实际上消息队列不为空。
            这种情况会导致消息延迟一帧处理，但影响不大。
            
            另一种做法是使用“lock(msgList){if(msgList.Length==0)}”去判断消息列表是否有数据，
            但因为引入了锁，可能导致主线程等待。
            【若不是万不得已，一般不要轻易在主线程上用锁，不正确的运用会降低程序运行效率。】 
            */
            if (msgCount == 0) return;

            for (int i = 0; i < MAX_MESSAGE_FIRE; i++)
            {
                MsgBase msg = null;
                lock (msgList)
                {
                    if (msgList.Count > 0)
                    {
                        msg = msgList[0];
                        msgList.RemoveAt(0);
                        msgCount--;
                    }
                }
                if (msg != null)
                {
                    //Debug.Log($"fire msg {msg.protoName} {msg}");
                    FireMsg(msg.protoName, msg);
                }
                else
                {
                    break;
                }
            }
        }
        #endregion

        #region 发送
        public static void Send(MsgBase msg)
        {
            if (socket == null || socket.Connected == false) return;

            if (isConnecting || isClosing) return;

            byte[] nameBytes = MsgBase.EncodeName(msg);
            byte[] bodyBytes = MsgBase.Encode(msg);
            int len = nameBytes.Length + bodyBytes.Length;
            byte[] sendBytes = new byte[2 + len];
            //仍然是小端来表示整个sendBytes - 2 bytes的长度位
            sendBytes[0] = (byte)(len % 256);
            sendBytes[1] = (byte)(len / 256);
            Array.Copy(nameBytes, 0, sendBytes, 2, nameBytes.Length);
            Array.Copy(bodyBytes, 0, sendBytes, 2 + nameBytes.Length, bodyBytes.Length);

            //Debug.Log($"send {BitConverter.ToString(sendBytes)} ");
            ByteArray ba = new ByteArray(sendBytes);
            int count = 0;//writeQueue的长度
            lock (writeQueue)
            {
                writeQueue.Enqueue(ba);
                count = writeQueue.Count;
            }
            if (count == 1)
            {
                socket.BeginSend(sendBytes, 0, sendBytes.Length,
                    0, SendCallback, socket);
            }
        }
        private static void SendCallback(IAsyncResult ar)
        {
            Socket s = (Socket)ar.AsyncState;
            if (s == null || s.Connected == false) return;

            int count = s.EndSend(ar);
            ByteArray ba = null;
            lock (writeQueue)
            {
                ba = writeQueue.First();
            }
            ba.readIdx += count;
            if (ba.length == 0) //完整发送了
            {
                lock (writeQueue)
                {
                    writeQueue.Dequeue();
                    ba = writeQueue.First();
                }
            }
            //继续发送
            if (ba != null)
            {
                s.BeginSend(ba.bytes, ba.readIdx, ba.length,
                    0, SendCallback, s);
            }
            //没有待发送数据，并且已经请求过要关闭socket，那么调用关闭api
            else if (isClosing)
            {
                s.Close();
            }
        }
        #endregion

        #region 心跳机制
        /*
        客户端会定时（如30秒）给服务端发送PING协议，服务端收到后会回应PONG协议。
        正常情况下，客户端每隔一段时间（如30秒）必然会收到服务端的PONG协议（就算网络不通畅，最慢120秒也总该收到了吧）​。
        如果客户端很长时间（如120秒）没有收到PONG协议，很大概率是网络不通畅或服务端挂掉，客户端程序可以释放Socket资源。
        
        其实对于客户端来说，释放不释放关系不大，毕竟只有一个Socket。
        但对服务端来说却很重要，因为服务端可能保持着数以万计的连接，
        当游戏在线人数很多时，只有及时释放资源，才能让玩家正常玩游戏（不然，内存爆满服务器挂掉大家都玩不了）​。 
        */
        private static void PingUpdate()
        {
            if(isUsePing == false) return;

            if(Time.time - lastPingTime > pingInterval)
            {
                MsgPing msgPing = new MsgPing();
                Send(msgPing);
                lastPingTime = Time.time;
            }

            if(Time.time - lastPongTime > pingInterval * 4)
            {
                Debug.Log("PingPong timeout close ");
                Close();
            }
        }

        private static void OnMsgPong(MsgBase msgBase)
        {
            lastPongTime = Time.time;

            Debug.Log("OnMsgPong");
        }
        #endregion

        /// <summary>
        /// 获取当前时间戳（秒）
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }
        public static long GetTimeStampMilliseconds()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds);
        }

        public static void Update()
        {
            MsgUpdate();
            PingUpdate();
        }
    }
}
