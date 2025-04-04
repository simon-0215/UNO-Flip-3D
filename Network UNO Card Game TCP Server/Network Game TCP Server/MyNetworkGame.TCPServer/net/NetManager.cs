using System.Net;
using System.Net.Sockets;
using System.Reflection;
using MyNetworkGame.TCPServer;

/*
通用服务端框架实现。
该框架基于Select多路复用处理网络消息，具有粘包半包处理、心跳机制等功能，还使用MySQL数据库存储玩家数据，
是一套功能较为完备的C#服务端程序。一般来说，单个服务端进程可以承载数百名玩家；若游戏大火，读者可以将它改成分布式结构，支撑更多玩家。

模块划分:
1、网络底层
2、消息处理
3、事件处理
4、数据库底层
5、存储结构

从服务端角度看，一个玩家经历的阶段：
    连接、登录、获取数据、操作交互、保存数据、退出

保存玩家数据的时机选择策略

*/
namespace MyNetworkGame.TCPServer
{

    class NetManager
    {
        private static int port = 8888;
        //监听Socket
        public static Socket listenfd;
        //客户端Socket及状态信息
        public static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();
        public static Dictionary<Socket, ClientState> Clients { get { return clients; } }

        //Select的检查列表
        static List<Socket> checkRead = new List<Socket>();
        //ping间隔
        public static long pingInterval = 30;
        public static long PingInterval { get { return pingInterval; } }

        public static void Main()
        {
            Console.WriteLine("UNO Card Game - TCP Server");

            StartLoop(port);

            Console.ReadLine();
        }

        public static void StartLoop(int listenPort)
        {
            //Socket
            listenfd = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            //Bind
            IPAddress ipAdr = IPAddress.Parse("0.0.0.0");
            IPEndPoint ipEp = new IPEndPoint(ipAdr, listenPort);
            listenfd.Bind(ipEp);
            //Listen
            listenfd.Listen(0);
            Console.WriteLine("[服务器]启动成功");
            // 获取本机的 IPv4 地址
            string ipAddress = GetLocalIPv4Address();
            Console.WriteLine($"本机ip地址:{ipAddress}");
            Console.WriteLine($"监听端口:{port}");

            MethodInfo? mi = typeof(EventHandler).GetMethod("OnAppStarted");
            if (mi != null)
            {
                object[] ob = { };
                mi.Invoke(null, ob);
            }

            //循环
            while (true)
            {
                ResetCheckRead();  //重置checkRead
                Socket.Select(checkRead, null, null, 1000);
                //检查可读对象
                for (int i = checkRead.Count - 1; i >= 0; i--)
                {
                    Socket s = checkRead[i];
                    if (s == listenfd)
                    {
                        ReadListenfd(s);
                    }
                    else
                    {
                        ReadClientfd(s);
                    }
                }
                //超时
                Timer();
            }
        }

        //填充checkRead列表
        public static void ResetCheckRead()
        {
            checkRead.Clear();
            checkRead.Add(listenfd);
            foreach (ClientState s in clients.Values)
            {
                checkRead.Add(s.socket);
            }
        }

