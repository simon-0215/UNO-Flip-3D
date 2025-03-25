using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetworkGame.TCPServer
{
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
        NINE, // 0到9

        SKIP,
        REVERSE,
        PLUS_TWO, //加2
        WILD,
        PLUS_FOUR //加4
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
}
