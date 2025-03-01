using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FlashlightHand : HandItem
{
    [SerializeField] private GameObject lightObj;
    [SerializeField] private AudioSource audioSource; // Reference to the AudioSource
    [SerializeField] private AudioClip turnOnSound;  // Sound for turning ON
    [SerializeField] private AudioClip turnOffSound; // Sound for turning OFF

    private NetworkVariable<bool> isOn = new NetworkVariable<bool>(false);

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
        PlayStateSoundClientRpc(isOn.Value); // Play appropriate sound
    }

    [ClientRpc]
    void PlayStateSoundClientRpc(bool newState)
    {
        if (audioSource != null)
        {
            if (newState && turnOnSound != null)
            {
                audioSource.PlayOneShot(turnOnSound);
            }
            else if (!newState && turnOffSound != null)
            {
                audioSource.PlayOneShot(turnOffSound);
            }
        }
    }

    void HandleOnStatusChange(bool previous, bool current)
    {
        lightObj.SetActive(current);
    }

    public override void Use()
    {
        ToggleFlashlightServerRpc();
    }
}