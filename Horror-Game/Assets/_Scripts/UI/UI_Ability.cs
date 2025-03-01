using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Ability : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI activateKeyText;
    [SerializeField] Image filledImage;

    public void SetActivateKey(KeyCode key) {
        activateKeyText.text = key.ToString();
    }

    public void SetFilledPortion(float portion) {
        filledImage.fillAmount = portion * 0.95f; // * 0.95f so that when there is a little left on UI it can already be used
    }
}
