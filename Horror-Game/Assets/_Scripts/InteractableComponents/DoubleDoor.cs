using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class DoubleDoor : NetworkBehaviour, IManagedInteractable
{
    public bool isClosed = true;
    public bool isLocked = false;
    public ItemType requiredKey = ItemType.none;

    [SerializeField] GameObject part1;
    [SerializeField] float part1OpenRotation = 90;
    [SerializeField] float part1ClosedRotation = 0;

    [SerializeField] GameObject part2;
    [SerializeField] float part2OpenRotation = 90;
    [SerializeField] float part2ClosedRotation = 0;

    [SerializeField] float animTime = 0.3f;

    [Tooltip("Do not forget to add 'Outline' component if set to true")]
    [SerializeField] bool outlineOnHover = false;
    [SerializeField] Outline outline;
    [SerializeField] InteractableMaster interactableMaster;

    [SerializeField] bool doDisableOpenCollider = true;
    [SerializeField] bool doDisableCollidersOnMovement = false;
    [SerializeField] float disableColliderFromProgress = 0;

    Interactable interactablePart1;
    Interactable interactablePart2;
    Lockable lockable;
    ToggleRotations toggleRotationsPart1;
    ToggleRotations toggleRotationsPart2;

    void Awake()
    {
        interactablePart1 = part1.GetComponent<Interactable>();
        interactablePart2 = part2.GetComponent<Interactable>();

        toggleRotationsPart1 = part1.GetComponent<ToggleRotations>();
        toggleRotationsPart2 = part2.GetComponent<ToggleRotations>();

        lockable = gameObject.GetComponent<Lockable>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        interactablePart1.onInteraction.AddListener(lockable.HandleInteraction);
        interactablePart2.onInteraction.AddListener(lockable.HandleInteraction);

        lockable.onNotLockedInteraction.AddListener(HandleNotLockedInteraction);

        if (isLocked)
        {
            string msg = "Unlock";
            if (requiredKey != ItemType.none)
            {
                ItemData itemData = ItemsDataManager.instance.GetItemData(requiredKey);
                msg = "Need " + itemData.name;
            }
            interactablePart1.SetHoverMessage(msg);
            interactablePart2.SetHoverMessage(msg);

            lockable.onUnlocked.AddListener(HandleUnclocked);
        }
        else
        {
            SetOpenCloseMessage();
        }
    }

    T AddComponentIfDoesNotHave<T>(GameObject obj) where T : UnityEngine.Component
    {
        T c = obj.GetComponent<T>();
        if (c == null)
        {
            c = obj.AddComponent<T>();
        }
        return c;
    }

    public Component[] SetupComponents()
    {
        lockable = AddComponentIfDoesNotHave<Lockable>(gameObject);

        interactablePart1 = AddComponentIfDoesNotHave<Interactable>(part1);
        toggleRotationsPart1 = AddComponentIfDoesNotHave<ToggleRotations>(part1);
        NetworkTransform part1NetworkTransform = AddComponentIfDoesNotHave<NetworkTransform>(part1);

        interactablePart2 = AddComponentIfDoesNotHave<Interactable>(part2);
        toggleRotationsPart2 = AddComponentIfDoesNotHave<ToggleRotations>(part2);
        NetworkTransform part2NetworkTransform = AddComponentIfDoesNotHave<NetworkTransform>(part2);

        interactablePart1.outlineOnHover = outlineOnHover;
        interactablePart1.outline = outline;
        interactablePart1.interactableMaster = interactableMaster;

        interactablePart2.outlineOnHover = outlineOnHover;
        interactablePart2.outline = outline;
        interactablePart2.interactableMaster = interactableMaster;

        lockable.requiredKey = requiredKey;

        lockable.initialIsLocked = isLocked;

        toggleRotationsPart1.initialIsClosed = isClosed;
        toggleRotationsPart2.initialIsClosed = isClosed;

        toggleRotationsPart1.openRotation = part1OpenRotation;
        toggleRotationsPart1.closedRotation = part1ClosedRotation;

        toggleRotationsPart2.openRotation = part2OpenRotation;
        toggleRotationsPart2.closedRotation = part2ClosedRotation;

        toggleRotationsPart1.animTime = animTime;
        toggleRotationsPart1.doDisableOpenCollider = doDisableOpenCollider;
        toggleRotationsPart1.doDisableCollidersOnMovement = doDisableCollidersOnMovement;
        toggleRotationsPart1.disableColliderFromProgress = disableColliderFromProgress;

        toggleRotationsPart2.animTime = animTime;
        toggleRotationsPart2.doDisableOpenCollider = doDisableOpenCollider;
        toggleRotationsPart2.doDisableCollidersOnMovement = doDisableCollidersOnMovement;
        toggleRotationsPart2.disableColliderFromProgress = disableColliderFromProgress;

        toggleRotationsPart1.movingPart = part1;
        toggleRotationsPart2.movingPart = part2;

        SetupNetworkTransform(part1NetworkTransform);
        SetupNetworkTransform(part2NetworkTransform);

        return new Component[] { this, lockable, interactablePart1, toggleRotationsPart1,
            interactablePart2, toggleRotationsPart2, part2NetworkTransform };
    }

    void SetupNetworkTransform(NetworkTransform networkTransform)
    {
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
    }

    void HandleNotLockedInteraction(Clicker clicker)
    {
        toggleRotationsPart1.Toggle(clicker);
        toggleRotationsPart2.Toggle(clicker);

        SetOpenCloseMessage();
    }

    void HandleUnclocked()
    {
        lockable.onUnlocked.RemoveListener(HandleUnclocked);
        SetOpenCloseMessage();
    }

    void SetOpenCloseMessage()
    {
        if (!IsServer)
            return;

        if (toggleRotationsPart1.GetIsClosed())
        {
            interactablePart1.SetHoverMessage("Open");
            interactablePart2.SetHoverMessage("Open");
        }
        else
        {
            interactablePart1.SetHoverMessage("Close");
            interactablePart2.SetHoverMessage("Close");
        }
    }

    public SetupInteractableMasterRes SetupInteractableMaster(InteractableMaster interactableMaster)
    {
        this.interactableMaster = interactableMaster;

        Component[] modifiedComponents = SetupComponents();
        interactablePart1 = part1.GetComponent<Interactable>();
        interactablePart2 = part2.GetComponent<Interactable>();
        return new SetupInteractableMasterRes(modifiedComponents, new List<Interactable> { interactablePart1, interactablePart2 });
    }
}
