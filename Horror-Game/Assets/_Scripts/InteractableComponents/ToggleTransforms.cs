using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleTransforms : ToggleOrientation
{
    public Transform openPoint;
    public Transform closedPoint;

    protected override void SetMovingPartTransform()
    {
        movingPart.transform.position = Vector3.Lerp(openPoint.position, closedPoint.position, Ease(curClosedProgress));
        movingPart.transform.rotation = Quaternion.Lerp(openPoint.rotation, closedPoint.rotation, Ease(curClosedProgress));
    }
}
