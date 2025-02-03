using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

    public List<Player> players = new List<Player>();
    
    [SerializeField] Deck deck;

    [SerializeField] Transform playerHandTransform; //HOLDS PLAYER HAND
    [SerializeField] List<Transform> aiHandTransform = new List<Transform>(); //HOLDS AI HANDS
    [SerializeField] GameObject cardPrefab;
    [SerializeField] int numberOfAiPlayers = 2;
    [SerializeField] int startingHandSize = 7;
    //int currentPlayer = 0;
    [SyncVar] private int currentPlayer = 0;
    int playDirection = 1; //1 //-1
    [Header("Game Play")]
    [SerializeField] Transform discardPileTransform;
    [SerializeField] CardDisplay topCard;
    [SerializeField] Transform directionArrow;
    [SerializeField] GameObject wildPanel;
    [SerializeField] WildButton redButton;
    [SerializeField] WildButton blueButton;
    [SerializeField] WildButton greenButton;
    [SerializeField] WildButton yellowButton;
    CardColour topColour = CardColour.NONE;

    bool unoCalled;

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

    public bool humanHasTurn{get; private set;} 


    void Awake()
    {
        instance = this;

    }

    void Start()
    {
        //HIDE WILD PANEL
        wildPanel.SetActive(false);
        redButton.SetImageColour(red);
        blueButton.SetImageColour(blue);
        greenButton.SetImageColour(green);
        yellowButton.SetImageColour(yellow);
        winPanel.SetActive(false);

        //INITIALIZE DECK
        deck.InitializeDeck();

        //INITIALIZE PLAYERS
        InitializePlayers();

        //DEAL CARDS
        StartCoroutine(DealStartingCards());

        //START GAME
        UpdateMessageBox("Welcome to UNO!");
    }

    void InitializePlayers()
    {
        players.Clear();
        players.Add(new Player("Player",true));
        players.Add(new Player("Player 2",true));

        for (int i = 0; i < numberOfAiPlayers; i++)
        {
            players.Add(new AiPlayer("AI" + (i+1)));
        }
    }
    public void OnStartServer()
    {
        base.OnStartServer();
        players.Clear();

        foreach (var player in FindObjectsByType<Player>(FindObjectsInactive.Include, FindObjectsSortMode.None)
)
        {
            players.Add(player);
        }

        StartCoroutine(DealStartingCards());
    }

    void DealCards()
    {
        for (int i = 0; i < startingHandSize; i++)
        {
            foreach (Player player in players)
            {
                player.DrawCard(deck.DrawCard());
            }
        }

        //DISPLAY CARDS
        StartCoroutine(DealStartingCards());

        //DISPLAY AI HAND  
    }
    // [ClientRpc]
    // void RpcMoveCardToPile(Card card)
    // {
    //     GameObject newCard = Instantiate(cardPrefab, discardPileTransform);
    //     CardDisplay display = newCard.GetComponentInChildren<CardDisplay>();
    //     display.SetCard(card, null);
    // }


    IEnumerator DealStartingCards()
    {
        for (int i = 0; i < startingHandSize; i++)
        {
            foreach (Player player in players)
            {
                Card drawnCard = deck.DrawCard();
                player.DrawCard(drawnCard);

                //VISUALISE CARDS
                Transform hand = player.IsHuman ? playerHandTransform : aiHandTransform[players.IndexOf(player)-1];
                GameObject card = Instantiate(cardPrefab, hand, false);

                //DRAW CARDS
                CardDisplay cardDisplay =  card.GetComponentInChildren<CardDisplay>();
                cardDisplay.SetCard(drawnCard, player); //ONLY FOR HUMAN

                //FOR AI PLAYERS
                if (player.IsHuman)
                {
                    //SHOW BACK OF CARD
                    cardDisplay.ShowCard();
                }

                yield return new WaitForSeconds(0.1f);
            }
        }

        //HAND OUT TOP CARD
        Card pileCard = deck.DrawCard();
        GameObject newCard = Instantiate(cardPrefab);
        MoveCardToPile(newCard);
        CardDisplay display = newCard.GetComponentInChildren<CardDisplay>();
        display.SetCard(pileCard,null);
        display.ShowCard();
        newCard.GetComponentInChildren<CardInteraction>().enabled = false;
        deck.AddUsedCard(pileCard);
        //SET TOP CARD
        topCard = display;
        topColour = pileCard.cardColour;
        //PICK RANDOM COLOUR IF WE HAVE WILD CARD
        if(topColour == CardColour.NONE)
        {
            topColour = PickRandomColour();
        }
        TintArrow();
        //START GAME
        Debug.Log("Game Started");
        UpdateMessageBox("PLAYER 1, IT'S YOUR TURN");
        humanHasTurn = true;
        UpdatePlayerHighlights();
    }

    CardColour PickRandomColour()
    {
        CardColour[] colours = (CardColour[])System.Enum.GetValues(typeof(CardColour));
        int randomIndex = UnityEngine.Random.Range(0, colours.Length - 1);
        return colours[randomIndex];
    }
    
    // public void CmdPlayCard(Player player, Card card)
    // {
    //     if (players[currentPlayer] == player && IsPlayable(card))
    //     {
    //         RpcMoveCardToPile(card);
    //         deck.AddUsedCard(card);
    //         OnCardPlayed(card);
    //         SwitchPlayer();
    //     }
    // }

    [Command]
    public void PlayCardWithDisplay(CardDisplay cardDisplay)
    {
        PlayCardInternal(cardDisplay, cardDisplay.MyCard);
    }

    [Command]
    public void PlayCardWithCard(Card card)
    {
        CardDisplay cardDisplay = FindCardDisplayForCard(card);
        PlayCardInternal(cardDisplay, card);
    }

    private void PlayCardInternal(CardDisplay cardDisplay, Card card)
    {
        if (cardDisplay == null || card == null)
        {
            Debug.Log("Invalid card or card display.");
            return;
        }

        // CHECK IF CARD CAN BE PLAYED
        if (!IsPlayable(card))
        {
            Debug.Log("Card cannot be played");
            return;
        }

        // REMOVE CARD FROM PLAYER HAND
        players[currentPlayer].PlayCard(card);

        // MOVE THE CARD TO DISCARD PILE
        MoveCardToPile(cardDisplay.transform.parent.gameObject);

        // UPDATE TOP CARD
        topCard = cardDisplay;
        topColour = card.cardColour;
        TintArrow();

        // IMPLEMENT WHAT SHOULD HAPPEN WHEN CARD IS PLAYED
        OnCardPlayed(card);

        // UNHIDE CARD
        cardDisplay.ShowCard();

        // DEACTIVATE CARD INTERACTION
        cardDisplay.GetComponentInChildren<CardInteraction>().enabled = false;

        // ADD THE CARD BACK TO USED CARDS DECK
        deck.AddUsedCard(card);

        // SWITCH PLAYER OR SKIP
        if (card.cardValue != CardValue.SKIP)
        {
            SwitchPlayer();
        }
        else
        {
            SkipPlayer();
        }
    }

    //FIND CARDISPLAY
    CardDisplay FindCardDisplayForCard(Card card)
    {
        Player player = players[currentPlayer];
        Transform hand = player.IsHuman ? playerHandTransform : aiHandTransform[players.IndexOf(player)-1];
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

    [Command]
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
        Quaternion randomRotation = Quaternion.Euler(0,0,randomZRotation);
        currentCard.transform.rotation = rotation * randomRotation;
    }
    void OnCardPlayed(Card playedCard)
    {

        ApplyCardEffects(playedCard);
        //DO ALL EFFECTS NEEDED
        // if(playedCard.cardValue != CardValue.SKIP)
        // {
        //     return;
        //     //SwitchPlayer();
        // }
        // if(playedCard.cardColour == CardColour.NONE && players[currentPlayer].IsHuman)
        // {
        //     return;
        // }
        // if(players[currentPlayer].IsHuman)
        // {
        //     SkipPlayer();
        // }
    }

    public Card DrawCardFromDeck()
    {
        Card drawnCard = deck.DrawCard(); // 服务器从牌堆抽卡
        Player player = players[currentPlayer];

        if (drawnCard != null)
        {
            player.DrawCard(drawnCard);

            // 显示卡片
            Transform hand = player.IsHuman ? playerHandTransform : aiHandTransform[players.IndexOf(player) - 1];
            GameObject card = Instantiate(cardPrefab, hand, false);

            // 设置卡片信息
            CardDisplay cardDisplay = card.GetComponentInChildren<CardDisplay>();
            cardDisplay.SetCard(drawnCard, player);

            // AI 处理
            if (player.IsHuman)
            {
                cardDisplay.ShowCard();
            }

            // 检查是否可以出牌，如果不能出牌，换下一个玩家
            if (!CanPlayAnyCard() && player.IsHuman)
            {
                Debug.Log("No Playable Card, Go next player");
                SwitchPlayer();
            }
        }

        return drawnCard; // ✅ 返回抽到的卡
    }


      // Ensures turn sync across all clients

    [Server] // Ensures this function is only executed on the server
    public void SwitchPlayer(bool skipTurn = false)
    {
        // if
        // humanHasTurn = false;
        int numberOfPlayer = players.Count;

        if(players[currentPlayer].playerHand.Count == 1 && !unoCalled)
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
        if(CheckWinCondition())
        {
            winPanel.SetActive(true);
            winningText.text = players[currentPlayer].playerName + " WINS";
            UpdateMessageBox(players[currentPlayer].playerName + " WINS");
            //END GAME
            Debug.Log(players[currentPlayer].playerName + " WINS");
            return;
        }
        
        if(skipTurn)
        {
            currentPlayer = (currentPlayer + 2 * playDirection + numberOfPlayer) % numberOfPlayer;
        }
        else
        {
            currentPlayer = (currentPlayer + playDirection + numberOfPlayer) % numberOfPlayer;
        }
        //UPDATE CARD AMOUNT AND HIGHLIGHT PLAYER
        UpdatePlayerHighlights();

        //RESET UNO CALLED
        unoCalled = false;
        if (players[currentPlayer].IsHuman)
        {
            UpdateMessageBox(players[currentPlayer].playerName + " TURN");
            humanHasTurn = true;
        }
        else //AI PLAYER
        {
            Debug.Log(players[currentPlayer].playerName + " AI TURN");
            StartCoroutine(HandleAiTurn());
        }
        RpcUpdateCurrentPlayer(currentPlayer);
    }

    // This updates all clients with the new currentPlayer value
    [ClientRpc]
    void RpcUpdateCurrentPlayer(int newPlayerIndex)
    {
        currentPlayer = newPlayerIndex;
    }


    public bool CanPlayAnyCard()
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

    bool IsPlayable(Card card)
    {
        return card.cardColour == topColour ||
                 card.cardValue == topCard.MyCard.cardValue ||
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
        if(players[currentPlayer].playerHand.Count == 0)
        {
            return true;
        }
        return false;
    }

    //END GAME

    //SKIP NEXT PLAYER
    void SkipPlayer()
    {
        SwitchPlayer(); 
    }

    //REVERSE PLAY ORDER
    void ReversePlayOrder()
    {
        playDirection *= -1;
        //VISUALIZE THE EFFECT BY THE ARROW
        Vector3 scale = directionArrow.localScale;
        scale.x = playDirection;
        directionArrow.localScale = scale;
        //SWITCH TURN TO NEXT PLAYER
    }

    //MAKE NEXT PLAYER DRAW 2/DRAW 4/DRAW 1 CARDS
    [ClientRpc]
    void MakeNextPlayerDrawCards(int cardAmount)
    {
        int numberOfPlayer = players.Count;
        int nextPlayer = (currentPlayer + playDirection + numberOfPlayer) % numberOfPlayer;
        Player player = players[nextPlayer];
        for (int i = 0; i < cardAmount; i++)
        {
            Card drawnCard = deck.DrawCard();

            if(drawnCard != null)
            {
                player.DrawCard(drawnCard);

                //VISUALISE CARDS
                Transform hand = player.IsHuman ? playerHandTransform : aiHandTransform[Mathf.Max(players.IndexOf(player) - 1, 0)];
                GameObject card = Instantiate(cardPrefab, hand, false);

                //DRAW CARDS
                CardDisplay cardDisplay =  card.GetComponentInChildren<CardDisplay>();
                cardDisplay.SetCard(drawnCard, player); //ONLY FOR HUMAN


                if (player.IsHuman)
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
        if (players[currentPlayer].IsHuman)
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
    [ClientRpc]
    void TintArrow()
    {
        switch (topColour)
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
        topColour = newColour;
        TintArrow();
        wildPanel.SetActive(false);
        if(players[currentPlayer].IsHuman)
        {
            SwitchPlayer();
        }
    }

    //AI TURN
    IEnumerator HandleAiTurn()
    {
        UpdateMessageBox(players[currentPlayer].playerName + " TURN");
        yield return new WaitForSeconds(2f);
        
        players[currentPlayer].TakeTurn(topCard.MyCard, topColour);

        //SwitchPlayer();
    }

    //GET NEXT PLAYER HAND SIZE
    public int GetNextPlayerHandSize()
    {
        int numberOfPlayer = players.Count;
        int nextPlayer = (currentPlayer + playDirection + players.Count) % players.Count;
        int nextPlayerHandSize = players[nextPlayer].playerHand.Count;
        return nextPlayerHandSize;
    }

    //UNO BUTTON
    public void UnoButton()
    {
        if (players[currentPlayer].playerHand.Count == 2) //Count == 1 && !unoCalled ?
        {
            unoCalled = true;
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
        UpdateMessageBox(players[currentPlayer].playerName + " HAS CALLED UNO");
        unoCalled = true;
    }

    [ClientRpc]
    public void UpdatePlayerHighlights()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (i == currentPlayer)
            {
                playerHighlights[i].color = yellow;
            }
            else
            {
                playerHighlights[i].color = Color.white;
            }

            //TEXT CARD AMOUNT
            playerCardCount[i].text = players[i].playerHand.Count.ToString();
        }
    }

    [ClientRpc]
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
}