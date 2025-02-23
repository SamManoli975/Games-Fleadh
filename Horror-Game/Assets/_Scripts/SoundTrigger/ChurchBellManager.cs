using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class ChurchBellManager : NetworkBehaviour
{
    public AudioSource bellAudioSource; 
    public float volumeMultiplier = 10f; 
    public float maxDistance = 400f; 
    public float TimeBetweenRings = 90f; 
   

    private void Start()
    {
        if (IsServer) // Only the server should start the bell sound
        {
            StartCoroutine(RingBellRoutine()); 
        }
    }

    // Coroutine to ring the bell at random intervals (between 1 and 2 minutes)
    private IEnumerator RingBellRoutine()
    {
        while (true)
        {
            
            
            yield return new WaitForSeconds(TimeBetweenRings);

            // Ring the bell
            RingBellServerRpc();
        }
    }

    [ServerRpc]
    private void RingBellServerRpc()
    {
        // Play the bell sound on the server
        if (!bellAudioSource.isPlaying)
        {
            bellAudioSource.loop = false; // Bell rings once
            bellAudioSource.volume = 1f * volumeMultiplier; // Loud bell sound
            bellAudioSource.spatialBlend = 1f; // 3D sound
            bellAudioSource.maxDistance = maxDistance; // Set max distance for the sound
            bellAudioSource.minDistance = 5f; // Set minimum distance for the sound
            bellAudioSource.Play();

            // Inform all clients to play the bell sound
            RingBellClientRpc();
        }
    }

    [ClientRpc]
    private void RingBellClientRpc()
    {
        // Play the bell sound on all clients
        if (!bellAudioSource.isPlaying)
        {
            bellAudioSource.loop = false; // Bell rings once
            bellAudioSource.volume = 1f * volumeMultiplier; // Loud bell sound
            bellAudioSource.spatialBlend = 1f; // 3D sound
            bellAudioSource.maxDistance = maxDistance; // Set max distance for the sound
            bellAudioSource.minDistance = 5f; // Set minimum distance for the sound
            bellAudioSource.Play();
        }
    }
}
