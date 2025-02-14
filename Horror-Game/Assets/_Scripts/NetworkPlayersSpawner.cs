using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetworkPlayersSpawner : MonoBehaviour
{
    public static NetworkPlayersSpawner instance;

    [SerializeField] string gameScene;
    [SerializeField] GameObject playerPrefab;

    HashSet<ulong> hasSpawnedPlayerObject = new HashSet<ulong>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void Start()
    {
        NetworkManager.Singleton.OnServerStarted += SetupSceneManagerCallbacks;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    void SetupSceneManagerCallbacks()
    {
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoaded;
    }


    private void OnSceneLoaded(ulong clientId, string sceneName, LoadSceneMode mode)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        if (sceneName == gameScene && NetworkManager.Singleton.IsServer)
        {
            // spawn player manually for each connected client
            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                if (!hasSpawnedPlayerObject.Contains(clientId))
                    SpawnPlayer(client.ClientId);
            }
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        Debug.Log($"New client connected: {clientId}");

        if (SceneManager.GetActiveScene().name == gameScene && NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Spawning player for newly connected client");
            if (!hasSpawnedPlayerObject.Contains(clientId))
                SpawnPlayer(clientId);
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        Debug.Log($"Client disconnected: {clientId}");
        hasSpawnedPlayerObject.Remove(clientId);
    }

    private void SpawnPlayer(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        if (hasSpawnedPlayerObject.Contains(clientId))
            return;

        GameObject playerInstance = Instantiate(playerPrefab);
        playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);

        hasSpawnedPlayerObject.Add(clientId);
    }

}
