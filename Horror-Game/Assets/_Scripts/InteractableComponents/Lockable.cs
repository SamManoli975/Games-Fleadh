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
        if (inventory == null)
            return;

        if (inventory.HasItem(requiredKey))
        {
            inventory.RemoveItem(requiredKey);
            Unclock();
        }
    }

    void Unclock()
    {
        isLocked = false;
        onUnlocked.Invoke();
    }
}
