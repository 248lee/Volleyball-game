using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float moveSpeed_sprint = 10f;
    public float jumpForce = 10f;
    public float secondJumpForce = 10f;
    public float sprintJumpForce = 20f;
    public Transform groundCheck;
    public float groundRadius = 0.1f;
    public float jumpSignalRadius = 1f;
    public LayerMask whatIsGround;
    public float doubleClickTime = .3f;
    private bool isGrounded = false;
    private bool isJumpSignalAvailable = false;
    private bool jumpSignal = false;
    private float moveInput = 0f;
    private bool isSprinting = false;
    private bool isSprintedBeforeJump = false;
    private float lastClickTime = 0f;
    private bool isSecondJumpAvailable = false;
    private bool secondJumpSignal = false;
    private float speedConstant = 1f;
    private bool isBrake = false;


    private void TickInitial_1(ref float oneToZero)
    {
        oneToZero = 1f;
    }
    private void TickDecrease(ref float oneToZero, float tickrate) // Decrease a float number from 1 to 0 in the rate of Time.deltaTime*tickrate
    {
        if (oneToZero >= 0f)
            oneToZero -= Time.deltaTime * tickrate;
    }
    private bool IsAlmostFallOnGround()
    {
        return (GetComponent<Rigidbody2D>().velocity.y < 0 && Physics2D.OverlapCircle(groundCheck.position, jumpSignalRadius, whatIsGround));
    }

    private void Update()
    {
        // horizontal moving function
        this.moveInput = Input.GetAxisRaw("Horizontal");
        // check for sprint input
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            float currentTime = Time.time;
            if (currentTime - lastClickTime <= doubleClickTime)
            {
                this.isSprinting = true;
            }
            this.lastClickTime = currentTime;
        }
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            this.isSprinting = false;
        }


        // jumping function
        if (Input.GetKey(KeyCode.Space) && isJumpSignalAvailable)
        {
            this.isBrake = true;
            this.TickDecrease(ref speedConstant, 2f);
            if (isSprinting)
                this.isSprintedBeforeJump = true;
            this.isSprinting = false;
        }
        if (Input.GetKeyUp(KeyCode.Space) && isJumpSignalAvailable)
        {
            this.jumpSignal = true;
            this.isBrake = false;
            this.TickInitial_1(ref speedConstant);
        }
        if (Input.GetKeyDown(KeyCode.Space) && isSecondJumpAvailable)
        {
            this.secondJumpSignal = true;
        }
        if (jumpSignal && isGrounded) // ordinary jump
        {
            // start performing jumping
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePos - (Vector2)transform.position).normalized;
            float actualJumpForce = isSprintedBeforeJump ? sprintJumpForce : jumpForce;
            GetComponent<Rigidbody2D>().velocity = direction * actualJumpForce;
            this.isGrounded = false;
            this.jumpSignal = false;
            this.isJumpSignalAvailable = false;
            if (isSprintedBeforeJump)
            {
                this.isSecondJumpAvailable = true;
            }
            this.isSprintedBeforeJump = false;
        }
        else if (secondJumpSignal)
        {
            // start performing second jumping
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePos - (Vector2)transform.position).normalized;
            GetComponent<Rigidbody2D>().velocity = direction * secondJumpForce;
            this.isSecondJumpAvailable = false;
            this.secondJumpSignal = false;
        }

    }

    private void FixedUpdate()
    {
        if (this.IsAlmostFallOnGround())
        {
            this.isSecondJumpAvailable = false;
        }
        this.isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
        this.isJumpSignalAvailable = IsAlmostFallOnGround() || isGrounded;

        if (isGrounded) // horizontal input only available on the ground.
        {
            float currentSpeed = isSprinting ? moveSpeed_sprint : moveSpeed;
            currentSpeed *= this.speedConstant;
            this.transform.position += new Vector3(moveInput * currentSpeed * speedConstant * Time.deltaTime, 0, 0);
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, jumpSignalRadius);
    }
}
