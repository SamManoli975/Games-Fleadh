using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CollectableItem : NetworkBehaviour, IManagedInteractable
{
    [SerializeField] ItemType itemType;

    [Tooltip("Do not forget to add 'Outline' component if set to true")]
    [SerializeField] bool outlineOnHover = false;
    [SerializeField] Outline outline;
    [SerializeField] InteractableMaster interactableMaster;

    public string soundName = "KeySound";


    Interactable interactable;

    

    void Awake()
    {
        interactable = gameObject.GetComponent<Interactable>();
      
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            interactable.SetHoverMessage("Pick up " + ItemsDataManager.instance.GetItemData(itemType).name);
            interactable.onInteraction.AddListener(Collect);
        }
    }

    T AddComponentIfDoesNotHave<T>() where T : UnityEngine.Component
    {
        T c = gameObject.GetComponent<T>();
        if (c == null)
        {
            c = gameObject.AddComponent<T>();
        }
        return c;
    }

    public Component[] SetupComponents()
    {
        interactable = AddComponentIfDoesNotHave<Interactable>();

        interactable.outlineOnHover = outlineOnHover;
        interactable.outline = outline;
        interactable.interactableMaster = interactableMaster;

        return new Component[] { this, interactable };
    }

    void Collect(Clicker clicker)
    {
        if (!IsServer)
            Debug.Log("collect called on a client and not a server");
            return;

        Inventory inventory = clicker.GetComponent<Inventory>();
        if (inventory == null)
            return;

        if (inventory.CanAddItem(itemType))
        {
            int prioritySlot = -1;
            Hand hand = clicker.GetComponent<Hand>();
            if (hand != null)
                prioritySlot = hand.GetSelectedSlot();

            inventory.AddItem(itemType, prioritySlot);

            if (itemType == ItemType.keyCommon || itemType == ItemType.gateKey || itemType == ItemType.keyRare)
            {
                Debug.Log("item type key, going into function");
                NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
                PlaySoundFromObjectClientRpc(networkObject);
            }

            //wait
            StartCoroutine(DelayedDespawn());


        }
    }



    [ClientRpc]
    public void PlaySoundFromObjectClientRpc(NetworkObjectReference networkObjectReference)
    {
        Debug.Log("PlaySoundFromObjectClientRpc called on client.");

        if (networkObjectReference.TryGet(out NetworkObject networkObject))
        {
            GameObject player = networkObject.gameObject;
            Debug.Log($"NetworkObject found: {player.name}");

            // Dynamically find the sound under 'Sounds/'
            AudioSource audioSource = player.transform.Find($"Sounds/{soundName}")?.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                Debug.Log("audio playing..");
                audioSource.Play();
            }
            else
            {
                Debug.LogWarning($"AudioSource not found at Sounds/{soundName} for player");
            }

        }
        else
        {
            Debug.LogError("Failed to retrieve NetworkObject from reference.");
        }
    }

    IEnumerator DelayedDespawn()
    {
        Debug.Log("despawning..");
        NetworkObject.Despawn(true);
    }


    public SetupInteractableMasterRes SetupInteractableMaster(InteractableMaster interactableMaster)
    {
        this.interactableMaster = interactableMaster;

        Component[] modifiedComponents = SetupComponents();
        interactable = gameObject.GetComponent<Interactable>();
        return new SetupInteractableMasterRes(modifiedComponents, new List<Interactable> { interactable });
    }
}
