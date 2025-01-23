using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum InteractionType
{
    leftMouse,
    rightMouse,
    interactionKey
}

public class Clicker : MonoBehaviour
{
    public UnityEvent<Interactable> onHoveredChange;

    [SerializeField] float reachRange = 3f;
    [SerializeField] LayerMask layerMask;
    [Tooltip("Usually camera's transform. Position and forward vector are used for ray's position and direction")]
    [SerializeField] Transform rayOrigin;

    Interactable curHovered = null;
    // keeps track of whether 'curHovered' was set to an interactable or not to handle situations where 'curHovered' object is destroyed
    bool hadCurHovered = false;

    void Update()
    {
        SelectInteractable();

        if (curHovered != null)
        {
            if (Input.GetMouseButtonDown(0))
                curHovered.Interact(InteractionType.leftMouse, this);

            if (Input.GetMouseButtonDown(1))
                curHovered.Interact(InteractionType.rightMouse, this);

            if (Input.GetKeyDown(KeyCode.E))
                curHovered.Interact(InteractionType.interactionKey, this);
        }
    }

    void SelectInteractable()
    {
        bool hoveredOnInteractable = false;

        if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out RaycastHit hit, reachRange, layerMask))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                hoveredOnInteractable = true;
                if (curHovered != interactable)
                {
                    SetNewHovered(interactable);
                }
            }
        }

        if (!hoveredOnInteractable && hadCurHovered)
            UnsetCurHovered();
    }

    void SetNewHovered(Interactable newInteractable)
    {
        if (curHovered != null)
            curHovered.StopHovering();

        newInteractable.StartHovering();
        curHovered = newInteractable;
        hadCurHovered = true;

        onHoveredChange.Invoke(newInteractable);
    }

    void UnsetCurHovered()
    {
        if (curHovered != null)
        {
            curHovered.StopHovering();
            curHovered = null;
        }

        hadCurHovered = false;

        onHoveredChange.Invoke(null);
    }
}
