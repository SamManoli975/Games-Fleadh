using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEngine;

[RequireComponent(typeof(NetworkTransform))]
public class NetworkTransformChild : MonoBehaviour
{
    public Transform target;
    Vector3 offset;
    bool isOffsetSet = false;

    void Awake()
    {
        if (!isOffsetSet && target != null)
        {
            offset = transform.position - target.transform.position;
            isOffsetSet = true;
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

    public void SetTarget(Transform target, Vector3 offset)
    {
        this.target = target;

        isOffsetSet = true;
        this.offset = offset;
    }
}
