using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSound : MonoBehaviour
{

    public EnvironmentType environmentType = EnvironmentType.House;


    void Start()
    {
        // Since we don't have access to 'other' here, we need to find the player differently
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            if (environmentType == EnvironmentType.House)
            {
                SoundManager.instance.PlayWindSound(player);
                SoundManager.instance.StopAllSounds();
                SoundManager.instance.SoundsPlayingAtRandomIntervals(player, 30f, 60f, new string[] { "Thunder" }, "Thunder");
            }
        }
    }

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

            if (environmentType == EnvironmentType.House)
            {
                // Stop the wind sound when the player enters the trigger
                SoundManager.instance.StopWindSound(other.gameObject);
                SoundManager.instance.PlayHowlSound(other.gameObject);

                // Start playing floor sounds at random intervals, between 2 and 5 seconds
                SoundManager.instance.SoundsPlayingAtRandomIntervals(other.gameObject, 3f, 10f, new string[] { "Floor1", "Floor2", "Floor3" }, "floor");

                // Start playing mouse sounds at random intervals, between 1 and 3 seconds
                SoundManager.instance.SoundsPlayingAtRandomIntervals(other.gameObject, 20f, 30f, new string[] { "Mouse1", "Mouse2" }, "mouse");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            if (environmentType == EnvironmentType.House)
            {
                // Restart the wind sound when the player exits the trigger
                SoundManager.instance.PlayWindSound(other.gameObject);

                // Stop all sounds (both floor and mouse)
                SoundManager.instance.StopAllSounds();

                // Play thunder sounds
                SoundManager.instance.SoundsPlayingAtRandomIntervals(other.gameObject, 30f, 60f, new string[] { "Thunder" }, "Thunder");
            }
        }
    }

 
}