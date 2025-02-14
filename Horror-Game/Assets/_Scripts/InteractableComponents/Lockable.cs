using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Lockable : NetworkBehaviour
{
    public UnityEvent<Clicker> onNotLockedInteraction = new UnityEvent<Clicker>();
    public UnityEvent onUnlocked = new UnityEvent();

    public ItemType requiredKey = ItemType.none;
    public bool initialIsLocked = true;

    NetworkVariable<bool> isLocked = new NetworkVariable<bool>(true);

    void Awake()
    {
        isLocked = new NetworkVariable<bool>(initialIsLocked);
    }

    public void HandleInteraction(Clicker clicker)
    {
        if (!isLocked.Value)
        {
            onNotLockedInteraction.Invoke(clicker);
            return;
        }

        if (requiredKey == ItemType.none)
        {
            Unclock();
            return;
        }

        Inventory inventory = clicker.GetComponent<Inventory>();
        Hand hand = clicker.GetComponent<Hand>();
        if (inventory == null || hand == null)
            return;

        int selectedSlot = hand.GetSelectedSlot();
        ItemStack selectedStack = inventory.GetItemStackAtSlot(selectedSlot);
        if (selectedStack.itemType == requiredKey)
        {
            inventory.RemoveItemFromSlot(requiredKey, selectedSlot);
            Unclock();
        }
    }

    void Unclock()
    {
        if (!IsServer)
            return;

        isLocked.Value = false;
        onUnlocked.Invoke();
    }
}
