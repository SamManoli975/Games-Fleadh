using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public enum PlayerRole
{
    none,
    survivor,
    monster
}


public class NetworkGameManager : MonoBehaviour
{
    public static NetworkGameManager instance;


    public event Action<ulong> OnClientConnectedCallback;
    public event Action<ulong> OnClientDisconnectCallback;

    [SerializeField] string mainMenu;

    Dictionary<ulong, PlayerRole> playersRoles = new Dictionary<ulong, PlayerRole>();

    string lobbyCode = "";

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
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        NetworkManager.Singleton.OnServerStopped += HandleServerStopped;
    }

    void HandleServerStopped(bool _someBool)
    {
        ResetState();
    }

    void ResetState()
    {
        playersRoles.Clear();
        NetworkPlayersSpawner.instance.Reset();
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        playersRoles[clientId] = GetNewPlayerRole(clientId);
        Debug.Log($"New client connected: {clientId}. Role: " + playersRoles[clientId]);

        OnClientConnectedCallback?.Invoke(clientId);
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log("Disconnect reason: " + NetworkManager.Singleton.DisconnectReason);
            Debug.Log("Local client disconnected, returning to main menu...");
            ReturnToMainMenu();
        }

        if (!NetworkManager.Singleton.IsServer)
            return;

        OnClientDisconnectCallback?.Invoke(clientId);
    }

    private void ReturnToMainMenu()
    {
        // Make sure network is fully shut down
        if (NetworkManager.Singleton.IsConnectedClient || NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.Shutdown();
        }

        // Unlock cursor just in case
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneLoader.instance.LoadScene(mainMenu);
    }

    PlayerRole GetNewPlayerRole(ulong clientId)
    {
        return clientId != 0 ? PlayerRole.survivor : PlayerRole.monster;
        // if (!playersRoles.ContainsValue(PlayerRole.monster))
        // {
        //     return PlayerRole.monster;
        // }

        // return PlayerRole.survivor;
    }

    // only can start if there is 1 monster and there are 2 players
    public bool CanStartGame()
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

    public PlayerRole GetPlayerRole(ulong clientId)
    {
        if (!playersRoles.ContainsKey(clientId))
            return PlayerRole.none;

        return playersRoles[clientId];
    }

    public Dictionary<ulong, PlayerRole> GetPlayersRoles()
    {
        return playersRoles;
    }


    public void SetLobbyCode(string lobbyCode)
    {
        this.lobbyCode = lobbyCode;
    }

    public string GetLobbyCode()
    {
        return lobbyCode;
    }
}
