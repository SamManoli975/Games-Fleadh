using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UI_Inventory : MonoBehaviour
{
    [SerializeField] List<UI_InventorySlot> slots;
    [SerializeField] TextMeshProUGUI curItemNameTextfield;

    public Inventory inventory;

    int curSelectedSlot = -1;

    void Start()
    {
        InitSlots();
        curItemNameTextfield.gameObject.SetActive(false);
    }

    void InitSlots()
    {
        for (int i = 0; i < slots.Count; i++)
            slots[i].UpdateSlot(new ItemStack(ItemType.none, 0));
    }

    public void UpdateSlots()
    {
        NetworkList<ItemStack> items = inventory.GetItems();
        for (int i = 0; i < slots.Count; i++)
            slots[i].UpdateSlot(i < items.Count ? items[i] : new ItemStack(ItemType.none, 0));

        UpdateSelectedItemName();
    }

    void UpdateSelectedItemName()
    {
        if (curSelectedSlot >= 0 && curSelectedSlot < slots.Count && slots[curSelectedSlot].curItemData != null)
        {
            curItemNameTextfield.text = slots[curSelectedSlot].curItemData.name;
            curItemNameTextfield.gameObject.SetActive(true);
        }
        else
        {
            curItemNameTextfield.gameObject.SetActive(false);
        }
    }

    public void UpdateSelectedSlot(int newSelectedSlot)
    {
        if (curSelectedSlot == newSelectedSlot)
            return;

        if (curSelectedSlot != -1)
            slots[curSelectedSlot].Deselect();

        slots[newSelectedSlot].Select();
        curSelectedSlot = newSelectedSlot;

        UpdateSelectedItemName();
    }
}
