using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FlashlightHand : HandItem
{
    [SerializeField] GameObject lightObj;

    NetworkVariable<bool> isOn = new NetworkVariable<bool>(false);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        lightObj.SetActive(isOn.Value);
        isOn.OnValueChanged += HandleOnStatusChange;
    }

    [ServerRpc]
    void ToggleFlashlightServerRpc()
    {
        isOn.Value = !isOn.Value;
    }

    void HandleOnStatusChange(bool previous, bool current)
    {
        lightObj.SetActive(current);
    }

    public override void Use()
    {
        // client anticipate
        // lightObj.SetActive(!isOn.Value);

        ToggleFlashlightServerRpc();
    }
}
