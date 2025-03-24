using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.EventSystems;
using UnityEngine.TestTools.Utils;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[TestFixture]
public class CardInteractionTests
{
    private GameObject cardObject;
    private CardInteraction cardInteraction;
    private CardDisplay cardDisplay;
    private Player testPlayer;
    private GameObject gameManagerObject;
    private GameManager gameManager;
    private EventSystem eventSystem;
    private Card testCard;
    private GameObject deckObject;
    private Deck deck;

    [SetUp]
    public void SetUp()
    {
        // Create EventSystem
        GameObject eventSystemObj = new GameObject("EventSystem");
        eventSystem = eventSystemObj.AddComponent<EventSystem>();
        eventSystemObj.AddComponent<StandaloneInputModule>();

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

        // Create main card object
        cardObject = new GameObject("CardHolder", typeof(RectTransform));
        cardObject.transform.localPosition = Vector3.zero;
        
        // Add Canvas components
        cardObject.AddComponent<Canvas>();
        cardObject.AddComponent<CanvasScaler>();
        cardObject.AddComponent<GraphicRaycaster>();

        // Add CardDisplay and CardInteraction
        cardDisplay = cardObject.AddComponent<CardDisplay>();
        cardInteraction = cardObject.AddComponent<CardInteraction>();

        // Create all the required UI components
        CreateCardUIComponents(cardObject, cardDisplay);

        // Set up the card with a player
        var player = new Player("Test Player", true);
        var card = new Card(CardColour.RED, CardValue.ONE);
        cardDisplay.SetCard(card, player);

        // Manually call Start to initialize CardInteraction
        SetPrivateField(cardInteraction, "cardDisplay", cardDisplay);
        SetPrivateField(cardInteraction, "originalPosition", Vector3.zero);
        SetPrivateField(cardInteraction, "liftAmount", 30f);
    }

    private WildButton CreateWildButton(string name)
    {
        var buttonObj = new GameObject(name);
        var button = buttonObj.AddComponent<WildButton>();
        buttonObj.AddComponent<Image>();
        return button;
    }

