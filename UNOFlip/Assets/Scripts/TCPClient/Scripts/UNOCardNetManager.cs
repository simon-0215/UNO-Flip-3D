using MyTcpClient;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using QFramework;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using System;

public enum NetCardGameState
{
    disconnect,
    connected,
    matched,
}

public class UNOCardNetManager : TcpMonoBehaviour, IController
{
    public static UNOCardNetManager instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }        
    }

    [SerializeField]
    private string serverIP = "127.0.0.1";
    [SerializeField]
    private int serverPort = 8888;

    //public RoomState roomState = RoomState.countDown;
    public NetCardGameState gameState = NetCardGameState.disconnect;
    public bool isClientHost = false;
    [HideInInspector] public string myName = string.Empty;
    [HideInInspector] public string opponentName = string.Empty;

    [SerializeField]
    private TMP_Text countDownText;

    [SerializeField]
    private Button PauseResumeButton;
    private TMP_Text pauseResumeBtnTxt;
    [SerializeField]
    private TMP_Text pauseResumeTxt;

    CardGameModel model;
    void Start()
    {
        model = this.GetModel<CardGameModel>();

        #region 事件
        NetManager.AddEventListener(NetManager.NetEvent.ConnectSucc, (string info) =>
        {
            print("OnConnect succ ： 已连接服务端");
            gameState = NetCardGameState.connected;

            EnqueueUIUpdate(() =>
            {
                print("in enqueue ui update ");
                if (connectedCallback != null)
                    connectedCallback.Invoke();
                else
                {
                    print("connectedCallback is null");
                }
            });
            print("OnConnect succ ： 进入游戏中 2");
        });

        NetManager.AddEventListener(NetManager.NetEvent.ConnectFail, (string info) =>
        {
            print("on connect fail : 连接失败，请重试 " + info);
            EnqueueUIUpdate(() =>
            {
                if (connectFailedCallback != null)
                    connectFailedCallback.Invoke();
            });
        });

        NetManager.AddEventListener(NetManager.NetEvent.Close, (string info) =>
        {
            print("socket closed ");
        });
        #endregion

        NetManager.AddMsgListener("MsgPlayerMatchRequest", OnMsgPlayerMatchRequest);//匹配上对手，进入房间
        //NetManager.AddMsgListener("MsgChooseLevel", OnMsgChooseLevel);//服务器广播本次游戏难度等级（球速：慢、中、快速）
                                                                      //NetManager.AddMsgListener("MsgBallLaunch", OnMsgBallLaunch);//开球
                                                                      //NetManager.AddMsgListener("MsgTouchScreen", OnMsgTouchScreen);//触屏移动球拍

       //NetManager.AddMsgListener("MsgCountDown", OnMsgCountDown); //倒计时

        //NetManager.AddMsgListener("MsgPaddleSync", OnMsgPaddleSync);//同步（玩家2）球拍
        //NetManager.AddMsgListener("MsgBallSync", OnMsgBallSync);//同步球的位置、速度等

        //NetManager.AddMsgListener("MsgScore", OnMsgScore);//得分
        NetManager.AddMsgListener("MsgWin", OnMsgWin);//我方胜利
        NetManager.AddMsgListener("MsgFail", OnMsgFail);//我方失败
        NetManager.AddMsgListener("MsgDisMatched", OnMsgDisMatched);//对方离开房间
        NetManager.AddMsgListener("MsgAgain", OnMsgAgain);//双方都选择【再来一次】

        NetManager.AddMsgListener("MsgPause", OnMsgPause);
        NetManager.AddMsgListener("MsgResumeRequest", OnMsgResumeRequest);
        NetManager.AddMsgListener("MsgResume", OnMsgResume);

        NetManager.AddMsgListener("MsgReady", OnMsgReady);

        //-- Start Game --

        //NetManager.AddMsgListener("MsgDealPlayer2Card", OnMsgDealPlayer2Card);
        NetManager.AddMsgListener("MsgInitDeck", OnMsgInitDeck);
        NetManager.AddMsgListener("MsgPlayerBInitDeckDone", OnMsgPlayerBInitDeckDone);
        NetManager.AddMsgListener("MsgPlayerBDealCardsDone", OnMsgPlayerBDealCardsDone);
        NetManager.AddMsgListener("MsgGetFirstPileCard", OnMsgGetFirstPileCard);

        NetManager.AddMsgListener("MsgPlayCard", OnMsgPlayCard);
        NetManager.AddMsgListener("MsgPlayerBSyncPlayCardDone", OnMsgPlayerBSyncPlayCardDone);
        NetManager.AddMsgListener("MsgSwitchPlayer", OnMsgSwitchPlayer);

        this.RegisterEvent<NetOnMsgCommand>(e =>
        {

        });

        /*
        pauseResumeBtnTxt = PauseResumeButton.GetComponentInChildren<TMP_Text>();
        PauseResumeButton.onClick.AddListener(() =>
        {
            //if (roomState == RoomState.playing)
            {
                //roomState = RoomState.paused;

                pauseResumeBtnTxt.text = "恢复";
                pauseResumeTxt.text = "暂停中";
                pauseResumeTxt.transform.parent.gameObject.SetActive(true);

                MsgPause msg = new MsgPause();
                // msg 成员变量设置
                NetManager.Send(msg);
            }
            //else if (roomState == RoomState.paused)
            {
                pauseResumeBtnTxt.text = "等待恢复";
                pauseResumeTxt.text = "等待对方准备好";
                pauseResumeTxt.transform.parent.gameObject.SetActive(true);

                MsgResume msg = new MsgResume();
                NetManager.Send(msg);
            }
        });
        */
        OnConnectClicked();
    }
    void SendOnMsgCommand(Type type, MsgBase msgBase)
    {
        EnqueueUIUpdate(() => //确保操作是在主线程里执行
        {
            model.currentMsg = msgBase;
            model.currentMsgType = type;
            this.SendCommand<NetOnMsgCommand>();
        });
    }
    void OnMsgReady(MsgBase msgBase)
    {
        SendOnMsgCommand(typeof(MsgReady), msgBase);
    }
    /*void OnMsgDealPlayer2Card(MsgBase msgBase)
    {
        SendOnMsgCommand(typeof(MsgDealPlayer2Card), msgBase);
    }*/
    void OnMsgInitDeck(MsgBase msgBase)
    {
        SendOnMsgCommand(typeof(MsgInitDeck), msgBase);
    }
    void OnMsgPlayerBInitDeckDone(MsgBase msgBase)
    {
        SendOnMsgCommand(typeof(MsgPlayerBInitDeckDone), msgBase);
    }
    void OnMsgPlayerBDealCardsDone(MsgBase msgBase)
    {
        SendOnMsgCommand(typeof(MsgPlayerBDealCardsDone), msgBase);
    }
    void OnMsgGetFirstPileCard(MsgBase msgBase)
    {
        SendOnMsgCommand(typeof(MsgGetFirstPileCard), msgBase);
    }
    
    void OnMsgPlayCard(MsgBase msgBase)
    {
        SendOnMsgCommand(typeof(MsgPlayCard), msgBase);
    }
    void OnMsgPlayerBSyncPlayCardDone(MsgBase msgBase)
    {
        SendOnMsgCommand(typeof(MsgPlayerBSyncPlayCardDone), msgBase);
    }
    void OnMsgSwitchPlayer(MsgBase msgBase)
    {
        SendOnMsgCommand(typeof(MsgSwitchPlayer), msgBase);
    }

    void OnMsgPlayerMatchRequest(MsgBase msgBase)
    {
        MsgPlayerMatchRequest msg = msgBase as MsgPlayerMatchRequest;
        isClientHost = msg.isHost;
        opponentName = msg.otherPlayerName;
        myName = msg.currentPlayerName;

        print($"OnMsgPlayerMatchRequest {isClientHost} {myName} {opponentName}");

        gameState = NetCardGameState.matched;

        EnqueueUIUpdate(() =>
        {
            model.MyPlayerName = msg.currentPlayerName;
            model.OpponentPlayerName = msg.otherPlayerName;
            model.isHost = msg.isHost;

            //paddleSpeed = msg.paddleSpeed;
            if(UNONetMainMenu.instance != null)
            {
                UNONetMainMenu.instance.SetOpponentName(opponentName);
            }

            if (matchedCallback != null)
            {
                //if (isClientHost)
                {
                    matchedCallback.Invoke();
                }
            }
        });
    }

    private void OnMsgPause(MsgBase msgBase)
    {
        EnqueueUIUpdate(() =>
        {
            MsgPause msg = (MsgPause)msgBase;
            //ball.Sync(msg.x, msg.y, msg.direX, msg.direY);
            //ball.transform.position = new Vector3(msg.x, msg.y, 0);

            //roomState = RoomState.paused;
            pauseResumeBtnTxt.text = "resume";
            pauseResumeTxt.text = "paused";
            pauseResumeTxt.transform.parent.gameObject.SetActive(true);
        });
    }

    private void OnMsgResume(MsgBase msgBase)
    {
        EnqueueUIUpdate(() =>
        {
            MsgResume msg = (MsgResume)msgBase;
            //ball.Sync(msg.x, msg.y, msg.direX, msg.direY);

            //roomState = RoomState.playing;
            pauseResumeBtnTxt.text = "暂停";
            pauseResumeTxt.transform.parent.gameObject.SetActive(false);
        });
    }
    private void OnMsgResumeRequest(MsgBase msgBase)
    {
        EnqueueUIUpdate(() =>
        {
            pauseResumeBtnTxt.text = "对方等待";
            pauseResumeTxt.text = "对方请求恢复暂停";
            pauseResumeTxt.transform.parent.gameObject.SetActive(true);

            MsgResumeRequest msg = (MsgResumeRequest)msgBase;
        });
    }

    new void Update()
    {
        base.Update();
    }

    public void OnConnectClicked()
    {
        //string _serverIP = PlayerPrefs.GetString(Utils.ServerIPStoreKey, serverIP);
        //string _serverIP = "127.0.0.1";
        Debug.LogWarning("请根据实际情况，设置正确的服务端ip");
        NetManager.Connect(serverIP, 8888);
    }

    public void OnCloseClick()
    {
        NetManager.Close();
    }

    [SerializeField]
    private UnityEvent connectedCallback = null;
    [SerializeField]
    private UnityEvent connectFailedCallback = null;

    [SerializeField]
    private UnityEvent matchedCallback = null;
    [SerializeField]
    private UnityEvent winCallback = null;
    [SerializeField]
    private UnityEvent failCallback = null;
    [SerializeField]
    private UnityEvent againCallback = null;

    void OnMsgDisMatched(MsgBase msg)
    {
        EnqueueUIUpdate(() =>
        {
            MisMatchOnclick();
        });
    }
    public void MisMatchOnclick()
    {
        OnCloseClick();
        SceneManager.LoadScene(0);
    }

    public void ChooseLvOnclick(int lv)
    {
        if (lv < 1 || lv > 3)
        {
            Debug.LogError("ChooseLvOnclick lv参数错误：" + lv);
            return;
        }

        MsgChooseLevel msg = new MsgChooseLevel();
        msg.level = lv;
        NetManager.Send(msg);
    }

    private void OnMsgWin(MsgBase msg)
    {
        EnqueueUIUpdate(() =>
        {
            if (winCallback != null)
                winCallback.Invoke();
        });
    }
    private void OnMsgFail(MsgBase msg)
    {
        EnqueueUIUpdate(() =>
        {
            if (failCallback != null) failCallback.Invoke();
        });
    }

    public void AgainOnclick()
    {
        NetManager.Send(new MsgAgain());
    }

    private void OnMsgAgain(MsgBase msg)
    {
        EnqueueUIUpdate(() =>
        {
            againCallback.Invoke();
        });
    }

    private void OnMsgCountDown(MsgBase msgBase)
    {
        EnqueueUIUpdate(() =>
        {
            MsgCountDown msg = (MsgCountDown)msgBase;

            if (msg.countDown == 0)
            {
                //roomState = RoomState.playing;
                countDownText.gameObject.SetActive(false);
            }
            else
            {
                //roomState = RoomState.countDown;
                countDownText.gameObject.SetActive(true);
                print($"OnMsgCountDown {msg.countDown} {Mathf.RoundToInt(msg.countDown / 1000f).ToString()}");
                countDownText.text = Mathf.RoundToInt(msg.countDown / 1000f).ToString();
            }
        });
    }

    public IArchitecture GetArchitecture()
    {
        return CardGameApp.Interface;
    }
}
