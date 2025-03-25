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

        public static void MsgReady(ClientState client, MsgBase msgBase)
        {
            client.isReadyToGame = true;

            if (client.Opponent.isReadyToGame)
            {
                NetManager.Send(client, msgBase);
                NetManager.Send(client.Opponent, msgBase);
            }
        }

        /*
        //房主给对面玩家发牌
        public static void MsgDealPlayer2Card(ClientState client, MsgBase msgBase)
        {
            MsgDealPlayer2Card msg = (MsgDealPlayer2Card)msgBase;
            Debug.Log($"发牌 {msg.card.cardColour} {msg.card.cardValue}");
            NetManager.Send(client.Opponent, msgBase);
        }*/

        public static void MsgInitDeck(ClientState client, MsgBase msgBase)
        {
            NetManager.Send(client.Opponent, msgBase);
        }
        public static void MsgPlayerBInitDeckDone(ClientState client, MsgBase msgBase)
        {
            NetManager.Send(client.Opponent, msgBase);
        }
        public static void MsgPlayerBDealCardsDone(ClientState client, MsgBase msgBase)
        {
            NetManager.Send(client.Opponent, msgBase);
        }

        public static void MsgGetFirstPileCard(ClientState client, MsgBase msgBase)
        {
            Debug.Log($"翻开第一张牌顶卡牌");
            NetManager.Send(client.Opponent, msgBase);
        }

        public static void MsgPlayCard(ClientState client, MsgBase msgBase)
        {
            NetManager.Send(client.Opponent, msgBase);
        }
        public static void MsgPlayerBSyncPlayCardDone(ClientState client, MsgBase msgBase)
        {
            NetManager.Send(client.Opponent, msgBase);
        }
        public static void MsgSwitchPlayer(ClientState client, MsgBase msgBase)
        {
            NetManager.Send(client.Opponent, msgBase);
        }

    }
}
