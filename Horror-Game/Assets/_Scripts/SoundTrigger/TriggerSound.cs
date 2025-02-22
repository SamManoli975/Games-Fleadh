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

            // Start playing floor sounds at random intervals, between 2 and 5 seconds
            SoundManager.instance.SoundsPlayingAtRandomIntervals(other.gameObject, 3f, 10f, new string[] { "Floor1", "Floor2", "Floor3" }, "floor");

            // Start playing mouse sounds at random intervals, between 1 and 3 seconds
            SoundManager.instance.SoundsPlayingAtRandomIntervals(other.gameObject, 10f, 30f, new string[] { "Mouse1", "Mouse2" }, "mouse");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Restart the wind sound when the player exits the trigger
            SoundManager.instance.PlayWindSound(other.gameObject);

            // Stop all sounds (both floor and mouse)
            SoundManager.instance.StopAllSounds();
        }
    }
}
