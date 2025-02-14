using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

// Script made for convenience. It automatically adds and links: Interactable, Lockable, ToggleRotations
public class Drawer : NetworkBehaviour, IManagedInteractable
{
    public bool isClosed = true;
    public bool isLocked = false;
    public ItemType requiredKey = ItemType.none;

    [SerializeField] Transform openPoint;
    [SerializeField] Transform closedPoint;

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
    ToggleTransforms toggleTransforms;

    void Awake()
    {
        interactable = gameObject.GetComponent<Interactable>();
        lockable = gameObject.GetComponent<Lockable>();
        toggleTransforms = gameObject.GetComponent<ToggleTransforms>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        interactable.onInteraction.AddListener(lockable.HandleInteraction);
        lockable.onNotLockedInteraction.AddListener(toggleTransforms.Toggle);

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
        toggleTransforms = AddComponentIfDoesNotHave<ToggleTransforms>();

        interactable.outlineOnHover = outlineOnHover;
        interactable.outline = outline;
        interactable.interactableMaster = interactableMaster;

        lockable.requiredKey = requiredKey;

        lockable.initialIsLocked = isLocked;
        toggleTransforms.initialIsClosed = isClosed;

        toggleTransforms.openPoint = openPoint;
        toggleTransforms.closedPoint = closedPoint;

        toggleTransforms.animTime = animTime;
        toggleTransforms.doDisableOpenCollider = doDisableOpenCollider;
        toggleTransforms.doDisableCollidersOnMovement = doDisableCollidersOnMovement;
        toggleTransforms.disableColliderFromProgress = disableColliderFromProgress;

        toggleTransforms.movingPart = gameObject;

        networkTransform.InLocalSpace = true;

        networkTransform.SyncPositionX = true;
        networkTransform.SyncPositionY = true;
        networkTransform.SyncPositionZ = true;

        networkTransform.SyncRotAngleX = false;
        networkTransform.SyncRotAngleY = false;
        networkTransform.SyncRotAngleZ = false;

        networkTransform.SyncScaleX = false;
        networkTransform.SyncScaleY = false;
        networkTransform.SyncScaleZ = false;

        return new Component[] { this, networkTransform, interactable, lockable, toggleTransforms };
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

        if (toggleTransforms.GetIsClosed())
            interactable.SetHoverMessage("Open");
        else
            interactable.SetHoverMessage("Close");
    }
}
