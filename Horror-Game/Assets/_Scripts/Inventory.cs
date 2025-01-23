using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    public UnityEvent<List<ItemStack>> onItemsUpdated;

    List<ItemStack> items = new List<ItemStack>();

    public void AddItem(ItemType itemType, int count = 1)
    {
        items.Add(new ItemStack(itemType, count));

        onItemsUpdated.Invoke(items);
    }
}
