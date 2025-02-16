using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
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

    [ClientRpc]
    void MoveObjectClientRpc(NetworkObjectReference objRef, Vector3 newPosition)
    {
        Debug.Log(newPosition);

        if (!IsOwner)
            return;

        Debug.Log("passed " + newPosition);

        if (!objRef.TryGet(out NetworkObject netObj))
        {
            Debug.LogError("Failed to retrieve object from its NetworkObjectReference");
            return;
        }

        Debug.Log("here " + newPosition);
        netObj.GetComponent<NetworkTransform>().Teleport(newPosition, netObj.transform.rotation, netObj.transform.localScale);
    }

    void SpawnPlayerObj()
    {
        GameObject prefabToSpawn = OwnerClientId == 0 ? survivorPrefab : monsterPrefab;
        Transform spawnpoint = OwnerClientId == 0 ? SpawnPoints.instance.survivorSpawnPoint : SpawnPoints.instance.monsterSpawnPoint;
        GameObject playerObj = Instantiate(prefabToSpawn, spawnpoint.position, spawnpoint.rotation);

        NetworkObject networkObject = playerObj.GetComponent<NetworkObject>();
        networkObject.SpawnWithOwnership(OwnerClientId, true);

        //MoveObjectClientRpc(networkObject, spawnpoint.transform.position);
    }
}
