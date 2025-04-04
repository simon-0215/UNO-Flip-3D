using MyTcpClient;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using QFramework;
using System;
using UnoFlipV2;

public class NetworkSystem: AbstractSystem
{
    protected override void OnInit()
    {
        //this.GetModel<>
        //this.RegisterEvent<>
        //this.GetSystem<>
        //this.GetUtility
    }

    public void GetMsg(MsgBase msg, System.Type type)
    {
        this.SendEvent(new NetOnMsgEventV2() { msg = msg, type = type });
    }
}
public class UNOFlipGameNetManagerV2 : TcpMonoBehaviour, IController
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    [SerializeField]
    private string serverIP = "127.0.0.1";
    [SerializeField]
    private int serverPort = 8888;
        
    UnoFlipModelV2 model;
    void Start()
    {
        model = this.GetModel<UnoFlipModelV2>();
        var netSystem = this.GetSystem<NetworkSystem>();
        void GetMsg(MsgBase msg, System.Type type)
        {
            //print("�յ���Ϣ " + type.Name);
            EnqueueUIUpdate(() =>
            {
                netSystem.GetMsg(msg, type);
            });
        }
        #region �¼�
        NetManager.AddEventListener(NetManager.NetEvent.ConnectSucc, (string info) =>
        {
            print("OnConnect succ �� �����ӷ����");

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
        NetManager.AddMsgListener("MsgReady", (MsgBase msg) => { netSystem.GetMsg(msg, typeof(MsgReady)); });

        NetManager.AddMsgListener("MsgInitFlipGame", (MsgBase msg) => { GetMsg(msg, typeof(MsgInitFlipGame)); });
        NetManager.AddMsgListener("MsgInitDealCards", (MsgBase msg) => { GetMsg(msg, typeof(MsgInitDealCards)); });
        NetManager.AddMsgListener("MsgFirstPileCard", (MsgBase msg) => { GetMsg(msg, typeof(MsgFirstPileCard)); });

        NetManager.AddMsgListener("MsgRoleActionResult", (MsgBase msg) => { GetMsg(msg, typeof(MsgRoleActionResult)); });
        NetManager.AddMsgListener("MsgRoleUno", (MsgBase msg) => { GetMsg(msg, typeof(MsgRoleUno)); });


        //NetManager.AddMsgListener("MsgWin", OnMsgWin);//�ҷ�ʤ��

        //-- Start Game --
        //NetManager.AddMsgListener("MsgDealPlayer2Card", OnMsgDealPlayer2Card);
        /*
        NetManager.AddMsgListener("MsgInitDeck", OnMsgInitDeck);
        NetManager.AddMsgListener("MsgPlayerBInitDeckDone", OnMsgPlayerBInitDeckDone);
        NetManager.AddMsgListener("MsgPlayerBDealCardsDone", OnMsgPlayerBDealCardsDone);
        NetManager.AddMsgListener("MsgGetFirstPileCard", OnMsgGetFirstPileCard);

        NetManager.AddMsgListener("MsgPlayCard", OnMsgPlayCard);
        NetManager.AddMsgListener("MsgPlayerBSyncPlayCardDone", OnMsgPlayerBSyncPlayCardDone);
        NetManager.AddMsgListener("MsgSwitchPlayer", OnMsgSwitchPlayer);
        NetManager.AddMsgListener("MsgUnoButtonClick", OnMsgUnoButtonClick);
        NetManager.AddMsgListener("MsgDrawCardFromDeck", OnMsgDrawCardFromDeck);
        */


        this.RegisterEvent<NetOnMsgEventV2>(e =>
        {

        });

        OnConnectClicked();
    }

    void OnMsgPlayerMatchRequest(MsgBase msgBase)
    {
        MsgPlayerMatchRequest msg = msgBase as MsgPlayerMatchRequest;
        
        //print($"OnMsgPlayerMatchRequest {msg.isHost} {msg.currentPlayerName} {msg.otherPlayerName}");

        EnqueueUIUpdate(() =>
        {
            //model.MyPlayerName = msg.currentPlayerName;
            //model.OpponentPlayerName = msg.otherPlayerName;
            //model.isHost = msg.isHost;

            //paddleSpeed = msg.paddleSpeed;
            if (UNONetMainMenu.instance != null)
            {
                UNONetMainMenu.instance.SetOpponentName(msg.otherPlayerName);
            }

            if (matchedCallback != null)
            {
                {
                    matchedCallback.Invoke();
                }
            }
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

    private void OnMsgWin(MsgBase msg)
    {
        EnqueueUIUpdate(() =>
        {
            if (winCallback != null)
                winCallback.Invoke();
        });
    }
    public IArchitecture GetArchitecture()
    {
        return UnoFlipAppV2.Interface;
    }
}
