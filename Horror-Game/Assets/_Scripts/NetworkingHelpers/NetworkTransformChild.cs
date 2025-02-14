using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEngine;

[RequireComponent(typeof(NetworkTransform))]
public class NetworkTransformChild : MonoBehaviour
{
    public Transform target;
    Vector3 offset;

    void Start()
    {
        if (target != null)
        {
            offset = transform.position - target.transform.position;
        }
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
        transform.position = target.transform.position + offset;
    }
}
