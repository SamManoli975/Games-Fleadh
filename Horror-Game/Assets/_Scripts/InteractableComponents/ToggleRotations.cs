using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleRotations : ToggleOrientation
{
    public float openRotation = 90;
    public float closedRotation = 0;

    public override void Start()
    {
        base.Start();
    }

    protected override void SetMovingPartTransform()
    {
        Vector3 curRotation = movingPart.transform.localRotation.eulerAngles;
        curRotation.y = Mathf.Lerp(openRotation, closedRotation, curClosedProgress);
        movingPart.transform.localRotation = Quaternion.Euler(curRotation);
    }
}
