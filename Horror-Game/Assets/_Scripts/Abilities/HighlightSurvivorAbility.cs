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
}
