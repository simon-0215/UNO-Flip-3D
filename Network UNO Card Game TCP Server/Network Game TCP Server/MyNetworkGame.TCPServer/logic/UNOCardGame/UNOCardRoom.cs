using MyNetworkGame.TCPServer.UnoFlipV2;

namespace MyNetworkGame.TCPServer
{
    public class UNOCardRoom
    {
        public UNOCardRoom(ClientState c1, ClientState c2)
        {
            client1 = c1;
            client2 = c2;

            c1.room = this;
            c2.room = this;

            //gameData = new UnoFlipGameData(c1, c2);//初始化本局游戏数据
        }

        public ClientState client1;
        public ClientState client2;

        public UnoFlipGameData gameData;
    }
}
