using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MyNetworkGame.TCPServer
{
    public class ClientState
    {
        public ClientState() 
        {
            lastPingTime = NetManager.GetTimeStamp();
        }

        public Socket? socket;
        public ByteArray readBuff = new ByteArray();

        public Queue<ByteArray> writeQueue = new Queue<ByteArray>();
        public bool isClosing = false;

        public long lastPingTime = 0;

        //public Player? player = null;
        public string PlayerName = string.Empty;

        /// <summary>
        /// 匹配的对手
        /// </summary>
        public ClientState? Opponent = null;

        /// <summary>
        /// 和对手所在的房间
        /// </summary>
        public UNOCardRoom? room = null;

        /// <summary>
        /// 是否准备好进入游戏
        /// </summary>
        public bool isReadyToGame = false;

        /// <summary>
        /// 是否请求恢复暂停（匹配打球的双方都请求恢复，才恢复暂停；打球的双方任何一个请求暂停，就暂停）
        /// </summary>
        public bool isRequestResume = false;
    }
}
