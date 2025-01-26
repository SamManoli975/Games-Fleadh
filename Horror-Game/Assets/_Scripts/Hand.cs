using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Inventory))]
public class Hand : MonoBehaviour
{
    public UnityEvent<int> onSelectedSlotChanged;

    [SerializeField] Transform handPlace;

    [SerializeField] float scrollSens = 3f;
    [SerializeField] float scrollInterval = 0.05f;
    [SerializeField] float minScrollAmount = 0.2f;
    [SerializeField] float maxStoredScroll = 3f;

    [SerializeField] Transform itemsDropPoint;

    int selectedSlot = 0;
    Inventory inventory;

    GameObject curHandItemObj = null;
    HandItem curHandItem = null;
    ItemType curHandItemType = ItemType.none;

    float scrollTimer = 0;
    float storedScroll = 0;

    void Start()
    {
        inventory = GetComponent<Inventory>();
        inventory.onItemsUpdated.AddListener(HandleItemsUpdated);
        ChangeSelectedSlot(0);
    }

    void Update()
    {
        ChooseSlot();

        if (Input.GetMouseButtonDown(0))
        {
            if (curHandItem != null)
                curHandItem.Use();
        }

        if (Input.GetKeyDown(KeyCode.Q) && curHandItemType != ItemType.none)
        {
            DropCurItem();
        }
    }

    void DropCurItem()
    {
        // double check
        if (inventory.GetItemStackAtSlot(selectedSlot) == null)
        {
            Debug.LogError("Trying to throw item which is not in the inventory");
            return;
        }

        ItemData itemData = ItemsDataManager.instance.GetItemData(curHandItemType);
        if (itemData.collectableItemPrefab != null)
        {
            GameObject collectableItem = Instantiate(itemData.collectableItemPrefab);
            collectableItem.transform.position = itemsDropPoint.transform.position;
        }

        inventory.RemoveItemFromSlot(curHandItemType, selectedSlot);
    }

    void ChooseSlot()
    {
        if (scrollTimer != 0)
        {
            scrollTimer -= Time.deltaTime;
            if (scrollTimer < 0)
                scrollTimer = 0;
        }

        float mouseScroll = -Mathf.Pow(Input.GetAxis("Mouse ScrollWheel"), 3) * scrollSens;
        if (mouseScroll != 0)
        {
            if (Mathf.Abs(mouseScroll) < minScrollAmount)
                mouseScroll = Mathf.Sign(mouseScroll) * minScrollAmount;

            if (Mathf.Sign(storedScroll) != Mathf.Sign(mouseScroll))
                storedScroll = 0;

            storedScroll += mouseScroll * Time.deltaTime;
            storedScroll = Mathf.Clamp(storedScroll, -maxStoredScroll, maxStoredScroll);
        }

        if (storedScroll != 0 && scrollTimer == 0)
        {
            scrollTimer = scrollInterval;

            int nextSelectedSlot = Mod(selectedSlot + (int)Mathf.Sign(storedScroll), inventory.GetSlotsCount());
            ChangeSelectedSlot(nextSelectedSlot);

            storedScroll -= Mathf.Sign(storedScroll);
            if (Mathf.Abs(storedScroll) < 1)
                storedScroll = 0;
        }

        if (Input.anyKeyDown)
        {
            for (int i = 0; i < inventory.GetSlotsCount(); i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    ChangeSelectedSlot(i);
                    break;
                }
            }
        }
    }

    // finds proper a % b with handling of negative numbers
    int Mod(int a, int b)
    {
        return (a % b + b) % b;
    }

    void ChangeSelectedSlot(int newValue)
    {
        selectedSlot = newValue;

        ChangeHandItem();

        onSelectedSlotChanged.Invoke(selectedSlot);
    }

    void ChangeHandItem()
    {
        if (curHandItemObj != null)
        {
            Destroy(curHandItemObj);
        }

        curHandItemObj = null;
        curHandItem = null;
        curHandItemType = ItemType.none;

        ItemStack itemStack = inventory.GetItemStackAtSlot(selectedSlot);
        if (itemStack != null)
        {
            ItemData itemData = ItemsDataManager.instance.GetItemData(itemStack.itemType);
            curHandItemType = itemStack.itemType;

            if (itemData.handItemPrefab != null)
            {
                curHandItemObj = Instantiate(itemData.handItemPrefab, handPlace);
                curHandItemObj.transform.localPosition = Vector3.zero;

                curHandItem = curHandItemObj.GetComponent<HandItem>();
            }
        }
    }

    void HandleItemsUpdated(ItemStack[] items)
    {
        if ((items[selectedSlot] == null && curHandItemType != ItemType.none) ||
            (items[selectedSlot] != null && items[selectedSlot].itemType != curHandItemType))
            ChangeHandItem();
    }

    public int GetSelectedSlot()
    {
        return selectedSlot;
    }
}
