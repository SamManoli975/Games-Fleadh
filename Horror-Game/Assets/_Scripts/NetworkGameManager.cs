using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System;

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

    Dictionary<ulong, PlayerRole> playersRoles = new Dictionary<ulong, PlayerRole>();

    string lobbyCode = "";

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
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
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
        if (!NetworkManager.Singleton.IsServer)
            return;

        OnClientDisconnectCallback?.Invoke(clientId);
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
