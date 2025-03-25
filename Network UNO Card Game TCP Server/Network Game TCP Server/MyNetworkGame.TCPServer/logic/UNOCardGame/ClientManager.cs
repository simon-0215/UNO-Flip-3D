using System.Net.Sockets;

namespace MyNetworkGame.TCPServer
{
    public class ClientManager
    {
        private static Dictionary<Socket, ClientState> unMatchedClients = new Dictionary<Socket, ClientState>();

        public static void AddUnMatchedClient(ClientState c)
        {
            unMatchedClients.Add(c.socket, c);
        }
        public static void RemoveUnMatchedClient(ClientState c)
        {
            if (unMatchedClients.ContainsKey(c.socket))
            {
                unMatchedClients.Remove(c.socket);
            }
        }

        public static bool IsMatched(Socket s)
        {
            return !unMatchedClients.ContainsKey(s);
        }

        public static ClientState? GetUnMatchClient()
        {
            ClientState[] arr = unMatchedClients.Values.ToArray();
            return arr.Length > 0 ? arr[0] : null;
        }

        public static void MatchClient(ClientState c1, ClientState c2)
        {
            Console.WriteLine($"MatchClient {c1.socket.RemoteEndPoint} {c2.socket.RemoteEndPoint}");

            if (unMatchedClients.ContainsKey(c1.socket))
                unMatchedClients.Remove(c1.socket);
            if (unMatchedClients.ContainsKey(c2.socket))
                unMatchedClients.Remove(c2.socket);

            c1.Opponent = c2;
            c2.Opponent = c1;

            UNOCardRoom room = new UNOCardRoom(c1, c2);
            c1.room = room;
            c2.room = room;

            MsgPlayerMatchRequest msg1 = new MsgPlayerMatchRequest();
            msg1.currentPlayerName = c1.PlayerName;
            msg1.otherPlayerName = c2.PlayerName;
            msg1.isHost = true;
            NetManager.Send(c1, msg1);

            MsgPlayerMatchRequest msg2 = new MsgPlayerMatchRequest();
            msg2.currentPlayerName = c2.PlayerName;
            msg2.otherPlayerName = c1.PlayerName;
            msg2.isHost = false;
            NetManager.Send(c2, msg2);
        }
    }
}
