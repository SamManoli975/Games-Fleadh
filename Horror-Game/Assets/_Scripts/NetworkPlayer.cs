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

    void SpawnPlayerObj()
    {
        GameObject prefabToSpawn = OwnerClientId == 0 ? survivorPrefab : monsterPrefab;
        Transform spawnpoint = OwnerClientId == 0 ? SpawnPoints.instance.survivorSpawnPoint : SpawnPoints.instance.monsterSpawnPoint;
        GameObject playerObj = Instantiate(prefabToSpawn, spawnpoint.position, spawnpoint.rotation);

        spawnedPlayer = playerObj.GetComponent<NetworkPlayerObject>();

        NetworkObject networkObject = playerObj.GetComponent<NetworkObject>();
        networkObject.SpawnWithOwnership(OwnerClientId, true);
    }

    public NetworkPlayerObject GetSpawnedPlayer()
    {
        return spawnedPlayer;
    }
}
