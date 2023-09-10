using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentIndicatorMouse : SegmentController
{
    public Transform player;
    private Vector3 startPoint;
    private GluedBool isUsingStartPoint = new GluedBool();
    // Start is called before the first frame update
    void Start()
    {
        this.isUsingStartPoint.ChangeValue(false);
    }

    // Update is called once per frame
    void Update()
    {
        DrawLine(player.position + (Vector3)fromOffset, (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }
    
}
