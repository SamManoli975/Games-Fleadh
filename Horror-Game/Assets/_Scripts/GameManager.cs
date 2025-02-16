using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

    bool gameIsRunning = false;

    private void Awake()
    {
        instance = this;

        NetworkPlayersManager.instance.onAllPlayersSpawned.AddListener(StartGame);
    }

    void StartGame()
    {
        if (gameIsRunning)
            return;

        gameIsRunning = true;
        Debug.Log("Starting the game");
    }

    [ClientRpc]
    private void EndGameClientRpc(bool isWinner, ClientRpcParams clientRpcParams = default)
    {
        Debug.Log("Here: " + NetworkManager.Singleton.LocalClientId);
        EndScreenType endScreenType = EndScreenType.lose;
        if (isWinner)
            endScreenType = EndScreenType.win;

        UI_EndScreen.instance.ShowEndScreen(endScreenType);
    }

    public void EndGame(PlayerRole winnersRole)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        Dictionary<ulong, PlayerRole> playersRoles = NetworkPlayersManager.instance.GetPlayersRoles();
        foreach (KeyValuePair<ulong, PlayerRole> entry in playersRoles)
        {
            Debug.Log(entry.Key);

            bool isWinner = entry.Value == winnersRole;

            EndGameClientRpc(isWinner, new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = new ulong[] { entry.Key } }
            });
        }

        NetworkManager.Singleton.Shutdown();
    }
}
