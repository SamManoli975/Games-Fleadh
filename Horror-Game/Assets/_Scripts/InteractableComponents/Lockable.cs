using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Lockable : MonoBehaviour
{

    public UnityEvent<Clicker> onNotLockedInteraction = new UnityEvent<Clicker>();
    public UnityEvent onUnlocked = new UnityEvent();

    public ItemType requiredKey = ItemType.none;
    public bool isLocked = true;

    public void HandleInteraction(Clicker clicker)
    {
        if (!isLocked)
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
        if (selectedStack != null && selectedStack.itemType == requiredKey)
        {
            inventory.RemoveItemFromSlot(requiredKey, selectedSlot);
            Unclock();
        }
    }

    void Unclock()
    {
        isLocked = false;
        onUnlocked.Invoke();
    }
}
