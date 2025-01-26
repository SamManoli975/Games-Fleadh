using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script made for convenience. It automatically adds and links: Interactable, Lockable, ToggleRotations
public class Drawer : MonoBehaviour
{
    public bool isClosed = true;
    public bool isLocked = false;
    public ItemType requiredKey = ItemType.none;

    [SerializeField] Transform openPoint;
    [SerializeField] Transform closedPoint;

    [SerializeField] float animTime = 0.3f;

    [Tooltip("Do not forget to add 'Outline' component if set to true")]
    [SerializeField] bool outlineOnHover = false;
    [SerializeField] Outline outline;

    [SerializeField] bool doDisableOpenCollider = true;
    [SerializeField] bool doDisableCollidersOnMovement = false;
    [SerializeField] float disableColliderFromProgress = 0;

    Interactable interactable;
    Lockable lockable;
    ToggleTransforms toggleTransforms;

    void Start()
    {
        interactable = gameObject.AddComponent<Interactable>();
        lockable = gameObject.AddComponent<Lockable>();
        toggleTransforms = gameObject.AddComponent<ToggleTransforms>();

        interactable.onInteraction.AddListener(lockable.HandleInteraction);
        lockable.onNotLockedInteraction.AddListener(toggleTransforms.Toggle);

        interactable.outlineOnHover = outlineOnHover;
        interactable.outline = outline;

        lockable.isLocked = isLocked;
        lockable.requiredKey = requiredKey;

        toggleTransforms.isClosed = isClosed;
        toggleTransforms.openPoint = openPoint;
        toggleTransforms.closedPoint = closedPoint;

        toggleTransforms.animTime = animTime;
        toggleTransforms.doDisableOpenCollider = doDisableOpenCollider;
        toggleTransforms.doDisableCollidersOnMovement = doDisableCollidersOnMovement;
        toggleTransforms.disableColliderFromProgress = disableColliderFromProgress;

        toggleTransforms.movingPart = gameObject;

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
        if (toggleTransforms.isClosed)
            interactable.SetHoverMessage("Open");
        else
            interactable.SetHoverMessage("Close");
    }
}
