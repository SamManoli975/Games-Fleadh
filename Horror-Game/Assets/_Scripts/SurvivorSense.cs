using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class SurvivorSense : NetworkBehaviour
{
    public float fromDistace = 100f;

    void Update()
    {
        if(!IsOwner)
            return;

        if(Survivor.instance == null) return;

        // Calculate distance
        float distance = Vector3.Distance(transform.position, Survivor.instance.transform.position);

        if(distance >= fromDistace) {
            UI_Manager.instance.ShowSurvivorFarMessage();
        }
        else {
            UI_Manager.instance.HideSurvivorFarMessage();
        }
    }

}
