using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] GameObject survivorPrefab;
    [SerializeField] GameObject monsterPrefab;

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

        NetworkObject networkObject = playerObj.GetComponent<NetworkObject>();
        networkObject.SpawnWithOwnership(OwnerClientId, true);
    }
}
