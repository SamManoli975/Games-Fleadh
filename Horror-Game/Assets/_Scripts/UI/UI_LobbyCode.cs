using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UI_LobbyCode : MonoBehaviour
{
    [SerializeField] TMP_InputField codeTextField;

    void Start()
    {
        if(!NetworkManager.Singleton.IsServer)
        {
            gameObject.SetActive(false);
            return;
        }
        
        codeTextField.text = NetworkGameManager.instance.GetLobbyCode();
    }
}
