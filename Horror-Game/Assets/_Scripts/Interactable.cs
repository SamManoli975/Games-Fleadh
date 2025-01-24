using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public UnityEvent<Clicker> onInteraction;

    [Tooltip("Message to display when the object is hovered")]
    [SerializeField] public string hoverMessage;

    [Tooltip("Do not forget to add 'Outline' component if set to true")]
    [SerializeField] bool outlineOnHover = true;

    Outline outline;

    protected virtual void Start()
    {
        if (outlineOnHover)
        {
            outline = GetComponent<Outline>();
            if (outline == null)
                Debug.LogError("Add 'Outline' component to an object if you have 'outlineOnHover' enabled");
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
}
