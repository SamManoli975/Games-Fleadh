using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStack
{
    public ItemType itemType;
    public int count;

    public ItemStack(ItemType itemType, int count)
    {
        this.itemType = itemType;
        this.count = count;
    }
}
