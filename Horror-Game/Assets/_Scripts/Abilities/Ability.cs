using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Ability : NetworkBehaviour
{
    [SerializeField] float rechargeTime = 5;
    [SerializeField] KeyCode activateKey = KeyCode.R;

    float timeToRecharge = 0;

    UI_Ability ui;

    protected void SetUI(UI_Ability ui) {
        this.ui = ui;
        ui.SetActivateKey(activateKey);

        SetFilledPortion();
    }

    void SetFilledPortion() {
        float filledPortion = (rechargeTime - timeToRecharge) / rechargeTime;
        ui.SetFilledPortion(filledPortion);
    }

    public virtual void Update()
    {
        if(!IsOwner)
            return;

        if(timeToRecharge != 0) {
            timeToRecharge -= Time.deltaTime;
            if(timeToRecharge < 0) {
                timeToRecharge = 0;
            }

            if(ui != null)
                SetFilledPortion();
        }

        if(timeToRecharge == 0) {
            if(Input.GetKeyDown(activateKey)) {
                timeToRecharge = rechargeTime;

                Activate();
            }
        }   
    }

    protected virtual void Activate() {
        Debug.Log("Activated");
    }
}
