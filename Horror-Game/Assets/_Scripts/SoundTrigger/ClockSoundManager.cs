using UnityEngine;
using Unity.Netcode;

public class ClockSoundManager : NetworkBehaviour
{
    public AudioSource clockAudioSource;
    public float volumeMultiplier = 2f;

    private void Start()
    {
        if (IsServer) 
        {
            StartClockSoundServerRpc();
        }
    }

    [ServerRpc]
    private void StartClockSoundServerRpc()
    {
        
        if (!clockAudioSource.isPlaying)
        {
            clockAudioSource.loop = true; // Ensure it loops continuously
            clockAudioSource.volume = 1f * volumeMultiplier;
            clockAudioSource.Play();
        }

        // Inform all clients to play the ticking sound
        PlayClockSoundClientRpc();
    }

    [ClientRpc]
    private void PlayClockSoundClientRpc()
    {
        if (!clockAudioSource.isPlaying)
        {
            clockAudioSource.loop = true;
            clockAudioSource.Play();
        }
    }
}
