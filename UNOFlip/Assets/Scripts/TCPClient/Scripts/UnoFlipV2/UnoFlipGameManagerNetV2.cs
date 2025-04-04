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
    //4����ɫ��player name �� hand cards count
    private TMP_Text txtPlayerName1;
    private TMP_Text txtPlayerHandNumber1;
    private TMP_Text txtPlayerName2;
    private TMP_Text txtPlayerHandNumber2;
    private TMP_Text txtPlayerName3;
    private TMP_Text txtPlayerHandNumber3;
    private TMP_Text txtPlayerName4;
    private TMP_Text txtPlayerHandNumber4;

    private TMP_Text txtMessage;//��ʾ��Ϣ�ı�

    private Transform handCardsPosition;//4����ɫ����λ��
    [SerializeField] GameObject cardPrefab;
    private Transform discardPileTransform; //���ƶ�

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
            print($"role {model.initData.roles[i]} ����:{string.Join("__", model.rolesHandCard[i].Select(c => c.ToString()))}");
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
                    print($"�յ���Ϣ init game {msgInitGame.roles.Count} ({msgInitGame.roles[0]},{msgInitGame.roles[1]},{msgInitGame.roles[2]},{msgInitGame.roles[3]}) {msgInitGame.firstToPlay} {msgInitGame.myIdx} {msgInitGame.startHandNum}");
                    model.initData = msgInitGame;
                    model.currentRoleIdx = model.initData.firstToPlay;

                    StartCoroutine(Init());
                    break;
                case nameof(MsgInitDealCards):
                    MsgInitDealCards msgDealCards = (MsgInitDealCards)e.msg;
                    print($"�յ���Ϣ ���� {msgDealCards.content}");

                    StartCoroutine(InitDealCards(msgDealCards));
                    break;
                case nameof(MsgFirstPileCard):
                    MsgFirstPileCard msgFirstCard = (MsgFirstPileCard)e.msg;
                    print($"�յ���Ϣ ��һ�ŷ��� {msgFirstCard.cardId} {msgFirstCard.cardContent} {UnoFlipV2.Card.Decode(msgFirstCard.cardContent)}");
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
                    print("------------------------------------------------ ͬ��ǰ��");
                    print($"�յ���Ϣ {model.initData.roles[msgR.gameState.lastRoleIdx]}_action���ͬ�� action:{msgR.action.ToString()} _ nextRole{model.initData.roles[msgR.gameState.currentRoleIdx]}");
                    print($"gameState: {msgR.gameState.ToString()}");
                    if (c != null) { print($"���γ���: {c.ToString()} __ {c.Encode()}"); }
                    
                    printHandCards();
                    StartCoroutine(GameStateSync(msgR));
                    break;
                case nameof(MsgRoleUno):
                    print("�յ���Ϣ role declare");
                    MsgRoleUno msgUno = (MsgRoleUno)e.msg;
                    ShowNotice($"{model.initData.roles[msgUno.roleIdx]} UNO!");
                    break;
            }
        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        this.RegisterEvent<CardClickedEvent>(e =>
        {
            if (IsMyTurn() == false) return;

            //�ж��Ƿ�Ϸ�����
            if (game.IsPlayable(e.cardData) == false)
            {
                ShowNotice($"Invalid move!");
                print($"side:{model.side} card:{e.cardData} sideData:{e.cardData.GetData(model.side)}");
                return;
            }

            model.gameState = FlipGameState.actionedWaitResult;

            //����RoleAction��������
            MsgPlayerAction msg = new MsgPlayerAction();
            CardColor testColor = PickRandomColour();//just for development debug
            msg.action = new RoleAction(RoleActionType.playCard, e.cardData.id, testColor);
            NetManager.Send(msg);

            ShowNotice("Processing your move...");
            CardSideData data = model.side == Side.Light ? e.cardData.light : e.cardData.dark;
            print($"--- ���� {e.cardData.id} {data.type} {data.color} {data.value}");
        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        this.RegisterEvent<DeckClickedEvent>(e =>
        {
            if(IsMyTurn() == false)
            {
                ShowNotice("Not Your Turn");
                return;
            }

            //��1���Ƶ��߼����ڷ�����Ϣ����ʵ�� ##1
            model.rolesHandCard[model.myIdx].Add(model.nextDeckCard);
            AddHandCard(model.myIdx, model.nextDeckCard);
            model.nextDeckCard = null;
            //yield return new WaitForSeconds(0.8f);

            model.gameState = FlipGameState.actionedWaitResult;
            MsgPlayerAction msg = new MsgPlayerAction();
            msg.action = new RoleAction(RoleActionType.drawCard);
            NetManager.Send(msg);
            ShowNotice("Processing your move...");
            print("--- ���ƶ�����");

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
    /// ���������ַ���
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
        UnoFlipV2.Card cardData = model.rolesHandCard[roleIdx][cardIdx];//��������
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
        //����UI my2 first1
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

        yield return new WaitForSeconds(1);//�ȴ�1����ٷ���ȷ����Ϣ
        NetManager.Send(new MsgInitFlipGame(model.initData.id));
    }
    //���µ�ǰ���ƽ�ɫ
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
    
    //��Ϸ��ʼ������
    IEnumerator InitDealCards(MsgInitDealCards msg)
    {
        string[] roleCards = msg.content.Split('|');
        model.rolesHandCard = roleCards.Select(r => ParseHandCards(r)).ToList();
        printHandCards();

        for (int i = 0; i < model.initData.startHandNum; i++)
        {
            //�����ֽ�ɫ��ʼ ���� ����
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
    /// ������һ�����ƶѿ���
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
        //��ȡ�ڷ����Ƶ�λ��
        int idx = model.initData.myIdx == 0 ? roleIdx : (roleIdx + 2) % 4;
        Transform handPos = handCardsPosition.GetChild(idx);

        //ʵ����
        GameObject go = Instantiate(cardPrefab, handPos, false);

        CardDisplay cardDisplay = go.GetComponentInChildren<CardDisplay>();
        //���ÿ�����ɫ��ͼ��
        cardDisplay.SetCardDataV2(cardData, roleIdx);

        //���ÿ����Ƿ���ʾ��ǰ�棨��ǰ�ͻ���ֻ�ܿ���4����ɫ���Լ���ɫ�ĵ�ǰ�棬���ǿ��Կ������н�ɫ����һ�棩
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

        //���ƶѶ���չʾ��ҲҪ����
        discardPileTransform.GetChild(discardPileTransform.childCount - 1).GetComponentInChildren<CardDisplay>().Flip();
    }

    IEnumerator GameStateSync(MsgRoleActionResult msg)
    {
        GameStateToSync state = msg.gameState;
        int roleIdx = state.lastRoleIdx;
        string roleName = model.initData.roles[roleIdx];//���ͬ�������ĸ���ɫ���ж�
        
        //������һ����ɫ��Ϊδ��uno�ĳ����߼�
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

                model.nextDeckCard = UnoFlipV2.Card.Decode(state.nextCardContent);//����״̬�����ƶ���һ����
            }
        }

        # region ��������߼����ڱ����ж���һ�׶��Ƿ������
        //���Ʋ��Ҳ��Ǳ���
        if (state.hasDrawCard && roleIdx != model.myIdx)
        {
            ShowNotice($"{roleName} draw a card");

            if(msg.action.cardId > 0)//���ƺ��г���
            {
                yield return new WaitForSeconds(2.5f);
                ShowNotice($"{roleName} play the new card");
                //�Ѹ��Ƽ�������
                AddHandCard(roleIdx, model.nextDeckCard);
                model.rolesHandCard[roleIdx].Add(model.nextDeckCard);
                model.nextDeckCard = UnoFlipV2.Card.Decode(state.nextCardContent);//����״̬�����ƶ���һ����
            }
            else//���ƺ�δ����
            {
                UnoFlipV2.Card cardData = model.nextDeckCard;//��������
                print($"--- ���ƺ�δ���� card data {cardData}");
                AddHandCard(roleIdx, model.nextDeckCard);
                model.rolesHandCard[roleIdx].Add(model.nextDeckCard);

                model.nextDeckCard = UnoFlipV2.Card.Decode(state.nextCardContent);//����״̬�����ƶ���һ����

                model.currentRoleIdx = state.currentRoleIdx;
                UpdateCurrentRole();
                UpdateHandCardsCount();

                if (state.currentRoleIdx == model.myIdx)
                    model.gameState = FlipGameState.myAction;
                else model.gameState = FlipGameState.waitOtherRoleAction;

                yield return new WaitForSeconds(1);
                //��һ����ɫ�Ƿ���Ҫ��δ��uno������
                yield return StartCoroutine(handleUnoDrawCards());

                yield return new WaitForSeconds(1);
                NetManager.Send(new MsgRoleActionResult(msg.id));

                printHandCards();
                print("------------------------------------------------ ͬ����");
                yield break;
            }
        }
        //���Ʋ����Ǳ��ˣ���δ����������
        else if(state.hasDrawCard && roleIdx == model.myIdx && msg.action.cardId < 1)
        {
            //todo����1���ƺ��Ƿ���������ĵ��߼����ڷ�����Ϣ����ʵ�� ##1

            model.nextDeckCard = UnoFlipV2.Card.Decode(state.nextCardContent);//����״̬�����ƶ���һ����

            if (state.currentRoleIdx == model.myIdx)
                model.gameState = FlipGameState.myAction;
            else model.gameState = FlipGameState.waitOtherRoleAction;

            yield return new WaitForSeconds(1);

            //��һ����ɫ�Ƿ���Ҫ��δ��uno������
            yield return StartCoroutine(handleUnoDrawCards());

            yield return new WaitForSeconds(1);
            NetManager.Send(new MsgRoleActionResult(msg.id));

            printHandCards();
            print("------------------------------------------------ ͬ����");
            yield break;
        }
        #endregion
        yield return new WaitForSeconds(1f);

        //���δ������
        UnoFlipV2.Card card = model.rolesHandCard[roleIdx].Find(c => c.id == msg.action.cardId);
        model.lastPlayedCard = card;
        UpdateColor();
        //if (state.hasDrawCard == false)//���Ƶ�δ���ƣ�������������
        {
            ShowNotice("top card updated");
            MoveCardToPile(FindCardDisplayForCard(msg.action.cardId, roleIdx));
            model.rolesHandCard[roleIdx].Remove(card);
            yield return new WaitForSeconds(1.0f);
        }

        //�������⿨type�������ı䷽����ɫ����ת
        switch (card.GetData(model.side).type)
        {
            case CardType.Reverse://��ת����
                model.direction = state.direction;

                Vector3 scale = directionArrow.localScale;
                scale.x = model.direction == PlayDirection.clockwise ? 1 : -1;
                directionArrow.localScale = scale;

                ShowNotice("switch direction");
                yield return new WaitForSeconds(1.8f);
                break;
            case CardType.Wild: //�ı���ɫ
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
            //case CardType.Draw://����
        }

        //����
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

        model.nextDeckCard = UnoFlipV2.Card.Decode(state.nextCardContent);//����״̬�����ƶ���һ����
        model.currentRoleIdx = state.currentRoleIdx;
        UpdateCurrentRole();
        UpdateHandCardsCount();

        //����ж��Ƿ����Ӯ�� 
        if (state.isLastRoleWin)
        {
            winPanel.SetActive(true);
            winningText.text = roleName + " WINS";
            ShowNotice(roleName + " WINS");
            //END GAME
            Debug.Log(roleName + $" {roleIdx} WINS");
            model.gameState = FlipGameState.finished;
        }
        else //δ����Ӯ�ң�������Ϸ���л��ж�Ȩ
        {
            if (state.currentRoleIdx == model.myIdx)
                model.gameState = FlipGameState.myAction;
            else model.gameState = FlipGameState.waitOtherRoleAction;
        }
        //state.handCardsNum //���н�ɫ�������� ����֤�Ƿ�״̬����

        //��һ����ɫ�Ƿ���Ҫ��δ��uno������
        yield return StartCoroutine(handleUnoDrawCards());

        yield return new WaitForSeconds(1);
        NetManager.Send(new MsgRoleActionResult(msg.id));

        printHandCards();
        print("------------------------------------------------ ͬ����");
    }
    #endregion
}
