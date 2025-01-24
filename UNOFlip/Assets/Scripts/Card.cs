using UnityEngine.UI;
using UnityEngine;
using TMPro;

public enum CardColour
    {
        RED,
        BLUE,
        GREEN,
        YELLOW,
        BLACK
    }

    public enum CardValue
    {
        ZERO,
        ONE,
        TWO,
        THREE,
        FOUR,
        FIVE,
        SIX,
        SEVEN,
        EIGHT,
        NINE,
        SKIP,
        REVERSE,
        PLUS_TWO,
        WILD,
        PLUS_FOUR
    }

public class Card
{
    
    public CardColour colour;
    public CardValue CardValue;    

    public Card(CardColour colour, CardValue value)
    {
        cardColour = colour;
        cardValue = value;
    }
}