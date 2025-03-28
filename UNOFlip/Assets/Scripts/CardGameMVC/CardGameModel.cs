using UnityEngine;
using QFramework;
using System.Collections.Generic;
using MyTcpClient;
using System;

public class CardGameModel : AbstractModel
{
    public string currentMessage = string.Empty;
    public CardDisplay currentCardDisplay;
    public Card currentCard;
    public CardColour currentCardColour;
    public MsgBase currentMsg;
    public Type currentMsgType;

    /// <summary>
    /// �յ��ķ�����Ϣ���У�����ͨ��Server���Ƿ�����ҷ��ƣ�
    /// </summary>
    //public Queue<MsgDealPlayer2Card> MsgDealPlayer2CardRecvQueue = new Queue<MsgDealPlayer2Card>();
    public bool PlayerBGetMsgInitDeck = false;//���b�յ�ͬ��ϴ�Ƶ���Ϣ
    public bool PlayerBInitDeckDone = false;//�����յ����b�ɹ�ͬ��ϴ�Ƶ���Ϣ
    public bool PlayerBDealCardsDone = false;//���b������ɸ��߷���
    public MsgGetFirstPileCard msgFirstPileCard = null;
    public bool PlayerBSyncPlayCardDone = false;//���bͬ�����Ʋ������

    public bool isHost = false;//��ǰ�ͻ����Ƿ�host��������һ������ƥ�������Ƿ�����AI�߼������ڷ����ͻ��ˣ�
    public string MyPlayerName = string.Empty;//�ҵ��ǳ�
    public string OpponentPlayerName = string.Empty;//��λ����ǳ�

    public List<Player> players = new List<Player>();
    public int currentPlayer = 0;
    public int playDirection = 1; //1 //-1

    public CardDisplay topCard;
    public CardColour topColour = CardColour.NONE;
    public bool unoCalled;

    public bool myTurn { get; set; }

    public Color32 red;
    public Color32 blue;
    public Color32 green;
    public Color32 yellow;
    public Color32 black;

    public (Color32 red, Color32 blue, Color32 green, Color32 yellow, Color32 black) Colours
    {
        get { return (red, blue, green, yellow, black); }
    }

    public int NextPlayerHandSize
    {
        get
        {
            int numberOfPlayer = players.Count;
            int nextPlayer = (currentPlayer + playDirection + players.Count) % players.Count;
            int nextPlayerHandSize = players[nextPlayer].playerHand.Count;
            return nextPlayerHandSize;
        }
    }

    public bool CanPlayAnyCard
    {
        get
        {
            foreach (Card card in players[currentPlayer].playerHand)
            {
                if (IsPlayable(card))
                {
                    return true;
                }
            }
            return false;
        }
    }

    bool IsPlayable(Card card)
    {
        return card.cardColour == topColour ||
                 card.cardValue == topCard.MyCard.cardValue ||
                    card.cardColour == CardColour.NONE;
    }


    protected override void OnInit()
    {
        //MsgDealPlayer2CardRecvQueue = new Queue<MsgDealPlayer2Card>();
        msgFirstPileCard = null;
    }
}
