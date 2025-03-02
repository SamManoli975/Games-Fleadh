using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HighlightSurvivorAbility : Ability
{
    [SerializeField] float hightlightTime = 3;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner) {
            UI_Ability ui = UI_Manager.instance.GetHighlightSurvivorAbilityUI();
            if(ui != null)
                SetUI(ui);
        }
    }

    public override void Update()
    {
        base.Update();

    }

    [ServerRpc]
    void ActivateServerRpc() {
        if(Survivor.instance != null) {
            Highlightable highlightable = Survivor.instance.GetComponent<Highlightable>();
            if(highlightable != null) {
                highlightable.SetIsHighlighted(true);
                NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
                playAbilitySoundClientRpc(new NetworkObjectReference(networkObject));

                StartCoroutine(Unhighlight(highlightable));
            }
        }
    }

    IEnumerator Unhighlight(Highlightable highlightable)
    {
        yield return new WaitForSeconds(hightlightTime);

        if(highlightable != null) {
            highlightable.SetIsHighlighted(false);
        }
    }

    protected override void Activate() {
        ActivateServerRpc();
    }
    
    [ClientRpc]
    void playAbilitySoundClientRpc(NetworkObjectReference networkObjectReference)
    {
        
        Debug.Log("PlaySoundFromObjectClientRpc called on client");

        if (!networkObjectReference.TryGet(out NetworkObject networkObject))
        {
            Debug.LogError("Failed to retrieve NetworkObject from reference");
            return;
        }

        GameObject player = networkObject.gameObject;
        Debug.Log($"NetworkObject found: {player.name}");

        // Find the AudioSource
        AudioSource audioSource = player.transform.Find("Sounds/ability")?.GetComponent<AudioSource>();

        if (audioSource != null)
        {
            Debug.Log("Playing unlock sound...");
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("AudioSource not found at Sounds/Unlock for player");
        }
    }
    
}
