using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Inventory : MonoBehaviour
{
    [SerializeField] List<UI_InventorySlot> slots;

    int curSelectedSlot = -1;

    void Start()
    {
        InitSlots();
    }

    void InitSlots()
    {
        for (int i = 0; i < slots.Count; i++)
            slots[i].UpdateSlot(null);
    }

    public void UpdateSlots(ItemStack[] items)
    {
        for (int i = 0; i < slots.Count; i++)
            slots[i].UpdateSlot(i < items.Length ? items[i] : null);
    }

    public void UpdateSelectedSlot(int newSelectedSlot)
    {
        if (curSelectedSlot == newSelectedSlot)
            return;

        if (curSelectedSlot != -1)
            slots[curSelectedSlot].Deselect();

        slots[newSelectedSlot].Select();
        curSelectedSlot = newSelectedSlot;
    }
}
