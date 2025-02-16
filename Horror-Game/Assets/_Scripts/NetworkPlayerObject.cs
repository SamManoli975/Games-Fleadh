using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerObject : NetworkBehaviour
{
    [ServerRpc]
    void ActualPlayerSpawnedServerRpc()
    {
        NetworkPlayersManager.instance.ActualPlayerSpawned(OwnerClientId);

    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
            ActualPlayerSpawnedServerRpc();
    }
}
