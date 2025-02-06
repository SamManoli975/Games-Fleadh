using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepsManager : MonoBehaviour
{
    public static FootstepsManager instance;

    [SerializeField] private FootstepsSoundsSO footstepsSoundsSO;

    private Dictionary<SurfaceType, FootstepSoundData> surfaceSoundsMap = new Dictionary<SurfaceType, FootstepSoundData>();

    void Awake()
    {
        instance = this;

        CreateSurfaceSoundsMap();
    }

    void CreateSurfaceSoundsMap()
    {
        for (int i = 0; i < footstepsSoundsSO.sounds.Count; i++)
        {
            FootstepSoundData footstepSoundData = footstepsSoundsSO.sounds[i];
            if (!surfaceSoundsMap.ContainsKey(footstepSoundData.surfaceType))
            {
                surfaceSoundsMap.Add(footstepSoundData.surfaceType, footstepSoundData);
            }
            else
            {
                Debug.LogError("There are multiple surface types sounds lists defined for surface '" + footstepSoundData.surfaceType + "'");
            }
        }
    }

    public SurfaceFootstepAudio GetSurfaceAudioClips(SurfaceType surfaceType)
    {
        if (!surfaceSoundsMap.ContainsKey(surfaceType))
        {
            Debug.LogError("There are no sounds for surface of type '" + surfaceType + "'");
            return null;
        }

        FootstepSoundData footstepSoundData = surfaceSoundsMap[surfaceType];
        List<AudioClip> footstepClips = footstepSoundData.footstepSoundsVariations;
        List<AudioClip> sweetenerClips = footstepSoundData.sweeteners;

        SurfaceFootstepAudio surfaceFootstepAudio = new SurfaceFootstepAudio();
        surfaceFootstepAudio.mainAudioClip = null;
        surfaceFootstepAudio.sweetenerAudioClip = null;

        if (footstepClips.Count != 0)
        {
            surfaceFootstepAudio.mainAudioClip = footstepClips[Random.Range(0, footstepClips.Count)];
        }

        if (sweetenerClips.Count != 0 && Random.Range(0f, 1f) <= footstepSoundData.sweetenersProbability)
        {
            surfaceFootstepAudio.sweetenerAudioClip = sweetenerClips[Random.Range(0, sweetenerClips.Count)];
        }

        return surfaceFootstepAudio;
    }
}
