using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField]  GameObject howToPlayPanel;

    public virtual void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void HowToPlayPanel()
    {  
        if(howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(true);
        }
    }

    public virtual void ExitGame()
    {
        Application.Quit();
    }

    public virtual void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}