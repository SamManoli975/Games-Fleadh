using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_LobbyCode : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI codeTextField;

    void Start()
    {
        codeTextField.text = NetworkPlayersManager.instance.lobbyCode;
    }
}
