using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public enum GameState
{
    preparing,
    running,
    ended
}

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

    public event Action onGameStarted;
    public event Action onGameEnded;

    NetworkVariable<GameState> gameState = new NetworkVariable<GameState>(GameState.preparing);

    private void Awake()
    {
        instance = this;

        NetworkPlayersSpawner.instance.onAllPlayersSpawned.AddListener(StartGame);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        gameState.OnValueChanged += (GameState previous, GameState current) =>
        {
            if (current == GameState.running)
            {
                onGameStarted?.Invoke();
            }
            else if (current == GameState.ended)
            {
                onGameEnded?.Invoke();
            }
        };

        if (IsServer)
        {
            NetworkGameManager.instance.OnClientDisconnectCallback += HandleClientDisconnected;
        }
    }

    void StartGame()
    {
        if (!IsServer)
            return;

        if (IsGameRunning())
            return;

        if (!NetworkGameManager.instance.CanStartGame())
            return;

        Debug.Log("Starting the game");
        gameState.Value = GameState.running;
    }

    void HandleClientDisconnected(ulong clientId)
    {
        if (!IsServer)
            return;

        if (gameState.Value == GameState.preparing || gameState.Value == GameState.running)
        {
            StopWholeGame();
        }
    }

    [ClientRpc]
    private void EndGameClientRpc(bool isWinner, ClientRpcParams clientRpcParams = default)
    {
        EndScreenType endScreenType = EndScreenType.lose;
        if (isWinner)
            endScreenType = EndScreenType.win;

        UI_EndScreen.instance.ShowEndScreen(endScreenType);
    }

    public void EndGame(PlayerRole winnersRole)
    {
        if (!IsServer)
            return;

        if (gameState.Value == GameState.ended)
            return;

        gameState.Value = GameState.ended;

        Dictionary<ulong, PlayerRole> playersRoles = NetworkGameManager.instance.GetPlayersRoles();
        foreach (KeyValuePair<ulong, PlayerRole> entry in playersRoles)
        {
            Debug.Log(entry.Key);

            bool isWinner = entry.Value == winnersRole;

            EndGameClientRpc(isWinner, new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = new ulong[] { entry.Key } }
            });
        }

        StopWholeGame();
    }

    void StopWholeGame()
    {
        Invoke(nameof(ShutdownNetwork), 1f);
    }

    void ShutdownNetwork()
    {
        NetworkManager.Singleton.Shutdown();
    }

    public bool IsGameRunning()
    {
        return gameState.Value == GameState.running;
    }

    public void QuitGame()
    {
        NetworkManager.Singleton.Shutdown();
    }

    public override void OnDestroy()
    {
        NetworkPlayersSpawner.instance.onAllPlayersSpawned.RemoveListener(StartGame);
        NetworkGameManager.instance.OnClientDisconnectCallback -= HandleClientDisconnected;

        base.OnDestroy();
    }
}
