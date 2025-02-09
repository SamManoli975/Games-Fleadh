using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Clicker))]
public class Monster : MonoBehaviour
{
    [SerializeField] UI_HoveredMessage uI_HoveredMessage;

    Clicker clicker;

    void Awake()
    {
        clicker = GetComponent<Clicker>();

        clicker.onHoveredChange.AddListener(uI_HoveredMessage.HandleHoveredChange);
    }
}
