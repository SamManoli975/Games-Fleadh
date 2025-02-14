using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnableOnlyForOwner : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            gameObject.SetActive(false);
    }
}
