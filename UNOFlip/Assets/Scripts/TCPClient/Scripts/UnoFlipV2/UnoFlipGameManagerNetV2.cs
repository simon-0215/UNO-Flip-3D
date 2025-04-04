using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

using QFramework;
using MyTcpClient;
using UnoFlipV2;
using UnityEngine.UI;
using UnityEditor;

public class UnoFlipGameManagerNetV2 : MonoBehaviour, IController
{
    public IArchitecture GetArchitecture()
    {
        return UnoFlipAppV2.Interface;
    }
    UnoFlipModelV2 model;
    UnoFlipGameSystemV2 game;
    static int logActionCount = 0;

    #region UI game objects
    [Header("Colours")]
    //light side
    [SerializeField] Color32 red;
    [SerializeField] Color32 blue;
    [SerializeField] Color32 green;
    [SerializeField] Color32 yellow;
    [SerializeField] Color32 black;

    //dark side
    [SerializeField] Color32 redDark;
    [SerializeField] Color32 blueDark;
    [SerializeField] Color32 greenDark;
    [SerializeField] Color32 yellowDark;
    Color32 getColor(CardColor color)
    {
        if(model.side == Side.Light)
        {

        }
        if(color == CardColor.RED)
        {
            return model.side == Side.Light ? red : redDark;
        }
        else if(color == CardColor.BLUE)
        {
            return model.side == Side.Light ? blue : blueDark;
        }
        else if(color == CardColor.GREEN)
        {
            return model.side == Side.Light ? green : greenDark;
        }
        else if(color == CardColor.YELLOW)
        {
            return model.side == Side.Light ? yellow : yellowDark;
        }
        return black;
    }
    //4个角色的player name 、 hand cards count
    private TMP_Text txtPlayerName1;
    private TMP_Text txtPlayerHandNumber1;
    private TMP_Text txtPlayerName2;
    private TMP_Text txtPlayerHandNumber2;
    private TMP_Text txtPlayerName3;
    private TMP_Text txtPlayerHandNumber3;
    private TMP_Text txtPlayerName4;
    private TMP_Text txtPlayerHandNumber4;

    private TMP_Text txtMessage;//提示消息文本

    private Transform handCardsPosition;//4个角色手牌位置
    [SerializeField] GameObject cardPrefab;
    private Transform discardPileTransform; //弃牌堆

    private Transform directionArrow;
    private Image nextColorImg;

    private GameObject wildPanel;
    private WildButton redButton;
    private WildButton blueButton;
    private WildButton greenButton;
    private WildButton yellowButton;

    private GameObject winPanel;
    private TMP_Text winningText;

    private Button unoButton;
    #endregion

    void printHandCards()
    {
        for (int i = 0; i < model.initData.roles.Count; i++)
        {
            print($"role {model.initData.roles[i]} 手牌:{string.Join("__", model.rolesHandCard[i].Select(c => c.ToString()))}");
        }
    }

