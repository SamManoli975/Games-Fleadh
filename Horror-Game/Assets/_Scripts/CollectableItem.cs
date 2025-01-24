using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;

public class CollectableItem : Interactable
{
    [SerializeField] ItemType itemType;

    protected override void Start()
    {
        base.Start();

        hoverMessage = "Pick up " + ItemsDataManager.instance.GetItemData(itemType).name;
        onInteraction.AddListener(Collect);
    }

    void Collect(Clicker clicker)
    {
        Inventory inventory = clicker.GetComponent<Inventory>();
        if (inventory == null)
        {
            Debug.LogWarning("Object without inventory is trying to collect an item");
            return;
        }

        if (inventory.CanAddItem(itemType))
        {
            int prioritySlot = -1;
            Hand hand = clicker.GetComponent<Hand>();
            if (hand != null)
                prioritySlot = hand.GetSelectedSlot();

            inventory.AddItem(itemType, prioritySlot);
            Destroy(gameObject);
        }
    }
}
