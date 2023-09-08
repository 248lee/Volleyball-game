using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public float serveForce = 5f;
    private float originalGravityScale;
    private Vector3 originalPos;
    private Vector2 originalVelocity;
    // Start is called before the first frame update
    void Start()
    {
        this.originalGravityScale = GetComponent<Rigidbody2D>().gravityScale;
        GetComponent<Rigidbody2D>().isKinematic = true;
        this.originalPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            GetComponent<Rigidbody2D>().isKinematic = false;
            GetComponent<Rigidbody2D>().AddForce(Vector2.left * serveForce);
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            GetComponent<Rigidbody2D>().isKinematic = true;
            transform.position = originalPos;
        }
    }
    public void PauseBall()
    {
        this.originalVelocity = GetComponent<Rigidbody2D>().velocity;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Rigidbody2D>().gravityScale = 0f;
    }
    public void ResumeBall()
    {
        GetComponent<Rigidbody2D>().velocity = originalVelocity;
        GetComponent<Rigidbody2D>().gravityScale = originalGravityScale;
    }
}
