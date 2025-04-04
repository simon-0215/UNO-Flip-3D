using System.Collections.Generic;

namespace MyTcpClient
{
    #region init
    public class MsgPlayerMatchRequest : MsgBase
    {
        public MsgPlayerMatchRequest() { protoName = "MsgPlayerMatchRequest"; }

        public string currentPlayerName = string.Empty;//C�˷���Ϣʱ������������������

        public bool isHost = false;//S��ͬ��ƥ����ʱ�����߾���C�����Ƿ�host����
        public string otherPlayerName = string.Empty;

        // ����ӵ�����ai��AI-1��AI-2��4����ɫ��λ�ã�˳ʱ�룩�� host AI-1 otherPlayer AI-2
        //����otherPlayer���ԣ��������������� otherPlayer AI-2 host AI-1
    }

    public class MsgReady : MsgBase
    {
        public MsgReady() { protoName = "MsgReady"; }
    }
    #endregion
    /// <summary>
    /// ����ϴ�ƺ�������Bͬ��ϴ�ƽ��
    /// </summary>
    public class MsgInitDeck : MsgBase
    {
        public MsgInitDeck() { protoName = "MsgInitDeck"; }
        //public List<Card> cards;
        public List<ShortCard> s;
    }

    /// <summary>
    /// ���B���߷����Լ�ͬ�����
    /// </summary>
    public class MsgPlayerBInitDeckDone : MsgBase
    {
        public MsgPlayerBInitDeckDone() { protoName = "MsgPlayerBInitDeckDone"; }
    }

    /// <summary>
    /// ���b���߷�����������ϣ����Գ�����
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
    /// �����ƶѵĵ�һ�ſ���
    /// </summary>
    public class MsgGetFirstPileCard : MsgBase
    {
        public MsgGetFirstPileCard() { protoName = "MsgGetFirstPileCard"; }
        public Card pileCard;
        public CardColour topColour;
    }

    /// <summary>
    /// ����֪ͨ���bͬ����ĳһ����ɫ����һ���ai�����һ����
    /// </summary>
    public class MsgPlayCard : MsgBase
    {
        public MsgPlayCard() { protoName = "MsgPlayCard"; }

        public int playerIdx;
        public Card card;
    }

    /// <summary>
    /// ���b֪ͨ������ͬ����һ�������
    /// </summary>
    public class MsgPlayerBSyncPlayCardDone : MsgBase
    {
        public MsgPlayerBSyncPlayCardDone() { protoName = "MsgPlayerBSyncPlayCardDone"; }
    }

    public class MsgSwitchPlayer: MsgBase
    {
        public MsgSwitchPlayer() { protoName = "MsgSwitchPlayer"; }

        public int playerIdx;
    }

    public class MsgUnoButtonClick: MsgBase
    {
        public MsgUnoButtonClick() { protoName = "MsgUnoButtonClick"; }
    }
    public class MsgDrawCardFromDeck: MsgBase
    {
        public MsgDrawCardFromDeck() { protoName = "MsgDrawCardFromDeck"; }
    }
}
/*
����server
ƥ��
ready  -- ������Ϸ
ϴ��ͬ��
���� - ���ƽ������жϣ� host����
������һ���� 

host���� -> ͬ��
AI���� -> ͬ��
֪ͨ���B���Ƿ�����ң�����
���B���� -> ͬ��
AI���� -> ͬ��

ĳһ��ʤ�� -> ͬ��
    ֻ���Է��������水ť

 
*/