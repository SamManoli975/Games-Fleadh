using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Shrinkable : NetworkBehaviour
{
    [SerializeField] float scaleFactor = 0.4f;

    NetworkVariable<bool> isShrinked = new NetworkVariable<bool>(false);

    Vector3 originalScale;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        isShrinked.OnValueChanged += HandleIsShrinkedChanged;
    }

    void HandleIsShrinkedChanged(bool previous, bool current) {
        transform.localScale = current ? originalScale * scaleFactor : originalScale;
    }

    public void SetIsShrinked(bool value) {
        isShrinked.Value = value;
    }
}