using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleOrientation : MonoBehaviour
{
    public bool isClosed = true;

    public float animTime = 0.3f;

    public GameObject movingPart;

    public bool doDisableOpenCollider = true;
    public bool doDisableCollidersOnMovement = false;
    public float disableColliderFromProgress = 0;

    Collider[] movingPartColliders = new Collider[0];
    protected float curClosedProgress = 0;

    public virtual void Start()
    {
        if (doDisableCollidersOnMovement || doDisableOpenCollider)
        {
            movingPartColliders = movingPart.GetComponents<Collider>();
            if (movingPartColliders.Length == 0)
            {
                doDisableCollidersOnMovement = false;
                doDisableOpenCollider = false;
            }
        }

        if (isClosed)
            Close();
        else
            Open();

        curClosedProgress = isClosed ? 1 : 0;
        SetMovingPartTransform();
    }

    void Update()
    {
        if ((isClosed && curClosedProgress < 1) || (!isClosed && curClosedProgress > 0))
        {
            float curProgress;

            if (isClosed)
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

            if (doDisableCollidersOnMovement)
            {
                bool isTrigger = curProgress != 1 && disableColliderFromProgress <= curProgress && curProgress <= 1 - disableColliderFromProgress;
                for (int i = 0; i < movingPartColliders.Length; i++)
                {
                    movingPartColliders[i].isTrigger = isTrigger;
                }
            }

            if (doDisableOpenCollider)
            {
                bool isTrigger = !isClosed || curClosedProgress != 1;
                for (int i = 0; i < movingPartColliders.Length; i++)
                {
                    movingPartColliders[i].isTrigger = isTrigger;
                }
            }

            SetMovingPartTransform();
        }
    }

    public void Open()
    {
        isClosed = false;
    }

    public void Close()
    {
        isClosed = true;
    }

    public void Toggle(Clicker clicker)
    {
        if (isClosed)
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
        return isClosed;
    }
}
