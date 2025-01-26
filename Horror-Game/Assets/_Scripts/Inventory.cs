using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    public UnityEvent<ItemStack[]> onItemsUpdated;

    [SerializeField] int slotsCount = 7;

    ItemStack[] items;

    void Awake()
    {
        items = new ItemStack[slotsCount];
        for (int i = 0; i < items.Length; i++)
            items[i] = null;
    }

    void RemoveOneItemFromSlot(int slot)
    {
        items[slot].count--;
        if (items[slot].count == 0)
            items[slot] = null;
    }

    public int GetSlotsCount()
    {
        return slotsCount;
    }

    public ItemStack GetItemStackAtSlot(int slot)
    {
        return items[slot];
    }

    public bool HasItem(ItemType itemType)
    {
        for (int i = 0; i < slotsCount; i++)
        {
            if (items[i] != null && items[i].itemType == itemType)
            {
                return true;
            }
        }

        return false;
    }

    public bool CanAddItem(ItemType itemType)
    {
        ItemData itemData = ItemsDataManager.instance.GetItemData(itemType);

        if (itemData.isStackable)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null && items[i].itemType == itemType)
                    return true;
            }
        }

        // could not add to existing stack, so looking for an empty place
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
                return true;
        }

        return false;
    }

    public void AddItem(ItemType itemType, int priorityAddPlace = -1)
    {
        if (!CanAddItem(itemType))
        {
            Debug.LogError("Cannot add this item to the inventory (probably inventory is full)");
            return;
        }

        ItemData itemData = ItemsDataManager.instance.GetItemData(itemType);

        bool addedToExistingStack = false;
        // first try to add to an existing stack
        if (itemData.isStackable)
        {
            for (int i = 0; i < slotsCount; i++)
            {
                if (items[i] != null && items[i].itemType == itemType)
                {
                    items[i].count += 1;
                    addedToExistingStack = true;
                    break;
                }
            }
        }

        if (!addedToExistingStack)
        {
            // if possible, add to priority slot
            if (priorityAddPlace >= 0 && priorityAddPlace < items.Length && items[priorityAddPlace] == null)
            {
                items[priorityAddPlace] = new ItemStack(itemType, 1);
            }
            else
            {
                // find empty place to put the item (searching from start to end)
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i] == null)
                    {
                        items[i] = new ItemStack(itemType, 1);
                        break;
                    }
                }
            }
        }

        onItemsUpdated.Invoke(items);
    }

    public void RemoveItem(ItemType itemType, int priorityRemovePlace = -1)
    {
        if (!HasItem(itemType))
        {
            Debug.LogError("No item of type '" + itemType + "' in the inventory to remove");
            return;
        }

        // trying to remove from the priority place first
        if (priorityRemovePlace >= 0 && priorityRemovePlace < items.Length
        && items[priorityRemovePlace] != null && items[priorityRemovePlace].itemType == itemType)
        {
            RemoveOneItemFromSlot(priorityRemovePlace);
        }
        else
        {
            for (int i = 0; i < slotsCount; i++)
            {
                if (items[i] != null && items[i].itemType == itemType)
                {
                    RemoveOneItemFromSlot(i);
                    break;
                }
            }
        }

        onItemsUpdated.Invoke(items);
    }

    public void RemoveItemFromSlot(ItemType itemType, int slot)
    {
        if (slot >= 0 && slot < items.Length
        && items[slot] != null && items[slot].itemType == itemType)
        {
            RemoveOneItemFromSlot(slot);
        }

        onItemsUpdated.Invoke(items);
    }

}
