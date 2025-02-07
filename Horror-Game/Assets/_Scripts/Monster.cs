using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Clicker))]
public class Monster : MonoBehaviour
{
    Clicker clicker;

    void Awake()
    {
        clicker = GetComponent<Clicker>();
    }
}
