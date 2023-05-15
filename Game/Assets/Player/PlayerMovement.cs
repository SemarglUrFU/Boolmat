using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement: MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Rigidbody2D playerRigidbody;
    [SerializeField] private GameObject playerVisual;
    private PlayerInput input;
    private bool isFacingLeft;

    private void FixedUpdate()
    {
        HandleInput();
        HandleGrounding();
        HandleWalking();
        HandleJumping();
        HandleWallSlide();
        HandleDashing();
        UpdatePosition();
    }

    #region Update position
    private Vector2 lastFramePosition;
    private Vector2 frameDelta;
    private void UpdatePosition()
    {
        var framePosition = (Vector2)playerRigidbody.transform.position;
        framePosition = Filter(framePosition);
        var delta = Filter(framePosition - lastFramePosition);
        if (frameDelta != Vector2.zero)
            onChangePosition.Invoke(delta);
        frameDelta = delta;
        lastFramePosition = framePosition;

        static Vector2 Filter(Vector2 position, float epsilon = 0.0001f)
        {
            position.x = Math.Abs(position.x) > epsilon ? position.x : 0;
            position.y = Math.Abs(position.y) > epsilon ? position.y : 0;
            return position;
        }
    }
    #endregion

    #region Inputs
    public void HandleInput()
    {
        input = playerController.GetMovementInput();
        isFacingLeft = input.direction.x != 1 && (input.direction.x == -1 || isFacingLeft);
    }

    private void SetFacingDirection(bool isLeft)
    {
        playerVisual.transform.localScale = new(isLeft ? -1 : 1, 1, 1);
    }

    #endregion

    #region Detection

    [Header("Detection")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float grounderOffset = -1, grounderRadius = 0.2f;
    [SerializeField] private float wallCheckOffset = 0.5f, wallCheckRadius = 0.05f;
    private bool isAgainstLeftWall, isAgainstRightWall, pushingLeftWall, pushingRightWall;
    public bool IsGrounded;

    private readonly Collider2D[] ground = new Collider2D[1];
    private readonly Collider2D[] leftWall = new Collider2D[1];
    private readonly Collider2D[] rightWall = new Collider2D[1];

    private void HandleGrounding()
    {
        var grounded = Physics2D.OverlapCircleNonAlloc((Vector2)transform.position + new Vector2(0, grounderOffset), grounderRadius, ground, groundMask) > 0;

        if (!IsGrounded && grounded)
        {
            IsGrounded = true;
            hasDashed = false;
            hasJumped = false;
            currentMovementLerpSpeed = 100;
            transform.SetParent(ground[0].transform);
            onGround.Invoke(IsGrounded);
        }
        else if (IsGrounded && !grounded)
        {
            IsGrounded = false;
            timeLastGroundJump = Time.time;
            transform.SetParent(null);
            onGround.Invoke(IsGrounded);
        }

        isAgainstLeftWall = Physics2D.OverlapCircleNonAlloc((Vector2)transform.position + new Vector2(-wallCheckOffset, 0), wallCheckRadius, leftWall, groundMask) > 0;
        isAgainstRightWall = Physics2D.OverlapCircleNonAlloc((Vector2)transform.position + new Vector2(wallCheckOffset, 0), wallCheckRadius, rightWall, groundMask) > 0;
        pushingLeftWall = isAgainstLeftWall && input.direction.x < 0;
        pushingRightWall = isAgainstRightWall && input.direction.x > 0;
    }
    #endregion

    #region Walking
    [Header("Walking")]
    [SerializeField] private float walkSpeed = 4;
    [SerializeField] private float acceleration = 2;
    [SerializeField] private float currentMovementLerpSpeed = 100;

    private void HandleWalking()
    {
        currentMovementLerpSpeed = Mathf.MoveTowards(currentMovementLerpSpeed, 100, wallJumpMovementLerp * Time.deltaTime);

        if (dashing) return;

        SetFacingDirection(isFacingLeft);

        var acceleration = IsGrounded ? this.acceleration : this.acceleration * 0.5f;

        if (input.direction.x == -1)
        {
            if (playerRigidbody.velocity.x > 0) 
                input.axes.x = 0;
            input.axes.x = Mathf.MoveTowards(input.axes.x, -1, acceleration * Time.deltaTime);
        }
        else if (input.direction.x == 1)
        {
            if (playerRigidbody.velocity.x < 0) 
                input.axes.x = 0;
            input.axes.x = Mathf.MoveTowards(input.axes.x, 1, acceleration * Time.deltaTime);
        }
        else
        {
            input.axes.x = Mathf.MoveTowards(input.axes.x, 0, acceleration * 2 * Time.deltaTime);
        }

        var idealVel = new Vector2(input.axes.x * walkSpeed, playerRigidbody.velocity.y);
        playerRigidbody.velocity = Vector2.MoveTowards(playerRigidbody.velocity, idealVel, currentMovementLerpSpeed * Time.deltaTime);
    }

    #endregion

    #region Jumping
    [Header("Jumping")]
    [SerializeField] private float jumpForce = 15;
    [SerializeField] private float fallMultiplier = 7;
    [SerializeField] private float jumpVelocityFalloff = 8;
    [SerializeField] private float wallJumpLock = 0.25f;
    [SerializeField] private float wallJumpMovementLerp = 5;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private bool  enableDoubleJump = true;
    private float timeLastGroundJump;
    private float timeLastWallJump;
    private bool hasJumped;
    private bool hasWallJumped;
    private bool hasDoubleJumped;
    private bool previousInputJump;

    private void HandleJumping()
    {
        if (dashing) return;
        hasWallJumped = false;

        if (input.jump && !previousInputJump)
        {
            if (!IsGrounded && (isAgainstLeftWall || isAgainstRightWall) && timeLastWallJump <= Time.time + wallJumpLock)
            {
                timeLastWallJump = Time.time;
                currentMovementLerpSpeed = wallJumpMovementLerp;
                hasWallJumped = true;
                SetFacingDirection(isAgainstRightWall);
                ExecuteJump(new (isAgainstLeftWall ? jumpForce : -jumpForce, jumpForce));
            }
            else if ((IsGrounded || timeLastGroundJump <= Time.time + coyoteTime || enableDoubleJump && !hasDoubleJumped) 
                && (!hasJumped || hasJumped && !hasDoubleJumped))
            {
                timeLastGroundJump = Time.time;
                ExecuteJump(new(playerRigidbody.velocity.x, jumpForce), hasJumped);
            }
        }
        previousInputJump = input.jump;

        void ExecuteJump(Vector2 direction, bool doubleJump = false)
        {
            playerRigidbody.velocity = direction;
            hasDoubleJumped = doubleJump;
            hasJumped = true;
            if (doubleJump)
                onDoubleJump.Invoke();
            else
                onJump.Invoke();
        }

        if (playerRigidbody.velocity.y < jumpVelocityFalloff || playerRigidbody.velocity.y > 0 && !input.jump)
            playerRigidbody.velocity += fallMultiplier * Physics2D.gravity.y * Time.deltaTime * Vector2.up;
    }

    #endregion

    #region Wall Slide
    [Header("Wall Slide")]
    [SerializeField] private float slideSpeed = 1;
    private bool wallSliding;

    private void HandleWallSlide()
    {
        var sliding = !IsGrounded && (pushingLeftWall || pushingRightWall) && !dashing && !hasWallJumped;

        if (sliding && !wallSliding)
        {
            transform.SetParent(pushingLeftWall ? leftWall[0].transform : rightWall[0].transform);
            wallSliding = true;
            onWallSlide.Invoke(true);
            hasDashed = false;
            hasJumped = false;
        }
        else if (!sliding && wallSliding)
        {
            transform.SetParent(null);
            IsGrounded = false;
            wallSliding = false;
            onWallSlide.Invoke(false);
        }

        if (sliding) 
            playerRigidbody.velocity = new(0, -slideSpeed);

    }
    #endregion

    #region Dash
    [Header("Dash")]
    [SerializeField] private float dashSpeed = 7;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float dashCooldown = 1;
    private bool hasDashed;
    private bool dashing;
    private float timeStartedDash;
    private int dashDirection;

    private void HandleDashing()
    {
        if (input.dash && !hasDashed && Time.time >= timeStartedDash + dashCooldown)
        {
            dashDirection = input.direction.x;

            if (dashDirection == 0) 
                dashDirection = isFacingLeft ? -1 : 1;

            if (isAgainstLeftWall || isAgainstRightWall)
            {
                dashDirection *= -1;
                SetFacingDirection(!isFacingLeft);
            }

            dashing = true;
            hasDashed = true;
            timeStartedDash = Time.time;
            playerRigidbody.gravityScale = 0;

            onDash.Invoke();
        }

        if (dashing)
        {
            playerRigidbody.velocity = new Vector2(dashDirection * dashSpeed, 0);

            if (Time.time >= timeStartedDash + dashTime)
            {
                dashing = false;
                playerRigidbody.velocity = new (Math.Min(5, Math.Abs(playerRigidbody.velocity.x)) * dashDirection, 0);
                playerRigidbody.gravityScale = 1;
                if (IsGrounded) 
                    hasDashed = false;
            }
        }
    }
    #endregion

    #region Debug
    private void DrawWallSlideGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(-wallCheckOffset, 0), wallCheckRadius);
        Gizmos.DrawWireSphere(transform.position + new Vector3(wallCheckOffset, 0), wallCheckRadius);
    }
    private void DrawGrounderGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, grounderOffset), grounderRadius);
    }

    private void OnDrawGizmos()
    {
        DrawGrounderGizmos();
        DrawWallSlideGizmos();
    }
    #endregion

    [Header("Events")]
    public UnityEvent<Vector2> onChangePosition;
    public UnityEvent onJump;
    public UnityEvent onDoubleJump;
    public UnityEvent<bool> onWallSlide;
    public UnityEvent onDash;
    public UnityEvent<bool> onGround;
}