    private void CreateCardUIComponents(GameObject parent, CardDisplay targetDisplay)
    {
        // Create base components
        var baseCard = CreateImageObject(parent, "BaseCard");
        var imageCenter = CreateImageObject(parent, "ImageCenter");
        var valueImageCenter = CreateImageObject(parent, "ValueImageCenter");
        var valueTextCenter = CreateTextObject(parent, "ValueTextCenter");
        var wildImageCenter = new GameObject("WildImageCenter", typeof(RectTransform));
        wildImageCenter.transform.SetParent(parent.transform, false);

        // Create corner components
        var topLeftCenter = CreateImageObject(parent, "TopLeftCenter");
        var bottomLeftCenter = CreateImageObject(parent, "BottomLeftCenter");
        var topRightCenter = CreateImageObject(parent, "TopRightCenter");
        var bottomRightCenter = CreateImageObject(parent, "BottomRightCenter");

        // Top Left Corner
        var valueImageTL = CreateImageObject(parent, "ValueImageTL");
        var valueTextTL = CreateTextObject(parent, "ValueTextTL");
        var wildImageTL = new GameObject("WildImageTL", typeof(RectTransform));
        wildImageTL.transform.SetParent(parent.transform, false);
        var topLeftTL = CreateImageObject(parent, "TopLeftTL");
        var bottomLeftTL = CreateImageObject(parent, "BottomLeftTL");
        var topRightTL = CreateImageObject(parent, "TopRightTL");
        var bottomRightTL = CreateImageObject(parent, "BottomRightTL");

        // Bottom Right Corner
        var valueImageBR = CreateImageObject(parent, "ValueImageBR");
        var valueTextBR = CreateTextObject(parent, "ValueTextBR");
        var wildImageBR = new GameObject("WildImageBR", typeof(RectTransform));
        wildImageBR.transform.SetParent(parent.transform, false);
        var topLeftBR = CreateImageObject(parent, "TopLeftBR");
        var bottomLeftBR = CreateImageObject(parent, "BottomLeftBR");
        var topRightBR = CreateImageObject(parent, "TopRightBR");
        var bottomRightBR = CreateImageObject(parent, "BottomRightBR");

        // Card Back
        var cardBack = new GameObject("CardBack", typeof(RectTransform));
        cardBack.transform.SetParent(parent.transform, false);
        cardBack.AddComponent<Image>();

        // Set references in CardDisplay
        SetPrivateField(targetDisplay, "baseCardColour", baseCard.GetComponent<Image>());
        SetPrivateField(targetDisplay, "imageCenter", imageCenter.GetComponent<Image>());
        SetPrivateField(targetDisplay, "valueImageCenter", valueImageCenter.GetComponent<Image>());
        SetPrivateField(targetDisplay, "valueTextCenter", valueTextCenter.GetComponent<TextMeshProUGUI>());
        SetPrivateField(targetDisplay, "wildImageCenter", wildImageCenter);
        SetPrivateField(targetDisplay, "topLeftCenter", topLeftCenter.GetComponent<Image>());
        SetPrivateField(targetDisplay, "bottomLeftCenter", bottomLeftCenter.GetComponent<Image>());
        SetPrivateField(targetDisplay, "topRightCenter", topRightCenter.GetComponent<Image>());
        SetPrivateField(targetDisplay, "bottomRightCenter", bottomRightCenter.GetComponent<Image>());
        SetPrivateField(targetDisplay, "valueImageTL", valueImageTL.GetComponent<Image>());
        SetPrivateField(targetDisplay, "valueTextTL", valueTextTL.GetComponent<TextMeshProUGUI>());
        SetPrivateField(targetDisplay, "wildImageTL", wildImageTL);
        SetPrivateField(targetDisplay, "topLeftTL", topLeftTL.GetComponent<Image>());
        SetPrivateField(targetDisplay, "bottomLeftTL", bottomLeftTL.GetComponent<Image>());
        SetPrivateField(targetDisplay, "topRightTL", topRightTL.GetComponent<Image>());
        SetPrivateField(targetDisplay, "bottomRightTL", bottomRightTL.GetComponent<Image>());
        SetPrivateField(targetDisplay, "valueImageBR", valueImageBR.GetComponent<Image>());
        SetPrivateField(targetDisplay, "valueTextBR", valueTextBR.GetComponent<TextMeshProUGUI>());
        SetPrivateField(targetDisplay, "wildImageBR", wildImageBR);
        SetPrivateField(targetDisplay, "topLeftBR", topLeftBR.GetComponent<Image>());
        SetPrivateField(targetDisplay, "bottomLeftBR", bottomLeftBR.GetComponent<Image>());
        SetPrivateField(targetDisplay, "topRightBR", topRightBR.GetComponent<Image>());
        SetPrivateField(targetDisplay, "bottomRightBR", bottomRightBR.GetComponent<Image>());
        SetPrivateField(targetDisplay, "cardBack", cardBack);
    }

    private GameObject CreateImageObject(GameObject parent, string name)
    {
        var obj = new GameObject(name, typeof(RectTransform));
        obj.transform.SetParent(parent.transform, false);
        obj.AddComponent<Image>();
        return obj;
    }

    private GameObject CreateTextObject(GameObject parent, string name)
    {
        var obj = new GameObject(name, typeof(RectTransform));
        obj.transform.SetParent(parent.transform, false);
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
        Object.DestroyImmediate(eventSystem.gameObject);
        Object.DestroyImmediate(deckObject);
    }

    [UnityTest]
    public IEnumerator Test_CardInteraction_HoverEffect()
    {
        // Wait for initial setup
        yield return new WaitForEndOfFrame();

        // Create pointer event data
        var eventData = new PointerEventData(eventSystem);

        // Store initial position
        Vector3 initialPosition = cardObject.transform.localPosition;
        Assert.AreEqual(Vector3.zero, initialPosition, "Initial position should be zero");

        // Trigger hover enter
        cardInteraction.OnPointerEnter(eventData);
        yield return null;

        // Check if card is lifted
        Vector3 liftedPosition = initialPosition + new Vector3(0, 30f, 0);
        Assert.AreEqual(liftedPosition, cardObject.transform.localPosition, "Card should be lifted when hovered");

        // Trigger hover exit
        cardInteraction.OnPointerExit(eventData);
        yield return null;

        // Check if card returns to original position
        Assert.AreEqual(initialPosition, cardObject.transform.localPosition, "Card should return to original position after hover");
    }

