using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Deck : MonoBehaviour, IPointerClickHandler
{
    List<Card> cardDeck = new List<Card>();
    List<Card> usedCardDeck = new List<Card>();

    public void InitializeDeck()
    {
        cardDeck.Clear(); // EMPTY THE DECK

        //HANDLE FLIPPED CARD INITIALIZATION NEEDS TO BE ADDDED
        // Generate UNO Flip deck with Light and Dark side properties
        foreach (LightCardColour lightColour in System.Enum.GetValues(typeof(LightCardColour)))
        {
            foreach (LightCardValue lightValue in System.Enum.GetValues(typeof(LightCardValue)))
            {
                if (lightColour != LightCardColour.NONE && lightValue != LightCardValue.WILD && lightValue != LightCardValue.PLUS_FOUR)
                {
                    // Assign random corresponding dark side properties
                    DarkCardColour darkColour = GetRandomDarkColour();
                    DarkCardValue darkValue = GetRandomDarkValue();

                    cardDeck.Add(new Card(lightColour, lightValue, darkColour, darkValue));
                    cardDeck.Add(new Card(lightColour, lightValue, darkColour, darkValue)); // ADD TWO OF EACH CARD
                }
            }
        }

        // Special Cards (Wild & Plus Four)
        for (int i = 0; i < 4; i++)
        {
            cardDeck.Add(new Card(LightCardColour.NONE, LightCardValue.WILD, DarkCardColour.NONE, DarkCardValue.WILD_DRAW_COLOR));
            cardDeck.Add(new Card(LightCardColour.NONE, LightCardValue.PLUS_FOUR, GetRandomDarkColour(), DarkCardValue.DRAW_FIVE));
            // cardDeck.Add(new Card(LightCardColour.NONE, LightCardValue.SKIP, GetRandomDarkColour(), DarkCardValue.SKIP_EVERYONE));
            // cardDeck.Add(new Card(LightCardColour.))
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

    public Card DrawCard()
    {
        if (cardDeck.Count == 0)
        {
            // DECK IS EMPTY -> SHUFFLE USED CARDS BACK IN
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
        if (GameManager.instance.humanHasTurn && !GameManager.instance.CanPlayAnyCard())
        {
            GameManager.instance.DrawCardFromDeck();
        }
    }

    // ADD USED CARDS TO DISCARD PILE
    public void AddUsedCard(Card card)
    {
        usedCardDeck.Add(card);
    }

    // Utility function to get a random dark side colour
    private DarkCardColour GetRandomDarkColour()
    {
        DarkCardColour[] darkColours = (DarkCardColour[])System.Enum.GetValues(typeof(DarkCardColour));
        return darkColours[Random.Range(0, darkColours.Length)];
    }

    // Utility function to get a random dark side value
    private DarkCardValue GetRandomDarkValue()
    {
        DarkCardValue[] darkValues = (DarkCardValue[])System.Enum.GetValues(typeof(DarkCardValue));
        return darkValues[Random.Range(0, darkValues.Length)];
    }
}
