using UnityEngine.UI;
using UnityEngine;
using TMPro;

// Light Side Colors
public enum LightCardColour 
{
    RED, BLUE, GREEN, YELLOW, NONE
}

// Dark Side Colors (NEW)
public enum DarkCardColour
{
    PINK, TEAL, ORANGE, PURPLE, NONE
}

// Light Side Actions
public enum LightCardValue
{
    ZERO, ONE, TWO, THREE, FOUR, FIVE, SIX, SEVEN, EIGHT, NINE,
    SKIP, REVERSE, PLUS_TWO, WILD, PLUS_FOUR
}

// Dark Side Actions (NEW)
public enum DarkCardValue
{
    ZERO, ONE, TWO, THREE, FOUR, FIVE, SIX, SEVEN, EIGHT, NINE,
    DRAW_FIVE, SKIP_EVERYONE, WILD_DRAW_COLOR
}

[System.Serializable]
public class Card
{
    public LightCardColour lightCardColour;  // Light Side Colour
    public LightCardValue lightCardValue;    // Light Side Value
    public DarkCardColour darkCardColour;    // Dark Side Colour
    public DarkCardValue darkCardValue;      // Dark Side Value

    public Card(LightCardColour lightColour, LightCardValue lightValue, DarkCardColour darkColour, DarkCardValue darkValue)
    {
        this.lightCardColour = lightColour;
        this.lightCardValue = lightValue;
        this.darkCardColour = darkColour;
        this.darkCardValue = darkValue;
    }

    // Returns the correct colour based on game state
    public object GetCurrentColour(bool isFlipped)
    {
        return isFlipped ? darkCardColour : lightCardColour;
    }

    // Returns the correct value based on game state
    public object GetCurrentValue(bool isFlipped)
    {
        return isFlipped ? darkCardValue : lightCardValue;
    }
}
