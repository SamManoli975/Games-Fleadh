using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UI_LobbyCode : MonoBehaviour
{
    [SerializeField] TMP_InputField codeTextField;
    [SerializeField] TextMeshProUGUI joinCodeHeading;

    void Start()
    {
        string code = NetworkGameManager.instance.GetLobbyCode();
        if(code.Length == 0)
        {
            codeTextField.gameObject.SetActive(false);
            joinCodeHeading.gameObject.SetActive(false);
        }
        else {
            codeTextField.gameObject.SetActive(true);
            joinCodeHeading.gameObject.SetActive(true);
            codeTextField.text = code;
        }
    }
}
