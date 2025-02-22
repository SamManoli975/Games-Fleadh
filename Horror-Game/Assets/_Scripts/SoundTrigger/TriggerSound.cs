using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSound : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H)) // Press "H" to trigger the howl
        {
            SoundManager.instance.PlayHowlSound(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Stop the wind sound when the player enters the trigger
            SoundManager.instance.StopWindSound(other.gameObject);
            SoundManager.instance.PlayHowlSound(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Restart the wind sound when the player exits the trigger
            SoundManager.instance.PlayWindSound(other.gameObject);
        }
    }
}
