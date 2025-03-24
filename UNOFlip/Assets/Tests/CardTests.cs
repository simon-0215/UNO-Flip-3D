using NUnit.Framework;

[TestFixture]
public class CardTests
{
    [Test]
    public void Test_Card_Creation()
    {
        Card card = new Card(CardColour.RED, CardValue.ONE);
        
        Assert.AreEqual(CardColour.RED, card.cardColour);
        Assert.AreEqual(CardValue.ONE, card.cardValue);
    }

    [Test]
    public void Test_Card_WildCard_Creation()
    {
        Card card = new Card(CardColour.NONE, CardValue.WILD);
        
        Assert.AreEqual(CardColour.NONE, card.cardColour);
        Assert.AreEqual(CardValue.WILD, card.cardValue);
    }

    [Test]
    public void Test_Card_PlusFourCard_Creation()
    {
        Card card = new Card(CardColour.NONE, CardValue.PLUS_FOUR);
        
        Assert.AreEqual(CardColour.NONE, card.cardColour);
        Assert.AreEqual(CardValue.PLUS_FOUR, card.cardValue);
    }

    [Test]
    public void Test_Card_ActionCard_Creation()
    {
        Card card = new Card(CardColour.BLUE, CardValue.SKIP);
        
        Assert.AreEqual(CardColour.BLUE, card.cardColour);
        Assert.AreEqual(CardValue.SKIP, card.cardValue);
    }

    [Test]
    public void Test_Card_Equality()
    {
        Card card1 = new Card(CardColour.RED, CardValue.ONE);
        Card card2 = new Card(CardColour.RED, CardValue.ONE);
        Card card3 = new Card(CardColour.BLUE, CardValue.ONE);
        
        Assert.AreEqual(card1.cardColour, card2.cardColour);
        Assert.AreEqual(card1.cardValue, card2.cardValue);
        Assert.AreNotEqual(card1.cardColour, card3.cardColour);
    }

    [Test]
    public void Test_Card_NumberCard_Values()
    {
        Card card1 = new Card(CardColour.RED, CardValue.ZERO);
        Card card2 = new Card(CardColour.RED, CardValue.ONE);
        Card card3 = new Card(CardColour.RED, CardValue.TWO);
        Card card4 = new Card(CardColour.RED, CardValue.THREE);
        Card card5 = new Card(CardColour.RED, CardValue.FOUR);
        Card card6 = new Card(CardColour.RED, CardValue.FIVE);
        Card card7 = new Card(CardColour.RED, CardValue.SIX);
        Card card8 = new Card(CardColour.RED, CardValue.SEVEN);
        Card card9 = new Card(CardColour.RED, CardValue.EIGHT);
        Card card10 = new Card(CardColour.RED, CardValue.NINE);

        Assert.AreEqual(CardValue.ZERO, card1.cardValue);
        Assert.AreEqual(CardValue.ONE, card2.cardValue);
        Assert.AreEqual(CardValue.TWO, card3.cardValue);
        Assert.AreEqual(CardValue.THREE, card4.cardValue);
        Assert.AreEqual(CardValue.FOUR, card5.cardValue);
        Assert.AreEqual(CardValue.FIVE, card6.cardValue);
        Assert.AreEqual(CardValue.SIX, card7.cardValue);
        Assert.AreEqual(CardValue.SEVEN, card8.cardValue);
        Assert.AreEqual(CardValue.EIGHT, card9.cardValue);
        Assert.AreEqual(CardValue.NINE, card10.cardValue);
    }

    [Test]
    public void Test_Card_AllColors()
    {
        Card redCard = new Card(CardColour.RED, CardValue.ONE);
        Card blueCard = new Card(CardColour.BLUE, CardValue.ONE);
        Card greenCard = new Card(CardColour.GREEN, CardValue.ONE);
        Card yellowCard = new Card(CardColour.YELLOW, CardValue.ONE);
        Card wildCard = new Card(CardColour.NONE, CardValue.WILD);

        Assert.AreEqual(CardColour.RED, redCard.cardColour);
        Assert.AreEqual(CardColour.BLUE, blueCard.cardColour);
        Assert.AreEqual(CardColour.GREEN, greenCard.cardColour);
        Assert.AreEqual(CardColour.YELLOW, yellowCard.cardColour);
        Assert.AreEqual(CardColour.NONE, wildCard.cardColour);
    }

    [Test]
    public void Test_Card_AllActionValues()
    {
        Card skipCard = new Card(CardColour.RED, CardValue.SKIP);
        Card reverseCard = new Card(CardColour.RED, CardValue.REVERSE);
        Card plusTwoCard = new Card(CardColour.RED, CardValue.PLUS_TWO);
        Card wildCard = new Card(CardColour.NONE, CardValue.WILD);
        Card plusFourCard = new Card(CardColour.NONE, CardValue.PLUS_FOUR);

        Assert.AreEqual(CardValue.SKIP, skipCard.cardValue);
        Assert.AreEqual(CardValue.REVERSE, reverseCard.cardValue);
        Assert.AreEqual(CardValue.PLUS_TWO, plusTwoCard.cardValue);
        Assert.AreEqual(CardValue.WILD, wildCard.cardValue);
        Assert.AreEqual(CardValue.PLUS_FOUR, plusFourCard.cardValue);
    }
} 