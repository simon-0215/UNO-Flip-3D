using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[TestFixture]
public class CardDisplayTests
{
    private GameObject cardObject;
    private CardDisplay cardDisplay;
    private Image baseCardColour;
    private Image imageCenter;
    private Image valueImageCenter;
    private TextMeshProUGUI valueTextCenter;
    private GameObject wildImageCenter;
    private GameObject wildImageTL;
    private GameObject wildImageBR;
    private Image valueImageTL;
    private Image valueImageBR;
    private TextMeshProUGUI valueTextTL;
    private TextMeshProUGUI valueTextBR;
    private GameObject gameManagerObject;
    
    // Wild card corner components
    private Image topLeftCenter;
    private Image bottomLeftCenter;
    private Image topRightCenter;
    private Image bottomRightCenter;
    private Image topLeftTL;
    private Image bottomLeftTL;
    private Image topRightTL;
    private Image bottomRightTL;
    private Image topLeftBR;
    private Image bottomLeftBR;
    private Image topRightBR;
    private Image bottomRightBR;

    [SetUp]
    public void SetUp()
    {
        // Create GameManager with all required components
        gameManagerObject = new GameObject("GameManager");
        var gameManager = gameManagerObject.AddComponent<GameManager>();

        // Create and setup required GameManager references
        var playerHand = new GameObject("PlayerHand", typeof(RectTransform));
        var discardPile = new GameObject("DiscardPile", typeof(RectTransform));
        var directionArrow = new GameObject("DirectionArrow", typeof(RectTransform));
        directionArrow.AddComponent<Image>();
        var wildPanel = new GameObject("WildPanel");
        var deck = new GameObject("Deck").AddComponent<Deck>();

        // Create Wild Buttons
        var redButton = CreateWildButton("Red");
        var blueButton = CreateWildButton("Blue");
        var greenButton = CreateWildButton("Green");
        var yellowButton = CreateWildButton("Yellow");

        // Create UI elements
        var messageText = new GameObject("MessageText").AddComponent<TextMeshProUGUI>();
        var winningText = new GameObject("WinningText").AddComponent<TextMeshProUGUI>();
        var winPanel = new GameObject("WinPanel");

        // Set GameManager fields
        SetPrivateField(gameManager, "playerHandTransform", playerHand.transform);
        SetPrivateField(gameManager, "discardPileTransform", discardPile.transform);
        SetPrivateField(gameManager, "directionArrow", directionArrow.transform);
        SetPrivateField(gameManager, "wildPanel", wildPanel);
        SetPrivateField(gameManager, "deck", deck);
        SetPrivateField(gameManager, "redButton", redButton);
        SetPrivateField(gameManager, "blueButton", blueButton);
        SetPrivateField(gameManager, "greenButton", greenButton);
        SetPrivateField(gameManager, "yellowButton", yellowButton);
        SetPrivateField(gameManager, "messageText", messageText);
        SetPrivateField(gameManager, "winningText", winningText);
        SetPrivateField(gameManager, "winPanel", winPanel);
        SetPrivateField(gameManager, "numberOfAiPlayers", 3);
        SetPrivateField(gameManager, "startingHandSize", 7);
        SetPrivateField(gameManager, "aiHandTransform", new List<Transform>());
        SetPrivateField(gameManager, "playerHighlights", new List<Image>());
        SetPrivateField(gameManager, "playerCardCount", new List<TMP_Text>());

        // Set GameManager colors
        SetPrivateField(gameManager, "red", new Color32(255, 0, 0, 255));
        SetPrivateField(gameManager, "blue", new Color32(0, 0, 255, 255));
        SetPrivateField(gameManager, "green", new Color32(0, 255, 0, 255));
        SetPrivateField(gameManager, "yellow", new Color32(255, 255, 0, 255));
        SetPrivateField(gameManager, "black", new Color32(0, 0, 0, 255));
        
        GameManager.instance = gameManager;

        // Create main CardHolder object
        cardObject = new GameObject("CardHolder", typeof(RectTransform));
        
        // Create CardPrefab object
        var cardPrefab = new GameObject("CardPrefab", typeof(RectTransform));
        cardPrefab.transform.SetParent(cardObject.transform, false);
        cardDisplay = cardPrefab.AddComponent<CardDisplay>();

        // Create Colour object
        var colourObj = new GameObject("Colour", typeof(RectTransform));
        colourObj.transform.SetParent(cardPrefab.transform, false);
        baseCardColour = colourObj.AddComponent<Image>();

        // Create CenterHolder object
        var centerHolder = new GameObject("CenterHolder", typeof(RectTransform));
        centerHolder.transform.SetParent(cardPrefab.transform, false);

        // Create Center object under CenterHolder
        var center = new GameObject("Center", typeof(RectTransform));
        center.transform.SetParent(centerHolder.transform, false);
        imageCenter = center.AddComponent<Image>();

        // Add wild card corner components for center
        topLeftCenter = new GameObject("TopLeftCenter", typeof(RectTransform)).AddComponent<Image>();
        bottomLeftCenter = new GameObject("BottomLeftCenter", typeof(RectTransform)).AddComponent<Image>();
        topRightCenter = new GameObject("TopRightCenter", typeof(RectTransform)).AddComponent<Image>();
        bottomRightCenter = new GameObject("BottomRightCenter", typeof(RectTransform)).AddComponent<Image>();
        topLeftCenter.transform.SetParent(center.transform, false);
        bottomLeftCenter.transform.SetParent(center.transform, false);
        topRightCenter.transform.SetParent(center.transform, false);
        bottomRightCenter.transform.SetParent(center.transform, false);

        // Create ValueImage under Center
        var valueImageObj = new GameObject("ValueImage", typeof(RectTransform));
        valueImageObj.transform.SetParent(center.transform, false);
        valueImageCenter = valueImageObj.AddComponent<Image>();

        // Create ValueText under Center
        var valueTextObj = new GameObject("ValueText", typeof(RectTransform));
        valueTextObj.transform.SetParent(center.transform, false);
        valueTextCenter = valueTextObj.AddComponent<TextMeshProUGUI>();

        // Create Wild objects
        wildImageCenter = new GameObject("Wild", typeof(RectTransform));
        wildImageCenter.transform.SetParent(center.transform, false);

        // Create TopLeftValue objects
        var topLeftValue = new GameObject("TopLeftValue", typeof(RectTransform));
        topLeftValue.transform.SetParent(centerHolder.transform, false);
        
        valueImageTL = new GameObject("ValueImageTL", typeof(RectTransform)).AddComponent<Image>();
        valueImageTL.transform.SetParent(topLeftValue.transform, false);
        
        valueTextTL = new GameObject("ValueTextTL", typeof(RectTransform)).AddComponent<TextMeshProUGUI>();
        valueTextTL.transform.SetParent(topLeftValue.transform, false);
        
        wildImageTL = new GameObject("WildTL", typeof(RectTransform));
        wildImageTL.transform.SetParent(topLeftValue.transform, false);

        // Add wild card corner components for top left
        topLeftTL = new GameObject("TopLeftTL", typeof(RectTransform)).AddComponent<Image>();
        bottomLeftTL = new GameObject("BottomLeftTL", typeof(RectTransform)).AddComponent<Image>();
        topRightTL = new GameObject("TopRightTL", typeof(RectTransform)).AddComponent<Image>();
        bottomRightTL = new GameObject("BottomRightTL", typeof(RectTransform)).AddComponent<Image>();
        topLeftTL.transform.SetParent(topLeftValue.transform, false);
        bottomLeftTL.transform.SetParent(topLeftValue.transform, false);
        topRightTL.transform.SetParent(topLeftValue.transform, false);
        bottomRightTL.transform.SetParent(topLeftValue.transform, false);

        // Create BottomRightValue objects
        var bottomRightValue = new GameObject("BottomRightValue", typeof(RectTransform));
        bottomRightValue.transform.SetParent(centerHolder.transform, false);
        
        valueImageBR = new GameObject("ValueImageBR", typeof(RectTransform)).AddComponent<Image>();
        valueImageBR.transform.SetParent(bottomRightValue.transform, false);
        
        valueTextBR = new GameObject("ValueTextBR", typeof(RectTransform)).AddComponent<TextMeshProUGUI>();
        valueTextBR.transform.SetParent(bottomRightValue.transform, false);
        
        wildImageBR = new GameObject("WildBR", typeof(RectTransform));
        wildImageBR.transform.SetParent(bottomRightValue.transform, false);

        // Add wild card corner components for bottom right
        topLeftBR = new GameObject("TopLeftBR", typeof(RectTransform)).AddComponent<Image>();
        bottomLeftBR = new GameObject("BottomLeftBR", typeof(RectTransform)).AddComponent<Image>();
        topRightBR = new GameObject("TopRightBR", typeof(RectTransform)).AddComponent<Image>();
        bottomRightBR = new GameObject("BottomRightBR", typeof(RectTransform)).AddComponent<Image>();
        topLeftBR.transform.SetParent(bottomRightValue.transform, false);
        bottomLeftBR.transform.SetParent(bottomRightValue.transform, false);
        topRightBR.transform.SetParent(bottomRightValue.transform, false);
        bottomRightBR.transform.SetParent(bottomRightValue.transform, false);

        // Set private fields
        SetPrivateField(cardDisplay, "baseCardColour", baseCardColour);
        SetPrivateField(cardDisplay, "imageCenter", imageCenter);
        SetPrivateField(cardDisplay, "valueImageCenter", valueImageCenter);
        SetPrivateField(cardDisplay, "valueTextCenter", valueTextCenter);
        SetPrivateField(cardDisplay, "wildImageCenter", wildImageCenter);
        SetPrivateField(cardDisplay, "valueImageTL", valueImageTL);
        SetPrivateField(cardDisplay, "valueTextTL", valueTextTL);
        SetPrivateField(cardDisplay, "wildImageTL", wildImageTL);
        SetPrivateField(cardDisplay, "valueImageBR", valueImageBR);
        SetPrivateField(cardDisplay, "valueTextBR", valueTextBR);
        SetPrivateField(cardDisplay, "wildImageBR", wildImageBR);
        SetPrivateField(cardDisplay, "topLeftCenter", topLeftCenter);
        SetPrivateField(cardDisplay, "bottomLeftCenter", bottomLeftCenter);
        SetPrivateField(cardDisplay, "topRightCenter", topRightCenter);
        SetPrivateField(cardDisplay, "bottomRightCenter", bottomRightCenter);
        SetPrivateField(cardDisplay, "topLeftTL", topLeftTL);
        SetPrivateField(cardDisplay, "bottomLeftTL", bottomLeftTL);
        SetPrivateField(cardDisplay, "topRightTL", topRightTL);
        SetPrivateField(cardDisplay, "bottomRightTL", bottomRightTL);
        SetPrivateField(cardDisplay, "topLeftBR", topLeftBR);
        SetPrivateField(cardDisplay, "bottomLeftBR", bottomLeftBR);
        SetPrivateField(cardDisplay, "topRightBR", topRightBR);
        SetPrivateField(cardDisplay, "bottomRightBR", bottomRightBR);

        // Set the color fields in CardDisplay
        SetPrivateField(cardDisplay, "red", new Color32(255, 0, 0, 255));
        SetPrivateField(cardDisplay, "blue", new Color32(0, 0, 255, 255));
        SetPrivateField(cardDisplay, "green", new Color32(0, 255, 0, 255));
        SetPrivateField(cardDisplay, "yellow", new Color32(255, 255, 0, 255));
        SetPrivateField(cardDisplay, "black", new Color32(0, 0, 0, 255));

        // Create dummy sprites for action cards
        var dummyTexture = new Texture2D(100, 100);
        var dummySprite = Sprite.Create(dummyTexture, new Rect(0, 0, 100, 100), Vector2.one * 0.5f);
        SetPrivateField(cardDisplay, "reverse", dummySprite);
        SetPrivateField(cardDisplay, "skip", dummySprite);
        SetPrivateField(cardDisplay, "plusTwo", dummySprite);
        SetPrivateField(cardDisplay, "plusFour", dummySprite);
    }

