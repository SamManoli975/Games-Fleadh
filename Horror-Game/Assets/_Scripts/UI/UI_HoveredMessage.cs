using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_HoveredMessage : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textfield;

    Interactable curInteractable;

    void Start()
    {
        textfield.gameObject.SetActive(false);
    }

    void SetHoverText(string text)
    {
        textfield.text = text + " (E)";
    }

    public void HandleHoveredChange(Interactable interactable)
    {
        if (curInteractable != null)
            curInteractable.onHoverMessageChanged.RemoveListener(HandleHoverMessageChanged);
        curInteractable = interactable;

        if (interactable == null)
        {
            textfield.gameObject.SetActive(false);
            return;
        }

        SetHoverText(interactable.GetHoverMessage());
        textfield.gameObject.SetActive(true);

        interactable.onHoverMessageChanged.AddListener(HandleHoverMessageChanged);
    }

    void HandleHoverMessageChanged(string msg)
    {
        SetHoverText(msg);
    }
}
