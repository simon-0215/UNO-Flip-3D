using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<Player> players = new List<Player>();
    [SerializeField] Deck deck;

    [SerializeField] Transform playerHandTransform; //HOLDS PLAYER HAND
    [SerializeField] List<Transform> aiHandTransform = new List<Transform>(); //HOLDS AI HANDS
    [SerializeField] GameObject cardPrefab;
    [SerializeField] int numberOfAiPlayers = 3;
    [SerializeField] int startingHandSize = 7;
    int currentPlayer = 0;
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

        //INITIALIZE DECK
        deck.InitializeDeck();

        //INITIALIZE PLAYERS
        InitializePlayers();

        //DEAL CARDS
        StartCoroutine(DealStartingCards());

        //START GAME
    }

    void InitializePlayers()
    {
        players.Clear();
        players.Add(new Player("Player",true));

        for (int i = 0; i < numberOfAiPlayers; i++)
        {
            players.Add(new AiPlayer("AI" + (i+1)));
        }
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
        TintArrow();
        //START GAME
        Debug.Log("Game Started");
        humanHasTurn = true;
    }

    public void PlayCard(CardDisplay cardDisplay)
    {
        Card cardToPlay = cardDisplay.MyCard;
        //CHECK IF CARD CAN BE PLAYED
        if (!IsPlayable(cardDisplay.MyCard))
        {
            Debug.Log("Card cannot be played");
            return;
        }
        //REMOVE CARD FROM PLAYER HAND
        players[currentPlayer].PlayCard(cardToPlay);
        //UNHIDE THE CARD IF AI PLAYER

        //MOVE THE CARD TO DISCARD PILE
        MoveCardToPile(cardDisplay.transform.parent.gameObject);
        //UPDATE TOP CARD
        topCard = cardDisplay;
        topColour = cardToPlay.cardColour;
        TintArrow();
        //IMPLEMENT WHAT SHOULD HAPPEN WHEN CARD PLAYED
        OnCardPlayed(topCard.MyCard);
        //UNHIDE CARD
        cardDisplay.ShowCard();
        //DEACTIVATE CARD INTERACTION
        cardDisplay.GetComponentInChildren<CardInteraction>().enabled = false;
        //ADD THE CARD BACK TO USED CARDS DECK
        deck.AddUsedCard(cardToPlay);
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
    }
    void OnCardPlayed(Card playedCard)
    {
        //CHECK IF GAME IS OVER
        if(players.Any(p => p.playerHand.Count == 0))
        {
            //END GAME
            //VISUALIZE WINNER
            return;
        }
        ApplyCardEffects(playedCard);
        //DO ALL EFFECTS NEEDED
        if(playedCard.cardValue != CardValue.SKIP)
        {
            return;
        }
        if(playedCard.cardColour == CardColour.NONE && players[currentPlayer].IsHuman)
        {
            return;
        }

        SwitchPlayer();
    }

    public void DrawCardFromDeck()
    {
        Card drawnCard = deck.DrawCard();
        Player player = players[currentPlayer];

        if (drawnCard!=null)
        {
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
            //SEE IF WE HAVE A PLAYABLE CARD - IF NOT SWITCH PLAYER
            if(!CanPlayAnyCard())
            {
                Debug.Log("No Playable Card, Go next player");
                SwitchPlayer();
                //MESSAGE TO PLAYER
            }
        }
    }

    public void SwitchPlayer(bool skipTurn = false)
    {
        humanHasTurn = false;
        int numberOfPlayer = players.Count;

        if(players[currentPlayer].playerHand.Count == 1 && !unoCalled)
        {
            
            //LET PLAYER KNOW FORGOT TO CALL UNO
            //DRAW 2 CARDS
            for (int i = 0; i < 2; i++)
            {
                DrawCardFromDeck();
            }
            return;
        }

        if(skipTurn)
        {
            currentPlayer = (currentPlayer + 2 *playDirection + numberOfPlayer) % numberOfPlayer;
        }
        else
        {
            currentPlayer = (currentPlayer + playDirection + numberOfPlayer) % numberOfPlayer;
        }

        //RESET UNO CALLED
        unoCalled = false;
        if (players[currentPlayer].IsHuman)
        {
            humanHasTurn = true;
        }
        else //AI PLAYER
        {
            StartCoroutine(HandleAiTurn());
        }
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
    void ApplyCardEffects(Card playedCard)
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
                SkipPlayer();
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
        playDirection *= -1;
        //VISUALIZE THE EFFECT BY THE ARROW
        Vector3 scale = directionArrow.localScale;
        scale.x = playDirection;
        directionArrow.localScale = scale;
        //SWITCH TURN TO NEXT PLAYER
    }

    //MAKE NEXT PLAYER DRAW 2/DRAW 4/DRAW 1 CARDS
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
                Transform hand = player.IsHuman ? playerHandTransform : aiHandTransform[players.IndexOf(player)-1];
                GameObject card = Instantiate(cardPrefab, hand, false);

                //DRAW CARDS
                CardDisplay cardDisplay =  card.GetComponentInChildren<CardDisplay>();
                cardDisplay.SetCard(drawnCard, player); //ONLY FOR HUMAN
            }
        }
    }

    //CHOSE NEXT COLOUR
    void ChooseNewColour()
    {
        if(players[currentPlayer].IsHuman)
        {
            wildPanel.SetActive(true);
            return;
        }
        else
        {
            //AI PLAYER
        }
    }

    public (Color32 red, Color32 blue, Color32 green, Color32 yellow, Color32 black) GetColours()
    {
        return (red, blue, green, yellow, black);
    }

    //UPDATE ARROW COLOUR
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
        SwitchPlayer();
    }

    //AI TURN
    IEnumerator HandleAiTurn()
    {
        yield return new WaitForSeconds(1f);
        
        players[currentPlayer].TakeTurn();

        SwitchPlayer();
    }

    //UNO BUTTON
    public void UnoButton()
    {
        if (players[currentPlayer].playerHand.Count == 2) //Count == 1 && !unoCalled ?
        {
            unoCalled = true;
            //FEEDBACK TO PLAYER
        }
        else
        {
            //PENALTY
            Debug.Log("UNO CALLED INCORRECTLY");
        }
    }
}