        //读取Listenfd
        public static void ReadListenfd(Socket listenfd)
        {
            try
            {
                Socket clientfd = listenfd.Accept();
                Console.WriteLine("Accept " + clientfd.RemoteEndPoint.ToString());
                ClientState state = new ClientState();
                state.socket = clientfd;
                state.lastPingTime = GetTimeStamp();
                clients.Add(clientfd, state);

                MethodInfo? mi = typeof(EventHandler).GetMethod("OnConnect");
                if (mi != null)
                {
                    object[] ob = { state };
                    mi.Invoke(null, ob);
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Accept fail" + ex.ToString());
            }
        }

        //关闭连接
        public static void Close(ClientState state)
        {
            //消息分发
            MethodInfo mei = typeof(EventHandler).GetMethod("OnDisconnect");
            object[] ob = { state };
            mei.Invoke(null, ob);
            //关闭
            state.socket.Close();
            clients.Remove(state.socket);

        }

        //读取Clientfd
        public static void ReadClientfd(Socket clientfd)
        {

            ClientState state = clients[clientfd];
            ByteArray readBuff = state.readBuff;
            //接收
            int count = 0;
            //缓冲区不够，清除，若依旧不够，只能返回
            //当单条协议超过缓冲区长度时会发生
            if (readBuff.remain <= 0)
            {
                OnReceiveData(state);
                readBuff.MoveBytes();
            };
            if (readBuff.remain <= 0)
            {
                Console.WriteLine("Receive fail , maybe msg length > buff capacity");
                Close(state);
                return;
            }
            try
            {
                count = clientfd.Receive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Receive SocketException " + ex.ToString());
                Close(state);
                return;
            }
            //客户端关闭
            if (count <= 0)
            {
                Console.WriteLine("Socket Close " + clientfd.RemoteEndPoint.ToString());
                Close(state);
                return;
            }
            //消息处理
            readBuff.writeIdx += count;
            //处理二进制消息
            OnReceiveData(state);
            //移动缓冲区
            readBuff.CheckAndMoveBytes();
        }


        //数据处理
        public static void OnReceiveData(ClientState state)
        {
            ByteArray readBuff = state.readBuff;
            //消息长度
            if (readBuff.length <= 2)
            {
                return;
            }
            //消息体长度
            int readIdx = readBuff.readIdx;
            byte[] bytes = readBuff.bytes;
            Int16 bodyLength = (Int16)((bytes[readIdx + 1] << 8) | bytes[readIdx]);
            if (readBuff.length < bodyLength)
            {
                return;
            }
            readBuff.readIdx += 2;
            //解析协议名
            int nameCount = 0;
            string protoName = MsgBase.DecodeName(readBuff.bytes, readBuff.readIdx, out nameCount);
            if (protoName == "")
            {
                Console.WriteLine("OnReceiveData MsgBase.DecodeName fail");
                Close(state);
                return;
            }
            readBuff.readIdx += nameCount;
            //解析协议体
            int bodyCount = bodyLength - nameCount;
            if (bodyCount <= 0)
            {
                Console.WriteLine("OnReceiveData fail, bodyCount <=0 ");
                Close(state);
                return;
            }
            MsgBase msgBase = MsgBase.Decode(protoName, readBuff.bytes, readBuff.readIdx, bodyCount);
            readBuff.readIdx += bodyCount;
            readBuff.CheckAndMoveBytes();
            //分发消息
            MethodInfo mi = typeof(MsgHandler).GetMethod(protoName);
            object[] o = { state, msgBase };
            Console.WriteLine("Receive " + protoName);
            if (mi != null)
            {
                mi.Invoke(null, o);
            }
            else
            {
                Console.WriteLine("OnReceiveData Invoke fail " + protoName);
            }
            //继续读取消息
            if (readBuff.length > 2)
            {
                OnReceiveData(state);
            }
        }

        //发送
        public static void Send(ClientState cs, MsgBase msg)
        {
            //状态判断
            if (cs == null)
            {
                return;
            }
            if (!cs.socket.Connected)
            {
                return;
            }
            //数据编码
            byte[] nameBytes = MsgBase.EncodeName(msg);
            byte[] bodyBytes = MsgBase.Encode(msg);
            int len = nameBytes.Length + bodyBytes.Length;
            byte[] sendBytes = new byte[2 + len];
            //组装长度
            sendBytes[0] = (byte)(len % 256);
            sendBytes[1] = (byte)(len / 256);
            //组装名字
            Array.Copy(nameBytes, 0, sendBytes, 2, nameBytes.Length);
            //组装消息体
            Array.Copy(bodyBytes, 0, sendBytes, 2 + nameBytes.Length, bodyBytes.Length);
            //为简化代码，不设置回调
            try
            {
                cs.socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, null, null);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Socket Close on BeginSend" + ex.ToString());
            }
        }

        //定时器
        static void Timer()
        {
            //消息分发
            MethodInfo mei = typeof(EventHandler).GetMethod("OnTimer");
            object[] ob = { };
            mei.Invoke(null, ob);
        }

        #region 辅助方法
        /// <summary>
        /// 获取当前时间戳（秒）
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }
        /// <summary>
        /// 获取当前时间戳：毫秒
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStampMilliseconds()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds);
        }

        // 获取本机的 IPv4 地址
        static string GetLocalIPv4Address()
        {
            // 获取本机的主机名
            string hostName = Dns.GetHostName();

            // 获取主机名对应的所有 IP 地址
            var ipAddresses = Dns.GetHostEntry(hostName).AddressList;

            // 过滤出 IPv4 地址
            var ipv4Address = ipAddresses
                .FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

            return ipv4Address?.ToString() ?? "未找到 IPv4 地址";
        }
        #endregion
    }
}
