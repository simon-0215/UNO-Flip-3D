// using UnityEngine;
// using Mirror;

// public class CustomNetworkManager : NetworkManager
// {
//     public GameObject gameManagerPrefab; // Assign this in Inspector

//     public void StartGame()
//     {
//         if (NetworkServer.active)
//         {
//             Debug.Log("🌍 [NetworkManager] Switching to Game Scene...");
//             ServerChangeScene("Game"); // Change scene for everyone
//         }
//     }

//     public override void OnServerSceneChanged(string sceneName)
//     {
//         base.OnServerSceneChanged(sceneName);

//         if (sceneName == "Game") // Only spawn GameManager in Game scene
//         {
//             Debug.Log("🎮 [NetworkManager] Spawning GameManager...");
//             GameObject gameManagerInstance = Instantiate(gameManagerPrefab);
//             NetworkServer.Spawn(gameManagerInstance);
//         }
//     }
// }
