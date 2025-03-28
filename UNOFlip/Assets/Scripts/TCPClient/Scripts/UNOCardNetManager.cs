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

        #region �¼�
        NetManager.AddEventListener(NetManager.NetEvent.ConnectSucc, (string info) =>
        {
            print("OnConnect succ �� �����ӷ����");
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
            print("OnConnect succ �� ������Ϸ�� 2");
        });

        NetManager.AddEventListener(NetManager.NetEvent.ConnectFail, (string info) =>
        {
            print("on connect fail : ����ʧ�ܣ������� " + info);
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

        NetManager.AddMsgListener("MsgPlayerMatchRequest", OnMsgPlayerMatchRequest);//ƥ���϶��֣����뷿��
        //NetManager.AddMsgListener("MsgChooseLevel", OnMsgChooseLevel);//�������㲥������Ϸ�Ѷȵȼ������٣������С����٣�
                                                                      //NetManager.AddMsgListener("MsgBallLaunch", OnMsgBallLaunch);//����
                                                                      //NetManager.AddMsgListener("MsgTouchScreen", OnMsgTouchScreen);//�����ƶ�����

       //NetManager.AddMsgListener("MsgCountDown", OnMsgCountDown); //����ʱ

        //NetManager.AddMsgListener("MsgPaddleSync", OnMsgPaddleSync);//ͬ�������2������
        //NetManager.AddMsgListener("MsgBallSync", OnMsgBallSync);//ͬ�����λ�á��ٶȵ�

        //NetManager.AddMsgListener("MsgScore", OnMsgScore);//�÷�
        NetManager.AddMsgListener("MsgWin", OnMsgWin);//�ҷ�ʤ��
        NetManager.AddMsgListener("MsgFail", OnMsgFail);//�ҷ�ʧ��
        NetManager.AddMsgListener("MsgDisMatched", OnMsgDisMatched);//�Է��뿪����
        NetManager.AddMsgListener("MsgAgain", OnMsgAgain);//˫����ѡ������һ�Ρ�

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

                pauseResumeBtnTxt.text = "�ָ�";
                pauseResumeTxt.text = "��ͣ��";
                pauseResumeTxt.transform.parent.gameObject.SetActive(true);

                MsgPause msg = new MsgPause();
                // msg ��Ա��������
                NetManager.Send(msg);
            }
            //else if (roomState == RoomState.paused)
            {
                pauseResumeBtnTxt.text = "�ȴ��ָ�";
                pauseResumeTxt.text = "�ȴ��Է�׼����";
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
        EnqueueUIUpdate(() => //ȷ�������������߳���ִ��
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
            pauseResumeBtnTxt.text = "��ͣ";
            pauseResumeTxt.transform.parent.gameObject.SetActive(false);
        });
    }
    private void OnMsgResumeRequest(MsgBase msgBase)
    {
        EnqueueUIUpdate(() =>
        {
            pauseResumeBtnTxt.text = "�Է��ȴ�";
            pauseResumeTxt.text = "�Է�����ָ���ͣ";
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
        Debug.LogWarning("�����ʵ�������������ȷ�ķ����ip");
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
            Debug.LogError("ChooseLvOnclick lv��������" + lv);
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
