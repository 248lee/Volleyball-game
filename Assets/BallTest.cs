using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallTest : MonoBehaviour
{
    public float serveForce = 5f;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody2D>().isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            GetComponent<Rigidbody2D>().isKinematic = false;
            GetComponent<Rigidbody2D>().AddForce(Vector2.left * serveForce);
        }
    }
}
