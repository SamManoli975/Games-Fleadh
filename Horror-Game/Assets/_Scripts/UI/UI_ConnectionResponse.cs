using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UI_ConnectionResponse : MonoBehaviour
{
    [SerializeField] GameObject content;
    [SerializeField] TextMeshProUGUI errorMessageText;

    void Start()
    {
        Hide();

        NetworkGameManager.instance.OnFailedToJoinGame += Show;
    }

    public void Show(string errorMessage = "") 
    {
        content.SetActive(true);
        if(errorMessage.Length > 0) {
            errorMessageText.gameObject.SetActive(true);
            errorMessageText.text = errorMessage;
        }
        else {
            errorMessageText.gameObject.SetActive(false);
        }
    }

    public void Hide() 
    {
        content.SetActive(false);
    }

    private void OnDestroy()
    {
        NetworkGameManager.instance.OnFailedToJoinGame -= Show;
    }
}
