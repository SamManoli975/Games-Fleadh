using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
[RequireComponent(typeof(Clicker))]
[RequireComponent(typeof(Hand))]
public class Survivor : MonoBehaviour
{
    UI_Inventory uI_Inventory;
    UI_HoveredMessage uI_HoveredMessage;

    Clicker clicker;
    Inventory inventory;
    Hand hand;

    void Awake()
    {
        clicker = GetComponent<Clicker>();
        inventory = GetComponent<Inventory>();
        hand = GetComponent<Hand>();

        uI_Inventory = UI_Manager.instance.GetInventoryUI();
        uI_HoveredMessage = UI_Manager.instance.GetHoveredMessageUI();

        clicker.onHoveredChange.AddListener(uI_HoveredMessage.HandleHoveredChange);
        inventory.onItemsUpdated.AddListener(uI_Inventory.UpdateSlots);
        hand.onSelectedSlotChanged.AddListener(uI_Inventory.UpdateSelectedSlot);
    }
}
