using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentIndicatorMouse : SegmentController
{
    public Transform player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DrawLine(player.position, (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }
}
