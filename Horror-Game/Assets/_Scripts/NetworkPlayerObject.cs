using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerObject : NetworkBehaviour
{
    NetworkPlayer networkPlayer;

    void Start()
    {
        if (IsOwner)
        {
            networkPlayer.PlayerObjectSpawned();
        }
    }

    public void SetNetworkPlayer(NetworkPlayer networkPlayer)
    {
        this.networkPlayer = networkPlayer;
    }
}
