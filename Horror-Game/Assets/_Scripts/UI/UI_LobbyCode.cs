using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_LobbyCode : MonoBehaviour
{
    [SerializeField] TMP_InputField codeTextField;

    void Start()
    {
        codeTextField.text = NetworkGameManager.instance.GetLobbyCode();
    }
}
