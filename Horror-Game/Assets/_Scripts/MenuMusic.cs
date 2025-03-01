using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMusic : MonoBehaviour
{
    public static MenuMusic instance;

    [SerializeField] AudioSource audioSource;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        instance.StartMusic(); 
    }

    void Start()
    {
        StartMusic();
    }

    public void StartMusic() {
        if(!audioSource.isPlaying)
            audioSource.Play();
    }

    public void StopMusic() {
        if(audioSource.isPlaying)
            audioSource.Stop();
    }
}
