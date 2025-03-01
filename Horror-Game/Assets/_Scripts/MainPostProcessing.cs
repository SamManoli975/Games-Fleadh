using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class MainPostProcessing : MonoBehaviour
{
    public static MainPostProcessing instance;

    void Awake()
    {
        instance = this; 

        PostProcessVolume postProcessVolume = GetComponent<PostProcessVolume>();

        // Clone the profile to avoid modifying the original asset
        PostProcessProfile runtimeProfile = Instantiate(postProcessVolume.profile);
        postProcessVolume.profile = runtimeProfile;
    }

}
