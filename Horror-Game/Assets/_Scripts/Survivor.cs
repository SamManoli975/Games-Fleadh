using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

[RequireComponent(typeof(Inventory))]
[RequireComponent(typeof(Clicker))]
[RequireComponent(typeof(Hand))]
public class Survivor : NetworkBehaviour
{
    public static Survivor instance;
    

    UI_Inventory uI_Inventory;
    UI_HoveredMessage uI_HoveredMessage;
    UI_Hearts uI_Hearts;
    UI_StaminaBar uI_StaminaBar;

    Clicker clicker;
    Inventory inventory;
    Hand hand;
    Health health;
    Movement movement;
    private Monster monster; // Store the reference to the Monster
    [SerializeField] private Volume postProcessingVolume; // Reference to Post-Processing Volume
    private ColorAdjustments colorAdjustments; // Holds color adjustments settings

    [SerializeField] private float maxDesaturationDistance = 10f; // Maximum distance where desaturation is applied
    [SerializeField] private float minSaturation = -1f; // Minimum saturation value when killer is very close

    void Awake()
    {
        instance = this;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        clicker = GetComponent<Clicker>();
        inventory = GetComponent<Inventory>();
        hand = GetComponent<Hand>();
        health = GetComponent<Health>();
        movement = GetComponent<Movement>();
        // Find Monster in the scene (fixes the error)
        monster = FindObjectOfType<Monster>();

        if (IsOwner)
        {
            if (UI_Manager.instance != null)
            {
                uI_Inventory = UI_Manager.instance.GetInventoryUI();
                uI_HoveredMessage = UI_Manager.instance.GetHoveredMessageUI();
                uI_Hearts = UI_Manager.instance.GetHeartsUI();
                uI_StaminaBar = UI_Manager.instance.GetStaminaBarUI();

                movement.staminaBar = uI_StaminaBar.GetStaminaBarFillImage();
                uI_Inventory.inventory = inventory;

                clicker.onHoveredChange.AddListener(uI_HoveredMessage.HandleHoveredChange);
                inventory.onItemsChanged.AddListener(uI_Inventory.UpdateSlots);
                hand.onSelectedSlotChanged.AddListener(uI_Inventory.UpdateSelectedSlot);
                health.onCurHealthUpdate.AddListener(uI_Hearts.HandleHealthUpdated);
            }
        }

        if (IsServer)
        {
            health.onDied.AddListener(HandleDeath);
        }

        // Try to get the Color Adjustments component from the Post-Processing Volume
        if (postProcessingVolume != null && postProcessingVolume.profile.TryGet(out colorAdjustments))
        {
            colorAdjustments.saturation.Override(0); // Set default saturation
        }
    }
    void Update()
    {
        if (IsOwner)
        {
            UpdateDesaturation(); // Continuously update desaturation based on killer proximity
        }
    }

    void UpdateDesaturation()
    {
        if (monster == null || colorAdjustments == null)
            return; // Exit if references are missing

        float distanceToKiller = Vector3.Distance(transform.position, monster.transform.position); // Calculate distance to killer

        float saturationValue = Mathf.Lerp(minSaturation, 0, distanceToKiller / maxDesaturationDistance); // Linearly interpolate saturation

        colorAdjustments.saturation.Override(saturationValue); // Apply new saturation value
    }
    void HandleDeath()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.EndGame(PlayerRole.monster);
        }
    }
}
