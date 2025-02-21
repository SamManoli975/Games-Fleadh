using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSound : MonoBehaviour
{
    public AudioSource audioSource; // Reference to the AudioSource
    private bool hasPlayed = false; // To ensure the sound plays only once

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the entering object is the player (or another tag of interest)
        if (!hasPlayed && other.CompareTag("Player"))
        {
            audioSource.Play(); // Play the sound
            hasPlayed = true;  // Ensure it plays only once
        }
    }
}