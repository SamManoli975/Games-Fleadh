using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class Highlightable : NetworkBehaviour
{
    NetworkVariable<bool> isHighlighted = new NetworkVariable<bool>(false);

    Outline outline;

    void Awake()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        isHighlighted.OnValueChanged += HandleIsHighlightedChanged;
    }

    void HandleIsHighlightedChanged(bool previous, bool current) {
        outline.enabled = current;
    }

    public void SetIsHighlighted(bool value) {
        isHighlighted.Value = value;
    }
}
