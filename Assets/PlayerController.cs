using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float moveSpeed_sprint = 10f;
    public float jumpForce = 10f;
    public float secondJumpSpeed = .5f;
    public float sprintJumpForce = 20f;
    public float stuckDuration = .2f;
    public float stuckY_limit = 0f;
    public float saveReceiveDuration = .4f;
    public float saveReceiveSpeed = 40f;
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
    private Vector2 secondJumpVelocity;
    private GluedBool isPerformingSecondJump = new GluedBool();
    private GluedBool isStucking = new GluedBool();
    private GluedBool isSaveReceiving = new GluedBool();
    private GluedBool saveReceiveSignal = new GluedBool();


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
        return (GetComponent<Rigidbody2D>().velocity.y <= 0.05f && Physics2D.OverlapCircle(groundCheck.position, jumpSignalRadius, whatIsGround));
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
            this.isBrake.ChangeValue(false);
            this.TickInitial_1(ref speedConstant);
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePos - (Vector2)transform.position).normalized;
            if (direction.y / Mathf.Abs(direction.x) > 1f)
                this.jumpSignal.ChangeValue(true);
            else
                this.saveReceiveSignal.ChangeValue(true);
        }

        // 3. Press space to second jump.
        if (Input.GetKeyDown(KeyCode.Space) && isSecondJumpAvailable.value())
        {
            this.secondJumpSignal.ChangeValue(true);
        }

        // Perform jumping (dealing with signals)
        if (jumpSignal.value() && isGrounded.value()) // ordinary jump
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePos - (Vector2)transform.position).normalized;
            
            // start performing jumping
            float actualJumpForce = isSprintedBeforeJump.value() ? sprintJumpForce : jumpForce;
            GetComponent<Rigidbody2D>().velocity = direction * actualJumpForce;

            if (isSprintedBeforeJump.value())
            {
                this.isSecondJumpAvailable.ChangeValue(true);
            }
            this.isSprintedBeforeJump.ChangeValue(false);
            this.jumpSignal.ChangeValue(false);
            _ = isJumpSignalAvailable.GluedChangeValue(false, 0.5f);
        }
        else if (secondJumpSignal.value() || isPerformingSecondJump.value() || isStucking.value()) // second jump
        {
            if (secondJumpSignal.value()) // second jump initialize
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                float secondJumpDuration = Vector2.SqrMagnitude((mousePos - (Vector2)transform.position)) * this.secondJumpSpeed / 30f;
                this.secondJumpVelocity = (mousePos - (Vector2)transform.position) / secondJumpDuration;
                // Start timer
                Timers.SetTimer("secondJumpTiktok", secondJumpDuration);
                this.secondJumpSignal.ChangeValue(false);
                this.isPerformingSecondJump.ChangeValue(true);
                _ = isSecondJumpAvailable.GluedChangeValue(false, 0.5f);
            }
            if (isPerformingSecondJump.value()) // second jumping
            {
                GetComponent<Rigidbody2D>().velocity = this.secondJumpVelocity;
                CameraController.SetFollowingPlayer(false);

                if (Timers.isTimerFinished("secondJumpTiktok"))
                {
                    if (transform.position.y > stuckY_limit)
                    {
                        this.isStucking.ChangeValue(true);
                        Timers.SetTimer("StuckTiktok", stuckDuration);
                    }
                    this.isPerformingSecondJump.ChangeValue(false);
                }
            }
            if (isStucking.value()) // stucking
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                if (Timers.isTimerFinished("StuckTiktok"))
                {
                    this.isStucking.ChangeValue(false);
                    CameraController.SetFollowingPlayer(true);
                }

            }
        }
        else if (this.saveReceiveSignal.value() || this.isSaveReceiving.value())
        {
            if (this.saveReceiveSignal.value())
            {
                Timers.SetTimer("SaveReceiving", saveReceiveDuration);
                this.isSaveReceiving.ChangeValue(true);
                this.saveReceiveSignal.ChangeValue(false);
            }
            if (this.isSaveReceiving.value())
            {
                float actualSaveSpeed = saveReceiveSpeed * (1f - Timers.GetTimerPrgress("SaveReceiving"));
                GetComponent<Rigidbody2D>().velocity = actualSaveSpeed * transform.right;
                if (Timers.isTimerFinished("SaveReceiving"))
                {
                    this.isSaveReceiving.ChangeValue(false);
                }
            }
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

        // perform horizontal moving
        if (isGrounded.value() && !isSaveReceiving.value()) // horizontal input only available on the ground.
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
