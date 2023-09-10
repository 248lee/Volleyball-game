using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SegmentController : MonoBehaviour
{
    LineRenderer lineRenderer;
    public Vector2 fromOffset = Vector2.zero;
    protected virtual void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer component not found on GameObject!");
        }
    }
    private void Start()
    {
        
    }
    protected void DrawLine(Vector3 position_from, Vector3 position_to)
    {
        Vector3[] vertices = {position_from, position_to};
        lineRenderer.SetPositions(vertices);
    }
    public void SetFromOffset(Vector2 set)
    {
        this.fromOffset = set;
    }
}
