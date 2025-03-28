using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Utils : MonoBehaviour, IPointerClickHandler
{

    public void Print(string log)
    {
        print(log);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void LoadScene(int sceneBuildIndex)
    {
        SceneManager.LoadScene(sceneBuildIndex);
    }

    public void ReloadScene()
    {
        Time.timeScale = 1.0f;//时间 恢复了
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    [Header("start之后延迟N秒执行")]
    public UnityEvent startDelayCallback = null;
    [Header("延迟的秒数")]
    public float startDelay = 2;

    [SerializeField]
    private UnityEvent initCallback = null;
    private void Start()
    {
        if(initCallback != null)
        {
            initCallback.Invoke();
        }
        /*
        WaitTimeManager.WaitTime(startDelay, () =>
        {
            if (startDelayCallback != null)
            {
                startDelayCallback.Invoke();
            }
        });
        */
    }

    private void OnEnable()
    {
        /*
        WaitTimeManager.WaitTime(startDelay, () =>
        {
            if (startDelayCallback != null)
            {
                startDelayCallback.Invoke();
            }
        });
        */
    }

    public void DelayCall()
    {
        WaitTimeManager.WaitTime(startDelay, () =>
        {
            if (startDelayCallback != null)
            {
                startDelayCallback.Invoke();
            }
        });
    }

    public void PlayerPrefsDeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }

    public void PauseResumeTime(bool isPause)
    {
        Time.timeScale = isPause ? 0 : 1;
    }

    [SerializeField]
    private UnityEvent onUIPointerClickCallback = null;

    public void OnPointerClick(PointerEventData eventData)
    {
        print("UI被点击");
        if(onUIPointerClickCallback != null) { onUIPointerClickCallback.Invoke();}
    }

    [SerializeField]
    private UnityEvent onMouseClicked = null;

    [SerializeField]
    private KeyCode keyToPress = KeyCode.None;
    [SerializeField]
    private UnityEvent keyPressedCallback = null;

    private void Update()
    {
        if(keyToPress != KeyCode.None && Input.GetKeyDown(keyToPress))
        {
            print($"key {keyToPress} is pressed");
            if(keyPressedCallback != null)
            {
                keyPressedCallback.Invoke();
            }
        }

        if(Input.GetMouseButtonDown(0))
        {
            if(onMouseClicked != null)
            {
                onMouseClicked.Invoke();
            }
        }
    }
}
