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
    /// 收到的发牌消息队列（房主通过Server给非房主玩家发牌）
    /// </summary>
    //public Queue<MsgDealPlayer2Card> MsgDealPlayer2CardRecvQueue = new Queue<MsgDealPlayer2Card>();
    public bool PlayerBGetMsgInitDeck = false;//玩家b收到同步洗牌的消息
    public bool PlayerBInitDeckDone = false;//房主收到玩家b成功同步洗牌的消息
    public bool PlayerBDealCardsDone = false;//玩家b发牌完成告诉房主
    public MsgGetFirstPileCard msgFirstPileCard = null;
    public bool PlayerBSyncPlayCardDone = false;//玩家b同步出牌操作完成

    public bool isHost = false;//当前客户端是否host房主（第一个进入匹配的玩家是房主，AI逻辑运行在房主客户端）
    public string MyPlayerName = string.Empty;//我的昵称
    public string OpponentPlayerName = string.Empty;//对位玩家昵称

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
