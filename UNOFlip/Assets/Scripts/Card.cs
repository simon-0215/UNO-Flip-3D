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
        NINE, // 0µ½9

        SKIP,
        REVERSE,
        PLUS_TWO, //¼Ó2
        WILD,
        PLUS_FOUR //¼Ó4
    }

[System.Serializable]
public class Card
{
    
    public CardColour cardColour;
    public CardValue cardValue;    

    public Card(CardColour colour, CardValue value)
    {
        this.cardColour = colour;
        this.cardValue = value;
    }
    public ShortCard GetShort()
    {
        ShortCard s = new ShortCard();
        s.c = cardColour;
        s.v = cardValue;
        return s;
    }
}

[System.Serializable]
public class ShortCard
{
    public CardColour c;
    public CardValue v;

    public Card GetCard()
    {
        return new Card(c, v);
    }
}