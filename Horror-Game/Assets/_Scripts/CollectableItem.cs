using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableItem : Interactable
{
    [SerializeField] ItemType itemType;

    protected override void Start()
    {
        base.Start();

        hoverMessage = "Pick up " + ItemsDataManager.instance.GetItemData(itemType).name;
        onClick.AddListener(Collect);
    }

    void Collect(Clicker clicker)
    {
        Inventory inventory = clicker.GetComponent<Inventory>();
        if (inventory == null)
        {
            Debug.LogWarning("Object without inventory is trying to collect an item");
            return;
        }

        inventory.AddItem(itemType);
        Destroy(gameObject);
    }
}
