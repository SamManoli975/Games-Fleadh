using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class TriggerBush : NetworkBehaviour
{
    public EnvironmentType environmentType = EnvironmentType.Bush;

    private bool isInBushTrigger = false; // Track if the player is inside this bush trigger
    public AudioSource bushAudioSource; // Reference to this bush's AudioSource

    private GameObject player; // Reference to the player

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("❌ Player not found! Make sure the player has the 'Player' tag.");
        }

        if (bushAudioSource == null)
        {
            Debug.LogError("❌ Bush AudioSource is missing! Assign it in the Inspector.");
        }
    }

    void Update()
    {
        if (isInBushTrigger)
        {
            if (IsPlayerMoving())
            {
                if (!bushAudioSource.isPlaying)
                {
                    PlayBushSoundServerRpc(); // Call ServerRpc to play sound for all players
                }
            }
            else
            {
                if (bushAudioSource.isPlaying)
                {
                    StopBushSoundServerRpc(); // Call ServerRpc to stop sound for all players
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (environmentType == EnvironmentType.Bush)
            {
                isInBushTrigger = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (environmentType == EnvironmentType.Bush)
            {
                isInBushTrigger = false;
                StopBushSoundServerRpc(); // Stop sound for all players
            }
        }
    }

    // ✅ ServerRpc to Play the Sound
    [ServerRpc(RequireOwnership = false)]
    private void PlayBushSoundServerRpc()
    {
        PlayBushSoundClientRpc();
    }

    // ✅ ClientRpc to Play the Sound for All Players
    [ClientRpc]
    private void PlayBushSoundClientRpc()
    {
        if (!bushAudioSource.isPlaying)
        {
            bushAudioSource.Play();
        }
    }

    // ✅ ServerRpc to Stop the Sound
    [ServerRpc(RequireOwnership = false)]
    private void StopBushSoundServerRpc()
    {
        StopBushSoundClientRpc();
    }

    // ✅ ClientRpc to Stop the Sound for All Players
    [ClientRpc]
    private void StopBushSoundClientRpc()
    {
        if (bushAudioSource.isPlaying)
        {
            bushAudioSource.Stop();
        }
    }

    private bool IsPlayerMoving()
    {
        if (player == null) return false;

        Rigidbody playerRigidbody = player.GetComponent<Rigidbody>();
        if (playerRigidbody != null)
        {
            return playerRigidbody.velocity.magnitude > 0.1f;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        return Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f;
    }
}
