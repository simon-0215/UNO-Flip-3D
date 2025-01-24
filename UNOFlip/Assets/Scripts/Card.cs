using UnityEngine.UI;
using UnityEngine;
using TMPro;

public enum CardColour
    {
        RED,
        BLUE,
        GREEN,
        YELLOW,
        NONE
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

[System.Serializable]
public class Card
{
    
    public CardColour colour;
    public CardValue CardValue;    

    public Card(CardColour colour, CardValue value)
    {
        this.colour = colour;
        this.CardValue = value;
    }
}