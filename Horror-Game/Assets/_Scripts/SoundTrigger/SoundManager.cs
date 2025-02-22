using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    private Coroutine floorSoundCoroutine;
    private Coroutine mouseSoundCoroutine;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    // Generalized function to stop a sound
    private void StopSound(GameObject player, string soundName)
    {
        if (player == null) return;

        AudioSource audioSource = player.transform.Find($"Sounds/{soundName}")?.GetComponent<AudioSource>();
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        else
        {
            Debug.LogWarning($"Sound '{soundName}' not found or not playing.");
        }
    }

    // Generalized function to play a sound
    private void PlaySound(GameObject player, string soundName)
    {
        if (player == null) return;

        AudioSource audioSource = player.transform.Find($"Sounds/{soundName}")?.GetComponent<AudioSource>();
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogError($"Sound '{soundName}' not found or already playing.");
        }
    }

    // Public methods for playing specific sounds
    public void StopWindSound(GameObject player)
    {
        StopSound(player, "wind");
    }

    public void PlayWindSound(GameObject player)
    {
        PlaySound(player, "wind");
    }

    public void PlayHowlSound(GameObject player)
    {
        PlaySound(player, "howl");
    }

    // Start playing sounds at random intervals for any sound array
    public void SoundsPlayingAtRandomIntervals(GameObject player, float minInterval, float maxInterval, string[] soundNames, string soundType)
    {
        if (soundType == "floor")
        {
            if (floorSoundCoroutine != null)
                StopCoroutine(floorSoundCoroutine);

            floorSoundCoroutine = StartCoroutine(PlaySoundAtRandomIntervals(player, minInterval, maxInterval, soundNames));
        }
        else if (soundType == "mouse")
        {
            if (mouseSoundCoroutine != null)
                StopCoroutine(mouseSoundCoroutine);

            mouseSoundCoroutine = StartCoroutine(PlaySoundAtRandomIntervals(player, minInterval, maxInterval, soundNames));
        }
    }

    // Coroutine to play a sound at random intervals
    private IEnumerator PlaySoundAtRandomIntervals(GameObject player, float minInterval, float maxInterval, string[] soundNames)
    {
        while (true) // Loop forever, or break after a condition is met
        {
            // Wait for a random amount of time between minInterval and maxInterval
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);

            // Pick a random sound from the provided array
            string randomSound = soundNames[Random.Range(0, soundNames.Length)];

            // Play the selected sound
            PlaySound(player, randomSound);
        }
    }

    // Method to stop all running sound coroutines
    public void StopAllSounds()
    {
        if (floorSoundCoroutine != null)
        {
            StopCoroutine(floorSoundCoroutine);
            floorSoundCoroutine = null;
        }

        if (mouseSoundCoroutine != null)
        {
            StopCoroutine(mouseSoundCoroutine);
            mouseSoundCoroutine = null;
        }
    }
}
