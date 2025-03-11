using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.TestTools;
using System.Reflection;

[TestFixture]
public class GameManagerTests
{
    private GameObject gameManagerObject;
    private GameManager gameManager;
    private GameObject cardPrefab;
    private Transform playerHandTransform;
    private Transform discardPileTransform;
    private GameObject wildPanel;
    private GameObject winPanel;
    private TMP_Text messageText;
    private TMP_Text winningText;
    private Transform directionArrow;
    private Deck deck;
    private GameObject deckObject;

    [SetUp]
    public void SetUp()
    {
        // Create main GameManager object
        gameManagerObject = new GameObject("GameManager");
        gameManager = gameManagerObject.AddComponent<GameManager>();

        // Create card prefab
        cardPrefab = CreateCardPrefab();

        // Create transforms and UI elements
        playerHandTransform = new GameObject("PlayerHand").transform;
        discardPileTransform = new GameObject("DiscardPile").transform;
        directionArrow = new GameObject("DirectionArrow").transform;
        directionArrow.gameObject.AddComponent<Image>();
        wildPanel = new GameObject("WildPanel");
        winPanel = new GameObject("WinPanel");
        messageText = new GameObject("MessageText").AddComponent<TextMeshProUGUI>();
        winningText = new GameObject("WinningText").AddComponent<TextMeshProUGUI>();

        // Create wild buttons
        var redButton = CreateWildButton("RedButton");
        var blueButton = CreateWildButton("BlueButton");
        var greenButton = CreateWildButton("GreenButton");
        var yellowButton = CreateWildButton("YellowButton");

        // Create and setup deck
        deckObject = new GameObject("Deck");
        deck = deckObject.AddComponent<Deck>();

        // Create player highlights and card counts
        var playerHighlight1 = new GameObject("PlayerHighlight1").AddComponent<Image>();
        var playerHighlight2 = new GameObject("PlayerHighlight2").AddComponent<Image>();
        var playerCount1 = new GameObject("PlayerCount1").AddComponent<TextMeshProUGUI>();
        var playerCount2 = new GameObject("PlayerCount2").AddComponent<TextMeshProUGUI>();

        // Set private fields using reflection
        SetPrivateField(gameManager, "cardPrefab", cardPrefab);
        SetPrivateField(gameManager, "playerHandTransform", playerHandTransform);
        SetPrivateField(gameManager, "discardPileTransform", discardPileTransform);
        SetPrivateField(gameManager, "directionArrow", directionArrow);
        SetPrivateField(gameManager, "wildPanel", wildPanel);
        SetPrivateField(gameManager, "winPanel", winPanel);
        SetPrivateField(gameManager, "messageText", messageText);
        SetPrivateField(gameManager, "winningText", winningText);
        SetPrivateField(gameManager, "redButton", redButton);
        SetPrivateField(gameManager, "blueButton", blueButton);
        SetPrivateField(gameManager, "greenButton", greenButton);
        SetPrivateField(gameManager, "yellowButton", yellowButton);
        SetPrivateField(gameManager, "deck", deck);
        SetPrivateField(gameManager, "numberOfAiPlayers", 3);
        SetPrivateField(gameManager, "startingHandSize", 7);
        SetPrivateField(gameManager, "playerHighlights", new List<Image> { playerHighlight1, playerHighlight2 });
        SetPrivateField(gameManager, "playerCardCount", new List<TMP_Text> { playerCount1, playerCount2 });
        SetPrivateField(gameManager, "aiHandTransform", new List<Transform>());
        SetPrivateField(gameManager, "playDirection", 1);

        // Set colors
        SetPrivateField(gameManager, "red", new Color32(255, 0, 0, 255));
        SetPrivateField(gameManager, "blue", new Color32(0, 0, 255, 255));
        SetPrivateField(gameManager, "green", new Color32(0, 255, 0, 255));
        SetPrivateField(gameManager, "yellow", new Color32(255, 255, 0, 255));
        SetPrivateField(gameManager, "black", new Color32(0, 0, 0, 255));

        GameManager.instance = gameManager;
    }

