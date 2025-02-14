using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ToggleRotations : ToggleOrientation
{
    public float openRotation = 90;
    public float closedRotation = 0;

    protected override void SetMovingPartTransform()
    {
        Vector3 curRotation = movingPart.transform.localRotation.eulerAngles;
        curRotation.y = Mathf.Lerp(openRotation, closedRotation, Ease(curClosedProgress));
        movingPart.transform.localRotation = Quaternion.Euler(curRotation);
    }
}
