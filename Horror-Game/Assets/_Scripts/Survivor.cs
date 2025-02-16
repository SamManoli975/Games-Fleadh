using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
[RequireComponent(typeof(Clicker))]
[RequireComponent(typeof(Hand))]
public class Survivor : NetworkBehaviour
{
    UI_Inventory uI_Inventory;
    UI_HoveredMessage uI_HoveredMessage;
    UI_Hearts uI_Hearts;

    Clicker clicker;
    Inventory inventory;
    Hand hand;
    Health health;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        clicker = GetComponent<Clicker>();
        inventory = GetComponent<Inventory>();
        hand = GetComponent<Hand>();
        health = GetComponent<Health>();

        if (IsOwner)
        {
            if (UI_Manager.instance != null)
            {
                uI_Inventory = UI_Manager.instance.GetInventoryUI();
                uI_HoveredMessage = UI_Manager.instance.GetHoveredMessageUI();
                uI_Hearts = UI_Manager.instance.GetHeartsUI();

                uI_Inventory.inventory = inventory;

                clicker.onHoveredChange.AddListener(uI_HoveredMessage.HandleHoveredChange);
                inventory.onItemsChanged.AddListener(uI_Inventory.UpdateSlots);
                hand.onSelectedSlotChanged.AddListener(uI_Inventory.UpdateSelectedSlot);
                health.onCurHealthUpdate.AddListener(uI_Hearts.HandleHealthUpdated);
            }
        }

        if (IsServer)
        {
            health.onDied.AddListener(HandleDeath);
        }
    }

    void HandleDeath()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.EndGame(PlayerRole.monster);
        }
    }
}
