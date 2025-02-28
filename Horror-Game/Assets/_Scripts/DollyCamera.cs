using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollyCamera : MonoBehaviour
{
    [SerializeField] float moveTime = 5;

    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;

    bool moving = false;
    float curTime = 0;

    void Update()
    {
        if(!moving)
            return;

        if(curTime != moveTime) {
            curTime += Time.deltaTime;
            if(curTime > moveTime) {
                curTime = moveTime;
                moving = false;
            }
        }

        float progress = curTime / moveTime;
        transform.position = Vector3.Lerp(startPoint.transform.position, endPoint.transform.position, progress);
    }

    [ContextMenu("Start Moving")]
    public void StartMoving() {
        moving = true;
        curTime = 0;
    }
}
