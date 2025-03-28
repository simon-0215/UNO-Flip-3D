using MyTcpClient;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using System;

public class GameManagerNet : MonoBehaviour, IController
{
    #region serielize fields
    [SerializeField] private Deck deck;

    [SerializeField] Transform playerHandTransform; //HOLDS PLAYER HAND
    [SerializeField] List<Transform> aiHandTransform = new List<Transform>(); //HOLDS AI HANDS
    [SerializeField] GameObject cardPrefab;

    [SerializeField] int startingHandSize = 7;//开局手牌数

    [Header("Game Play")]
    [SerializeField] Transform discardPileTransform;
    //[SerializeField] CardDisplay topCard;
    [SerializeField] Transform directionArrow;
    [SerializeField] GameObject wildPanel;
    [SerializeField] WildButton redButton;
    [SerializeField] WildButton blueButton;
    [SerializeField] WildButton greenButton;
    [SerializeField] WildButton yellowButton;

    [Header("Colours")]
    [SerializeField] Color32 red;
    [SerializeField] Color32 blue;
    [SerializeField] Color32 green;
    [SerializeField] Color32 yellow;
    [SerializeField] Color32 black;

    [Header("UI Elements")]
    [SerializeField] TMP_Text winningText;
    [SerializeField] GameObject winPanel;
    [SerializeField] List<Image> playerHighlights = new List<Image>();
    [SerializeField] List<TMP_Text> playerCardCount = new List<TMP_Text>();
    [SerializeField] TMP_Text messageText;
    #endregion
    #region private 
    private CardGameModel model;

    private void Awake()
    {
        model = this.GetModel<CardGameModel>();

        model.red = red;
        model.blue = blue;
        model.green = green;
        model.yellow = yellow;
        model.black = black;
    }
    #endregion

