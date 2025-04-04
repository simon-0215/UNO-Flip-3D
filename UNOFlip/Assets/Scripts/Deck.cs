using QFramework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnoFlipV2;

public class Deck : MonoBehaviour, IPointerClickHandler, IController
{
    List<Card> cardDeck = new List<Card>();
    public List<Card> Cards { get { return cardDeck; } }

    List<Card> usedCardDeck = new List<Card>();

    // Start is called before the first frame update
    // void Start()
    // {
    //     InitializeDeck();
    // }

    CardGameModel model;
    private void Start()
    {
        //model = this.GetModel<CardGameModel>();
    }
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
    public void InitializeDeck(List<ShortCard> shortCards)
    {
        cardDeck.Clear();
        cardDeck.AddRange(shortCards.Select(s => s.GetCard()));
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
        this.SendCommand<DeckClickedCommand>();
    }

    //ADD USED CARDS TO USED DECK
    public void AddUsedCard(Card card)
    {
        usedCardDeck.Add(card);
    }

    public IArchitecture GetArchitecture()
    {
        return UnoFlipAppV2.Interface;
    }
}