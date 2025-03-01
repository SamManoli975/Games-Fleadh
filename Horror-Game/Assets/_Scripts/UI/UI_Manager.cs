using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance;

    [SerializeField] UI_Inventory uI_Inventory;
    [SerializeField] UI_HoveredMessage uI_HoveredMessage;
    [SerializeField] UI_Hearts uI_Hearts;
    [SerializeField] UI_StaminaBar uI_StaminaBar;

    [SerializeField] UI_Ability highlightSurvivorAbility;
    [SerializeField] UI_Ability shrinkAbility;

    [SerializeField] GameObject survivorHelpMesage;
    [SerializeField] GameObject monsterHelpMesage;
   

    void Awake()
    {
        instance = this;

        uI_Inventory.gameObject.SetActive(false);
        uI_HoveredMessage.gameObject.SetActive(false);
        uI_Hearts.gameObject.SetActive(false);
        uI_StaminaBar.gameObject.SetActive(false);

        if(highlightSurvivorAbility != null)
            highlightSurvivorAbility.gameObject.SetActive(false);
        if(shrinkAbility != null)
            shrinkAbility.gameObject.SetActive(false);

        if(survivorHelpMesage != null)
            survivorHelpMesage.SetActive(false);
        if(monsterHelpMesage != null)
            monsterHelpMesage.SetActive(false);
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

    public UI_StaminaBar GetStaminaBarUI()
    {
        uI_StaminaBar.gameObject.SetActive(true);
        return uI_StaminaBar;
    }

    public UI_Ability GetHighlightSurvivorAbilityUI()
    {
        if(highlightSurvivorAbility == null)
            return null;

        highlightSurvivorAbility.gameObject.SetActive(true);
        return highlightSurvivorAbility;
    }

    public UI_Ability GetShrinkAbilityUI()
    {
        if(shrinkAbility == null)
            return null;

        shrinkAbility.gameObject.SetActive(true);
        return shrinkAbility;
    }

    public void ShowSurvivorHelpMessage() {
        if(survivorHelpMesage != null)
            survivorHelpMesage.SetActive(true);
    }

    public void ShowMonsterHelpMessage() {
        if(monsterHelpMesage != null)
            monsterHelpMesage.SetActive(true);
    }
}
