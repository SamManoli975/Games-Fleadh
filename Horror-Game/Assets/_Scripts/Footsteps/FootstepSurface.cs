using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SurfaceType
{
    standard,
    ground,
    grass,
    wood,
    stone,
    puddle,
    water,
    bush
}

public class FootstepSurface : MonoBehaviour
{
    [SerializeField] private SurfaceType surfaceType;

    public SurfaceType GetSufraceType()
    {
        return surfaceType;
    }
}
