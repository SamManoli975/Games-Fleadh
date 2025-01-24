using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightHand : HandItem
{
    [SerializeField] GameObject lightObj;

    bool isOn = false;

    void Start()
    {
        lightObj.SetActive(isOn);
    }

    public override void Use()
    {
        isOn = !isOn;
        lightObj.SetActive(isOn);
    }
}
