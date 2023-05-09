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

    private void Update()
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
        SetFacingDirection(isFacingLeft);
    }

    private void SetFacingDirection(bool isLeft)
    {
        playerVisual.transform.localScale = new(isLeft ? -1 : 1, 1, 1);
    }

    #endregion

    #region Detection

    [Header("Detection")]
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _grounderOffset = -1, _grounderRadius = 0.2f;
    [SerializeField] private float _wallCheckOffset = 0.5f, _wallCheckRadius = 0.05f;
    private bool isAgainstLeftWall, isAgainstRightWall, pushingLeftWall, pushingRightWall;
    public bool IsGrounded;

    private readonly Collider2D[] _ground = new Collider2D[1];
    private readonly Collider2D[] _leftWall = new Collider2D[1];
    private readonly Collider2D[] _rightWall = new Collider2D[1];

    private void HandleGrounding()
    {
        var grounded = Physics2D.OverlapCircleNonAlloc((Vector2)transform.position + new Vector2(0, _grounderOffset), _grounderRadius, _ground, _groundMask) > 0;

        if (!IsGrounded && grounded)
        {
            IsGrounded = true;
            hasDashed = false;
            hasJumped = false;
            currentMovementLerpSpeed = 100;
            transform.SetParent(_ground[0].transform);
            onGround.Invoke(IsGrounded);
        }
        else if (IsGrounded && !grounded)
        {
            IsGrounded = false;
            timeLastGroundJump = Time.time;
            transform.SetParent(null);
            onGround.Invoke(IsGrounded);
        }

        isAgainstLeftWall = Physics2D.OverlapCircleNonAlloc((Vector2)transform.position + new Vector2(-_wallCheckOffset, 0), _wallCheckRadius, _leftWall, _groundMask) > 0;
        isAgainstRightWall = Physics2D.OverlapCircleNonAlloc((Vector2)transform.position + new Vector2(_wallCheckOffset, 0), _wallCheckRadius, _rightWall, _groundMask) > 0;
        pushingLeftWall = isAgainstLeftWall && input.direction.x < 0;
        pushingRightWall = isAgainstRightWall && input.direction.x > 0;
    }
    #endregion

    #region Walking
    [Header("Walking")]
    [SerializeField] private float _walkSpeed = 4;
    [SerializeField] private float _acceleration = 2;
    [SerializeField] private float currentMovementLerpSpeed = 100;

    private void HandleWalking()
    {
        currentMovementLerpSpeed = Mathf.MoveTowards(currentMovementLerpSpeed, 100, wallJumpMovementLerp * Time.deltaTime);

        if (dashing) 
            return;

        var acceleration = IsGrounded ? _acceleration : _acceleration * 0.5f;

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

        var idealVel = new Vector2(input.axes.x * _walkSpeed, playerRigidbody.velocity.y);
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
    private bool hasDoubleJumped;
    private bool previousInputJump;

    private void HandleJumping()
    {
        if (dashing) 
            return;

        if (input.jump && !previousInputJump)
        {
            if (!IsGrounded && (isAgainstLeftWall || isAgainstRightWall) && timeLastWallJump <= Time.time + wallJumpLock)
            {
                timeLastWallJump = Time.time;
                currentMovementLerpSpeed = wallJumpMovementLerp;
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
    [SerializeField] private float _slideSpeed = 1;
    private bool wallSliding;

    private void HandleWallSlide()
    {
        var sliding = !IsGrounded && (pushingLeftWall || pushingRightWall);

        if (sliding && !wallSliding)
        {
            transform.SetParent(pushingLeftWall ? _leftWall[0].transform : _rightWall[0].transform);
            wallSliding = true;
            onWallSlide.Invoke(true);
        }
        else if (!sliding && wallSliding)
        {
            transform.SetParent(null);
            IsGrounded = false;
            wallSliding = false;
            onWallSlide.Invoke(false);
        }

        if (sliding) 
            playerRigidbody.velocity = new(0, -_slideSpeed);

    }
    #endregion

    #region Dash
    [Header("Dash")]
    [SerializeField] private float dashSpeed = 3;
    [SerializeField] private float dashLength = 0.1f;
    [SerializeField] private float dashCooldown = 1;
    private bool hasDashed;
    private bool dashing;
    private float timeStartedDash;
    private Vector2 dashDirection;

    private void HandleDashing()
    {
        if (input.dash && !hasDashed && Time.time >= timeStartedDash + dashCooldown)
        {
            dashDirection = new Vector2(input.axes.x, input.axes.y).normalized;

            if (dashDirection == Vector2.zero) 
                dashDirection = isFacingLeft ? Vector2.left : Vector2.right;

            if (wallSliding)
                dashDirection.x *= -1;

            dashing = true;
            hasDashed = true;
            timeStartedDash = Time.time;
            playerRigidbody.gravityScale = 0;

            onDash.Invoke();
        }

        if (dashing)
        {
            playerRigidbody.velocity += dashDirection * dashSpeed;

            if (Time.time >= timeStartedDash + dashLength)
            {
                dashing = false;
                playerRigidbody.velocity = new (playerRigidbody.velocity.x, Math.Max(3, playerRigidbody.velocity.y));
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
        Gizmos.DrawWireSphere(transform.position + new Vector3(-_wallCheckOffset, 0), _wallCheckRadius);
        Gizmos.DrawWireSphere(transform.position + new Vector3(_wallCheckOffset, 0), _wallCheckRadius);
    }
    private void DrawGrounderGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, _grounderOffset), _grounderRadius);
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