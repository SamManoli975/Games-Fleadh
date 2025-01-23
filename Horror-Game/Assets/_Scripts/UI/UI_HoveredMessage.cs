using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_HoveredMessage : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textfield;

    void Start()
    {
        textfield.gameObject.SetActive(false);
    }

    public void HandleHoveredChange(Interactable interactable)
    {
        if (interactable == null)
        {
            textfield.gameObject.SetActive(false);
            return;
        }

        textfield.text = interactable.hoverMessage;
        textfield.gameObject.SetActive(true);
    }

}
