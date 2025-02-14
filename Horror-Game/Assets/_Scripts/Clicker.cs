using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Clicker : NetworkBehaviour
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
        if (!IsOwner)
            return;

        SelectInteractable();

        if (curHovered != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
                InteractWithSelectedObject();
        }
    }

    [ServerRpc]
    void InteractWithObjectServerRpc(NetworkObjectReference objRef, int interactableNum = -1)
    {
        if (!objRef.TryGet(out NetworkObject netObj))
        {
            Debug.LogError("Failed to retrieve object from its NetworkObjectReference");
            return;
        }

        Interactable interactable;
        if (interactableNum != -1)
        {
            InteractableMaster interactableMaster = netObj.GetComponent<InteractableMaster>();
            interactable = interactableMaster.GetInteractableByNum(interactableNum);
        }
        else
        {
            interactable = netObj.GetComponent<Interactable>();
        }

        if (interactable != null)
        {
            interactable.Interact(this);
        }
    }
    void InteractWithSelectedObject()
    {
        // client prediction
        if (!IsServer)
            curHovered.Interact(this);

        NetworkObjectReference objRef = default;
        int interactableNum = -1;
        if (curHovered.interactableMaster != null)
        {
            InteractableMaster interactableMaster = curHovered.interactableMaster;
            objRef = interactableMaster.gameObject.GetComponent<NetworkObject>();
            interactableNum = interactableMaster.GetInteractableNum(curHovered);
        }
        else
        {
            objRef = curHovered.GetComponent<NetworkObject>();
        }
        InteractWithObjectServerRpc(objRef, interactableNum);
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
