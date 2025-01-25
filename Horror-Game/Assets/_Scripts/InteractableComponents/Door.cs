using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script made for convenience. It automatically and links: Interactable, Lockable, ToggleRotations
public class Door : MonoBehaviour
{
    public bool isClosed = true;
    public bool isLocked = false;
    public ItemType requiredKey = ItemType.none;

    [SerializeField] float openRotation = 90;
    [SerializeField] float closedRotation = 0;

    [SerializeField] float animTime = 0.3f;

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
    }
}
