namespace MyNetworkGame.TCPServer
{
    public partial class MsgHandler
    {
        public static void MsgPlayerMatchRequest(ClientState client, MsgBase msgBase)
        {
            MsgPlayerMatchRequest msg = (MsgPlayerMatchRequest)msgBase;
            Console.WriteLine($"MsgPlayerMatchRequest {msg.protoName} {msg.currentPlayerName}");

            client.PlayerName = msg.currentPlayerName;

            ClientState? unMatchClient = ClientManager.GetUnMatchClient();
            if (unMatchClient != null)
            {
                Debug.Log("match clients by " + msg.currentPlayerName);
                ClientManager.MatchClient(unMatchClient, client);
            }
            else
            {
                Debug.Log("add un matched client " + msg.currentPlayerName);
                ClientManager.AddUnMatchedClient(client);
            }
        }
    }
}
