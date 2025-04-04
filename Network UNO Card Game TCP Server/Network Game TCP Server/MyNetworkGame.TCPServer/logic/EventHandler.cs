using MyNetworkGame.TCPServer.UnoFlipV2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetworkGame.TCPServer
{
    public enum NetEvent
    {
        ConnectSucc = 1,
        ConnectFail = 2,
        Close = 3
    }

    public partial class EventHandler
    {
        public static void OnAppStarted()
        {
            Debug.Log("On App Started..");
            DeckData.InitSides();
        }

        public static void OnConnect(ClientState client)
        {
            Console.WriteLine("OnConnect " + client.socket.RemoteEndPoint);
        }

        public static void OnDisconnect(ClientState client)
        {
            Console.WriteLine("OnDisconnect");

            //PingPongGameManager.Disconnect(state);

            if (client.room != null)
            {
                /*
                MsgDisMatched msg = new MsgDisMatched();
                //NetManager.Send(client, msg);
                NetManager.Send(client.Opponent, msg);
                */
            }

            ClientManager.RemoveUnMatchedClient(client);
        }

        public static void OnTimer()
        {
            //Console.WriteLine("OnTimer");
            CheckPing();
        }

        private static void CheckPing()
        {
            long timeNow = NetManager.GetTimeStamp();
            foreach(ClientState c in NetManager.Clients.Values)
            {
                if(timeNow - c.lastPingTime > NetManager.PingInterval * 4)
                {
                    Console.WriteLine($"Ping timeOut close {c.socket.RemoteEndPoint.ToString()}");
                    NetManager.Close(c);
                    return;
                }
            }
        }
    }
}