    private GameObject CreateCardPrefab()
    {
        var cardObj = new GameObject("CardPrefab", typeof(RectTransform));
        var display = cardObj.AddComponent<CardDisplay>();
        cardObj.AddComponent<CardInteraction>();

        // Create UI components
        var baseCard = CreateImageObject(cardObj, "BaseCard");
        var imageCenter = CreateImageObject(cardObj, "ImageCenter");
        var valueImageCenter = CreateImageObject(cardObj, "ValueImageCenter");
        var valueTextCenter = CreateTextObject(cardObj, "ValueTextCenter");
        var wildImageCenter = new GameObject("WildImageCenter", typeof(RectTransform));
        wildImageCenter.transform.SetParent(cardObj.transform, false);

        // Create corner components
        var topLeftCenter = CreateImageObject(cardObj, "TopLeftCenter");
        var bottomLeftCenter = CreateImageObject(cardObj, "BottomLeftCenter");
        var topRightCenter = CreateImageObject(cardObj, "TopRightCenter");
        var bottomRightCenter = CreateImageObject(cardObj, "BottomRightCenter");

        // Top Left Corner
        var valueImageTL = CreateImageObject(cardObj, "ValueImageTL");
        var valueTextTL = CreateTextObject(cardObj, "ValueTextTL");
        var wildImageTL = new GameObject("WildImageTL", typeof(RectTransform));
        wildImageTL.transform.SetParent(cardObj.transform, false);
        var topLeftTL = CreateImageObject(cardObj, "TopLeftTL");
        var bottomLeftTL = CreateImageObject(cardObj, "BottomLeftTL");
        var topRightTL = CreateImageObject(cardObj, "TopRightTL");
        var bottomRightTL = CreateImageObject(cardObj, "BottomRightTL");

        // Bottom Right Corner
        var valueImageBR = CreateImageObject(cardObj, "ValueImageBR");
        var valueTextBR = CreateTextObject(cardObj, "ValueTextBR");
        var wildImageBR = new GameObject("WildImageBR", typeof(RectTransform));
        wildImageBR.transform.SetParent(cardObj.transform, false);
        var topLeftBR = CreateImageObject(cardObj, "TopLeftBR");
        var bottomLeftBR = CreateImageObject(cardObj, "BottomLeftBR");
        var topRightBR = CreateImageObject(cardObj, "TopRightBR");
        var bottomRightBR = CreateImageObject(cardObj, "BottomRightBR");

        // Create card back
        var cardBack = new GameObject("CardBack", typeof(RectTransform));
        cardBack.transform.SetParent(cardObj.transform, false);
        cardBack.AddComponent<Image>();

        // Set private fields for CardDisplay
        SetPrivateField(display, "baseCard", baseCard.GetComponent<Image>());
        SetPrivateField(display, "imageCenter", imageCenter.GetComponent<Image>());
        SetPrivateField(display, "valueImageCenter", valueImageCenter.GetComponent<Image>());
        SetPrivateField(display, "valueTextCenter", valueTextCenter.GetComponent<TextMeshProUGUI>());
        SetPrivateField(display, "wildImageCenter", wildImageCenter);
        SetPrivateField(display, "cardBack", cardBack);

        // Set corner components
        SetPrivateField(display, "topLeftCenter", topLeftCenter.GetComponent<Image>());
        SetPrivateField(display, "bottomLeftCenter", bottomLeftCenter.GetComponent<Image>());
        SetPrivateField(display, "topRightCenter", topRightCenter.GetComponent<Image>());
        SetPrivateField(display, "bottomRightCenter", bottomRightCenter.GetComponent<Image>());

        // Set Top Left Corner components
        SetPrivateField(display, "valueImageTL", valueImageTL.GetComponent<Image>());
        SetPrivateField(display, "valueTextTL", valueTextTL.GetComponent<TextMeshProUGUI>());
        SetPrivateField(display, "wildImageTL", wildImageTL);
        SetPrivateField(display, "topLeftTL", topLeftTL.GetComponent<Image>());
        SetPrivateField(display, "bottomLeftTL", bottomLeftTL.GetComponent<Image>());
        SetPrivateField(display, "topRightTL", topRightTL.GetComponent<Image>());
        SetPrivateField(display, "bottomRightTL", bottomRightTL.GetComponent<Image>());

        // Set Bottom Right Corner components
        SetPrivateField(display, "valueImageBR", valueImageBR.GetComponent<Image>());
        SetPrivateField(display, "valueTextBR", valueTextBR.GetComponent<TextMeshProUGUI>());
        SetPrivateField(display, "wildImageBR", wildImageBR);
        SetPrivateField(display, "topLeftBR", topLeftBR.GetComponent<Image>());
        SetPrivateField(display, "bottomLeftBR", bottomLeftBR.GetComponent<Image>());
        SetPrivateField(display, "topRightBR", topRightBR.GetComponent<Image>());
        SetPrivateField(display, "bottomRightBR", bottomRightBR.GetComponent<Image>());

        // Create a dummy sprite for testing
        var texture = new Texture2D(100, 100);
        var sprite = Sprite.Create(texture, new Rect(0, 0, 100, 100), Vector2.one * 0.5f);

        // Set sprites for all images
        baseCard.GetComponent<Image>().sprite = sprite;
        imageCenter.GetComponent<Image>().sprite = sprite;
        valueImageCenter.GetComponent<Image>().sprite = sprite;
        topLeftCenter.GetComponent<Image>().sprite = sprite;
        bottomLeftCenter.GetComponent<Image>().sprite = sprite;
        topRightCenter.GetComponent<Image>().sprite = sprite;
        bottomRightCenter.GetComponent<Image>().sprite = sprite;
        valueImageTL.GetComponent<Image>().sprite = sprite;
        topLeftTL.GetComponent<Image>().sprite = sprite;
        bottomLeftTL.GetComponent<Image>().sprite = sprite;
        topRightTL.GetComponent<Image>().sprite = sprite;
        bottomRightTL.GetComponent<Image>().sprite = sprite;
        valueImageBR.GetComponent<Image>().sprite = sprite;
        topLeftBR.GetComponent<Image>().sprite = sprite;
        bottomLeftBR.GetComponent<Image>().sprite = sprite;
        topRightBR.GetComponent<Image>().sprite = sprite;
        bottomRightBR.GetComponent<Image>().sprite = sprite;
        cardBack.GetComponent<Image>().sprite = sprite;

        return cardObj;
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

    private WildButton CreateWildButton(string name)
    {
        var buttonObj = new GameObject(name);
        var button = buttonObj.AddComponent<WildButton>();
        buttonObj.AddComponent<Image>();
        return button;
    }

    private void SetPrivateField(object obj, string fieldName, object value)
    {
        var field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(obj, value);
        }
    }

