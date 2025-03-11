using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using TMPro;
using System.Linq;

[TestFixture]
public class AiPlayerTests
{
    private AiPlayer aiPlayer;
    private GameObject gameManagerObj;
    private GameManager gameManager;
    private Transform discardPileTransform;

    [SetUp]
    public void SetUp()
    {
        aiPlayer = new AiPlayer("AI Test");
        
        // Setup GameManager instance
        gameManagerObj = new GameObject("GameManager");
        gameManager = gameManagerObj.AddComponent<GameManager>();
        
        // Setup required components for GameManager
        var deckObj = new GameObject("Deck");
        var deck = deckObj.AddComponent<Deck>();
        discardPileTransform = new GameObject("DiscardPile", typeof(RectTransform)).transform;
        var playerHand = new GameObject("PlayerHand", typeof(RectTransform)).transform;
        var directionArrow = new GameObject("DirectionArrow", typeof(RectTransform)).transform;
        directionArrow.gameObject.AddComponent<UnityEngine.UI.Image>();
        
        // Set private fields using reflection
        var gameManagerType = typeof(GameManager);
        gameManagerType.GetField("deck", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(gameManager, deck);
        gameManagerType.GetField("discardPileTransform", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(gameManager, discardPileTransform);
        gameManagerType.GetField("playerHandTransform", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(gameManager, playerHand);
        gameManagerType.GetField("directionArrow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(gameManager, directionArrow);
        gameManagerType.GetField("aiHandTransform", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(gameManager, new List<Transform>());

        // Set up colors in GameManager
        gameManagerType.GetField("red", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(gameManager, new Color32(255, 0, 0, 255));
        gameManagerType.GetField("blue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(gameManager, new Color32(0, 0, 255, 255));
        gameManagerType.GetField("green", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(gameManager, new Color32(0, 255, 0, 255));
        gameManagerType.GetField("yellow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(gameManager, new Color32(255, 255, 0, 255));
        gameManagerType.GetField("black", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(gameManager, new Color32(0, 0, 0, 255));
        
        GameManager.instance = gameManager;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(gameManagerObj);
    }

    [Test]
    public void Test_AiPlayer_Creation()
    {
        Assert.IsNotNull(aiPlayer);
        Assert.AreEqual("AI Test", aiPlayer.playerName);
    }

    [Test]
    public void Test_AiPlayer_IsNotHuman()
    {
        Assert.IsFalse(aiPlayer.IsHuman);
    }

    [Test]
    public void Test_AiPlayer_StartsWithEmptyHand()
    {
        Assert.IsNotNull(aiPlayer.playerHand);
        Assert.AreEqual(0, aiPlayer.playerHand.Count);
    }

    [Test]
    public void Test_AiPlayer_CanDrawCard()
    {
        Card testCard = new Card(CardColour.RED, CardValue.ONE);
        aiPlayer.DrawCard(testCard);
        
        Assert.AreEqual(1, aiPlayer.playerHand.Count);
        Assert.Contains(testCard, aiPlayer.playerHand);
    }

    [Test]
    public void Test_AiPlayer_CanPlayCard()
    {
        Card testCard = new Card(CardColour.RED, CardValue.ONE);
        aiPlayer.DrawCard(testCard);
        aiPlayer.PlayCard(testCard);
        
        Assert.AreEqual(0, aiPlayer.playerHand.Count);
        Assert.IsFalse(aiPlayer.playerHand.Contains(testCard));
    }

    [Test]
    public void Test_AiPlayer_SelectsBestColor()
    {
        // Give AI player a hand with mostly RED cards
        aiPlayer.DrawCard(new Card(CardColour.RED, CardValue.ONE));
        aiPlayer.DrawCard(new Card(CardColour.RED, CardValue.TWO));
        aiPlayer.DrawCard(new Card(CardColour.RED, CardValue.THREE));
        aiPlayer.DrawCard(new Card(CardColour.BLUE, CardValue.ONE));
        aiPlayer.DrawCard(new Card(CardColour.GREEN, CardValue.ONE));

        // AI should choose RED as it has the most cards of that color
        CardColour selectedColor = aiPlayer.SelectBestColor();
        Assert.AreEqual(CardColour.RED, selectedColor);
    }

    [Test]
    public void Test_AiPlayer_GetPlayableCards_MatchingColor()
    {
        // Setup a hand with some playable and non-playable cards
        Card topCard = new Card(CardColour.RED, CardValue.ONE);
        aiPlayer.DrawCard(new Card(CardColour.RED, CardValue.TWO));    // Playable - color match
        aiPlayer.DrawCard(new Card(CardColour.BLUE, CardValue.ONE));   // Playable - value match
        aiPlayer.DrawCard(new Card(CardColour.BLUE, CardValue.THREE)); // Not playable
        aiPlayer.DrawCard(new Card(CardColour.NONE, CardValue.WILD));  // Playable - wild card

        List<Card> playableCards = aiPlayer.GetPlayableCards(topCard, CardColour.RED);
        
        Assert.AreEqual(3, playableCards.Count);
        Assert.IsTrue(playableCards.Exists(card => card.cardColour == CardColour.RED));
        Assert.IsTrue(playableCards.Exists(card => card.cardValue == CardValue.ONE));
        Assert.IsTrue(playableCards.Exists(card => card.cardColour == CardColour.NONE));
    }

    [Test]
    public void Test_AiPlayer_GetPlayableCards_NoMatches()
    {
        // Setup a hand with no playable cards
        Card topCard = new Card(CardColour.RED, CardValue.ONE);
        aiPlayer.DrawCard(new Card(CardColour.BLUE, CardValue.TWO));
        aiPlayer.DrawCard(new Card(CardColour.GREEN, CardValue.THREE));

        List<Card> playableCards = aiPlayer.GetPlayableCards(topCard, CardColour.RED);
        
        Assert.AreEqual(0, playableCards.Count);
    }

    [Test]
    public void Test_AiPlayer_ChooseBestCard_PreferActionCards()
    {
        // Setup playable cards including action cards
        List<Card> playableCards = new List<Card>
        {
            new Card(CardColour.RED, CardValue.ONE),
            new Card(CardColour.RED, CardValue.PLUS_TWO),
            new Card(CardColour.BLUE, CardValue.SKIP)
        };

        // Add players to GameManager's player list
        gameManager.players.Clear();
        var humanPlayer = new Player("Test Player", true);
        gameManager.players.Add(humanPlayer);
        gameManager.players.Add(aiPlayer);

        // Give the human player 2 cards to trigger action card preference
        humanPlayer.DrawCard(new Card(CardColour.RED, CardValue.ONE));
        humanPlayer.DrawCard(new Card(CardColour.BLUE, CardValue.TWO));

        // Set current player to AI (index 1)
        var gameManagerType = typeof(GameManager);
        gameManagerType.GetField("currentPlayer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(gameManager, 1);
        gameManagerType.GetField("playDirection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(gameManager, 1);

        // Choose best card - should prefer action cards when opponent has few cards
        Card bestCard = aiPlayer.ChooseBestCard(playableCards);
        
        Assert.IsTrue(bestCard.cardValue == CardValue.PLUS_TWO || bestCard.cardValue == CardValue.SKIP);
    }

    [Test]
    public void Test_AiPlayer_SelectBestColor_EqualDistribution()
    {
        // Give AI player a hand with equal distribution of colors
        aiPlayer.DrawCard(new Card(CardColour.RED, CardValue.ONE));
        aiPlayer.DrawCard(new Card(CardColour.BLUE, CardValue.TWO));
        aiPlayer.DrawCard(new Card(CardColour.GREEN, CardValue.THREE));
        aiPlayer.DrawCard(new Card(CardColour.YELLOW, CardValue.FOUR));

        // AI should choose any color (we just verify it's a valid color)
        CardColour selectedColor = aiPlayer.SelectBestColor();
        Assert.IsTrue(selectedColor == CardColour.RED || 
                     selectedColor == CardColour.BLUE || 
                     selectedColor == CardColour.GREEN || 
                     selectedColor == CardColour.YELLOW);
    }

    [Test]
    public void Test_AiPlayer_SelectBestColor_WithWildCards()
    {
        // Give AI player a hand with wild cards and colored cards
        aiPlayer.DrawCard(new Card(CardColour.RED, CardValue.ONE));
        aiPlayer.DrawCard(new Card(CardColour.RED, CardValue.TWO));
        aiPlayer.DrawCard(new Card(CardColour.NONE, CardValue.WILD));
        aiPlayer.DrawCard(new Card(CardColour.NONE, CardValue.PLUS_FOUR));

        // Should still choose RED as it has the most non-wild cards
        CardColour selectedColor = aiPlayer.SelectBestColor();
        Assert.AreEqual(CardColour.RED, selectedColor);
    }

    [Test]
    public void Test_AiPlayer_GetPlayableCards_WildCardsAlwaysPlayable()
    {
        // Setup a hand with wild cards
        Card topCard = new Card(CardColour.RED, CardValue.ONE);
        aiPlayer.DrawCard(new Card(CardColour.NONE, CardValue.WILD));
        aiPlayer.DrawCard(new Card(CardColour.NONE, CardValue.PLUS_FOUR));
        aiPlayer.DrawCard(new Card(CardColour.BLUE, CardValue.TWO)); // Not playable

        List<Card> playableCards = aiPlayer.GetPlayableCards(topCard, CardColour.RED);
        
        Assert.AreEqual(2, playableCards.Count);
        Assert.IsTrue(playableCards.TrueForAll(card => card.cardColour == CardColour.NONE));
    }

    [Test]
    public void Test_AiPlayer_ChooseBestCard_WithTwoCards()
    {
        // Setup playable cards
        List<Card> playableCards = new List<Card>
        {
            new Card(CardColour.RED, CardValue.ONE),
            new Card(CardColour.RED, CardValue.TWO)
        };

        // Give AI exactly 2 cards
        aiPlayer.DrawCard(playableCards[0]);
        aiPlayer.DrawCard(playableCards[1]);

        // Setup GameManager with players and direction
        gameManager.players.Clear();
        var humanPlayer = new Player("Test Player", true);
        humanPlayer.DrawCard(new Card(CardColour.BLUE, CardValue.ONE)); // Give human a card to avoid divide by zero
        gameManager.players.Add(humanPlayer);
        gameManager.players.Add(aiPlayer);

        var gameManagerType = typeof(GameManager);
        gameManagerType.GetField("currentPlayer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(gameManager, 1);
        gameManagerType.GetField("playDirection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(gameManager, 1);

        // Choose best card
        Card bestCard = aiPlayer.ChooseBestCard(playableCards);

        // Verify a valid card was chosen
        Assert.IsNotNull(bestCard);
        Assert.Contains(bestCard, playableCards);
    }

    [Test]
    public void Test_AiPlayer_SelectsColorForWildCard()
    {
        // Give AI player a hand with multiple red cards and a wild card
        aiPlayer.DrawCard(new Card(CardColour.RED, CardValue.ONE));
        aiPlayer.DrawCard(new Card(CardColour.RED, CardValue.TWO));
        aiPlayer.DrawCard(new Card(CardColour.RED, CardValue.THREE));
        aiPlayer.DrawCard(new Card(CardColour.BLUE, CardValue.ONE));
        aiPlayer.DrawCard(new Card(CardColour.NONE, CardValue.WILD));

        // Setup GameManager with players
        gameManager.players.Clear();
        var humanPlayer = new Player("Test Player", true);
        humanPlayer.DrawCard(new Card(CardColour.BLUE, CardValue.ONE));
        gameManager.players.Add(humanPlayer);
        gameManager.players.Add(aiPlayer);

        // Set current player to AI (index 1)
        var gameManagerType = typeof(GameManager);
        gameManagerType.GetField("currentPlayer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(gameManager, 1);

        // Get playable cards (should include the wild card)
        Card topCard = new Card(CardColour.YELLOW, CardValue.ONE);
        List<Card> playableCards = aiPlayer.GetPlayableCards(topCard, CardColour.YELLOW);

        // Find the wild card in playable cards
        Card wildCard = playableCards.Find(card => card.cardValue == CardValue.WILD);
        Assert.IsNotNull(wildCard, "Wild card should be in playable cards");

        // Choose best card (should be the wild card since we can choose red color)
        Card chosenCard = aiPlayer.ChooseBestCard(playableCards);
        Assert.AreEqual(CardValue.WILD, chosenCard.cardValue, "AI should choose the wild card");

        // Verify AI selects red as the best color (since it has the most red cards)
        CardColour selectedColor = aiPlayer.SelectBestColor();
        Assert.AreEqual(CardColour.RED, selectedColor, "AI should select red as the best color for the wild card");
    }

    [Test]
    public void Test_AiPlayer_ChooseBestCard_BasedOnOpponentHandSize()
    {
        // Setup playable cards including multiple action cards
        List<Card> playableCards = new List<Card>
        {
            new Card(CardColour.RED, CardValue.ONE),
            new Card(CardColour.RED, CardValue.PLUS_TWO),
            new Card(CardColour.RED, CardValue.SKIP),
            new Card(CardColour.RED, CardValue.REVERSE)
        };

        // Add players to GameManager's player list
        gameManager.players.Clear();
        var humanPlayer = new Player("Test Player", true);
        gameManager.players.Add(humanPlayer);
        gameManager.players.Add(aiPlayer);

        // Set current player to AI (index 1)
        var gameManagerType = typeof(GameManager);
        gameManagerType.GetField("currentPlayer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(gameManager, 1);
        gameManagerType.GetField("playDirection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(gameManager, 1);

        // Test scenario 1: Opponent has many cards (7)
        for (int i = 0; i < 7; i++)
        {
            humanPlayer.DrawCard(new Card(CardColour.BLUE, CardValue.ONE));
        }
        Card bestCard = aiPlayer.ChooseBestCard(playableCards);
        Assert.IsTrue(bestCard.cardValue == CardValue.PLUS_TWO || 
                     bestCard.cardValue == CardValue.SKIP || 
                     bestCard.cardValue == CardValue.REVERSE,
                     "AI should prefer action cards even when opponent has many cards");

        // Test scenario 2: Opponent has few cards (2)
        humanPlayer.playerHand.Clear();
        humanPlayer.DrawCard(new Card(CardColour.BLUE, CardValue.ONE));
        humanPlayer.DrawCard(new Card(CardColour.BLUE, CardValue.TWO));
        
        bestCard = aiPlayer.ChooseBestCard(playableCards);
        Assert.IsTrue(bestCard.cardValue == CardValue.PLUS_TWO || 
                     bestCard.cardValue == CardValue.SKIP || 
                     bestCard.cardValue == CardValue.REVERSE,
                     "AI should choose action card when opponent has few cards");

        // Test scenario 3: Only number cards available
        playableCards = new List<Card>
        {
            new Card(CardColour.RED, CardValue.ONE),
            new Card(CardColour.RED, CardValue.TWO),
            new Card(CardColour.RED, CardValue.THREE)
        };

        bestCard = aiPlayer.ChooseBestCard(playableCards);
        Assert.IsTrue(bestCard.cardValue == CardValue.ONE || 
                     bestCard.cardValue == CardValue.TWO || 
                     bestCard.cardValue == CardValue.THREE,
                     "AI should choose a number card when no action cards are available");
    }

    [Test]
    public void Test_AiPlayer_ChooseBestCard_PreferMatchingColor()
    {
        // Setup AI hand with multiple cards of the same value but different colors
        aiPlayer.DrawCard(new Card(CardColour.RED, CardValue.ONE));
        aiPlayer.DrawCard(new Card(CardColour.RED, CardValue.TWO));
        aiPlayer.DrawCard(new Card(CardColour.RED, CardValue.THREE));
        aiPlayer.DrawCard(new Card(CardColour.BLUE, CardValue.ONE));
        aiPlayer.DrawCard(new Card(CardColour.GREEN, CardValue.ONE));

        // Setup playable cards (all ONE cards are playable)
        List<Card> playableCards = aiPlayer.GetPlayableCards(new Card(CardColour.RED, CardValue.ONE), CardColour.RED);

        // Add players to GameManager's player list
        gameManager.players.Clear();
        var humanPlayer = new Player("Test Player", true);
        humanPlayer.DrawCard(new Card(CardColour.BLUE, CardValue.TWO));
        gameManager.players.Add(humanPlayer);
        gameManager.players.Add(aiPlayer);

        // Set current player to AI (index 1)
        var gameManagerType = typeof(GameManager);
        gameManagerType.GetField("currentPlayer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(gameManager, 1);

        // Test scenario 1: When top card is RED ONE
        Card bestCard = aiPlayer.ChooseBestCard(playableCards);
        Assert.AreEqual(CardColour.RED, bestCard.cardColour, 
            "AI should prefer the card matching both color and value");

        // Test scenario 2: When top card is BLUE ONE
        playableCards = aiPlayer.GetPlayableCards(new Card(CardColour.BLUE, CardValue.ONE), CardColour.BLUE);
        bestCard = aiPlayer.ChooseBestCard(playableCards);
        Assert.AreEqual(CardColour.RED, bestCard.cardColour, 
            "AI should prefer cards of its dominant color (RED) even when another color is on top");

        // Test scenario 3: When top card is GREEN ONE
        playableCards = aiPlayer.GetPlayableCards(new Card(CardColour.GREEN, CardValue.ONE), CardColour.GREEN);
        bestCard = aiPlayer.ChooseBestCard(playableCards);
        Assert.AreEqual(CardColour.RED, bestCard.cardColour, 
            "AI should prefer cards of its dominant color (RED) even when another color is on top");

        // Verify that all ONE cards were considered playable in each scenario
        Assert.AreEqual(3, playableCards.Count, 
            "All cards with matching value should be playable");
        Assert.IsTrue(playableCards.All(card => card.cardValue == CardValue.ONE), 
            "All playable cards should have the same value");
    }

    private CardDisplay CreateCompleteCardDisplay()
    {
        var cardObject = new GameObject("CardDisplay", typeof(RectTransform));
        var cardDisplay = cardObject.AddComponent<CardDisplay>();
        cardObject.AddComponent<CardInteraction>();

        // Create a dummy sprite for testing
        var texture = new Texture2D(100, 100);
        var sprite = Sprite.Create(texture, new Rect(0, 0, 100, 100), Vector2.one * 0.5f);

        // Create base card image
        var baseCardObj = new GameObject("BaseCard", typeof(RectTransform));
        baseCardObj.transform.SetParent(cardObject.transform, false);
        var baseCard = baseCardObj.AddComponent<Image>();
        baseCard.sprite = sprite;

        // Create center image
        var imageCenterObj = new GameObject("ImageCenter", typeof(RectTransform));
        imageCenterObj.transform.SetParent(cardObject.transform, false);
        var imageCenter = imageCenterObj.AddComponent<Image>();
        imageCenter.sprite = sprite;

        // Create value image center
        var valueImageCenterObj = new GameObject("ValueImageCenter", typeof(RectTransform));
        valueImageCenterObj.transform.SetParent(cardObject.transform, false);
        var valueImageCenter = valueImageCenterObj.AddComponent<Image>();
        valueImageCenter.sprite = sprite;

        // Create value text center
        var valueTextCenterObj = new GameObject("ValueTextCenter", typeof(RectTransform));
        valueTextCenterObj.transform.SetParent(cardObject.transform, false);
        var valueTextCenter = valueTextCenterObj.AddComponent<TextMeshProUGUI>();

        // Create wild image center
        var wildImageCenter = new GameObject("WildImageCenter", typeof(RectTransform));
        wildImageCenter.transform.SetParent(cardObject.transform, false);

        // Create center corners
        var topLeftCenter = new GameObject("TopLeftCenter", typeof(RectTransform));
        topLeftCenter.transform.SetParent(cardObject.transform, false);
        var topLeftCenterImage = topLeftCenter.AddComponent<Image>();
        topLeftCenterImage.sprite = sprite;

        var bottomLeftCenter = new GameObject("BottomLeftCenter", typeof(RectTransform));
        bottomLeftCenter.transform.SetParent(cardObject.transform, false);
        var bottomLeftCenterImage = bottomLeftCenter.AddComponent<Image>();
        bottomLeftCenterImage.sprite = sprite;

        var topRightCenter = new GameObject("TopRightCenter", typeof(RectTransform));
        topRightCenter.transform.SetParent(cardObject.transform, false);
        var topRightCenterImage = topRightCenter.AddComponent<Image>();
        topRightCenterImage.sprite = sprite;

        var bottomRightCenter = new GameObject("BottomRightCenter", typeof(RectTransform));
        bottomRightCenter.transform.SetParent(cardObject.transform, false);
        var bottomRightCenterImage = bottomRightCenter.AddComponent<Image>();
        bottomRightCenterImage.sprite = sprite;

        // Create top left corner components
        var valueImageTL = new GameObject("ValueImageTL", typeof(RectTransform));
        valueImageTL.transform.SetParent(cardObject.transform, false);
        var valueImageTLComponent = valueImageTL.AddComponent<Image>();
        valueImageTLComponent.sprite = sprite;

        var valueTextTL = new GameObject("ValueTextTL", typeof(RectTransform));
        valueTextTL.transform.SetParent(cardObject.transform, false);
        var valueTextTLComponent = valueTextTL.AddComponent<TextMeshProUGUI>();

        var wildImageTL = new GameObject("WildImageTL", typeof(RectTransform));
        wildImageTL.transform.SetParent(cardObject.transform, false);

        var topLeftTL = new GameObject("TopLeftTL", typeof(RectTransform));
        topLeftTL.transform.SetParent(cardObject.transform, false);
        var topLeftTLImage = topLeftTL.AddComponent<Image>();
        topLeftTLImage.sprite = sprite;

        var bottomLeftTL = new GameObject("BottomLeftTL", typeof(RectTransform));
        bottomLeftTL.transform.SetParent(cardObject.transform, false);
        var bottomLeftTLImage = bottomLeftTL.AddComponent<Image>();
        bottomLeftTLImage.sprite = sprite;

        var topRightTL = new GameObject("TopRightTL", typeof(RectTransform));
        topRightTL.transform.SetParent(cardObject.transform, false);
        var topRightTLImage = topRightTL.AddComponent<Image>();
        topRightTLImage.sprite = sprite;

        var bottomRightTL = new GameObject("BottomRightTL", typeof(RectTransform));
        bottomRightTL.transform.SetParent(cardObject.transform, false);
        var bottomRightTLImage = bottomRightTL.AddComponent<Image>();
        bottomRightTLImage.sprite = sprite;

        // Create bottom right corner components
        var valueImageBR = new GameObject("ValueImageBR", typeof(RectTransform));
        valueImageBR.transform.SetParent(cardObject.transform, false);
        var valueImageBRComponent = valueImageBR.AddComponent<Image>();
        valueImageBRComponent.sprite = sprite;

        var valueTextBR = new GameObject("ValueTextBR", typeof(RectTransform));
        valueTextBR.transform.SetParent(cardObject.transform, false);
        var valueTextBRComponent = valueTextBR.AddComponent<TextMeshProUGUI>();

        var wildImageBR = new GameObject("WildImageBR", typeof(RectTransform));
        wildImageBR.transform.SetParent(cardObject.transform, false);

        var topLeftBR = new GameObject("TopLeftBR", typeof(RectTransform));
        topLeftBR.transform.SetParent(cardObject.transform, false);
        var topLeftBRImage = topLeftBR.AddComponent<Image>();
        topLeftBRImage.sprite = sprite;

        var bottomLeftBR = new GameObject("BottomLeftBR", typeof(RectTransform));
        bottomLeftBR.transform.SetParent(cardObject.transform, false);
        var bottomLeftBRImage = bottomLeftBR.AddComponent<Image>();
        bottomLeftBRImage.sprite = sprite;

        var topRightBR = new GameObject("TopRightBR", typeof(RectTransform));
        topRightBR.transform.SetParent(cardObject.transform, false);
        var topRightBRImage = topRightBR.AddComponent<Image>();
        topRightBRImage.sprite = sprite;

        var bottomRightBR = new GameObject("BottomRightBR", typeof(RectTransform));
        bottomRightBR.transform.SetParent(cardObject.transform, false);
        var bottomRightBRImage = bottomRightBR.AddComponent<Image>();
        bottomRightBRImage.sprite = sprite;

        // Create card back
        var cardBackObj = new GameObject("CardBack", typeof(RectTransform));
        cardBackObj.transform.SetParent(cardObject.transform, false);
        var cardBackImage = cardBackObj.AddComponent<Image>();
        cardBackImage.sprite = sprite;

        // Set private fields using reflection
        var cardDisplayType = typeof(CardDisplay);
        cardDisplayType.GetField("baseCardColour", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cardDisplay, baseCard);
        cardDisplayType.GetField("imageCenter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cardDisplay, imageCenter);
        cardDisplayType.GetField("valueImageCenter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cardDisplay, valueImageCenter);
        cardDisplayType.GetField("valueTextCenter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cardDisplay, valueTextCenter);
        cardDisplayType.GetField("wildImageCenter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cardDisplay, wildImageCenter);
        cardDisplayType.GetField("topLeftCenter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cardDisplay, topLeftCenterImage);
        cardDisplayType.GetField("bottomLeftCenter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cardDisplay, bottomLeftCenterImage);
        cardDisplayType.GetField("topRightCenter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cardDisplay, topRightCenterImage);
        cardDisplayType.GetField("bottomRightCenter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cardDisplay, bottomRightCenterImage);
        cardDisplayType.GetField("valueImageTL", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cardDisplay, valueImageTLComponent);
        cardDisplayType.GetField("valueTextTL", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cardDisplay, valueTextTLComponent);
        cardDisplayType.GetField("wildImageTL", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cardDisplay, wildImageTL);
        cardDisplayType.GetField("topLeftTL", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cardDisplay, topLeftTLImage);
        cardDisplayType.GetField("bottomLeftTL", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cardDisplay, bottomLeftTLImage);
        cardDisplayType.GetField("topRightTL", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cardDisplay, topRightTLImage);
        cardDisplayType.GetField("bottomRightTL", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cardDisplay, bottomRightTLImage);
        cardDisplayType.GetField("valueImageBR", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cardDisplay, valueImageBRComponent);
        cardDisplayType.GetField("valueTextBR", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cardDisplay, valueTextBRComponent);
        cardDisplayType.GetField("wildImageBR", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cardDisplay, wildImageBR);
        cardDisplayType.GetField("topLeftBR", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cardDisplay, topLeftBRImage);
        cardDisplayType.GetField("bottomLeftBR", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cardDisplay, bottomLeftBRImage);
        cardDisplayType.GetField("topRightBR", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cardDisplay, topRightBRImage);
        cardDisplayType.GetField("bottomRightBR", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cardDisplay, bottomRightBRImage);
        cardDisplayType.GetField("cardBack", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cardDisplay, cardBackObj);

        // Set action card sprites
        cardDisplayType.GetField("reverse", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cardDisplay, sprite);
        cardDisplayType.GetField("skip", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cardDisplay, sprite);
        cardDisplayType.GetField("plusTwo", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cardDisplay, sprite);
        cardDisplayType.GetField("plusFour", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cardDisplay, sprite);

        return cardDisplay;
    }
}