    private void Start()
    {
        model = this.GetModel<UnoFlipModelV2>();
        game = this.GetSystem<UnoFlipGameSystemV2>();

        #region UI game objects
        CardDisplay.red = red;
        CardDisplay.blue = blue;
        CardDisplay.green = green;
        CardDisplay.yellow = yellow;
        CardDisplay.black = black;

        CardDisplay.redDark = redDark;
        CardDisplay.blueDark = blueDark;
        CardDisplay.greenDark = greenDark;
        CardDisplay.yellowDark = yellowDark;

        txtPlayerName1 = transform.Find("Canvas/Positions/TextName1").GetComponent<TMP_Text>();
        txtPlayerHandNumber1 = transform.Find("Canvas/Positions/TextName1/Highlight/Number").GetComponent<TMP_Text>();

        txtPlayerName2 = transform.Find("Canvas/Positions/TextName2").GetComponent<TMP_Text>();
        txtPlayerHandNumber2 = transform.Find("Canvas/Positions/TextName2/Highlight/Number").GetComponent<TMP_Text>();

        txtPlayerName3 = transform.Find("Canvas/Positions/TextName3").GetComponent<TMP_Text>();
        txtPlayerHandNumber3 = transform.Find("Canvas/Positions/TextName3/Highlight/Number").GetComponent<TMP_Text>();

        txtPlayerName4 = transform.Find("Canvas/Positions/TextName4").GetComponent<TMP_Text>();
        txtPlayerHandNumber4 = transform.Find("Canvas/Positions/TextName4/Highlight/Number").GetComponent<TMP_Text>();

        txtMessage = transform.Find("Canvas/MessageBox/MessageText").GetComponent<TMP_Text>();
        ShowNotice("Welcome");

        handCardsPosition = transform.Find("Canvas/Positions/HandCardsPos");

        discardPileTransform = transform.Find("Canvas/Positions/DiscardPile");

        directionArrow = transform.Find("Canvas/DirectionArrow");
        nextColorImg = transform.Find("Canvas/NextColor").GetComponent<Image>();

        wildPanel = transform.Find("Canvas/WildPanel").gameObject;
        redButton = transform.Find("Canvas/WildPanel/Grid/Red").GetComponent<WildButton>();
        blueButton = transform.Find("Canvas/WildPanel/Grid/Blue").GetComponent<WildButton>();
        greenButton = transform.Find("Canvas/WildPanel/Grid/Green").GetComponent<WildButton>();
        yellowButton = transform.Find("Canvas/WildPanel/Grid/Yellow").GetComponent<WildButton>();

        wildPanel.SetActive(false);
        redButton.SetImageColour(red);
        blueButton.SetImageColour(blue);
        greenButton.SetImageColour(green);
        yellowButton.SetImageColour(yellow);

        winPanel = transform.Find("Canvas/WinPanel").gameObject;
        winningText = transform.Find("Canvas/WinPanel/Panel/WinningText").GetComponent<TMP_Text>();

        unoButton = transform.Find("Canvas/UnoButton/Button").GetComponent<Button>();
        unoButton.onClick.AddListener(() =>
        {
            game.UnoClick();
        });
        #endregion

        this.RegisterEvent<NetOnMsgEventV2>(e =>
        {
            switch (e.type.Name)
            {
                case nameof(MsgInitFlipGame):
                    MsgInitFlipGame msgInitGame = (MsgInitFlipGame)e.msg;
                    print($"收到消息 init game {msgInitGame.roles.Count} ({msgInitGame.roles[0]},{msgInitGame.roles[1]},{msgInitGame.roles[2]},{msgInitGame.roles[3]}) {msgInitGame.firstToPlay} {msgInitGame.myIdx} {msgInitGame.startHandNum}");
                    model.initData = msgInitGame;
                    model.currentRoleIdx = model.initData.firstToPlay;

                    StartCoroutine(Init());
                    break;
                case nameof(MsgInitDealCards):
                    MsgInitDealCards msgDealCards = (MsgInitDealCards)e.msg;
                    print($"收到消息 发牌 {msgDealCards.content}");

                    StartCoroutine(InitDealCards(msgDealCards));
                    break;
                case nameof(MsgFirstPileCard):
                    MsgFirstPileCard msgFirstCard = (MsgFirstPileCard)e.msg;
                    print($"收到消息 第一张翻牌 {msgFirstCard.cardId} {msgFirstCard.cardContent} {UnoFlipV2.Card.Decode(msgFirstCard.cardContent)}");
                    StartCoroutine(InitFirstPileCard(msgFirstCard));
                    break;
                case nameof(MsgRoleActionResult):
                    MsgRoleActionResult msgR = (MsgRoleActionResult)e.msg;
                    UnoFlipV2.Card c = model.rolesHandCard[msgR.gameState.lastRoleIdx].Find(c => c.id == msgR.action.cardId);

                    Debug.Log("");
                    Debug.Log("");
                    Debug.Log("");
                    logActionCount++;
                    Debug.Log($"action count:{logActionCount.ToString()}");
                    print("------------------------------------------------ 同步前：");
                    print($"收到消息 {model.initData.roles[msgR.gameState.lastRoleIdx]}_action结果同步 action:{msgR.action.ToString()} _ nextRole{model.initData.roles[msgR.gameState.currentRoleIdx]}");
                    print($"gameState: {msgR.gameState.ToString()}");
                    if (c != null) { print($"本次出牌: {c.ToString()} __ {c.Encode()}"); }
                    
                    printHandCards();
                    StartCoroutine(GameStateSync(msgR));
                    break;
                case nameof(MsgRoleUno):
                    print("收到消息 role declare");
                    MsgRoleUno msgUno = (MsgRoleUno)e.msg;
                    ShowNotice($"{model.initData.roles[msgUno.roleIdx]} UNO!");
                    break;
            }
        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        this.RegisterEvent<CardClickedEvent>(e =>
        {
            if (IsMyTurn() == false) return;

            //判断是否合法出牌
            if (game.IsPlayable(e.cardData) == false)
            {
                ShowNotice($"Invalid move!");
                print($"side:{model.side} card:{e.cardData} sideData:{e.cardData.GetData(model.side)}");
                return;
            }

            model.gameState = FlipGameState.actionedWaitResult;

            //发起RoleAction网络请求
            MsgPlayerAction msg = new MsgPlayerAction();
            CardColor testColor = PickRandomColour();//just for development debug
            msg.action = new RoleAction(RoleActionType.playCard, e.cardData.id, testColor);
            NetManager.Send(msg);

            ShowNotice("Processing your move...");
            CardSideData data = model.side == Side.Light ? e.cardData.light : e.cardData.dark;
            print($"--- 出牌 {e.cardData.id} {data.type} {data.color} {data.value}");
        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        this.RegisterEvent<DeckClickedEvent>(e =>
        {
            if(IsMyTurn() == false)
            {
                ShowNotice("Not Your Turn");
                return;
            }

            //抽1张牌的逻辑，在发送消息那里实现 ##1
            model.rolesHandCard[model.myIdx].Add(model.nextDeckCard);
            AddHandCard(model.myIdx, model.nextDeckCard);
            model.nextDeckCard = null;
            //yield return new WaitForSeconds(0.8f);

            model.gameState = FlipGameState.actionedWaitResult;
            MsgPlayerAction msg = new MsgPlayerAction();
            msg.action = new RoleAction(RoleActionType.drawCard);
            NetManager.Send(msg);
            ShowNotice("Processing your move...");
            print("--- 从牌堆摸牌");

        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        this.RegisterEvent<UnoClickEvent>(e =>
        {
            if (model.rolesHandCard[model.myIdx].Count != 1)
            {
                ShowNotice("1 card left to UNO!");
                return;
            }
            if (model.unoDeclared)
            {
                ShowNotice("already UNO!");
                return;
            }
            NetManager.Send(new MsgRoleUno(model.myIdx));
        });
    }

    #region view control
    #region utils
    void ShowNotice(string message)
    {
        print("--- notice: " + message);
        txtMessage.text = message;
    }
    /// <summary>
    /// 解析手牌字符串
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static List<UnoFlipV2.Card> ParseHandCards(string str)
    {
        return UnoFlipV2.Card.ParseHandCards(str);
    }

    void GenerateCardGameObject(int roleIdx, int cardIdx)
    {
        //print($"--- roleIdx {roleIdx}  cardidx {cardIdx}");
        UnoFlipV2.Card cardData = model.rolesHandCard[roleIdx][cardIdx];//卡牌数据
        //print($"--- card data {cardData}");

        AddHandCard(roleIdx, cardData);
    }

    void MoveCardToPile(CardDisplay cardDisplay)
    {
        GameObject currentCard = cardDisplay.transform.parent.gameObject;
        currentCard.transform.SetParent(discardPileTransform);
        currentCard.transform.localPosition = Vector3.zero;
        currentCard.transform.localScale = Vector3.one;

        RectTransform cardRect = currentCard.GetComponent<RectTransform>();
        RectTransform pileRect = discardPileTransform.GetComponent<RectTransform>();

        cardRect.sizeDelta = pileRect.sizeDelta;
        //UNHIDE CARD

        //FIX HOW CARDS ARE PLACED ON DISCARD PILE
        Quaternion rotation = discardPileTransform.rotation;
        float randomZRotation = UnityEngine.Random.Range(-10f, 10f);
        Quaternion randomRotation = Quaternion.Euler(0, 0, randomZRotation);
        currentCard.transform.rotation = rotation * randomRotation;

        cardDisplay.ShowCard();
        cardDisplay.HideFace2();
    }

    CardColor PickRandomColour()
    {
        CardColor[] colours = (CardColor[])System.Enum.GetValues(typeof(CardColor));
        int randomIndex = UnityEngine.Random.Range(0, colours.Length - 1);
        return colours[randomIndex];
    }

    bool IsMyTurn()
    {
        //if(model.gameState == FlipGameState.myAction && model.myIdx == model.currentRoleIdx)
        if(model.isMyTurn)
        {
            return true;
        }

        ShowNotice("Not Your Turn");
        print($"--- not your turn : {model.gameState} {model.myIdx} {model.currentRoleIdx} ");
        return false;
    }
    #endregion
    #region Init
    IEnumerator Init()
    {
        //更新UI my2 first1
        txtPlayerName1.text = model.initData.roles[model.initData.myIdx];
        txtPlayerHandNumber1.text = model.initData.startHandNum.ToString();
        
        txtPlayerName2.text = model.initData.roles[model.initData.myIdx + 1];
        txtPlayerHandNumber2.text = model.initData.startHandNum.ToString();
        
        int i3 = (model.initData.myIdx + 2) % 4;
        txtPlayerName3.text = model.initData.roles[i3];
        txtPlayerHandNumber3.text = model.initData.startHandNum.ToString();
        
        int i4 = (model.initData.myIdx + 3) % 4;
        txtPlayerName4.text = model.initData.roles[i4];
        txtPlayerHandNumber4.text = model.initData.startHandNum.ToString();
        
        UpdateCurrentRole();

        yield return new WaitForSeconds(1);//等待1秒后再发送确认消息
        NetManager.Send(new MsgInitFlipGame(model.initData.id));
    }
    //更新当前出牌角色
    void UpdateCurrentRole()
    {
        ShowNotice($"{model.initData.roles[model.currentRoleIdx]}'s Turn");

        txtPlayerName1.color = model.currentRoleIdx == model.initData.myIdx ? yellow : Color.white;
        txtPlayerName2.color = model.currentRoleIdx == model.initData.myIdx + 1 ? yellow : Color.white;
        int i3 = (model.initData.myIdx + 2) % 4;
        int i4 = (model.initData.myIdx + 3) % 4;
        txtPlayerName3.color = model.currentRoleIdx == i3 ? yellow : Color.white;
        txtPlayerName4.color = model.currentRoleIdx == i4 ? yellow : Color.white;
    }
    void UpdateHandCardsCount()
    {
        int i3 = (model.initData.myIdx + 2) % 4;
        int i4 = (model.initData.myIdx + 3) % 4;
        
        txtPlayerHandNumber1.text = model.rolesHandCard[model.myIdx].Count.ToString();
        txtPlayerHandNumber2.text = model.rolesHandCard[model.myIdx + 1].Count.ToString();
        txtPlayerHandNumber3.text = model.rolesHandCard[i3].Count.ToString();
        txtPlayerHandNumber4.text = model.rolesHandCard[i4].Count.ToString();
    }
    
    //游戏开始发手牌
    IEnumerator InitDealCards(MsgInitDealCards msg)
    {
        string[] roleCards = msg.content.Split('|');
        model.rolesHandCard = roleCards.Select(r => ParseHandCards(r)).ToList();
        printHandCards();

        for (int i = 0; i < model.initData.startHandNum; i++)
        {
            //从先手角色开始 轮流 发牌
            GenerateCardGameObject(model.currentRoleIdx, i);
            yield return new WaitForSeconds(0.1f);
            GenerateCardGameObject((model.currentRoleIdx + 1)%4, i);
            yield return new WaitForSeconds(0.1f);
            GenerateCardGameObject((model.currentRoleIdx + 2) % 4, i);
            yield return new WaitForSeconds(0.1f);
            GenerateCardGameObject((model.currentRoleIdx + 3) % 4, i);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.1f);

        NetManager.Send(new MsgInitDealCards(msg.id));
    }

    /// <summary>
    /// 翻开第一张弃牌堆卡牌
    /// </summary>
    IEnumerator InitFirstPileCard(MsgFirstPileCard msg)
    {
        model.nextDeckCard = UnoFlipV2.Card.Decode(msg.nextCardContent);

        UnoFlipV2.Card card = UnoFlipV2.Card.Decode(msg.cardContent);
        GameObject newCard = Instantiate(cardPrefab);
        CardDisplay display = newCard.GetComponentInChildren<CardDisplay>();
        display.SetCardDataV2(card, -1);
        display.ShowCard();
        newCard.GetComponentInChildren<CardInteractionV2>().enabled = false;

        MoveCardToPile(display);

        model.lastPlayedCard = card;
        model.nextColor = card.light.color;
        
        UpdateColor();

        yield return new WaitForSeconds(1.5f);
        NetManager.Send(new MsgFirstPileCard(msg.id));

        if(model.initData.myIdx == model.currentRoleIdx)
        {
            model.gameState = FlipGameState.myAction;
        }
        else
        {
            model.gameState = FlipGameState.waitOtherRoleAction;
        }
    }
    #endregion

    void AddHandCard(int roleIdx, UnoFlipV2.Card cardData)
    {
        //获取摆放手牌的位置
        int idx = model.initData.myIdx == 0 ? roleIdx : (roleIdx + 2) % 4;
        Transform handPos = handCardsPosition.GetChild(idx);

        //实例化
        GameObject go = Instantiate(cardPrefab, handPos, false);

        CardDisplay cardDisplay = go.GetComponentInChildren<CardDisplay>();
        //设置卡牌颜色和图案
        cardDisplay.SetCardDataV2(cardData, roleIdx);

        //设置卡牌是否显示当前面（当前客户端只能看到4个角色中自己角色的当前面，但是可以看到所有角色的另一面）
        if (roleIdx == model.initData.myIdx)
            cardDisplay.ShowCard();
    }

    CardDisplay FindCardDisplayForCard(UnoFlipV2.Card card, int roleIdx)
    {
        int idx = model.initData.myIdx == 0 ? roleIdx : (roleIdx + 2) % 4;
        Transform handPos = handCardsPosition.GetChild(idx);

        foreach (Transform cardTransform in handPos)
        {
            CardDisplay tempDisplay = cardTransform.GetComponentInChildren<CardDisplay>();
            if (tempDisplay.CardData == card)
            {
                return tempDisplay;
            }
        }
        return null;
    }
    CardDisplay FindCardDisplayForCard(int cardId, int roleIdx)
    {
        int idx = model.initData.myIdx == 0 ? roleIdx : (roleIdx + 2) % 4;
        Transform handPos = handCardsPosition.GetChild(idx);

        foreach (Transform cardTransform in handPos)
        {
            CardDisplay tempDisplay = cardTransform.GetComponentInChildren<CardDisplay>();
            if (tempDisplay.CardData.id == cardId)
            {
                return tempDisplay;
            }
        }
        return null;
    }
    void UpdateColor()
    {
        //PICK RANDOM COLOUR IF WE HAVE WILD CARD
        if (model.nextColor == CardColor.NONE)
        {
            model.nextColor = PickRandomColour();
        }
        nextColorImg.color = getColor(model.nextColor);
    }
    void FlipCards()
    {
        for(int i = 0; i < handCardsPosition.childCount; i++)
        {
            Transform handPos = handCardsPosition.GetChild(i);

            foreach (Transform cardTransform in handPos)
            {
                CardDisplay tempDisplay = cardTransform.GetComponentInChildren<CardDisplay>();
                tempDisplay.Flip();
            }
        }

        //弃牌堆顶的展示牌也要翻面
        discardPileTransform.GetChild(discardPileTransform.childCount - 1).GetComponentInChildren<CardDisplay>().Flip();
    }

    IEnumerator GameStateSync(MsgRoleActionResult msg)
    {
        GameStateToSync state = msg.gameState;
        int roleIdx = state.lastRoleIdx;
        string roleName = model.initData.roles[roleIdx];//这次同步的是哪个角色的行动
        
        //处理下一个角色因为未喊uno的抽牌逻辑
        IEnumerator handleUnoDrawCards()
        {
            yield return new WaitForSeconds(0.1f);
            if(string.IsNullOrEmpty(state.unoDrawCards) == false)
            {
                ShowNotice($"{model.initData.roles[state.currentRoleIdx]} Forgot UNO? Draw 2!");

                List<UnoFlipV2.Card> cards = ParseHandCards(state.unoDrawCards);
                foreach (UnoFlipV2.Card c in cards)
                {
                    model.rolesHandCard[state.currentRoleIdx].Add(c);
                    AddHandCard(state.currentRoleIdx, c);
                    yield return new WaitForSeconds(0.8f);
                }

                model.nextDeckCard = UnoFlipV2.Card.Decode(state.nextCardContent);//更新状态：抽牌堆下一张牌
            }
        }

        # region 处理抽牌逻辑：在本次行动第一阶段是否抽牌了
        //抽牌并且不是本人
        if (state.hasDrawCard && roleIdx != model.myIdx)
        {
            ShowNotice($"{roleName} draw a card");

            if(msg.action.cardId > 0)//抽牌后有出牌
            {
                yield return new WaitForSeconds(2.5f);
                ShowNotice($"{roleName} play the new card");
                //把该牌加入手牌
                AddHandCard(roleIdx, model.nextDeckCard);
                model.rolesHandCard[roleIdx].Add(model.nextDeckCard);
                model.nextDeckCard = UnoFlipV2.Card.Decode(state.nextCardContent);//更新状态：抽牌堆下一张牌
            }
            else//抽牌后未出牌
            {
                UnoFlipV2.Card cardData = model.nextDeckCard;//卡牌数据
                print($"--- 抽牌后未出牌 card data {cardData}");
                AddHandCard(roleIdx, model.nextDeckCard);
                model.rolesHandCard[roleIdx].Add(model.nextDeckCard);

                model.nextDeckCard = UnoFlipV2.Card.Decode(state.nextCardContent);//更新状态：抽牌堆下一张牌

                model.currentRoleIdx = state.currentRoleIdx;
                UpdateCurrentRole();
                UpdateHandCardsCount();

                if (state.currentRoleIdx == model.myIdx)
                    model.gameState = FlipGameState.myAction;
                else model.gameState = FlipGameState.waitOtherRoleAction;

                yield return new WaitForSeconds(1);
                //下一个角色是否需要因未喊uno罚抽牌
                yield return StartCoroutine(handleUnoDrawCards());

                yield return new WaitForSeconds(1);
                NetManager.Send(new MsgRoleActionResult(msg.id));

                printHandCards();
                print("------------------------------------------------ 同步后。");
                yield break;
            }
        }
        //抽牌并且是本人，但未出掉这张牌
        else if(state.hasDrawCard && roleIdx == model.myIdx && msg.action.cardId < 1)
        {
            //todo：抽1张牌后是否出掉这张拍的逻辑，在发送消息那里实现 ##1

            model.nextDeckCard = UnoFlipV2.Card.Decode(state.nextCardContent);//更新状态：抽牌堆下一张牌

            if (state.currentRoleIdx == model.myIdx)
                model.gameState = FlipGameState.myAction;
            else model.gameState = FlipGameState.waitOtherRoleAction;

            yield return new WaitForSeconds(1);

            //下一个角色是否需要因未喊uno罚抽牌
            yield return StartCoroutine(handleUnoDrawCards());

            yield return new WaitForSeconds(1);
            NetManager.Send(new MsgRoleActionResult(msg.id));

            printHandCards();
            print("------------------------------------------------ 同步后。");
            yield break;
        }
        #endregion
        yield return new WaitForSeconds(1f);

        //本次打出的牌
        UnoFlipV2.Card card = model.rolesHandCard[roleIdx].Find(c => c.id == msg.action.cardId);
        model.lastPlayedCard = card;
        UpdateColor();
        //if (state.hasDrawCard == false)//出牌但未抽牌，即出的是手牌
        {
            ShowNotice("top card updated");
            MoveCardToPile(FindCardDisplayForCard(msg.action.cardId, roleIdx));
            model.rolesHandCard[roleIdx].Remove(card);
            yield return new WaitForSeconds(1.0f);
        }

        //根据特殊卡type做处理：改变方向、颜色、翻转
        switch (card.GetData(model.side).type)
        {
            case CardType.Reverse://翻转方向
                model.direction = state.direction;

                Vector3 scale = directionArrow.localScale;
                scale.x = model.direction == PlayDirection.clockwise ? 1 : -1;
                directionArrow.localScale = scale;

                ShowNotice("switch direction");
                yield return new WaitForSeconds(1.8f);
                break;
            case CardType.Wild: //改变颜色
            case CardType.WildDraw:
                model.nextColor = state.nextColor;
                UpdateColor();
                ShowNotice("change color");
                yield return new WaitForSeconds(1.8f);
                break;
            case CardType.Flip:
                ShowNotice($"flip face [{model.side}] -> [{state.side}]");
                model.side = state.side;
                //todo
                FlipCards();

                model.nextColor = state.nextColor;
                UpdateColor();

                yield return new WaitForSeconds(2.8f);
                break;
            //case CardType.Draw://抽牌
        }

        //抽牌
        if (state.drawCardsCount > 0 && string.IsNullOrEmpty(state.drawCards) == false)
        {
            ShowNotice($"{model.initData.roles[state.drawCardRoleIdx]} drawing {state.drawCardsCount} cards");
            print($"--- drawCardsContent {state.drawCards}");

            List<UnoFlipV2.Card> cards = ParseHandCards(state.drawCards);
            foreach(UnoFlipV2.Card c in cards)
            {
                model.rolesHandCard[state.drawCardRoleIdx].Add(c);
                AddHandCard(state.drawCardRoleIdx, c);
                yield return new WaitForSeconds(0.8f);
            }
        }

        model.nextDeckCard = UnoFlipV2.Card.Decode(state.nextCardContent);//更新状态：抽牌堆下一张牌
        model.currentRoleIdx = state.currentRoleIdx;
        UpdateCurrentRole();
        UpdateHandCardsCount();

        //最后判断是否出现赢家 
        if (state.isLastRoleWin)
        {
            winPanel.SetActive(true);
            winningText.text = roleName + " WINS";
            ShowNotice(roleName + " WINS");
            //END GAME
            Debug.Log(roleName + $" {roleIdx} WINS");
            model.gameState = FlipGameState.finished;
        }
        else //未出现赢家，继续游戏，切换行动权
        {
            if (state.currentRoleIdx == model.myIdx)
                model.gameState = FlipGameState.myAction;
            else model.gameState = FlipGameState.waitOtherRoleAction;
        }
        //state.handCardsNum //所有角色手牌数量 ：验证是否状态错误

        //下一个角色是否需要因未喊uno罚抽牌
        yield return StartCoroutine(handleUnoDrawCards());

        yield return new WaitForSeconds(1);
        NetManager.Send(new MsgRoleActionResult(msg.id));

        printHandCards();
        print("------------------------------------------------ 同步后。");
    }
    #endregion
}
