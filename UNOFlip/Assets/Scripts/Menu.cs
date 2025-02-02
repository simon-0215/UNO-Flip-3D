using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject howToPlayPanel;

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void HostGame()
    {
        if (NetworkManager.singleton != null)
        {
            NetworkManager.singleton.StartHost();
        }
        else
        {
            Debug.LogError("NetworkManager.singleton is null. Ensure there is a NetworkManager in the scene.");
        }
    }

    public void JoinGame()
    {
        if (NetworkManager.singleton != null)
        {
            NetworkManager.singleton.StartClient();
        }
        else
        {
            Debug.LogError("NetworkManager.singleton is null. Ensure there is a NetworkManager in the scene.");
        }
    }

    public void HowToPlayPanel()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(true);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}