using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float moveSpeed_sprint = 10f;
    public float jumpForce = 10f;
    public float secondJumpSpeed = .5f;
    public float sprintJumpForce = 20f;
    public float stuckDuration = .2f;
    public float killStuckDuration = .2f;
    public float stuckY_limit = 0f;
    public float saveReceiveDuration = .4f;
    public float saveReceiveSpeed = 40f;
    public Transform groundCheck;
    public float groundRadius = 0.1f;
    public float jumpSignalRadius = 1f;
    public LayerMask whatIsGround;
    public float doubleClickTime = .3f;
    private GluedBool killStuckSignal = new GluedBool();
    private GluedBool isGrounded = new GluedBool();
    private GluedBool isJumpAndSaveSignalAvailable = new GluedBool();
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
    private GluedBool isKillStucking = new GluedBool();
    private GluedBool isSaveReceiving = new GluedBool();
    private GluedBool saveReceiveSignal = new GluedBool();
    private GluedBool enableAutoTurn = new GluedBool();
    private float originalGravityScale;

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
        this.enableAutoTurn.ChangeValue(true);
        CameraController.SetFollowingPlayer(false);
    }
    private void Start()
    {
        this.originalGravityScale = GetComponent<Rigidbody2D>().gravityScale;
        this.isKillStucking.ChangeValue(false);
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


        // Below are the actions the player can make.
        // 1. Holding space to brake
        if (Input.GetKey(KeyCode.Space) && isJumpAndSaveSignalAvailable.value())
        {
            this.isBrake.ChangeValue(true) ;
            this.TickDecrease(ref speedConstant, 2f);
            if (isSprinting.value())
                this.isSprintedBeforeJump.ChangeValue(true);
            this.isSprinting.ChangeValue(false);
        }

        // 2. Releasing space to jump
        if (Input.GetKeyUp(KeyCode.Space) && isJumpAndSaveSignalAvailable.value())
        {
            this.isBrake.ChangeValue(false);
            this.TickInitial_1(ref speedConstant);
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePos - (Vector2)transform.position).normalized;
            if (direction.y / Mathf.Abs(direction.x) > 0.5f)
                this.jumpSignal.ChangeValue(true);
            else
                this.saveReceiveSignal.ChangeValue(true);
        }

        // 3. Press space to second jump.
        if (Input.GetKeyDown(KeyCode.Space) && isSecondJumpAvailable.value())
        {
            this.secondJumpSignal.ChangeValue(true);
        }

        // 4. When can kill, press mouse(0) to stuck and aim
        if (GetComponent<BallActionController>().GetCanKill() && Input.GetMouseButton(0))
        {
            this.killStuckSignal.ChangeValue(true);
        }

        // Dealing with Signals  ¶¶§Ç: killStuck -> firstjump -> saveReceive -> secondjump
        if (killStuckSignal.value() || isKillStucking.value()) // Kill Stuck
        {
            this.killStuckSignal.ChangeValue(false);
            if (!isKillStucking.value())
            {
                this.isKillStucking.ChangeValue(true);
                Timers.SetTimer("KillStucking", killStuckDuration, true);
                GetComponent<BallActionController>().StuckTheBall(); // Pause the ball.
                GetComponent<Animator>().SetInteger("StatesEncoded", 1); // play ha~~~~~ animation
            }
            if (isKillStucking.value())
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                GetComponent<Rigidbody2D>().gravityScale = 0f;       // this and the above line stuck the player in the air
                if (Timers.isTimerFinished("KillStucking")) // if the player holds the mouse button until the stuck(charging) time ends
                {
                    this.isKillStucking.ChangeValue(false);
                    GetComponent<Animator>().SetTrigger("kill");
                    GetComponent<Rigidbody2D>().gravityScale = originalGravityScale;
                    GetComponent<BallActionController>().StartKill(); // Resume and Kill the ball
                }
                else if (Input.GetMouseButtonUp(0)) // else if the player gives up charging
                {
                    this.isKillStucking.ChangeValue(false);
                    GetComponent<Rigidbody2D>().gravityScale = originalGravityScale;
                    GetComponent<Animator>().SetInteger("StatesEncoded", 0); // play idle animation (disable ha~~~~~ animation)
                    GetComponent<BallActionController>().ResumeTheBall(); // Resume the ball
                }
            }
        }
        else if (jumpSignal.value() && isGrounded.value()) // Ordinary Jump
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePos - (Vector2)transform.position).normalized;

            // interupt save receiving
            this.isSaveReceiving.ChangeValue(false);
            this.SetColliderStandup();

            // start performing jumping
            float actualJumpForce = isSprintedBeforeJump.value() ? sprintJumpForce : jumpForce;
            GetComponent<Rigidbody2D>().velocity = direction * actualJumpForce;

            if (isSprintedBeforeJump.value())
            {
                this.isSecondJumpAvailable.ChangeValue(true);
            }
            this.isSprintedBeforeJump.ChangeValue(false);
            this.jumpSignal.ChangeValue(false);
            _ = isJumpAndSaveSignalAvailable.GluedChangeValue(false, 0.5f);
        }
        else if (this.saveReceiveSignal.value() || this.isSaveReceiving.value()) // Save Receive
        {
            if (this.saveReceiveSignal.value()) // saveReceive initializing
            {
                // let player lie down
                GetComponent<Animator>().SetInteger("StatesEncoded", 2);
                this.SetColliderLiedown();

                // let player face direction
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                bool direction = (mousePos - (Vector2)transform.position).x > 0;
                if (direction)
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                else
                    transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));

                Timers.SetTimer("SaveReceiving", saveReceiveDuration);
                this.isSaveReceiving.ChangeValue(true);
                this.saveReceiveSignal.ChangeValue(false);
                this.isSprintedBeforeJump.ChangeValue(false);

                // interupt second jump
                this.isSecondJumpAvailable.ChangeValue(false);
                this.secondJumpSignal.ChangeValue(false);
                this.isPerformingSecondJump.ChangeValue(false);
                this.isStucking.ChangeValue(false);
                //CameraController.SetFollowingPlayer(true);
            }
            if (this.isSaveReceiving.value())
            {
                // start performing saving
                float actualSaveSpeed = saveReceiveSpeed * (1f - Timers.GetTimerPrgress("SaveReceiving"));
                GetComponent<Rigidbody2D>().velocity = actualSaveSpeed * transform.right;
                if (Timers.isTimerFinished("SaveReceiving"))
                {
                    this.isSaveReceiving.ChangeValue(false);
                    GetComponent<Animator>().SetInteger("StatesEncoded", 0);
                    this.SetColliderStandup();
                }
            }
        }
        else if (secondJumpSignal.value() || isPerformingSecondJump.value() || isStucking.value()) // Second Jump
        {
            if (secondJumpSignal.value()) // second jump initialize
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                float secondJumpDuration = Vector2.Distance(mousePos, (Vector2)transform.position) / this.secondJumpSpeed;
                this.secondJumpVelocity = (mousePos - (Vector2)transform.position) / secondJumpDuration;
                // Start timer
                Timers.SetTimer("secondJumpTiktok", secondJumpDuration);
                this.secondJumpSignal.ChangeValue(false);
                this.isPerformingSecondJump.ChangeValue(true);
                _ = isSecondJumpAvailable.GluedChangeValue(false, 0.5f);
            }
            if (isPerformingSecondJump.value())
            {
                GetComponent<Rigidbody2D>().velocity = this.secondJumpVelocity; // Performing second jump
                //CameraController.SetFollowingPlayer(false);

                if (Timers.isTimerFinished("secondJumpTiktok"))
                {
                    GetComponent<Rigidbody2D>().velocity = new Vector2(0, -3f); // Eliminate the inertia
                    if (transform.position.y > stuckY_limit) // The player should be high enough to stuck.
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
                GetComponent<Rigidbody2D>().gravityScale = 0f;
                if (Timers.isTimerFinished("StuckTiktok"))
                {
                    this.isStucking.ChangeValue(false);
                    GetComponent<Rigidbody2D>().gravityScale = originalGravityScale;
                    //CameraController.SetFollowingPlayer(true);
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
        this.isJumpAndSaveSignalAvailable.ChangeValue(IsAlmostFallOnGround() || isGrounded.value());

        // perform horizontal moving
        if (isGrounded.value() && !isSaveReceiving.value()) // horizontal input only available on the ground.
        {
            float currentSpeed = isSprinting.value() ? moveSpeed_sprint : moveSpeed;
            currentSpeed *= this.speedConstant;
            this.transform.position += new Vector3(moveInput * currentSpeed * speedConstant * Time.deltaTime, 0, 0);
            if (this.moveInput > 0)
                this.SetPlayerDirection("right");
            else if (this.moveInput < 0)
                this.SetPlayerDirection("left");
        }

        // perform autoturn
        if (this.enableAutoTurn.value())
        {
            if (GetComponent<Rigidbody2D>().velocity.x > 0f)
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            else if (GetComponent<Rigidbody2D>().velocity.x < 0f)
                transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, jumpSignalRadius);
    }

    private void SetColliderStandup()
    {
        GetComponent<CapsuleCollider2D>().direction = CapsuleDirection2D.Vertical;
        GetComponent<CapsuleCollider2D>().offset = new Vector2(0, -0.31f);
        GetComponent<CapsuleCollider2D>().size = new Vector2(.55f, 1.52f);
    }

    private void SetColliderLiedown()
    {
        GetComponent<CapsuleCollider2D>().direction = CapsuleDirection2D.Horizontal;
        GetComponent<CapsuleCollider2D>().offset = new Vector2(0, -0.77f);
        GetComponent<CapsuleCollider2D>().size = new Vector2(1.52f, .55f);
    }

    public bool GetIsSaveReceiving()
    {
        return this.isSaveReceiving.value();
    }
    public void SetPlayerDirection(string direction)
    {
        if (direction == "right")
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        else if (direction == "left")
            transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        else
            Debug.Log("Wrong command in the function SetPlayerDirection");
    }
    public void SetEnableAutoTurn(bool value)
    {
        this.enableAutoTurn.ChangeValue(value);
    }
}
