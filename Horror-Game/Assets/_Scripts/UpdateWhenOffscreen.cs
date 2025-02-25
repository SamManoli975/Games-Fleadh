using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateWhenOffscreen : MonoBehaviour
{
    void Start()
    {
        foreach(SkinnedMeshRenderer mr in GetComponentsInChildren<SkinnedMeshRenderer>()) {
            mr.updateWhenOffscreen = true;
        }
    }

}
