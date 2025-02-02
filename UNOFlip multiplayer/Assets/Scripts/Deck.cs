using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Mirror;

public class Deck : NetworkBehaviour, IPointerClickHandler
{
    List<Card> cardDeck = new List<Card>();
    List<Card> usedCardDeck = new List<Card>();

    // Start is called before the first frame update
    // void Start()
    // {
    //     InitializeDeck();
    // }

    public void InitializeDeck()
    {
        cardDeck.Clear(); //EMPTY THE DECK

        foreach (CardColour color in System.Enum.GetValues(typeof(CardColour)))
        {
            foreach (CardValue cardValue in System.Enum.GetValues(typeof(CardValue)))
            {
                if (color != CardColour.NONE && cardValue != CardValue.WILD && cardValue != CardValue.PLUS_FOUR)
                {
                    cardDeck.Add(new Card(color, cardValue));
                    cardDeck.Add(new Card(color, cardValue)); //ADD TWO OF EACH CARD
                }
            }
        }
        //SPECIAL CARDS
        for (int i = 0; i < 4; i++)
        {
            cardDeck.Add(new Card(CardColour.NONE, CardValue.WILD));
            cardDeck.Add(new Card(CardColour.NONE, CardValue.PLUS_FOUR));
        }

        ShuffleDeck();
    }

    public void ShuffleDeck()
    {
        for (int i = 0; i < cardDeck.Count; i++)
        {
            Card temp = cardDeck[i];
            int randomIndex = Random.Range(0, cardDeck.Count);
            cardDeck[i] = cardDeck[randomIndex];
            cardDeck[randomIndex] = temp;
        }
    }
    [Command] // Ensures only the server runs this function
    void CmdDrawCard(Player player)
    {
        Card drawnCard = DrawCard(); // Server draws a card

        if (drawnCard != null)
        {
            player.RpcDrawCard(drawnCard); // Notify the client about the drawn card
        }
    }


    public Card DrawCard()
    {
        if (cardDeck.Count == 0)
        {
            //DECK IS EMPTY SHUFFLE IN USED CARDS
            cardDeck.AddRange(usedCardDeck);
            usedCardDeck.Clear();
            ShuffleDeck();
            
            if (usedCardDeck.Count == 0)
            {
                return null;
            }
            return null;
        }
        
            Card drawnCard = cardDeck[0];
            cardDeck.RemoveAt(0);
            return drawnCard;
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(GameManager.instance.humanHasTurn && !GameManager.instance.CanPlayAnyCard())
        {
            GameManager.instance.DrawCardFromDeck();
        }
    }

    //ADD USED CARDS TO USED DECK
    public void AddUsedCard(Card card)
    {
        usedCardDeck.Add(card);
    }
}