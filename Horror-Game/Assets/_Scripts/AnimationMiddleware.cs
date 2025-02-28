using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationMiddleware : MonoBehaviour
{
    [SerializeField] Hitter hitter;
    [SerializeField] Movement movement;

    public void OnHitAttackPoint()
    {
        if(hitter != null)
            hitter.OnHitAttackPoint();
    }

    public void OnHitEnd()
    {
        if(hitter != null)
            hitter.OnHitEnd();
    }

    public void OnFootstep()
    {
        if(movement != null)
            movement.OnFootstep();
    }

    public void OnEndTaunt() {
        if(movement != null)
            movement.OnEndTaunt();
        if(hitter != null)
            hitter.OnEndTaunt();
    }

    public void OnStartTaunt() {
        if(movement != null)
            movement.OnStartTaunt();
    }
}
