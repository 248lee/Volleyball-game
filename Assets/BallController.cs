using UnityEngine;

public class BallController : MonoBehaviour
{
    public GameObject ballObject;
    public float ballSpeed = 10f;
    private GameObject currentBall;
    private bool isAttached = true;

    private void Start()
    {
        this.currentBall = ballObject;
        this.currentBall.GetComponent<Rigidbody2D>().isKinematic = true;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isAttached)
            {
                this.currentBall.transform.parent = null;
                this.isAttached = false;
            }
            else
            {
                this.currentBall.GetComponent<Rigidbody2D>().isKinematic = false;
                Vector2 direction = new Vector2(1, -1).normalized;
                this.currentBall.GetComponent<Rigidbody2D>().AddForce(direction * ballSpeed);
            }
        }
    }
}
