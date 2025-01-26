using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script made for convenience. It automatically adds and links: Interactable, Lockable, ToggleRotations
public class Door : MonoBehaviour
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

    [SerializeField] bool doDisableOpenCollider = true;
    [SerializeField] bool doDisableCollidersOnMovement = false;
    [SerializeField] float disableColliderFromProgress = 0;

    Interactable interactable;
    Lockable lockable;
    ToggleRotations toggleRotations;

    void Start()
    {
        interactable = gameObject.AddComponent<Interactable>();
        lockable = gameObject.AddComponent<Lockable>();
        toggleRotations = gameObject.AddComponent<ToggleRotations>();

        interactable.onInteraction.AddListener(lockable.HandleInteraction);
        lockable.onNotLockedInteraction.AddListener(toggleRotations.Toggle);

        interactable.outlineOnHover = outlineOnHover;
        interactable.outline = outline;

        lockable.isLocked = isLocked;
        lockable.requiredKey = requiredKey;

        toggleRotations.isClosed = isClosed;
        toggleRotations.openRotation = openRotation;
        toggleRotations.closedRotation = closedRotation;

        toggleRotations.animTime = animTime;
        toggleRotations.doDisableOpenCollider = doDisableOpenCollider;
        toggleRotations.doDisableCollidersOnMovement = doDisableCollidersOnMovement;
        toggleRotations.disableColliderFromProgress = disableColliderFromProgress;

        toggleRotations.movingPart = gameObject;

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

    void HandleUnclocked()
    {
        lockable.onUnlocked.RemoveListener(HandleUnclocked);

        SetOpenCloseMessage(null);
        interactable.onInteraction.AddListener(SetOpenCloseMessage);
    }

    void SetOpenCloseMessage(Clicker clicker)
    {
        if (toggleRotations.isClosed)
            interactable.SetHoverMessage("Open");
        else
            interactable.SetHoverMessage("Close");
    }
}
