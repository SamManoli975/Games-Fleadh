using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnableOnlyForOwner : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        Debug.Log(OwnerClientId + " owner client for enabling? camera " + ", is owner: " + IsOwner + ", current client id: " + NetworkManager.Singleton.LocalClientId);
        if (!IsOwner)
            gameObject.SetActive(false);
    }
}
