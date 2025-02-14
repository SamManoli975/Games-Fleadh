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

        if (UI_Manager.instance != null)
        {
            uI_HoveredMessage = UI_Manager.instance.GetHoveredMessageUI();

            clicker.onHoveredChange.AddListener(uI_HoveredMessage.HandleHoveredChange);
        }
    }
}
