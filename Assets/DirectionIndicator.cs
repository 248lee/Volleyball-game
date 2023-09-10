using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionIndicator : SegmentController
{
    public Transform from;
    public Transform pointTo;
    public float length;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 fromPos = this.from.position + (Vector3)fromOffset;
        Vector3 pointToPos = this.pointTo.position;
        Vector3 direction = pointToPos - fromPos;
        Vector3 endPoint = fromPos + (direction.normalized * length);
        DrawLine(fromPos, endPoint);
    }
    
}
