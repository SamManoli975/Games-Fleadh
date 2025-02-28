using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Clicker))]
public class Monster : NetworkBehaviour
{
    [SerializeField] UI_HoveredMessage uI_HoveredMessage;

    Clicker clicker;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        clicker = GetComponent<Clicker>();

        if (IsOwner)
        {
            if (UI_Manager.instance != null)
            {
                uI_HoveredMessage = UI_Manager.instance.GetHoveredMessageUI();
                UI_Manager.instance.ShowMonsterHelpMessage();

                clicker.onHoveredChange.AddListener(uI_HoveredMessage.HandleHoveredChange);
            }
        }
    }
}
