using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

// Script made for convenience. It automatically adds and links: Interactable, Lockable, ToggleRotations
public class Door : NetworkBehaviour, IManagedInteractable
{
    public bool isClosed = true;
    public bool isLocked = false;
    public ItemType requiredKey = ItemType.none;

    [SerializeField] float openRotation = 90;
    [SerializeField] float closedRotation = 0;

    [SerializeField] float animTime = 0.4f;

    [Tooltip("Do not forget to add 'Outline' component if set to true")]
    [SerializeField] bool outlineOnHover = false;
    [SerializeField] Outline outline;
    [SerializeField] InteractableMaster interactableMaster;

    [SerializeField] bool doDisableOpenCollider = true;
    [SerializeField] bool doDisableCollidersOnMovement = false;
    [SerializeField] float disableColliderFromProgress = 0;

    Interactable interactable;
    Lockable lockable;
    ToggleRotations toggleRotations;

    void Awake()
    {
        interactable = gameObject.GetComponent<Interactable>();
        lockable = gameObject.GetComponent<Lockable>();
        toggleRotations = gameObject.GetComponent<ToggleRotations>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        interactable.onInteraction.AddListener(lockable.HandleInteraction);
        lockable.onNotLockedInteraction.AddListener(toggleRotations.Toggle);

        if (isLocked)
        {
            string msg = "Unlock";
            if (requiredKey != ItemType.none)
            {
                ItemData itemData = ItemsDataManager.instance.GetItemData(requiredKey);
                msg = "Need " + itemData.name;
            }
            interactable.SetHoverMessage(msg);

            lockable.onUnlocked.AddListener(HandleUnclocked);
        }
        else
        {
            SetOpenCloseMessage(null);
            interactable.onInteraction.AddListener(SetOpenCloseMessage);
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
        NetworkTransform networkTransform = AddComponentIfDoesNotHave<NetworkTransform>();
        interactable = AddComponentIfDoesNotHave<Interactable>();
        lockable = AddComponentIfDoesNotHave<Lockable>();
        toggleRotations = AddComponentIfDoesNotHave<ToggleRotations>();

        interactable.outlineOnHover = outlineOnHover;
        interactable.outline = outline;
        interactable.interactableMaster = interactableMaster;

        lockable.requiredKey = requiredKey;

        lockable.initialIsLocked = isLocked;
        toggleRotations.initialIsClosed = isClosed;

        toggleRotations.openRotation = openRotation;
        toggleRotations.closedRotation = closedRotation;

        toggleRotations.animTime = animTime;
        toggleRotations.doDisableOpenCollider = doDisableOpenCollider;
        toggleRotations.doDisableCollidersOnMovement = doDisableCollidersOnMovement;
        toggleRotations.disableColliderFromProgress = disableColliderFromProgress;

        toggleRotations.movingPart = gameObject;

        networkTransform.InLocalSpace = true;

        networkTransform.SyncPositionX = false;
        networkTransform.SyncPositionY = false;
        networkTransform.SyncPositionZ = false;

        networkTransform.SyncRotAngleX = false;
        networkTransform.SyncRotAngleY = true;
        networkTransform.SyncRotAngleZ = false;

        networkTransform.SyncScaleX = false;
        networkTransform.SyncScaleY = false;
        networkTransform.SyncScaleZ = false;

        return new Component[] { this, networkTransform, interactable, lockable, toggleRotations };
    }

    public SetupInteractableMasterRes SetupInteractableMaster(InteractableMaster interactableMaster)
    {
        this.interactableMaster = interactableMaster;

        Component[] modifiedComponents = SetupComponents();
        interactable = gameObject.GetComponent<Interactable>();
        return new SetupInteractableMasterRes(modifiedComponents, new List<Interactable> { interactable });
    }

    void HandleUnclocked()
    {
        lockable.onUnlocked.RemoveListener(HandleUnclocked);

        SetOpenCloseMessage(null);
        interactable.onInteraction.AddListener(SetOpenCloseMessage);
    }

    void SetOpenCloseMessage(Clicker clicker)
    {
        if (!IsServer)
            return;

        if (toggleRotations.GetIsClosed())
            interactable.SetHoverMessage("Open");
        else
            interactable.SetHoverMessage("Close");
    }
}
