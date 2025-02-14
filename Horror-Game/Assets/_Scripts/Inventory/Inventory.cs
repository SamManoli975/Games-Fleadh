using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : NetworkBehaviour
{
    public UnityEvent onItemsChanged = new UnityEvent();

    const int slotsCount = 7;

    NetworkList<ItemStack> items;

    void Awake()
    {
        items = new NetworkList<ItemStack>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            for (int i = 0; i < slotsCount; i++)
                items.Add(new ItemStack(ItemType.none, 0));
        }

        items.OnListChanged += (NetworkListEvent<ItemStack> changeEvent) => onItemsChanged.Invoke();
    }

    void RemoveOneItemFromSlot(int slot)
    {
        ItemStack itemStack = items[slot];
        itemStack.count--;
        if (itemStack.count == 0)
            itemStack.itemType = ItemType.none;

        items[slot] = itemStack;
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
            if (items[i].itemType == itemType)
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
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].itemType == itemType)
                    return true;
            }
        }

        // could not add to existing stack, so looking for an empty place
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemType == ItemType.none)
                return true;
        }

        return false;
    }

    public void AddItem(ItemType itemType, int priorityAddPlace = -1)
    {
        if (!IsServer)
            return;

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
                if (items[i].itemType == itemType)
                {
                    items[i] = new ItemStack(items[i].itemType, items[i].count + 1);
                    addedToExistingStack = true;
                    break;
                }
            }
        }

        if (!addedToExistingStack)
        {
            // if possible, add to priority slot
            if (priorityAddPlace >= 0 && priorityAddPlace < items.Count && items[priorityAddPlace].itemType == ItemType.none)
            {
                items[priorityAddPlace] = new ItemStack(itemType, 1);
            }
            else
            {
                // find empty place to put the item (searching from start to end)
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].itemType == ItemType.none)
                    {
                        items[i] = new ItemStack(itemType, 1);
                        break;
                    }
                }
            }
        }
    }

    public void RemoveItem(ItemType itemType, int priorityRemovePlace = -1)
    {
        if (!IsServer)
            return;

        if (!HasItem(itemType))
        {
            Debug.LogError("No item of type '" + itemType + "' in the inventory to remove");
            return;
        }

        // trying to remove from the priority place first
        if (priorityRemovePlace >= 0 && priorityRemovePlace < items.Count && items[priorityRemovePlace].itemType == itemType)
        {
            RemoveOneItemFromSlot(priorityRemovePlace);
        }
        else
        {
            for (int i = 0; i < slotsCount; i++)
            {
                if (items[i].itemType == itemType)
                {
                    RemoveOneItemFromSlot(i);
                    break;
                }
            }
        }
    }

    public void RemoveItemFromSlot(ItemType itemType, int slot)
    {
        if (!IsServer)
            return;

        if (slot >= 0 && slot < items.Count && items[slot].itemType == itemType)
        {
            RemoveOneItemFromSlot(slot);
        }
    }

    public NetworkList<ItemStack> GetItems()
    {
        return items;
    }
}
