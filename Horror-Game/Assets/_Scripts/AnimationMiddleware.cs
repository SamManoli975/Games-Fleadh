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
}
