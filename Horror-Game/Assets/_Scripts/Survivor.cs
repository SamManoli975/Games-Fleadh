using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
[RequireComponent(typeof(Clicker))]
[RequireComponent(typeof(Hand))]
public class Survivor : MonoBehaviour
{
    [SerializeField] UI_Inventory uI_Inventory;
    [SerializeField] UI_HoveredMessage uI_HoveredMessage;

    Clicker clicker;
    Inventory inventory;
    Hand hand;

    void Awake()
    {
        clicker = GetComponent<Clicker>();
        inventory = GetComponent<Inventory>();
        hand = GetComponent<Hand>();

        clicker.onHoveredChange.AddListener(uI_HoveredMessage.HandleHoveredChange);
        inventory.onItemsUpdated.AddListener(uI_Inventory.UpdateSlots);
        hand.onSelectedSlotChanged.AddListener(uI_Inventory.UpdateSelectedSlot);
    }
}
