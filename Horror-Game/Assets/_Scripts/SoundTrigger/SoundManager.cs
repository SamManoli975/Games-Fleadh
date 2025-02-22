using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

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

    // Public methods that call the generalized functions
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
}
