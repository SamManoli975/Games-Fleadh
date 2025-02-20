using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class NetworkPlayersSpawner : MonoBehaviour
{
    public static NetworkPlayersSpawner instance;

    [HideInInspector]
    public UnityEvent onAllPlayersSpawned = new UnityEvent();

    [SerializeField] List<string> spawnPlayerInScenes;
    [SerializeField] GameObject playerPrefab;

    Dictionary<ulong, NetworkPlayer> spawnedPlayerObject = new Dictionary<ulong, NetworkPlayer>();
    Dictionary<ulong, bool> spawnedActualPlayer = new Dictionary<ulong, bool>();

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

    void Start()
    {
        NetworkManager.Singleton.OnServerStarted += SetupSceneManagerCallbacks;
        NetworkGameManager.instance.OnClientConnectedCallback += OnClientConnected;
        NetworkGameManager.instance.OnClientDisconnectCallback += OnClientDisconnected;
    }

    void SetupSceneManagerCallbacks()
    {
        if (!NetworkManager.Singleton.IsServer)
            return;
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnSceneLoadEvent;
    }

    void OnSceneLoadEvent(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        Debug.Log("Load event " + clientsCompleted.Count);
        if (clientsCompleted.Count == NetworkManager.Singleton.ConnectedClients.Count)
        {
            OnSceneLoaded(sceneName);
        }
    }

    void OnSceneLoaded(string sceneName)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        // players spawning handling
        if (spawnPlayerInScenes.Contains(sceneName) && NetworkManager.Singleton.IsServer)
        {
            // spawn player manually for each connected client
            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                if (!spawnedPlayerObject.ContainsKey(client.ClientId))
                    SpawnPlayer(client.ClientId);
            }
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        Debug.Log("in spawn manager");

        // players spawning handling
        if (spawnPlayerInScenes.Contains(SceneManager.GetActiveScene().name) && NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Spawning player for newly connected client");
            if (!spawnedPlayerObject.ContainsKey(clientId))
                SpawnPlayer(clientId);
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        Debug.Log($"Client disconnected: {clientId}");
        spawnedPlayerObject.Remove(clientId);
    }

    private void SpawnPlayer(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        Debug.Log("spawning for " + clientId);

        if (spawnedPlayerObject.ContainsKey(clientId))
            return;

        GameObject playerInstance = Instantiate(playerPrefab);
        playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);

        spawnedPlayerObject[clientId] = playerInstance.GetComponent<NetworkPlayer>();
    }

    public void ActualPlayerSpawned(ulong spawnedCliendId)
    {
        spawnedActualPlayer[spawnedCliendId] = true;

        bool allPlayersSpawned = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClients.Keys)
        {
            if (!spawnedActualPlayer.ContainsKey(clientId))
            {
                allPlayersSpawned = false;
                break;
            }
        }

        if (allPlayersSpawned)
        {
            onAllPlayersSpawned.Invoke();
        }
    }

    public Dictionary<ulong, NetworkPlayer> GetSpawnedPlayerObject()
    {
        return spawnedPlayerObject;
    }

    public void Reset()
    {
        spawnedPlayerObject.Clear();
        spawnedActualPlayer.Clear();
    }
}
