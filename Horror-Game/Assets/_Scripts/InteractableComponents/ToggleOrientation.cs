using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ToggleOrientation : NetworkBehaviour
{
    public bool initialIsClosed = true;
    NetworkVariable<bool> isClosed = new NetworkVariable<bool>(true);
    private bool localPredictionIsClosed;

    public float animTime = 0.3f;

    public GameObject movingPart;

    public bool doDisableOpenCollider = true;
    public bool doDisableCollidersOnMovement = false;
    public float disableColliderFromProgress = 0;

    protected Collider[] movingPartColliders = new Collider[0];
    protected float curClosedProgress = 0;

    NetworkVariable<bool> isCollidersTrigger = new NetworkVariable<bool>(true);

    void Awake()
    {
        isClosed = new NetworkVariable<bool>(initialIsClosed);
        localPredictionIsClosed = initialIsClosed;

        if (doDisableCollidersOnMovement || doDisableOpenCollider)
        {
            movingPartColliders = movingPart.GetComponents<Collider>();
            if (movingPartColliders.Length == 0)
            {
                doDisableCollidersOnMovement = false;
                doDisableOpenCollider = false;
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (isClosed.Value)
            Close();
        else
            Open();

        curClosedProgress = isClosed.Value ? 1 : 0;
        SetMovingPartTransform();

        isCollidersTrigger.OnValueChanged += (bool previous, bool current) => SetCollidersTrigger(current);
        isClosed.OnValueChanged += (bool previous, bool current) => { localPredictionIsClosed = current; };
    }


    void SetCollidersTrigger(bool isTrigger)
    {
        for (int i = 0; i < movingPartColliders.Length; i++)
        {
            movingPartColliders[i].isTrigger = isTrigger;
        }
    }

    void Update()
    {
        bool curIsClosed = GetIsClosed();

        if ((curIsClosed && curClosedProgress < 1) || (!curIsClosed && curClosedProgress > 0))
        {
            float curProgress;

            if (curIsClosed)
            {
                curClosedProgress += Time.deltaTime / animTime;
                if (curClosedProgress > 1)
                    curClosedProgress = 1;

                curProgress = curClosedProgress;
            }
            else
            {
                curClosedProgress -= Time.deltaTime / animTime;
                if (curClosedProgress < 0)
                    curClosedProgress = 0;

                curProgress = 1 - curClosedProgress;
            }

            if (IsServer)
            {
                if (doDisableCollidersOnMovement)
                {
                    bool isTrigger = curProgress != 1 && disableColliderFromProgress <= curProgress && curProgress <= 1 - disableColliderFromProgress;
                    isCollidersTrigger.Value = isTrigger;
                }

                if (doDisableOpenCollider)
                {
                    bool isTrigger = !isClosed.Value || curClosedProgress != 1;
                    isCollidersTrigger.Value = isTrigger;
                }
            }

            SetMovingPartTransform();
        }
    }

    public void Open()
    {
        localPredictionIsClosed = false;

        if (!IsServer)
            return;

        isClosed.Value = false;
    }

    public void Close()
    {
        localPredictionIsClosed = true;

        if (!IsServer)
            return;

        isClosed.Value = true;
    }

    public void Toggle(Clicker clicker)
    {
        if (GetIsClosed())
            Open();
        else
            Close();
    }

    protected float Ease(float t)
    {
        t = Mathf.Clamp01(t);

        // Smoothstep easing: t^2 * (3 - 2 * t)
        return t * t * (3f - 2f * t);
    }

    protected virtual void SetMovingPartTransform()
    {
    }

    public bool GetIsClosed()
    {
        bool curIsClosed = IsServer ? isClosed.Value : localPredictionIsClosed;
        return curIsClosed;
    }
}
