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
    private GluedBool isGrounded = new GluedBool();
    private GluedBool isJumpSignalAvailable = new GluedBool();
    private GluedBool jumpSignal = new GluedBool();
    private float moveInput = 0f;
    private GluedBool isSprinting = new GluedBool();
    private GluedBool isSprintedBeforeJump = new GluedBool();
    private float lastClickTime = 0f;
    private GluedBool isSecondJumpAvailable = new GluedBool();
    private GluedBool secondJumpSignal = new GluedBool();
    private float speedConstant = 1f;
    private GluedBool isBrake = new GluedBool();


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
    private void OnEnable()
    {
        this.isGrounded = new GluedBool();
        this.isJumpSignalAvailable = new GluedBool();
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
                this.isSprinting.ChangeValue(true);
            }
            this.lastClickTime = currentTime;
        }
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            this.isSprinting.ChangeValue(false);
        }


        // jumping function
        // 1. Holding space to brake
        if (Input.GetKey(KeyCode.Space) && isJumpSignalAvailable.value())
        {
            this.isBrake.ChangeValue(true) ;
            this.TickDecrease(ref speedConstant, 2f);
            if (isSprinting.value())
                this.isSprintedBeforeJump.ChangeValue(true);
            this.isSprinting.ChangeValue(false);
        }

        // 2. Releasing space to jump
        if (Input.GetKeyUp(KeyCode.Space) && isJumpSignalAvailable.value())
        {
            this.jumpSignal.ChangeValue(true);
            this.isBrake.ChangeValue(false);
            this.TickInitial_1(ref speedConstant);
        }

        // 3. Press space to second jump.
        if (Input.GetKeyDown(KeyCode.Space) && isSecondJumpAvailable.value())
        {
            this.secondJumpSignal.ChangeValue(true);
        }

        // Perform jumping
        if (jumpSignal.value() && isGrounded.value()) // ordinary jump
        {
            // start performing jumping
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePos - (Vector2)transform.position).normalized;
            float actualJumpForce = isSprintedBeforeJump.value() ? sprintJumpForce : jumpForce;
            GetComponent<Rigidbody2D>().velocity = direction * actualJumpForce;
            this.jumpSignal.ChangeValue(false);
            _ = isJumpSignalAvailable.GluedChangeValue(false, 0.5f);
            if (isSprintedBeforeJump.value())
            {
                this.isSecondJumpAvailable.ChangeValue(true);
            }
            this.isSprintedBeforeJump.ChangeValue(false);
        }
        else if (secondJumpSignal.value()) // second jump
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePos - (Vector2)transform.position).normalized;
            GetComponent<Rigidbody2D>().velocity = direction * secondJumpForce;
            _ = isSecondJumpAvailable.GluedChangeValue(false, 0.5f);
            this.secondJumpSignal.ChangeValue(false);
        }

    }

    private void FixedUpdate()
    {
        if (this.IsAlmostFallOnGround())
        {
            this.isSecondJumpAvailable.ChangeValue(false);
        }
        this.isGrounded.ChangeValue(Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround));
        this.isJumpSignalAvailable.ChangeValue(IsAlmostFallOnGround() || isGrounded.value());

        if (isGrounded.value()) // horizontal input only available on the ground.
        {
            float currentSpeed = isSprinting.value() ? moveSpeed_sprint : moveSpeed;
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
