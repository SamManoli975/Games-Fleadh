using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TriggerBush : NetworkBehaviour
{
    public EnvironmentType environmentType = EnvironmentType.Bush;

    private bool isInBushTrigger = false; // Track if the player is inside this bush trigger
    public AudioSource bushAudioSource; // Reference to this bush's AudioSource

    //[SerializeField] private float maxUpdateDistance = 100f; // Maximum distance to update the bush

    private GameObject player; // Reference to the player

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.Log("❌ Player not found! Make sure the player has the 'Player' tag.");
        }

        if (bushAudioSource == null)
        {
            Debug.Log("❌ Bush AudioSource is missing! Assign it in the Inspector.");
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H)) // Press "H" to trigger the howl
        {
            SoundManager.instance.PlayHowlSound(gameObject);
        }

        float distance = Vector3.Distance(transform.position, player.transform.position);
        Debug.Log($"📏 Player distance from bush: {distance}");

        //if (player != null && distance <= maxUpdateDistance)
        //{
        //    Debug.Log("✅ Player is in bush distance");
        //}
        //else
        //{
        //    Debug.Log("🚫 Player not in range (Distance: " + distance + ")");
        //}


        
        Debug.Log("player is in bush distance");
        // Only update this bush if the player is close
        if (isInBushTrigger)
        {
            Debug.Log("player is in bush");
            if (IsPlayerMoving())
            {
                Debug.Log("player is moving");
                // Play this bush's sound while the player is moving
                if (!bushAudioSource.isPlaying)
                {
                    Debug.Log("Audio source playing");
                    bushAudioSource.Play();
                }
            }
            else
            {
                // Stop this bush's sound when the player stops moving
                if (bushAudioSource.isPlaying)
                {
                    Debug.Log("player stopped moving so audio stopped");
                    bushAudioSource.Stop();
                }
            }

        }
        else
        {
            // If the player is too far away, stop the bush sound (if it's playing)
            if (bushAudioSource.isPlaying)
            {
                bushAudioSource.Stop();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (environmentType == EnvironmentType.Bush) // Check if this is a bush trigger
            {
                Debug.Log("player in bush trigger");
                isInBushTrigger = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (environmentType == EnvironmentType.Bush) // Check if this is a bush trigger
            {
                isInBushTrigger = false;

                // Stop this bush's sound when the player exits the trigger
                if (bushAudioSource.isPlaying)
                {
                    bushAudioSource.Stop();
                }
            }
            
        }
    }

    // Helper method to check if the player is moving
    private bool IsPlayerMoving()
    {
        if (player == null) return false;

        // Check if the player's velocity is greater than a small threshold
        Rigidbody playerRigidbody = player.GetComponent<Rigidbody>();
        if (playerRigidbody != null)
        {
            return playerRigidbody.velocity.magnitude > 0.1f;
        }

        // If no Rigidbody, check input (e.g., for a character controller)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        return Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f;
    }
}