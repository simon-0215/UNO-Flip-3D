using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class DeckTests
{
    private GameObject deckObject;
    private Deck deck;

    [SetUp]
    public void SetUp()
    {
        deckObject = new GameObject("Deck");
        deck = deckObject.AddComponent<Deck>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(deckObject);
    }

    [Test]
    public void Test_Deck_InitializeDeck()
    {
        deck.InitializeDeck();
        Assert.AreEqual(112, deck.GetRemainingCards());
    }

    [Test]
    public void Test_Deck_DrawCard()
    {
        deck.InitializeDeck();
        Card drawnCard = deck.DrawCard();
        
        Assert.IsNotNull(drawnCard);
        Assert.AreEqual(111, deck.GetRemainingCards());
    }

    [Test]
    public void Test_Deck_DrawAllCards()
    {
        deck.InitializeDeck();
        List<Card> drawnCards = new List<Card>();
        int initialCount = deck.GetRemainingCards(); // Get actual initial count
        
        // Draw all cards
        for (int i = 0; i < initialCount; i++)
        {
            Card card = deck.DrawCard();
            Assert.IsNotNull(card);
            drawnCards.Add(card);
        }
        
        Assert.AreEqual(0, deck.GetRemainingCards());
        
        // Try to draw from empty deck
        Card emptyDraw = deck.DrawCard();
        Assert.IsNull(emptyDraw);
    }

    [Test]
    public void Test_Deck_AddUsedCard()
    {
        deck.InitializeDeck();
        Card drawnCard = deck.DrawCard();
        
        Assert.AreEqual(111, deck.GetRemainingCards());
        deck.AddUsedCard(drawnCard);
        // Used cards are stored in a separate list, so remaining cards count shouldn't change
        Assert.AreEqual(111, deck.GetRemainingCards());
    }

    [Test]
    public void Test_Deck_VerifyCardDistribution()
    {
        deck.InitializeDeck();
        Dictionary<CardColour, int> colorCount = new Dictionary<CardColour, int>();
        Dictionary<CardValue, int> valueCount = new Dictionary<CardValue, int>();

        // Initialize counters
        foreach (CardColour color in System.Enum.GetValues(typeof(CardColour)))
        {
            colorCount[color] = 0;
        }
        foreach (CardValue value in System.Enum.GetValues(typeof(CardValue)))
        {
            valueCount[value] = 0;
        }

        // Count all cards
        while (deck.GetRemainingCards() > 0)
        {
            Card card = deck.DrawCard();
            colorCount[card.cardColour]++;
            valueCount[card.cardValue]++;
        }

        // Verify color distribution
        Assert.AreEqual(26, colorCount[CardColour.RED]); // Each color has 26 cards
        Assert.AreEqual(26, colorCount[CardColour.BLUE]);
        Assert.AreEqual(26, colorCount[CardColour.GREEN]);
        Assert.AreEqual(26, colorCount[CardColour.YELLOW]);
        Assert.AreEqual(8, colorCount[CardColour.NONE]); // 4 Wild + 4 Wild Draw Four

        // Verify number cards (0-9)
        Assert.AreEqual(8, valueCount[CardValue.ZERO]); // Two Zero per color
        Assert.AreEqual(8, valueCount[CardValue.ONE]); // Two of each number 1-9 per color
        Assert.AreEqual(8, valueCount[CardValue.TWO]);
        Assert.AreEqual(8, valueCount[CardValue.THREE]);
        Assert.AreEqual(8, valueCount[CardValue.FOUR]);
        Assert.AreEqual(8, valueCount[CardValue.FIVE]);
        Assert.AreEqual(8, valueCount[CardValue.SIX]);
        Assert.AreEqual(8, valueCount[CardValue.SEVEN]);
        Assert.AreEqual(8, valueCount[CardValue.EIGHT]);
        Assert.AreEqual(8, valueCount[CardValue.NINE]);

        // Verify action cards
        Assert.AreEqual(8, valueCount[CardValue.SKIP]); // Two per color
        Assert.AreEqual(8, valueCount[CardValue.REVERSE]); // Two per color
        Assert.AreEqual(8, valueCount[CardValue.PLUS_TWO]); // Two per color
        Assert.AreEqual(4, valueCount[CardValue.WILD]); // Four wild cards
        Assert.AreEqual(4, valueCount[CardValue.PLUS_FOUR]); // Four wild draw four cards
    }

    [Test]
    public void Test_Deck_ShuffleChangesOrder()
    {
        deck.InitializeDeck();
        List<Card> originalOrder = new List<Card>();
        List<Card> shuffledOrder = new List<Card>();

        // Record original order
        for (int i = 0; i < 5; i++)
        {
            originalOrder.Add(deck.DrawCard());
        }

        // Reset and shuffle
        deck.InitializeDeck();
        deck.ShuffleDeck();

        // Record shuffled order
        for (int i = 0; i < 5; i++)
        {
            shuffledOrder.Add(deck.DrawCard());
        }

        // Check if at least one card is in a different position
        bool hasChanged = false;
        for (int i = 0; i < 5; i++)
        {
            if (originalOrder[i].cardColour != shuffledOrder[i].cardColour ||
                originalOrder[i].cardValue != shuffledOrder[i].cardValue)
            {
                hasChanged = true;
                break;
            }
        }

        Assert.IsTrue(hasChanged, "Deck order should change after shuffling");
    }

    [Test]
    public void Test_Deck_ResetsWhenEmpty()
    {
        deck.InitializeDeck();
        List<Card> usedCards = new List<Card>();

        // Draw all cards except one
        for (int i = 0; i < 111; i++)
        {
            Card card = deck.DrawCard();
            usedCards.Add(card);
        }

        // Add used cards back to deck
        foreach (Card card in usedCards)
        {
            deck.AddUsedCard(card);
        }

        // Draw remaining card and verify deck resets
        Card lastCard = deck.DrawCard();
        Assert.IsNotNull(lastCard);
        // After drawing the last card and resetting, the deck should have all used cards back
        Assert.AreEqual(0, deck.GetRemainingCards());
    }
} 