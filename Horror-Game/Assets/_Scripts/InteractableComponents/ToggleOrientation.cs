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

    Collider movingPartCollider = null;
    protected float curClosedProgress = 0;

    public virtual void Start()
    {
        if (doDisableCollidersOnMovement || doDisableOpenCollider)
        {
            movingPartCollider = movingPart.GetComponent<Collider>();
            if (movingPartCollider != null && movingPartCollider.isTrigger)
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
                if (curProgress != 1 && disableColliderFromProgress <= curProgress && curProgress <= 1 - disableColliderFromProgress)
                    movingPartCollider.isTrigger = true;
                else
                    movingPartCollider.isTrigger = false;
            }

            if (doDisableOpenCollider)
            {
                if (isClosed && curClosedProgress == 1)
                    movingPartCollider.isTrigger = false;
                else
                    movingPartCollider.isTrigger = true;
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

    protected virtual void SetMovingPartTransform()
    {
    }

    public bool GetIsClosed()
    {
        return isClosed;
    }
}
