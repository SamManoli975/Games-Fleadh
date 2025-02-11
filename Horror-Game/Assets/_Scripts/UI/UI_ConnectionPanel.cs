using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UI_ConnectionPanel : MonoBehaviour
{
    [SerializeField] Button hostBtn;
    [SerializeField] Button clientBtn;

    void Start()
    {
        hostBtn.onClick.AddListener(() => NetworkManager.Singleton.StartHost());
        clientBtn.onClick.AddListener(() => NetworkManager.Singleton.StartClient());
    }
}
