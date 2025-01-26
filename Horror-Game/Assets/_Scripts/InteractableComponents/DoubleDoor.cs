using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleDoor : MonoBehaviour
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

    [SerializeField] bool outlineOnHover = false;
    [SerializeField] Outline outline;

    [SerializeField] bool doDisableOpenCollider = true;
    [SerializeField] bool doDisableCollidersOnMovement = false;
    [SerializeField] float disableColliderFromProgress = 0;

    Interactable interactablePart1;
    Interactable interactablePart2;
    Lockable lockable;
    ToggleRotations toggleRotationsPart1;
    ToggleRotations toggleRotationsPart2;

    void Start()
    {
        interactablePart1 = part1.AddComponent<Interactable>();
        interactablePart2 = part2.AddComponent<Interactable>();

        toggleRotationsPart1 = gameObject.AddComponent<ToggleRotations>();
        toggleRotationsPart2 = gameObject.AddComponent<ToggleRotations>();

        lockable = gameObject.AddComponent<Lockable>();

        interactablePart1.onInteraction.AddListener(lockable.HandleInteraction);
        interactablePart2.onInteraction.AddListener(lockable.HandleInteraction);

        lockable.onNotLockedInteraction.AddListener(HandleNotLockedInteraction);

        interactablePart1.outlineOnHover = outlineOnHover;
        interactablePart1.outline = outline;

        interactablePart2.outlineOnHover = outlineOnHover;
        interactablePart2.outline = outline;

        lockable.isLocked = isLocked;
        lockable.requiredKey = requiredKey;

        toggleRotationsPart1.isClosed = isClosed;
        toggleRotationsPart1.openRotation = part1OpenRotation;
        toggleRotationsPart1.closedRotation = part1ClosedRotation;

        toggleRotationsPart2.isClosed = isClosed;
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
        if (toggleRotationsPart1.isClosed)
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
}
