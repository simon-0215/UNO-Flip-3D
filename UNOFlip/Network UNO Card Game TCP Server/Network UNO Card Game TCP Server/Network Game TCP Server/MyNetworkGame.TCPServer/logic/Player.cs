using MyNetworkGame.TCPServer;

namespace MyNetworkGame.TCPServer
{
    public class Player
    {
        public string id = "";
        public ClientState state;
        public int x;
        public int y;
        public int z;

        //存入数据库的数据
        public PlayerData data;

        public Player(ClientState _state) { 
            state = _state;
        }

        public void Send(MsgBase msg)
        {
            NetManager.Send(state, msg);
        }
    }
}
