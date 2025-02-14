using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

[RequireComponent(typeof(NetworkTransform))]
public class FollowTransform : NetworkBehaviour
{
    public Transform target;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (target != null)
            SyncTransform();
    }

    void Update()
    {
        if (target != null)
        {
            SyncTransform();
        }
    }

    void SyncTransform()
    {
        transform.position = target.transform.position;
        transform.rotation = target.transform.rotation;
    }
}
