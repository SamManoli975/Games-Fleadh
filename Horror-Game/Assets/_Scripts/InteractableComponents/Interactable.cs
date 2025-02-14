using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : NetworkBehaviour, IManagedInteractable
{
    public UnityEvent<Clicker> onInteraction = new UnityEvent<Clicker>();
    public UnityEvent<string> onHoverMessageChanged = new UnityEvent<string>();

    [Tooltip("Message to display when the object is hovered")]
    public string initialHoverMessage = "";

    [Tooltip("Do not forget to add 'Outline' component if set to true")]
    public bool outlineOnHover = true;

    public Outline outline;
    public InteractableMaster interactableMaster;

    NetworkVariable<FixedString128Bytes> hoverMessage;

    void Awake()
    {
        hoverMessage = new NetworkVariable<FixedString128Bytes>(initialHoverMessage);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        hoverMessage.OnValueChanged += (oldValue, newValue) =>
        {
            onHoverMessageChanged?.Invoke(newValue.ToString());
        };
    }

    void Start()
    {
        if (outlineOnHover)
        {
            if (outline == null)
            {
                outline = GetComponent<Outline>();
                if (outline == null)
                    Debug.LogError("Add 'Outline' component to an object if you have 'outlineOnHover' enabled");
            }
            outline.enabled = false;
        }
    }

    public void Interact(Clicker clicker)
    {
        onInteraction.Invoke(clicker);
    }

    public void StartHovering()
    {
        if (outlineOnHover)
            outline.enabled = true;
    }

    public void StopHovering()
    {
        if (outlineOnHover)
            outline.enabled = false;
    }

    public void SetHoverMessage(string msg)
    {
        if (IsServer)
            hoverMessage.Value = new FixedString128Bytes(msg);
    }

    public string GetHoverMessage()
    {
        return hoverMessage.Value.ToString();
    }

    public SetupInteractableMasterRes SetupInteractableMaster(InteractableMaster interactableMaster)
    {
        this.interactableMaster = interactableMaster;
        return new SetupInteractableMasterRes(new Component[] { this }, new List<Interactable> { this });
    }
}
