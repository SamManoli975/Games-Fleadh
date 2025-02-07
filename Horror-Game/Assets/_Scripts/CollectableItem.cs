using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    [SerializeField] ItemType itemType;

    [Tooltip("Do not forget to add 'Outline' component if set to true")]
    [SerializeField] bool outlineOnHover = false;
    [SerializeField] Outline outline;

    Interactable interactable;

    void Start()
    {
        interactable = gameObject.AddComponent<Interactable>();

        interactable.outlineOnHover = outlineOnHover;
        interactable.outline = outline;

        interactable.SetHoverMessage("Pick up " + ItemsDataManager.instance.GetItemData(itemType).name);
        interactable.onInteraction.AddListener(Collect);
    }

    void Collect(Clicker clicker)
    {
        Inventory inventory = clicker.GetComponent<Inventory>();
        if (inventory == null)
            return;

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
