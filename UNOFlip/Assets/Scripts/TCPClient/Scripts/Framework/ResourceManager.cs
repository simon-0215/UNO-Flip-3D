using UnityEngine;

namespace MyTcpClient
{
    public class ResourceManager : MonoBehaviour
    {
        public static GameObject LoadPrefab(string path)
        {
            return Resources.Load<GameObject>(path);
        }
    }
}
