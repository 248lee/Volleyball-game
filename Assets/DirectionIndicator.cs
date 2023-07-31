using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionIndicator : MonoBehaviour
{
    public Transform player;
    private LineRenderer lr;
    // Start is called before the first frame update
    void Start()
    {
        this.lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3[] vertices = { player.position, (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) };
        lr.SetPositions(vertices);
    }
}
