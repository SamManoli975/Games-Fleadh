using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ShrinkAbility : Ability
{
    [SerializeField] float shrinkTime = 3;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner) {
            UI_Ability ui = UI_Manager.instance.GetShrinkAbilityUI();
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
        Shrinkable shrinkable = GetComponent<Shrinkable>();
        shrinkable.SetIsShrinked(true);
        StartCoroutine(Unshrink(shrinkable));
    }

    IEnumerator Unshrink(Shrinkable shrinkable)
    {
        yield return new WaitForSeconds(shrinkTime);

        if(shrinkable != null) {
             shrinkable.SetIsShrinked(false);
        }
    }

    protected override void Activate() {
        ActivateServerRpc();
    }
}
