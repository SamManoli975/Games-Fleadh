using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance;

    [SerializeField] UI_Inventory uI_Inventory;
    [SerializeField] UI_HoveredMessage uI_HoveredMessage;
    [SerializeField] UI_Hearts uI_Hearts;

    void Awake()
    {
        instance = this;

        uI_Inventory.gameObject.SetActive(false);
        uI_HoveredMessage.gameObject.SetActive(false);
        uI_Hearts.gameObject.SetActive(false);
    }

    public UI_Inventory GetInventoryUI()
    {
        uI_Inventory.gameObject.SetActive(true);
        return uI_Inventory;
    }

    public UI_HoveredMessage GetHoveredMessageUI()
    {
        uI_HoveredMessage.gameObject.SetActive(true);
        return uI_HoveredMessage;
    }

    public UI_Hearts GetHeartsUI()
    {
        uI_Hearts.gameObject.SetActive(true);
        return uI_Hearts;
    }
}