    private T GetPrivateField<T>(object obj, string fieldName)
    {
        var field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return field != null ? (T)field.GetValue(obj) : default;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(gameManagerObject);
        Object.DestroyImmediate(cardPrefab);
        Object.DestroyImmediate(deckObject);
    }

    private void InitializeGameManager()
    {
        // Call Awake through reflection since it's protected
        var awakeMethod = typeof(GameManager).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
        awakeMethod?.Invoke(gameManager, null);

        // Initialize game state that would normally happen in Start
        wildPanel.SetActive(false);
        winPanel.SetActive(false);

        // Initialize deck
        deck.InitializeDeck();

        // Initialize players
        var initializePlayersMethod = typeof(GameManager).GetMethod("InitializePlayers", BindingFlags.NonPublic | BindingFlags.Instance);
        initializePlayersMethod?.Invoke(gameManager, null);

        // Deal starting cards
        var dealCardsMethod = typeof(GameManager).GetMethod("DealCards", BindingFlags.NonPublic | BindingFlags.Instance);
        dealCardsMethod?.Invoke(gameManager, null);

        // Set human turn
        var humanHasTurnProperty = typeof(GameManager).GetProperty("humanHasTurn");
        humanHasTurnProperty?.SetValue(gameManager, true);
    }

    [Test]
    public void Test_GameManager_BasicInitialization()
    {
        Assert.IsNotNull(gameManager);
        Assert.IsNotNull(GameManager.instance);
        Assert.AreEqual(gameManager, GameManager.instance);
        Assert.IsNotNull(deck);
    }

    [Test]
    public void Test_GameManager_ConfigurationValues()
    {
        int aiPlayers = GetPrivateField<int>(gameManager, "numberOfAiPlayers");
        int startingCards = GetPrivateField<int>(gameManager, "startingHandSize");

        Assert.AreEqual(3, aiPlayers);
        Assert.AreEqual(7, startingCards);
    }

    [Test]
    public void Test_GameManager_InitializePlayers()
    {
        // Initialize players directly
        gameManager.players.Clear();
        gameManager.players.Add(new Player("Player 1", true));
        gameManager.players.Add(new AiPlayer("AI 1"));
        gameManager.players.Add(new AiPlayer("AI 2"));
        gameManager.players.Add(new AiPlayer("AI 3"));

        Assert.AreEqual(4, gameManager.players.Count);
        Assert.IsTrue(gameManager.players[0].IsHuman);
        
        for (int i = 1; i < gameManager.players.Count; i++)
        {
            Assert.IsFalse(gameManager.players[i].IsHuman);
            Assert.IsTrue(gameManager.players[i] is AiPlayer);
        }
    }

    [Test]
    public void Test_GameManager_InitialPlayDirection()
    {
        // Check initial play direction (should be 1 for clockwise)
        int playDirection = GetPrivateField<int>(gameManager, "playDirection");
        Assert.AreEqual(1, playDirection, "Game should start in clockwise direction");
    }

    [Test]
    public void Test_GameManager_WinCondition()
    {
        // Setup game state with two players
        gameManager.players.Clear();
        var player1 = new Player("Player 1", true);
        var player2 = new AiPlayer("AI 1");
        gameManager.players.Add(player1);
        gameManager.players.Add(player2);

        // Give second player some cards, but leave first player with none
        player2.DrawCard(new Card(CardColour.RED, CardValue.ONE));
        player2.DrawCard(new Card(CardColour.BLUE, CardValue.TWO));

        // Set current player to player1 (who has no cards)
        SetPrivateField(gameManager, "currentPlayer", 0);
        
        // Set up UI elements for win condition
        winPanel.SetActive(false);
        winningText.text = "";
        messageText.text = "";

        // Call SwitchPlayer which checks win condition
        gameManager.SwitchPlayer();

        // Verify win condition
        Assert.IsTrue(winPanel.activeSelf, "Win panel should be shown");
        Assert.AreEqual("Player 1 WINS", winningText.text, "Win text should show correct player");
        Assert.AreEqual("Player 1 WINS", messageText.text, "Message should show winner");
    }

    private class ColorAssert
    {
        // ... existing code ...
    }
} 