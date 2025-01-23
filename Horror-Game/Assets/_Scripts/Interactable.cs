using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    // For detecting default interaction
    public UnityEvent onClick;
    // For detecting specific types of interactions
    public UnityEvent<InteractionType> onInteraction;

    [Tooltip("Message to display when the object is hovered")]
    [SerializeField] public string hoverMessage;

    [Tooltip("Do not forget to add 'Outline' component if set to true")]
    [SerializeField] bool outlineOnHover = false;

    Outline outline;

    void Start()
    {
        if (outlineOnHover)
        {
            outline = GetComponent<Outline>();
            outline.enabled = false;
        }
    }

    public void Interact(InteractionType interactionType)
    {
        // Here default interaction is defined as leftMouse button
        if (interactionType == InteractionType.leftMouse)
            onClick.Invoke();

        onInteraction.Invoke(interactionType);
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
}
