using UnityEngine.UI;
using UnityEngine;
using TMPro;

public enum FlippedCardColour
    {
        RED,
        BLUE,
        GREEN,
        YELLOW,
        NONE
    }

    public enum FlippedCardValue
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
public class FlippedCard
{
    
    public FlippedCardColour cardColour;
    public FlippedCardValue cardValue;    

    public FlippedCard(FlippedCardColour colour, FlippedCardValue value)
    {
        this.cardColour = colour;
        this.cardValue = value;
    }
}
