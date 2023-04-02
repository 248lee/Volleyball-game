using UnityEngine;

public class BallController : MonoBehaviour
{
    public float jumpForce = 10f;
    public float closeDistance = 2f;
    public Rigidbody2D ballRigidbody;
    private bool canJump = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canJump)
        {
            ballRigidbody.velocity = new Vector2(0f, 0f);
            ballRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            canJump = false;
        }
    }

    private void FixedUpdate()
    {
        float distance = Vector2.Distance(transform.position, ballRigidbody.transform.position);
        if (distance <= closeDistance)
        {
            canJump = true;
        }
    }
}
