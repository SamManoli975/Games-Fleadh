using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class KillerSense : NetworkBehaviour
{
    public float fromDistace = 3f;
    public float maxDistance = 15f; // The maximum distance for full saturation change

    public float minSaturation = -20f;
    public float maxSaturation = 0f;

    public float minChromaticAberration = 0f;
    public float maxChromaticAberration = 1f;

    public float minGrain = 0f;
    public float maxGrain = 1f;

    ColorGrading colorGrading;
    ChromaticAberration chromaticAberration;
    Grain grain;

    void Start()
    {
        if(MainPostProcessing.instance != null) {
            PostProcessVolume postProcessVolume = MainPostProcessing.instance.GetComponent<PostProcessVolume>();
            postProcessVolume.profile.TryGetSettings(out colorGrading);
            postProcessVolume.profile.TryGetSettings(out chromaticAberration);
            postProcessVolume.profile.TryGetSettings(out grain);
        }
    }

    void Update()
    {
        if(!IsOwner)
            return;
            
        if (colorGrading == null || chromaticAberration == null || grain == null 
            || Monster.instance == null) return;

        // Calculate distance
        float distance = Vector3.Distance(transform.position, Monster.instance.transform.position);
        float t = Mathf.Clamp01((distance - fromDistace) / maxDistance); // Normalize between 0 and 1

        // Interpolate saturation
        colorGrading.saturation.value = Mathf.Lerp(minSaturation, maxSaturation, t);
        chromaticAberration.intensity.value = Mathf.Lerp(minChromaticAberration, maxChromaticAberration, t);
        grain.intensity.value = Mathf.Lerp(minGrain, maxGrain, t);
    }
}
