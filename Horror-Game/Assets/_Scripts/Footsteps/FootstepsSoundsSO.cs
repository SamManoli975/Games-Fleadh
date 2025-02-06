using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FootstepSoundData
{
    public SurfaceType surfaceType;
    public List<AudioClip> footstepSoundsVariations;
    public List<AudioClip> sweeteners;
    public float sweetenersProbability;
}

[CreateAssetMenu(fileName = "FootstepsSounds", menuName = "ScriptableObjects/FootstepsSoundsSO", order = 1)]
public class FootstepsSoundsSO : ScriptableObject
{
    public List<FootstepSoundData> sounds;
}