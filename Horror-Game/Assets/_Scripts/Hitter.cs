using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Hitter : NetworkBehaviour
{
    [SerializeField] private float hitRechargeTime = 0.2f;
    [SerializeField] private float hitRange = 3f;
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] Transform hitRayOrigin;

    bool readyToHit = true;
    bool hitting = false;

    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!IsOwner)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (!hitting && readyToHit)
                StartHit();

        }
    }

    void StartHit()
    {
        hitting = true;
        readyToHit = false;

        animator.SetTrigger("AxeAttack");
    }

    IEnumerator RechargeHit()
    {
        yield return new WaitForSeconds(hitRechargeTime);
        readyToHit = true;
    }

    public void OnHitAttackPoint()
    {
        if (Physics.Raycast(hitRayOrigin.position, hitRayOrigin.forward, out RaycastHit hit, hitRange, hitLayer))
        {
            IHitable hitable = hit.collider.GetComponent<IHitable>();
            if (hitable != null)
            {
                hitable.GetHit();
            }
        }
    }

    public void OnHitEnd()
    {
        hitting = false;
        StartCoroutine(RechargeHit());
    }
}
