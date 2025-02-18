using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] GameObject survivorPrefab;
    [SerializeField] GameObject monsterPrefab;

    NetworkPlayerObject spawnedPlayer;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
            SpawnPlayerObj();
    }

    [ClientRpc]
    public void SetNetworkPlayerClientRpc(NetworkObjectReference spawnedPlayerObjRef)
    {
        if (!spawnedPlayerObjRef.TryGet(out NetworkObject netObj))
        {
            Debug.LogError("Failed to retrieve object from its NetworkObjectReference");
            return;
        }

        netObj.GetComponent<NetworkPlayerObject>().SetNetworkPlayer(this);
        Debug.Log("here");
    }

    void SpawnPlayerObj()
    {
        PlayerRole playerRole = NetworkGameManager.instance.GetPlayerRole(OwnerClientId);
        if (playerRole == PlayerRole.none)
        {
            Debug.LogError("Player with id " + OwnerClientId + " has 'none' role");
            return;
        }

        GameObject prefabToSpawn = playerRole == PlayerRole.survivor ? survivorPrefab : monsterPrefab;
        Transform spawnpoint = playerRole == PlayerRole.survivor ? SpawnPoints.instance.survivorSpawnPoint : SpawnPoints.instance.monsterSpawnPoint;
        GameObject playerObj = Instantiate(prefabToSpawn, spawnpoint.position, spawnpoint.rotation);

        spawnedPlayer = playerObj.GetComponent<NetworkPlayerObject>();
        spawnedPlayer.SetNetworkPlayer(this);

        NetworkObject networkObject = playerObj.GetComponent<NetworkObject>();
        networkObject.SpawnWithOwnership(OwnerClientId, true);
        SetNetworkPlayerClientRpc(networkObject);
    }

    public NetworkPlayerObject GetSpawnedPlayer()
    {
        return spawnedPlayer;
    }

    [ServerRpc]
    void PlayerObjectSpawnedServerRpc()
    {
        NetworkPlayersSpawner.instance.ActualPlayerSpawned(OwnerClientId);

    }

    public void PlayerObjectSpawned()
    {
        if (!IsOwner)
            return;
        PlayerObjectSpawnedServerRpc();
    }
}
