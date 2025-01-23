using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Inventory : MonoBehaviour
{
    [SerializeField] List<UI_InventorySlot> slots;

    void Start()
    {
        InitSlots();
    }

    void InitSlots()
    {
        for (int i = 0; i < slots.Count; i++)
            slots[i].UpdateSlot(null);
    }

    public void UpdateSlots(List<ItemStack> items)
    {
        for (int i = 0; i < slots.Count; i++)
            slots[i].UpdateSlot(i < items.Count ? items[i] : null);
    }
}
