using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Inventory))]
public class Hand : NetworkBehaviour
{
    public UnityEvent<int> onSelectedSlotChanged;

    [SerializeField] Transform handPlace;

    [SerializeField] float scrollSens = 3f;
    [SerializeField] float scrollInterval = 0.05f;
    [SerializeField] float minScrollAmount = 0.2f;
    [SerializeField] float maxStoredScroll = 3f;

    [SerializeField] Transform itemsDropPoint;

    NetworkVariable<int> selectedSlot = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    Inventory inventory;

    NetworkVariable<ItemType> curHandItemType = new NetworkVariable<ItemType>(ItemType.none);
    NetworkVariable<NetworkObjectReference> curHandItemObjRef = new NetworkVariable<NetworkObjectReference>();

    GameObject curHandItemObj = null;
    HandItem curHandItem = null;

    float scrollTimer = 0;
    float storedScroll = 0;

    void Awake()
    {
        inventory = GetComponent<Inventory>();
        inventory.onItemsChanged.AddListener(HandleItemsUpdated);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        selectedSlot.OnValueChanged += (int previous, int current) => onSelectedSlotChanged.Invoke(current);
        if (IsServer)
        {
            selectedSlot.OnValueChanged += (int previous, int current) => ChangeHandItemBase();
        }

        curHandItemObjRef.OnValueChanged += HandleCurItemChanged;

        if (IsOwner)
        {
            ChangeSelectedSlot(0);
        }
    }

    void Update()
    {
        if (!IsOwner)
            return;

        ChooseSlot();

        if (Input.GetMouseButtonDown(0))
        {
            if (curHandItem != null)
                curHandItem.Use();
        }

        if (Input.GetKeyDown(KeyCode.Q) && curHandItemType.Value != ItemType.none)
        {
            DropCurItem();
        }
    }

    [ServerRpc]
    void DropCurItemServerRpc()
    {
        // just if to double check
        if (inventory.GetItemStackAtSlot(selectedSlot.Value).itemType == ItemType.none)
        {
            Debug.LogError("Trying to throw item which is not in the inventory");
            return;
        }

        ItemData itemData = ItemsDataManager.instance.GetItemData(curHandItemType.Value);
        if (itemData.collectableItemPrefab != null)
        {
            GameObject collectableItem = Instantiate(itemData.collectableItemPrefab);
            collectableItem.transform.position = itemsDropPoint.transform.position;
            collectableItem.GetComponent<NetworkObject>().Spawn(true);
        }

        inventory.RemoveItemFromSlot(curHandItemType.Value, selectedSlot.Value);
        curHandItemType.Value = ItemType.none;
    }

    void DropCurItem()
    {
        DropCurItemServerRpc();
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

            int nextSelectedSlot = Mod(selectedSlot.Value + (int)Mathf.Sign(storedScroll), inventory.GetSlotsCount());
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
        selectedSlot.Value = newValue;
    }

    void HandleCurItemChanged(NetworkObjectReference previous, NetworkObjectReference current)
    {
        if (!current.TryGet(out NetworkObject netObj))
        {
            curHandItemObj = null;
            curHandItem = null;
            return;
        }

        curHandItemObj = netObj.gameObject;
        curHandItem = netObj.gameObject.GetComponent<HandItem>();
        netObj.GetComponent<FollowTransform>().target = handPlace;
    }

    void ChangeHandItemBase()
    {
        if (!IsServer)
            return;

        if (selectedSlot.Value < 0 || selectedSlot.Value > inventory.GetSlotsCount())
            return;

        if (curHandItemObj != null)
        {
            curHandItemObj.GetComponent<NetworkObject>().Despawn(true);
        }

        curHandItemObjRef.Value = default;
        curHandItemType.Value = ItemType.none;

        ItemStack itemStack = inventory.GetItemStackAtSlot(selectedSlot.Value);
        if (itemStack.itemType != ItemType.none)
        {
            ItemData itemData = ItemsDataManager.instance.GetItemData(itemStack.itemType);
            curHandItemType.Value = itemStack.itemType;

            if (itemData.handItemPrefab != null)
            {
                GameObject newItem = Instantiate(itemData.handItemPrefab);
                newItem.GetComponent<FollowTransform>().target = handPlace;
                newItem.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId, true);

                curHandItemObjRef.Value = newItem.GetComponent<NetworkObject>();
            }
        }
    }

    [ServerRpc]
    void ChangeHandItemServerRpc()
    {
        ChangeHandItemBase();
    }
    void ChangeHandItem()
    {
        if (IsOwner)
            ChangeHandItemServerRpc();
    }

    void HandleItemsUpdated()
    {
        var items = inventory.GetItems();

        if (selectedSlot.Value < 0 || selectedSlot.Value > inventory.GetSlotsCount())
            return;

        if (items[selectedSlot.Value].itemType != curHandItemType.Value)
        {
            ChangeHandItem();
        }
    }

    public int GetSelectedSlot()
    {
        return selectedSlot.Value;
    }
}
