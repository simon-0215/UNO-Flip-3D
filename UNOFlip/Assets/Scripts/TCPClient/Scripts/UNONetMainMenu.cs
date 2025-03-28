using MyTcpClient;
using QFramework;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UNONetMainMenu : MonoBehaviour, IController
{
    public static UNONetMainMenu instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    [SerializeField] private TMP_InputField inputPlayerName;
    [SerializeField] private Button buttonMatch;
    [SerializeField] private TMP_Text txtMatchStatus;

    [SerializeField] private TMP_Text txtMyName;
    [SerializeField] private TMP_Text txtOpponentName;

    private void Start()
    {
        buttonMatch.onClick.AddListener(MatchOnClick);

        this.RegisterEvent<NetOnMsgEvent>(e =>
        {
            if(e.type == typeof(MsgReady))
            {
                SceneManager.LoadScene(1);
            }
        });
    }

    public void showNotice(string info, bool autoHide = true, Color? fontColor = null)
    {
        txtMatchStatus.text = info;
        txtMatchStatus.gameObject.SetActive(true);
        txtMatchStatus.color = fontColor != null ? (Color)fontColor : Color.red;

        if (autoHide)
            StartCoroutine(hideNotice());
    }

    IEnumerator hideNotice()
    {
        yield return new WaitForSeconds(2);
        txtMatchStatus.gameObject.SetActive(false);
    }
    void MatchOnClick()
    {
        if(UNOCardNetManager.instance.gameState == NetCardGameState.disconnect)
        {
            showNotice("Not yet connected to the server.");
            return;
        }

        string playerName = inputPlayerName.text.Trim();
        if (string.IsNullOrEmpty(playerName))
        {
            showNotice("Player nickname must not be blank.");
            return;
        }
        txtMyName.text = playerName;

        MsgPlayerMatchRequest msg = new MsgPlayerMatchRequest();
        msg.currentPlayerName = playerName;
        NetManager.Send(msg);

        showNotice("Searching for opponents...", false, Color.green);
    }

    public void SetOpponentName(string name)
    {
        txtOpponentName.text = name;

        txtOpponentName.transform.parent.parent.gameObject.SetActive(true);//显示MatchedPanel
    }

    //匹配成功后，点击 Play Game 发送ready协议（准备进入游戏场景）
    public void ReadyOnClick()
    {
        NetManager.Send(new MsgReady());
    }

    public IArchitecture GetArchitecture()
    {
        return CardGameApp.Interface;
    }
}
