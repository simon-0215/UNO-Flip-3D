using System.Collections.Generic;
using NUnit.Framework;

[TestFixture]
public class PlayerTests
{
    private Player player;

    [SetUp]
    public void SetUp()
    {
        player = new Player("Test Player", true);
    }

    [Test]
    public void Test_Player_Creation()
    {
        Assert.IsNotNull(player);
        Assert.AreEqual("Test Player", player.playerName);
        Assert.IsTrue(player.IsHuman);
        Assert.IsNotNull(player.playerHand);
        Assert.AreEqual(0, player.playerHand.Count);
    }

    [Test]
    public void Test_Player_DrawCard()
    {
        Card testCard = new Card(CardColour.RED, CardValue.ONE);
        player.DrawCard(testCard);

        Assert.AreEqual(1, player.playerHand.Count);
        Assert.Contains(testCard, player.playerHand);
    }

    [Test]
    public void Test_Player_PlayCard()
    {
        Card testCard = new Card(CardColour.RED, CardValue.ONE);
        player.DrawCard(testCard);
        
        Assert.AreEqual(1, player.playerHand.Count);
        player.PlayCard(testCard);
        Assert.AreEqual(0, player.playerHand.Count);
        Assert.IsFalse(player.playerHand.Contains(testCard));
    }

    [Test]
    public void Test_Player_PlayInvalidCard()
    {
        Card testCard = new Card(CardColour.RED, CardValue.ONE);
        Card invalidCard = new Card(CardColour.BLUE, CardValue.TWO);
        
        player.DrawCard(testCard);
        Assert.AreEqual(1, player.playerHand.Count);
        
        // Try to play a card that's not in hand
        player.PlayCard(invalidCard);
        Assert.AreEqual(1, player.playerHand.Count);
        Assert.Contains(testCard, player.playerHand);
    }

    [Test]
    public void Test_Player_DrawMultipleCards()
    {
        List<Card> cards = new List<Card>
        {
            new Card(CardColour.RED, CardValue.ONE),
            new Card(CardColour.BLUE, CardValue.TWO),
            new Card(CardColour.GREEN, CardValue.THREE)
        };

        foreach (Card card in cards)
        {
            player.DrawCard(card);
        }

        Assert.AreEqual(cards.Count, player.playerHand.Count);
        foreach (Card card in cards)
        {
            Assert.Contains(card, player.playerHand);
        }
    }

    [Test]
    public void Test_Player_PlayMultipleCards()
    {
        List<Card> cards = new List<Card>
        {
            new Card(CardColour.RED, CardValue.ONE),
            new Card(CardColour.BLUE, CardValue.TWO),
            new Card(CardColour.GREEN, CardValue.THREE)
        };

        foreach (Card card in cards)
        {
            player.DrawCard(card);
        }

        Assert.AreEqual(3, player.playerHand.Count);

        foreach (Card card in cards)
        {
            player.PlayCard(card);
        }

        Assert.AreEqual(0, player.playerHand.Count);
    }

    [Test]
    public void Test_Player_NonHumanCreation()
    {
        Player aiPlayer = new Player("AI Player", false);
        Assert.IsFalse(aiPlayer.IsHuman);
        Assert.AreEqual("AI Player", aiPlayer.playerName);
    }

    [Test]
    public void Test_Player_HandManagement()
    {
        // Draw some cards
        player.DrawCard(new Card(CardColour.RED, CardValue.ONE));
        player.DrawCard(new Card(CardColour.BLUE, CardValue.TWO));
        Assert.AreEqual(2, player.playerHand.Count);

        // Play a card
        Card cardToPlay = player.playerHand[0];
        player.PlayCard(cardToPlay);
        Assert.AreEqual(1, player.playerHand.Count);
        Assert.IsFalse(player.playerHand.Contains(cardToPlay));

        // Draw more cards
        player.DrawCard(new Card(CardColour.GREEN, CardValue.THREE));
        player.DrawCard(new Card(CardColour.YELLOW, CardValue.FOUR));
        Assert.AreEqual(3, player.playerHand.Count);
    }

    [Test]
    public void Test_Player_DrawNullCard()
    {
        // Draw a real card first to have a non-empty hand
        Card testCard = new Card(CardColour.RED, CardValue.ONE);
        player.DrawCard(testCard);
        int initialHandSize = player.playerHand.Count;
        
        player.DrawCard(null);
        Assert.AreEqual(initialHandSize + 1, player.playerHand.Count);
    }

    [Test]
    public void Test_Player_PlayNullCard()
    {
        Card testCard = new Card(CardColour.RED, CardValue.ONE);
        player.DrawCard(testCard);
        int initialHandSize = player.playerHand.Count;
        
        player.PlayCard(null);
        Assert.AreEqual(initialHandSize, player.playerHand.Count);
        Assert.Contains(testCard, player.playerHand);
    }
} 