    private WildButton CreateWildButton(string name)
    {
        var buttonObj = new GameObject(name);
        var button = buttonObj.AddComponent<WildButton>();
        buttonObj.AddComponent<Image>();
        return button;
    }

    private GameObject CreateImageObject(string name)
    {
        var obj = new GameObject(name, typeof(RectTransform));
        obj.transform.SetParent(cardObject.transform, false);
        obj.AddComponent<Image>();
        return obj;
    }

    private GameObject CreateTextObject(string name)
    {
        var obj = new GameObject(name, typeof(RectTransform));
        obj.transform.SetParent(cardObject.transform, false);
        obj.AddComponent<TextMeshProUGUI>();
        return obj;
    }

    private void SetPrivateField(object obj, string fieldName, object value)
    {
        var field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(obj, value);
        }
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(cardObject);
        Object.DestroyImmediate(gameManagerObject);
    }

    [Test]
    public void Test_CardDisplay_SetCard_NumberCard()
    {
        // Create a simple red number card
        Card numberCard = new Card(CardColour.RED, CardValue.FIVE);
        
        // Set the card
        cardDisplay.SetCard(numberCard, null);

        // Get expected color
        var expectedColor = GameManager.instance.GetColours().red;

        // Verify only the essential properties
        ColorAssert.AreEqual(expectedColor, baseCardColour.color);
        ColorAssert.AreEqual(expectedColor, imageCenter.color);
        Assert.AreEqual("5", valueTextCenter.text, "Value text should display '5'");
        Assert.AreEqual("5", valueTextTL.text, "Top left value text should display '5'");
        Assert.AreEqual("5", valueTextBR.text, "Bottom right value text should display '5'");
    }

    [Test]
    public void Test_CardDisplay_SetCard_SkipCard()
    {
        // Create a blue skip card
        Card skipCard = new Card(CardColour.BLUE, CardValue.SKIP);
        
        // Set the card
        cardDisplay.SetCard(skipCard, null);

        // Get expected color
        var expectedColor = GameManager.instance.GetColours().blue;

        // Verify properties
        ColorAssert.AreEqual(expectedColor, baseCardColour.color);
        ColorAssert.AreEqual(expectedColor, imageCenter.color);
        Assert.IsTrue(valueImageCenter.gameObject.activeSelf, "Value image should be active for skip card");
        Assert.IsTrue(valueImageTL.gameObject.activeSelf, "Top left value image should be active");
        Assert.IsTrue(valueImageBR.gameObject.activeSelf, "Bottom right value image should be active");
        Assert.AreEqual("", valueTextCenter.text, "Value text should be empty for skip card");
        Assert.AreEqual("", valueTextTL.text, "Top left text should be empty");
        Assert.AreEqual("", valueTextBR.text, "Bottom right text should be empty");
    }

    [Test]
    public void Test_CardDisplay_SetCard_PlusTwoCard()
    {
        // Create a green plus two card
        Card plusTwoCard = new Card(CardColour.GREEN, CardValue.PLUS_TWO);
        
        // Set the card
        cardDisplay.SetCard(plusTwoCard, null);

        // Get expected color
        var expectedColor = GameManager.instance.GetColours().green;

        // Verify properties
        ColorAssert.AreEqual(expectedColor, baseCardColour.color);
        ColorAssert.AreEqual(expectedColor, imageCenter.color);
        Assert.IsTrue(valueImageCenter.gameObject.activeSelf, "Value image should be active for +2 card");
        Assert.AreEqual("", valueTextCenter.text, "Center text should be empty");
        Assert.AreEqual("+2", valueTextTL.text, "Top left text should show +2");
        Assert.AreEqual("+2", valueTextBR.text, "Bottom right text should show +2");
    }

    [Test]
    public void Test_CardDisplay_SetCard_WildCard()
    {
        // Create a wild card
        Card wildCard = new Card(CardColour.NONE, CardValue.WILD);
        
        // Set the card
        cardDisplay.SetCard(wildCard, null);

        // Get expected colors
        var expectedBlack = GameManager.instance.GetColours().black;
        var expectedRed = GameManager.instance.GetColours().red;
        var expectedBlue = GameManager.instance.GetColours().blue;
        var expectedGreen = GameManager.instance.GetColours().green;
        var expectedYellow = GameManager.instance.GetColours().yellow;

        // Verify base properties
        ColorAssert.AreEqual(expectedBlack, baseCardColour.color);
        ColorAssert.AreEqual(expectedBlack, imageCenter.color);
        Assert.IsTrue(wildImageCenter.activeSelf, "Wild image center should be active");
        Assert.IsTrue(wildImageTL.activeSelf, "Wild image top left should be active");
        Assert.IsTrue(wildImageBR.activeSelf, "Wild image bottom right should be active");
        Assert.AreEqual("", valueTextCenter.text, "All text fields should be empty");
        Assert.AreEqual("", valueTextTL.text, "All text fields should be empty");
        Assert.AreEqual("", valueTextBR.text, "All text fields should be empty");

        // Verify corner colors for center wild
        ColorAssert.AreEqual(expectedRed, topLeftCenter.color);
        ColorAssert.AreEqual(expectedYellow, bottomLeftCenter.color);
        ColorAssert.AreEqual(expectedBlue, topRightCenter.color);
        ColorAssert.AreEqual(expectedGreen, bottomRightCenter.color);

        // Verify corner colors for top left wild
        ColorAssert.AreEqual(expectedRed, topLeftTL.color);
        ColorAssert.AreEqual(expectedYellow, bottomLeftTL.color);
        ColorAssert.AreEqual(expectedBlue, topRightTL.color);
        ColorAssert.AreEqual(expectedGreen, bottomRightTL.color);

        // Verify corner colors for bottom right wild
        ColorAssert.AreEqual(expectedRed, topLeftBR.color);
        ColorAssert.AreEqual(expectedYellow, bottomLeftBR.color);
        ColorAssert.AreEqual(expectedBlue, topRightBR.color);
        ColorAssert.AreEqual(expectedGreen, bottomRightBR.color);
    }

    [Test]
    public void Test_CardDisplay_ShowHideCard()
    {
        // Create and set a card
        Card card = new Card(CardColour.YELLOW, CardValue.ONE);
        cardDisplay.SetCard(card, null);

        // Create and set up card back
        var cardBack = new GameObject("CardBack");
        cardBack.transform.SetParent(cardObject.transform, false);
        SetPrivateField(cardDisplay, "cardBack", cardBack);

        // Test show/hide functionality
        cardDisplay.HideCard();
        Assert.IsTrue(cardBack.activeSelf, "Card back should be active when hidden");

        cardDisplay.ShowCard();
        Assert.IsFalse(cardBack.activeSelf, "Card back should be inactive when shown");
    }

    private class ColorAssert
    {
        public static void AreEqual(Color32 expected, Color actual, string message = "")
        {
            var normalizedExpected = new Color(expected.r / 255f, expected.g / 255f, expected.b / 255f, expected.a / 255f);
            Assert.That(actual.r, Is.EqualTo(normalizedExpected.r).Within(0.01f), $"{message} Red component mismatch");
            Assert.That(actual.g, Is.EqualTo(normalizedExpected.g).Within(0.01f), $"{message} Green component mismatch");
            Assert.That(actual.b, Is.EqualTo(normalizedExpected.b).Within(0.01f), $"{message} Blue component mismatch");
            Assert.That(actual.a, Is.EqualTo(normalizedExpected.a).Within(0.01f), $"{message} Alpha component mismatch");
        }
    }
} 