    void InitializePlayers()
    {
        model.players.Clear();
        model.players.Add(new Player(model.MyPlayerName, true, model.isHost));
        model.players.Add(new AiPlayer(model.isHost ? "AI-1" : "AI-2"));
        model.players.Add(new Player(model.OpponentPlayerName, true, !model.isHost));
        model.players.Add(new AiPlayer(model.isHost ? "AI-2" : "AI-1"));
    }
    void Start()
    {
        #region init
        //HIDE WILD PANEL
        wildPanel.SetActive(false);
        redButton.SetImageColour(red);
        blueButton.SetImageColour(blue);
        greenButton.SetImageColour(green);
        yellowButton.SetImageColour(yellow);
        winPanel.SetActive(false);

        //INITIALIZE PLAYERS
        InitializePlayers();
        
        UpdateMessageBox("Welcome to UNO!");
        #endregion
        #region register events
        this.RegisterEvent<UpdateMessageBoxEvent>(e =>
        {
            UpdateMessageBox(e.message);
        }).UnRegisterWhenGameObjectDestroyed(gameObject);
        //打出一张牌
        this.RegisterEvent<PlayCardEvent>(e =>
        {
            PlayCard(e.cardDisplay, e.card);
        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        this.RegisterEvent<DrawCardFromDeckEvent>(e =>
        {
            DrawCardFromDeck();
        }).UnRegisterWhenGameObjectDestroyed(gameObject);
        
        this.RegisterEvent<AiSwitchPlayerEvent>(e =>
        {
            AiSwitchPlayer();
        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        this.RegisterEvent<UpdatePlayerHighlightsEvent>(e =>
        {
            UpdatePlayerHighlights();
        }).UnRegisterWhenGameObjectDestroyed(gameObject);
                
        this.RegisterEvent<ChosenColourEvent>(e =>
        {
            ChosenColour(e.cardColour);
        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        #endregion

        this.RegisterEvent<NetOnMsgEvent>(e =>
        {
            switch (e.type.Name)
            {
                /*case nameof(MsgDealPlayer2Card):
                    MsgDealPlayer2Card msg = (MsgDealPlayer2Card)e.msg;
                    model.MsgDealPlayer2CardRecvQueue.Enqueue(msg);//入队
                    break;*/
                case nameof(MsgInitDeck):
                    MsgInitDeck msg = (MsgInitDeck)e.msg;
                    deck.InitializeDeck(msg.s);
                    model.PlayerBGetMsgInitDeck = true;
                    break;
                case nameof(MsgPlayerBInitDeckDone):
                    model.PlayerBInitDeckDone = true;
                    break;
                case nameof(MsgPlayerBDealCardsDone):
                    model.PlayerBDealCardsDone = true;
                    break;
                case nameof(MsgGetFirstPileCard):
                    model.msgFirstPileCard = (MsgGetFirstPileCard)e.msg;
                    break;

                case nameof(MsgPlayCard):
                    MsgPlayCard msgPlayCard = (MsgPlayCard)e.msg;
                    int idx = (2 + msgPlayCard.playerIdx) % 4;
                    model.currentPlayer = idx;
                    model.myTurn = false;
                    Card card = model.players[idx].playerHand.Find(c => c.cardValue == msgPlayCard.card.cardValue && c.cardColour == msgPlayCard.card.cardColour);
                    StartCoroutine(PlayCardNetwork(idx, null, card));

                    print($"收到MsgPlayCard {msgPlayCard.playerIdx} {idx} {msgPlayCard.card.cardValue} {msgPlayCard.card.cardColour}");
                    print($"玩家b同步出牌 {card.cardColour} {card.cardValue}");

                    if (model.isHost)
                    {

                    }
                    else
                    {   
                    }
                    break;
                case nameof(MsgPlayerBSyncPlayCardDone):
                    model.PlayerBSyncPlayCardDone = true;
                    break;
                case nameof(MsgSwitchPlayer):
                    MsgSwitchPlayer switchPlayer = (MsgSwitchPlayer)e.msg;

                    model.currentPlayer = (2 + switchPlayer.playerIdx) % 4;
                    UpdatePlayerHighlights();
                    UpdateMessageBox(model.players[model.currentPlayer].playerName + " TURN");

                    //RESET UNO CALLED
                    model.unoCalled = false;
                    if (model.players[model.currentPlayer].IsHuman)
                    {
                        model.myTurn = model.isHost == model.players[model.currentPlayer].IsHost;
                    }
                    break;
            }
        });
        #region Start Game
        if (model.isHost)
        {
            //INITIALIZE DECK
            deck.InitializeDeck();
            // Send net msg to sync Game State
            MsgInitDeck msgInitDeck = new MsgInitDeck();
            msgInitDeck.s = deck.Cards.ToArray().Select(c => c.GetShort()).ToList();
            NetManager.Send(msgInitDeck);
        }

        //DEAL CARDS then Start Game
        StartCoroutine(DealStartingCards());
        #endregion
    }
    #region wait helper function
    /*
    /// <summary>
    /// 等待收到Server转发的房主Host发牌消息
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitForGetMsgDealPlayer2Card()
    {
        while(model.MsgDealPlayer2CardRecvQueue.Count < 1)
        {
            yield return null;//等待下一帧
        }
    }*/
    IEnumerator WaitForGetMsgInitDeck()//玩家B等待收到洗牌同步消息
    {
        while(model.PlayerBGetMsgInitDeck == false)
        {
            yield return null;
        }
    }
    IEnumerator WaitForPlayerBInitDeckDone()//房主等待收到b洗牌完成消息
    {
        while(model.PlayerBInitDeckDone == false)
        {
            yield return null;//wait util next frame to recheck
        }
    }
    IEnumerator WaitForPlayerBDealCardsDone()//房主等待收到b发牌完成消息
    {
        while (model.PlayerBDealCardsDone == false)
            yield return null;
    }
    IEnumerator WaitForPlayerBSyncPlayCardDone()
    {
        while (model.PlayerBSyncPlayCardDone == false)
            yield return null;
    }
    /// <summary>
    /// 等待收到牌顶第一张牌的消息
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitForGetMsgFirstPileCard()
    {
        while(model.msgFirstPileCard == null)
        {
            yield return null;
        }
    }
    #endregion
    #region ..
    CardColour PickRandomColour()
    {
        CardColour[] colours = (CardColour[])System.Enum.GetValues(typeof(CardColour));
        int randomIndex = UnityEngine.Random.Range(0, colours.Length - 1);
        return colours[randomIndex];
    }
    void SetTopCard()
    {
        //HAND OUT TOP CARD
        Card pileCard = model.msgFirstPileCard.pileCard;
        GameObject newCard = Instantiate(cardPrefab);
        MoveCardToPile(newCard);
        CardDisplay display = newCard.GetComponentInChildren<CardDisplay>();
        display.SetCard(pileCard, null);
        display.ShowCard();
        newCard.GetComponentInChildren<CardInteraction>().enabled = false;
        
        //SET TOP CARD
        model.topCard = display;
        model.topColour = model.msgFirstPileCard.topColour;
    }

    /// <summary>
    /// 房主client逻辑负责发牌
    /// </summary>
    /// <returns></returns>
    IEnumerator HostDealStartingCards()
    {
        for (int i = 0; i < startingHandSize; i++)
        {
            foreach (Player player in model.players)
            {
                Card drawnCard = deck.DrawCard();
                player.DrawCard(drawnCard);

                //VISUALISE CARDS
                Transform hand = player.IsHuman && player.IsHost == model.isHost ? playerHandTransform : aiHandTransform[model.players.IndexOf(player) - 1];
                GameObject card = Instantiate(cardPrefab, hand, false);

                //DRAW CARDS
                CardDisplay cardDisplay = card.GetComponentInChildren<CardDisplay>();
                cardDisplay.SetCard(drawnCard, player);

                //FOR Current Player
                if (player.IsHuman && player.IsHost == model.isHost)
                {
                    cardDisplay.ShowCard();//SHOW front OF CARD
                }
                yield return new WaitForSeconds(0.1f);
            }
        }

        //HAND OUT TOP CARD
        Card pileCard = deck.DrawCard();
        GameObject newCard = Instantiate(cardPrefab);
        MoveCardToPile(newCard);
        CardDisplay display = newCard.GetComponentInChildren<CardDisplay>();
        display.SetCard(pileCard, null);
        display.ShowCard();
        newCard.GetComponentInChildren<CardInteraction>().enabled = false;
        deck.AddUsedCard(pileCard);

        //SET TOP CARD
        model.topCard = display;
        model.topColour = pileCard.cardColour;
        //PICK RANDOM COLOUR IF WE HAVE WILD CARD
        if (model.topColour == CardColour.NONE)
        {
            model.topColour = PickRandomColour();
        }

        MsgGetFirstPileCard topCardMsg = new MsgGetFirstPileCard();
        topCardMsg.pileCard = pileCard;
        topCardMsg.topColour = model.topColour;
        NetManager.Send(topCardMsg);
    }
    #endregion
    IEnumerator DealStartingCards()
    {
        if (model.isHost)//房主负责发牌
        {
            yield return StartCoroutine(WaitForPlayerBInitDeckDone());
            yield return StartCoroutine(HostDealStartingCards());
        }
        # region 非房主玩家（房主的对面玩家）发牌
        else
        {
            yield return StartCoroutine(WaitForGetMsgInitDeck());
            //同步洗牌结果完成，发消息告诉房主
            MsgPlayerBInitDeckDone msgInitDeckDone = new MsgPlayerBInitDeckDone();
            NetManager.Send(msgInitDeckDone);

            //玩家B发牌：B和房主玩家的发牌开始位置不同
            List<Player> players = new List<Player>() {
                model.players[2],
                model.players[3],
                model.players[0],
                model.players[1],
            };
            for (int i = 0; i < startingHandSize; i++)
            {
                foreach (Player player in players)
                {
                    Card drawnCard = deck.DrawCard();
                    player.DrawCard(drawnCard);

                    //VISUALISE CARDS
                    Transform hand = player.IsHuman && player.IsHost == model.isHost ? playerHandTransform : aiHandTransform[model.players.IndexOf(player) - 1];
                    GameObject card = Instantiate(cardPrefab, hand, false);

                    //DRAW CARDS
                    CardDisplay cardDisplay = card.GetComponentInChildren<CardDisplay>();
                    cardDisplay.SetCard(drawnCard, player);

                    //FOR Current Player
                    if (player.IsHuman && player.IsHost == model.isHost)
                    {
                        cardDisplay.ShowCard();//SHOW front OF CARD
                    }

                    yield return new WaitForSeconds(0.1f);
                }
            }
            //发牌完成后，发送消息
            MsgPlayerBDealCardsDone msgDealCardsDone = new MsgPlayerBDealCardsDone();
            NetManager.Send(msgDealCardsDone);

            yield return StartCoroutine(WaitForGetMsgFirstPileCard());
            print("### 2");
            SetTopCard();
            print("### 3");

            model.currentPlayer = 2;//刚开始的时候，0房主 1 ai1 2玩家B  3 ai2
        }
        #endregion

        if (model.isHost)
        {
            model.myTurn = false;//在b发牌完成之前，房主也不可以点击出牌
            //等待玩家b发牌完成
            yield return StartCoroutine(WaitForPlayerBDealCardsDone());
        }
        
        TintArrow();
        //START GAME
        Debug.Log("Game Started");
        //先手玩家是host
        string firstPlayerName = model.isHost ? model.MyPlayerName : model.OpponentPlayerName;
        UpdateMessageBox($"{firstPlayerName} TURN");
        model.myTurn = model.isHost;
        UpdatePlayerHighlights();
    }
        
    //玩家或AI打出一张牌
    public void PlayCard(CardDisplay cardDisplay = null, Card card = null)
    {
        StartCoroutine(PlayCardNetwork(model.currentPlayer, cardDisplay, card));
    }
    IEnumerator PlayCardNetwork(int playerIdx, CardDisplay cardDisplay = null, Card card = null)
    {
        Card cardToPlay = cardDisplay?.MyCard ?? card;

        if (cardDisplay == null && card != null)
        {
            //cardDisplay = FindCardDisplayForCard(card);
            cardDisplay = FindCardDisplayForCard(card, playerIdx);
        }
        //CHECK IF CARD CAN BE PLAYED
        if (cardDisplay != null && !IsPlayable(cardDisplay.MyCard))
        {
            Debug.Log("Card cannot be played");
            yield break;
        }
        //REMOVE CARD FROM PLAYER HAND
        model.players[playerIdx].PlayCard(cardToPlay);

        //UNHIDE THE CARD IF AI PLAYER
        //MOVE THE CARD TO DISCARD PILE
        MoveCardToPile(cardDisplay.transform.parent.gameObject);
        //UPDATE TOP CARD
        model.topCard = cardDisplay;
        model.topColour = cardToPlay.cardColour;
        TintArrow();
        //IMPLEMENT WHAT SHOULD HAPPEN WHEN CARD PLAYED
        OnCardPlayed(model.topCard.MyCard);
        //UNHIDE CARD
        cardDisplay.ShowCard();
        //DEACTIVATE CARD INTERACTION
        cardDisplay.GetComponentInChildren<CardInteraction>().enabled = false;
        //ADD THE CARD BACK TO USED CARDS DECK
        deck.AddUsedCard(cardToPlay);

        if (model.isHost)
        {
            //Send Network Msg
            MsgPlayCard msg = new MsgPlayCard();
            msg.card = cardToPlay;
            msg.playerIdx = model.currentPlayer;
            NetManager.Send(msg);

            //等待玩家B返回同步出牌完成消息
            yield return StartCoroutine(WaitForPlayerBSyncPlayCardDone());

            // SWITCH PLAYER
            if (cardToPlay.cardValue != CardValue.SKIP)
            {
                SwitchPlayer();
            }
            else
            {
                SkipPlayer();
            }
        }
        else
        {
            UpdatePlayerHighlights();

            MsgPlayerBSyncPlayCardDone msg = new MsgPlayerBSyncPlayCardDone();
            NetManager.Send(msg);
        }
    }

    //FIND CARDISPLAY
    CardDisplay FindCardDisplayForCard(Card card)
    {
        Player player = model.players[model.currentPlayer];
        Transform hand = player.IsHuman && player.IsHost == model.isHost ? playerHandTransform : aiHandTransform[model.players.IndexOf(player) - 1];
        foreach (Transform cardTransform in hand)
        {
            CardDisplay tempDisplay = cardTransform.GetComponentInChildren<CardDisplay>();
            if (tempDisplay.MyCard == card)
            {
                return tempDisplay;
            }
        }
        return null;
    }
    CardDisplay FindCardDisplayForCard(Card card, int playerIdx)
    {
        Player player = model.players[playerIdx];
        Transform hand = player.IsHuman && player.IsHost == model.isHost ? playerHandTransform : aiHandTransform[model.players.IndexOf(player) - 1];
        foreach (Transform cardTransform in hand)
        {
            CardDisplay tempDisplay = cardTransform.GetComponentInChildren<CardDisplay>();
            if (tempDisplay.MyCard == card)
            {
                return tempDisplay;
            }
        }
        return null;
    }

    void MoveCardToPile(GameObject currentCard)
    {
        currentCard.transform.SetParent(discardPileTransform);
        currentCard.transform.localPosition = Vector3.zero;
        currentCard.transform.localScale = Vector3.one;

        RectTransform cardRect = currentCard.GetComponent<RectTransform>();
        RectTransform pileRect = discardPileTransform.GetComponent<RectTransform>();

        cardRect.sizeDelta = pileRect.sizeDelta;
        //UNHIDE CARD

        //FIX HOW CARDS ARE PLACED ON DISCARD PILE
        Quaternion rotation = discardPileTransform.rotation;
        float randomZRotation = UnityEngine.Random.Range(-10f, 10f);
        Quaternion randomRotation = Quaternion.Euler(0, 0, randomZRotation);
        currentCard.transform.rotation = rotation * randomRotation;
    }
    void OnCardPlayed(Card playedCard)
    {
        ApplyCardEffects(playedCard);
    }

    public void DrawCardFromDeck()
    {
        Card drawnCard = deck.DrawCard();
        Player player = model.players[model.currentPlayer];

        if (drawnCard != null)
        {
            player.DrawCard(drawnCard);

            //VISUALISE CARDS
            Transform hand = player.IsHuman ? playerHandTransform : aiHandTransform[model.players.IndexOf(player) - 1];
            GameObject card = Instantiate(cardPrefab, hand, false);

            //DRAW CARDS
            CardDisplay cardDisplay = card.GetComponentInChildren<CardDisplay>();
            cardDisplay.SetCard(drawnCard, player); //ONLY FOR HUMAN

            //FOR AI PLAYERS
            if (player.IsHuman)
            {
                //SHOW BACK OF CARD
                cardDisplay.ShowCard();
            }
            //SEE IF WE HAVE A PLAYABLE CARD - IF NOT SWITCH PLAYER
            if (!CanPlayAnyCard() && player.IsHuman)
            {
                Debug.Log("No Playable Card, Go next player");
                SwitchPlayer();
                //MESSAGE TO PLAYER
            }
        }
    }

    public void SwitchPlayer(bool skipTurn = false)
    {
        model.myTurn = false;
        int numberOfPlayer = model.players.Count;

        //玩家手牌只有1张，如果不点Uno，则抽两张牌
        if (model.players[model.currentPlayer].playerHand.Count == 1 && !model.unoCalled)
        {
            //LET PLAYER KNOW FORGOT TO CALL UNO
            UpdateMessageBox("YOU FORGOT TO CALL UNO, DRAW 2 CARDS");
            //DRAW 2 CARDS
            for (int i = 0; i < 2; i++)
            {
                DrawCardFromDeck();
            }
            SwitchPlayer();
            return;
        }
        //CHECK WIN CONDITION
        if (CheckWinCondition())
        {
            winPanel.SetActive(true);
            winningText.text = model.players[model.currentPlayer].playerName + " WINS";
            UpdateMessageBox(model.players[model.currentPlayer].playerName + " WINS");
            //END GAME
            Debug.Log(model.players[model.currentPlayer].playerName + " WINS");
            return;
        }

        if (skipTurn)
        {
            model.currentPlayer = (model.currentPlayer + 2 * model.playDirection + numberOfPlayer) % numberOfPlayer;
        }
        else
        {
            model.currentPlayer = (model.currentPlayer + model.playDirection + numberOfPlayer) % numberOfPlayer;
        }

        if (model.isHost)
        {
            MsgSwitchPlayer msg = new MsgSwitchPlayer();
            msg.playerIdx = model.currentPlayer;
            NetManager.Send(msg);
        }

        //UPDATE CARD AMOUNT AND HIGHLIGHT PLAYER
        UpdatePlayerHighlights();

        //RESET UNO CALLED
        model.unoCalled = false;
        if (model.players[model.currentPlayer].IsHuman)
        {
            UpdateMessageBox(model.players[model.currentPlayer].playerName + " TURN");
            model.myTurn = model.isHost == model.players[model.currentPlayer].IsHost;
        }
        else //AI PLAYER
        {
            Debug.Log(model.players[model.currentPlayer].playerName + " AI TURN");
            StartCoroutine(HandleAiTurn());
        }
    }

    public bool CanPlayAnyCard()
    {
        foreach (Card card in model.players[model.currentPlayer].playerHand)
        {
            if (IsPlayable(card))
            {
                return true;
            }
        }
        return false;
    }

    bool IsPlayable(Card card)
    {
        return card.cardColour == model.topColour ||
                 card.cardValue == model.topCard.MyCard.cardValue ||
                    card.cardColour == CardColour.NONE;
    }

    //APPLY SPECIAL CARD EFFECTS
    void ApplyCardEffects(Card playedCard, bool skipTurn = false)
    {
        switch (playedCard.cardValue)
        {
            case CardValue.PLUS_TWO:
                MakeNextPlayerDrawCards(2);
                break;
            case CardValue.PLUS_FOUR:
                ChooseNewColour();
                MakeNextPlayerDrawCards(4);
                break;
            case CardValue.SKIP:
                // SkipPlayer();
                break;
            case CardValue.REVERSE:
                ReversePlayOrder();
                break;
            case CardValue.WILD:
                ChooseNewColour();
                break;
            default:
                break;
        }
    }

    //CHECK WIN CONDITION
    bool CheckWinCondition()
    {
        if (model.players[model.currentPlayer].playerHand.Count == 0)
        {
            return true;
        }
        return false;
    }

    //END GAME

    //SKIP NEXT PLAYER
    void SkipPlayer()
    {
        SwitchPlayer(true); //SKIP TURN
        //FEEDBACK TO HUMAN PLAYER
    }

    //REVERSE PLAY ORDER
    void ReversePlayOrder()
    {
        model.playDirection *= -1;
        //VISUALIZE THE EFFECT BY THE ARROW
        Vector3 scale = directionArrow.localScale;
        scale.x = model.playDirection;
        directionArrow.localScale = scale;
        //SWITCH TURN TO NEXT PLAYER
    }

    //MAKE NEXT PLAYER DRAW 2/DRAW 4/DRAW 1 CARDS
    void MakeNextPlayerDrawCards(int cardAmount)
    {
        int numberOfPlayer = model.players.Count;
        int nextPlayer = (model.currentPlayer + model.playDirection + numberOfPlayer) % numberOfPlayer;
        Player player = model.players[nextPlayer];
        for (int i = 0; i < cardAmount; i++)
        {
            Card drawnCard = deck.DrawCard();

            if (drawnCard != null)
            {
                player.DrawCard(drawnCard);

                //VISUALISE CARDS
                Transform hand = player.IsHuman && player.IsHost == model.isHost? playerHandTransform : aiHandTransform[Mathf.Max(model.players.IndexOf(player) - 1, 0)];
                GameObject card = Instantiate(cardPrefab, hand, false);

                //DRAW CARDS
                CardDisplay cardDisplay = card.GetComponentInChildren<CardDisplay>();
                cardDisplay.SetCard(drawnCard, player); //ONLY FOR HUMAN

                if (player.IsHuman && player.IsHost == model.isHost)
                {
                    //SHOW BACK OF CARD
                    cardDisplay.ShowCard();
                }
            }
        }
        //MESSAGE TO PLAYER
        UpdateMessageBox(player.playerName + " DRAWS " + cardAmount + " CARDS");
    }

    //CHOSE NEXT COLOUR
    IEnumerator ChooseNewColourCoroutine()
    {
        UpdateMessageBox("CHOOSE NEW COLOUR");
        wildPanel.SetActive(true);
        yield return StartCoroutine(Delay()); // Now it properly delays
    }

    void ChooseNewColour()
    {
        if (model.players[model.currentPlayer].IsHuman)
        {
            StartCoroutine(ChooseNewColourCoroutine()); // Start coroutine instead
            return;
        }
        else
        {
            // AI PLAYER logic
        }
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(1f);
    }

    public (Color32 red, Color32 blue, Color32 green, Color32 yellow, Color32 black) GetColours()
    {
        return (red, blue, green, yellow, black);
    }

    //UPDATE ARROW COLOUR
    void TintArrow()
    {
        switch (model.topColour)
        {
            case CardColour.RED:
                directionArrow.GetComponent<Image>().color = red;
                break;
            case CardColour.BLUE:
                directionArrow.GetComponent<Image>().color = blue;
                break;
            case CardColour.GREEN:
                directionArrow.GetComponent<Image>().color = green;
                break;
            case CardColour.YELLOW:
                directionArrow.GetComponent<Image>().color = yellow;
                break;
            default:
                directionArrow.GetComponent<Image>().color = black;
                break;
        }
    }

    public void ChosenColour(CardColour newColour)
    {
        model.topColour = newColour;
        TintArrow();
        wildPanel.SetActive(false);
        if (model.players[model.currentPlayer].IsHuman)
        {
            SwitchPlayer();
        }
    }

    //AI TURN
    IEnumerator HandleAiTurn()
    {
        UpdateMessageBox(model.players[model.currentPlayer].playerName + " TURN");
        yield return new WaitForSeconds(2f);

        model.players[model.currentPlayer].TakeTurn(model.topCard.MyCard, model.topColour);

        //SwitchPlayer();
    }

    //GET NEXT PLAYER HAND SIZE
    public int GetNextPlayerHandSize()
    {
        int numberOfPlayer = model.players.Count;
        int nextPlayer = (model.currentPlayer + model.playDirection + model.players.Count) % model.players.Count;
        int nextPlayerHandSize = model.players[nextPlayer].playerHand.Count;
        return nextPlayerHandSize;
    }

    //UNO BUTTON , 手牌为2的时候可以点击
    public void UnoButton()
    {
        if (model.players[model.currentPlayer].playerHand.Count == 2) //Count == 1 && !unoCalled ?
        {
            model.unoCalled = true;
            //FEEDBACK TO PLAYER
            UpdateMessageBox("UNO BUTTON CLICKED");
        }
        else
        {
            //PENALTY
            UpdateMessageBox("UNO BUTTON CLICKED BUT INCORRECTLY");
        }
    }

    public void SetUnoByAi()
    {
        UpdateMessageBox(model.players[model.currentPlayer].playerName + " HAS CALLED UNO");
        model.unoCalled = true;
    }

    public void UpdatePlayerHighlights()
    {
        for (int i = 0; i < model.players.Count; i++)
        {
            if (i == model.currentPlayer)
            {
                playerHighlights[i].color = yellow;
            }
            else
            {
                playerHighlights[i].color = Color.white;
            }

            //TEXT CARD AMOUNT
            playerCardCount[i].text = model.players[i].playerHand.Count.ToString();
        }
    }

    public void UpdateMessageBox(string message)
    {
        messageText.text = message;
    }

    public void AiSwitchPlayer()
    {
        StartCoroutine(SwitchPlayerDelayed());
    }

    IEnumerator SwitchPlayerDelayed()
    {
        yield return new WaitForSeconds(3f);
        SwitchPlayer();
    }

    public IArchitecture GetArchitecture()
    {
        return CardGameApp.Interface;
    }
}