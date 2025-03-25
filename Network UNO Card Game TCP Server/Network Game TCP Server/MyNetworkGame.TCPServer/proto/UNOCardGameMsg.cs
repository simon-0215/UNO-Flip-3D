namespace MyNetworkGame.TCPServer
{

    public class MsgPlayerMatchRequest : MsgBase
    {
        public MsgPlayerMatchRequest() { protoName = "MsgPlayerMatchRequest"; }

        public string currentPlayerName = string.Empty;//C端发消息时：发起请求的玩家名称

        public bool isHost = false;//S端同步匹配结果时：告诉具体C端它是否host房主
        public string otherPlayerName = string.Empty;

        // 另外加的两个ai，AI-1、AI-2，4个角色的位置（顺时针）： host AI-1 otherPlayer AI-2
        //对于otherPlayer而言，看起来是这样： otherPlayer AI-2 host AI-1
    }

    public class MsgReady : MsgBase
    {
        public MsgReady() { protoName = "MsgReady"; }
    }

    // ------------------------- start game

    public class MsgInitDeck : MsgBase
    {
        public MsgInitDeck() { protoName = "MsgInitDeck"; }
        //public List<Card> cards;
        public List<ShortCard> s;
    }
    /// <summary>
    /// 玩家B告诉房主自己同步完成
    /// </summary>
    public class MsgPlayerBInitDeckDone : MsgBase
    {
        public MsgPlayerBInitDeckDone() { protoName = "MsgPlayerBInitDeckDone"; }
    }

    /// <summary>
    /// 玩家b告诉房主，发牌完毕，可以出牌了
    /// </summary>
    public class MsgPlayerBDealCardsDone : MsgBase
    {
        public MsgPlayerBDealCardsDone() { protoName = "MsgPlayerBDealCardsDone"; }
    }
    /*
    public class MsgDealPlayer2Card : MsgBase
    {
        public MsgDealPlayer2Card() { protoName = "MsgDealPlayer2Card"; }

        public Card card;
    }*/

    /// <summary>
    /// 翻开牌堆的第一张卡牌
    /// </summary>
    public class MsgGetFirstPileCard : MsgBase
    {
        public MsgGetFirstPileCard() { protoName = "MsgGetFirstPileCard"; }
        public Card pileCard;
        public CardColour topColour;
    }

    /// <summary>
    /// 房主通知玩家b同步（某一个角色：玩家或者ai）打出一张牌
    /// </summary>
    public class MsgPlayCard : MsgBase
    {
        public MsgPlayCard() { protoName = "MsgPlayCard"; }

        public int playerIdx;
        public Card card;
    }

    /// <summary>
    /// 玩家b通知房主，同步打一张牌完成
    /// </summary>
    public class MsgPlayerBSyncPlayCardDone : MsgBase
    {
        public MsgPlayerBSyncPlayCardDone() { protoName = "MsgPlayerBSyncPlayCardDone"; }
    }

    public class MsgSwitchPlayer : MsgBase
    {
        public MsgSwitchPlayer() { protoName = "MsgSwitchPlayer"; }

        public int playerIdx;
    }
}