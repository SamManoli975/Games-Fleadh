using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] GameObject survivorPrefab;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            SpawnPlayerObj();
    }

    void SpawnPlayerObj()
    {
        GameObject playerObj = Instantiate(survivorPrefab);

        NetworkObject networkObject = playerObj.GetComponent<NetworkObject>();
        networkObject.Spawn(true);
        //networkObject.SpawnWithOwnership(OwnerClientId, true);
    }
}
