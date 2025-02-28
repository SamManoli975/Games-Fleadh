using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Hitter : NetworkBehaviour
{
    [SerializeField] private float hitRechargeTime = 0.2f;
    [SerializeField] private float hitRange = 3f;
    [SerializeField] private float sphereCastRadius = 0.2f;
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] Transform hitRayOrigin;

    bool readyToHit = true;
    bool hitting = false;

    [SerializeField] Animator animator;
    [SerializeField] GameObject localVisual;
    Animator localAnimator;

    [SerializeField] private AudioSource swingAudio;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if(!IsOwner) {
            localVisual.SetActive(false);
        }
    }

    void Start()
    {
        localAnimator = localVisual.GetComponent<Animator>();
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
        localAnimator.SetTrigger("AxeAttack");

        PlaySwingSound(); 
    }

    IEnumerator RechargeHit()
    {
        yield return new WaitForSeconds(hitRechargeTime);
        readyToHit = true;
    }

    void AfterSuccessfulHit() {
        animator.SetTrigger("HitTaunt");
        localAnimator.SetTrigger("HitTaunt");
    }

    [ServerRpc]
    void DoHitServerRpc()
    {
        if (Physics.SphereCast(hitRayOrigin.position, sphereCastRadius, hitRayOrigin.forward, out RaycastHit hit, hitRange, hitLayer))
        {
            IHitable hitable = hit.collider.GetComponent<IHitable>();
            if (hitable != null)
            {
                hitable.GetHit();
                AfterSuccessfulHit();
            }
        }
    }

    public void OnHitAttackPoint()
    {
        if (IsOwner)
            DoHitServerRpc();
    }

    public void OnHitEnd()
    {
        hitting = false;
        StartCoroutine(RechargeHit());
    }
    void PlaySwingSound() 
    {
        if (swingAudio != null) 
        {
            swingAudio.Play();
        }
    }
}
