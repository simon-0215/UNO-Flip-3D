using MyNetworkGame.TCPServer;

namespace MyNetworkGame.TCPServer
{
    public partial class MsgHandler
    {
        public static void MsgPing(ClientState c, MsgBase msgBase)
        {
            //Console.WriteLine("MsgHandler MsgPing");
            c.lastPingTime = NetManager.GetTimeStamp();
            MsgPong msgPong = new MsgPong();
            NetManager.Send(c, msgPong);
        }
    }
}