    [UnityTest]
    public IEnumerator Test_CardInteraction_ClickToPlayCard()
    {
        // Wait for initial setup
        yield return new WaitForEndOfFrame();

        // Set up game state for playing a card
        var humanPlayer = new Player("Test Player", true);
        var testCard = new Card(CardColour.RED, CardValue.ONE);
        humanPlayer.DrawCard(testCard);

        // Set up GameManager's player list and game state
        var players = new List<Player> { humanPlayer };
        SetPrivateField(GameManager.instance, "players", players);
        SetPrivateField(GameManager.instance, "currentPlayer", 0); // Set current player to human player
        SetPrivateField(GameManager.instance, "gameStarted", true);
        SetPrivateField(GameManager.instance, "gameOver", false);
        SetPrivateField(GameManager.instance, "playDirection", 1);

        // Set up the card display
        cardDisplay.SetCard(testCard, humanPlayer);

        // Create top card display
        var topCardObject = new GameObject("TopCard", typeof(RectTransform));
        var topCardDisplay = topCardObject.AddComponent<CardDisplay>();
        CreateCardUIComponents(topCardObject, topCardDisplay);
        var topCard = new Card(CardColour.RED, CardValue.FIVE);
        topCardDisplay.SetCard(topCard, null); // null player since it's the top card

        // Set up GameManager state
        SetPrivateField(GameManager.instance, "topCard", topCardDisplay);
        SetPrivateField(GameManager.instance, "currentColour", CardColour.RED);
        SetPrivateField(GameManager.instance, "currentValue", CardValue.FIVE);
        SetPrivateField(GameManager.instance, "humanHasTurn", true);

        // Set up discard pile
        var discardPile = new List<Card> { topCard };
        SetPrivateField(GameManager.instance, "discardPile", discardPile);

        // Create pointer event data
        var eventData = new PointerEventData(eventSystem);

        // Store initial position
        Vector3 initialPosition = cardObject.transform.localPosition;

        // First lift the card
        cardInteraction.OnPointerEnter(eventData);
        yield return null;

        // Verify card is lifted
        Vector3 liftedPosition = initialPosition + new Vector3(0, 30f, 0);
        Assert.AreEqual(liftedPosition, cardObject.transform.localPosition, "Card should be lifted before clicking");

        // Verify initial state
        Assert.AreEqual(1, humanPlayer.playerHand.Count, "Player should have one card before playing");
        Assert.IsTrue((bool)GetPrivateField(GameManager.instance, "humanHasTurn"), "Should be human's turn");
        Assert.AreEqual(CardColour.RED, (CardColour)GetPrivateField(GameManager.instance, "currentColour"), "Current color should be red");
        Assert.AreEqual(0, (int)GetPrivateField(GameManager.instance, "currentPlayer"), "Current player should be 0 (human)");

        // Click the card
        cardInteraction.OnPointerClick(eventData);
        yield return null;

        // Trigger hover exit to ensure card is lowered
        cardInteraction.OnPointerExit(eventData);
        yield return new WaitForSeconds(0.1f); // Wait a bit for any animations

        // Verify card returns to original position after click
        Assert.AreEqual(initialPosition, cardObject.transform.localPosition, "Card should return to original position after click");

        // Verify the card was played (removed from player's hand)
        Assert.AreEqual(0, humanPlayer.playerHand.Count, "Card should be removed from player's hand");
        Assert.AreEqual(2, ((List<Card>)GetPrivateField(GameManager.instance, "discardPile")).Count, "Discard pile should have 2 cards");

        // Clean up
        Object.DestroyImmediate(topCardObject);
    }

    private object GetPrivateField(object obj, string fieldName)
    {
        var field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return field?.GetValue(obj);
    }
} 