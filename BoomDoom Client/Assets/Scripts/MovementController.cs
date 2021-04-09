using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : Singleton<MovementController>
{
    public Transform petPosition;
    #region variables
    [Header("Movement")]
    [SerializeField] float movementSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] int extraJumps;
    [SerializeField] float diveForce;
    /// <summary>
    /// The least vertical input value that is needed in order for the player to jump
    /// </summary>
    [Range(0.5f, 1)]
    [SerializeField] float minVerticalInputJump;
    [Header("Grounded")]
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius;
    [Header("Climb")]
    [Range(1, 100)]
    [SerializeField] int wallFriction = 50;
    [SerializeField] Vector2 wallCheckLeft;
    [SerializeField] Vector2 wallCheckRight;
    [SerializeField] float wallCheckRadius;

    private int horizontalInput;
    private bool joystickJumpReady = true;
    private float originalGravity;
    private bool justLandedOnWall = true;
    private int extraJumpsLeft;
    private int verticalInput;
    private bool looksRight = true;
    private bool canControllSpeed = true;
    private Rigidbody2D rb;
    private Animator animator;
    private enum State { Run, Idle, Jump, WallClimb, Airborne }
    private State currentState;

    public bool Grounded { get; private set; }
    public int TouchesWall { get; private set; }
    private bool mustJoystickJump { get { return (joystickJumpReady && verticalInput > minVerticalInputJump); } }
    #endregion

    #region unity functions
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalGravity = rb.gravityScale;
        animator = GetComponent<Animator>();
        currentState = State.Idle;
        extraJumpsLeft = extraJumps;
    }
    private void Update()
    {      
        horizontalInput = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal") + Joystick.instance.Output.x);
        verticalInput = Mathf.RoundToInt(Input.GetAxisRaw("Vertical") + Joystick.instance.Output.y);
        if (verticalInput <= minVerticalInputJump)
            joystickJumpReady = true;
        StateUpdate();
    }
    private void FixedUpdate()
    {
        if(canControllSpeed)
            rb.velocity = new Vector2(horizontalInput * movementSpeed, rb.velocity.y); 
        Grounded = Physics2D.CircleCast(groundCheck.position, groundCheckRadius, Vector2.up, 0, whatIsGround);
        if (Grounded && rb.velocity.y <= 0)
            RestoreJumps();
        TouchesWall = CheckWall();
        if(TouchesWall!=0)
        {
            RestoreJumps();
        }
    }
    #endregion
    #region state machine
    private void StateUpdate()
    {
        #region applies to every state
        // change to wallClimb state
        if (TouchesWall != 0 && !Grounded)
            ChangeState(State.WallClimb);
        // default jump behavior
        if (currentState != State.WallClimb)
        {
            if (Input.GetKeyDown(KeyCode.Space) || mustJoystickJump)
            {
                Jump();
                joystickJumpReady = false;
            }
            if (!Grounded)
                ChangeState(State.Airborne);
        }

        #endregion

        // state specific
        switch (currentState)
        {
                                // idle
            case State.Idle:
                if (horizontalInput != 0)
                    ChangeState(State.Run);
                break;
                                // run
            case State.Run:
                if (horizontalInput == 0)
                    ChangeState(State.Idle);
                else
                {
                    ManageFlip();
                }
                break;
                                // wallClimb
            case State.WallClimb:
                if (rb.velocity.y > 0)
                    rb.gravityScale = originalGravity;
                else if (rb.velocity.y <= 0)
                {
                    if (justLandedOnWall)
                    {
                        rb.velocity = Vector2.down;
                        justLandedOnWall = false;
                    }                
                    rb.gravityScale = originalGravity / wallFriction;
                }
                    
                if (Input.GetKeyDown(KeyCode.Space) || mustJoystickJump)
                {
                    Jump(new Vector2(-1 * TouchesWall, 2));
                    joystickJumpReady = false;
                }
                if (TouchesWall == 0 || Grounded)
                    ChangeState(State.Idle);
                break;
                            // airborne
            case State.Airborne:
                if (verticalInput < 0)
                    rb.AddForce(Vector2.down * diveForce, ForceMode2D.Force);
                if (Grounded)
                    ChangeState(State.Idle);
                ManageFlip();
                break;
                                   
        }
    }

    private void ManageFlip()
    {
        if (horizontalInput > 0)
        {
            if (!looksRight)
                Flip();
        }
        else if (horizontalInput < 0)
        {
            if (looksRight)
                Flip();
        }
    }

    private void ChangeState(State newState)
    {
        if (currentState != newState)
        {
            OnStatesEnter(newState);
            currentState = newState;
            animator.Play(newState.ToString());
        }
    }

    private void OnStatesEnter(State newState)
    {
        if (currentState == State.WallClimb)
            rb.gravityScale = originalGravity;
        if (newState == State.WallClimb)
            justLandedOnWall = true;
    }
    #endregion
    #region enviroment check
    /// <summary>
    /// returns whether a wall is detected, 1 for right wall, -1 for left wall, 0 for none
    /// </summary>
    /// <returns></returns>
    private int CheckWall()
    {
        RaycastHit2D leftWall = Physics2D.CircleCast(transform.position + (Vector3)wallCheckLeft, wallCheckRadius, Vector2.up, 0, whatIsGround);
        RaycastHit2D rightWall = Physics2D.CircleCast(transform.position + (Vector3)wallCheckRight, wallCheckRadius, Vector2.up, 0, whatIsGround);
        if (leftWall)
            return -1;
        else if (rightWall)
            return 1;
        return 0;
    }
    #endregion
    #region player control
    private void Flip()
    {
        looksRight = !looksRight;
        transform.Rotate(0, 180, 0);
    }
    private void Jump()
    {
        // regular jump
        if(Grounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        // extra jump
        else if(extraJumpsLeft > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            extraJumpsLeft--;
        }
            

    }

    private void Jump(Vector2 direction)
    {
        if (extraJumpsLeft > 0)
        {
            StartCoroutine(LoseSpeedControl(0.25f));
            rb.velocity = direction.normalized * jumpForce;
            extraJumpsLeft--;
        }       
    }
    private void RestoreJumps()
    {
        if (extraJumpsLeft != extraJumps)
            extraJumpsLeft = extraJumps;
    }

    IEnumerator LoseSpeedControl(float forSeconds)
    {
        canControllSpeed = false;
        yield return new WaitForSeconds(forSeconds);
        canControllSpeed = true;
    }
    #endregion
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawWireSphere(transform.position + (Vector3)wallCheckLeft, wallCheckRadius);
        Gizmos.DrawWireSphere(transform.position + (Vector3)wallCheckRight, wallCheckRadius);
    }


}
