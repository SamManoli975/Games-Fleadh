using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CollectableItem : NetworkBehaviour, IManagedInteractable
{
    [SerializeField] ItemType itemType;

    [Tooltip("Do not forget to add 'Outline' component if set to true")]
    [SerializeField] bool outlineOnHover = false;
    [SerializeField] Outline outline;
    [SerializeField] InteractableMaster interactableMaster;

    Interactable interactable;

    void Awake()
    {
        interactable = gameObject.GetComponent<Interactable>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            interactable.SetHoverMessage("Pick up " + ItemsDataManager.instance.GetItemData(itemType).name);
            interactable.onInteraction.AddListener(Collect);
        }
    }

    T AddComponentIfDoesNotHave<T>() where T : UnityEngine.Component
    {
        T c = gameObject.GetComponent<T>();
        if (c == null)
        {
            c = gameObject.AddComponent<T>();
        }
        return c;
    }

    public Component[] SetupComponents()
    {
        interactable = AddComponentIfDoesNotHave<Interactable>();

        interactable.outlineOnHover = outlineOnHover;
        interactable.outline = outline;
        interactable.interactableMaster = interactableMaster;

        return new Component[] { this, interactable };
    }

    void Collect(Clicker clicker)
    {
        if (!IsServer)
            return;

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
            NetworkObject.Despawn(true);
        }
    }

    public SetupInteractableMasterRes SetupInteractableMaster(InteractableMaster interactableMaster)
    {
        this.interactableMaster = interactableMaster;

        Component[] modifiedComponents = SetupComponents();
        interactable = gameObject.GetComponent<Interactable>();
        return new SetupInteractableMasterRes(modifiedComponents, new List<Interactable> { interactable });
    }
}
