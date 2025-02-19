using UnityEngine;
using Unity.Netcode;

public class NetworkManagerSingleton : MonoBehaviour
{
    private static NetworkManagerSingleton instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}