using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleTransforms : ToggleOrientation
{
    public Transform openPoint;
    public Transform closedPoint;

    public override void Start()
    {
        base.Start();
    }

    protected override void SetMovingPartTransform()
    {
        movingPart.transform.position = Vector3.Lerp(openPoint.position, closedPoint.position, curClosedProgress);
        movingPart.transform.rotation = Quaternion.Lerp(openPoint.rotation, closedPoint.rotation, curClosedProgress);
    }
}
