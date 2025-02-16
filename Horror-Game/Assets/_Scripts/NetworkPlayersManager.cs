using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public enum PlayerRole
{
    none,
    survivor,
    monster
}

public class NetworkPlayersManager : MonoBehaviour
{
    public static NetworkPlayersManager instance;

    [HideInInspector]
    public UnityEvent onAllPlayersSpawned = new UnityEvent();

    [SerializeField] List<string> spawnPlayerInScenes;
    [SerializeField] GameObject playerPrefab;

    Dictionary<ulong, NetworkPlayer> spawnedPlayerObject = new Dictionary<ulong, NetworkPlayer>();
    Dictionary<ulong, bool> spawnedActualPlayer = new Dictionary<ulong, bool>();
    Dictionary<ulong, PlayerRole> playersRoles = new Dictionary<ulong, PlayerRole>();

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

        // players spawning handling
        if (spawnPlayerInScenes.Contains(sceneName) && NetworkManager.Singleton.IsServer)
        {
            // spawn player manually for each connected client
            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                if (!spawnedPlayerObject.ContainsKey(clientId))
                    SpawnPlayer(client.ClientId);
            }
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        Debug.Log($"New client connected: {clientId}");
        playersRoles[clientId] = GetNewPlayerRole(clientId);

        // players spawning handling
        if (spawnPlayerInScenes.Contains(SceneManager.GetActiveScene().name) && NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Spawning player for newly connected client");
            if (!spawnedPlayerObject.ContainsKey(clientId))
                SpawnPlayer(clientId);
        }
    }

    private void SpawnPlayer(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        if (spawnedPlayerObject.ContainsKey(clientId))
            return;

        GameObject playerInstance = Instantiate(playerPrefab);
        playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);

        spawnedPlayerObject[clientId] = playerInstance.GetComponent<NetworkPlayer>();
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        Debug.Log($"Client disconnected: {clientId}");
        spawnedPlayerObject.Remove(clientId);
    }


    PlayerRole GetNewPlayerRole(ulong clientId)
    {
        return clientId == 0 ? PlayerRole.survivor : PlayerRole.monster;
        // if (!playersRoles.ContainsValue(PlayerRole.monster))
        // {
        //     return PlayerRole.monster;
        // }

        // return PlayerRole.survivor;
    }

    // only can start if there is 1 monster and there are 2 players
    bool CanStartGame()
    {
        if (playersRoles.Count != 2)
            return false;

        int monstersCount = 0;
        foreach (KeyValuePair<ulong, PlayerRole> entry in playersRoles)
        {
            if (entry.Value == PlayerRole.monster)
                monstersCount++;
        }

        if (monstersCount != 1)
            return false;

        return true;
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

    public PlayerRole GetPlayerRole(ulong clientId)
    {
        if (!playersRoles.ContainsKey(clientId))
            return PlayerRole.none;

        return playersRoles[clientId];
    }

    public Dictionary<ulong, NetworkPlayer> GetSpawnedPlayerObject()
    {
        return spawnedPlayerObject;
    }

    public Dictionary<ulong, PlayerRole> GetPlayersRoles()
    {
        return playersRoles;
    }
}
