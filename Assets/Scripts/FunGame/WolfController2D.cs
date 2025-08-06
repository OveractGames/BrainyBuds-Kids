using System;
using UnityEngine;

/// <summary>
/// 2D Wolf Controller: left/right + jump with coyote time, jump buffer,
/// variable jump height, better fall gravity, and sprite flip.
/// Prevents infinite jumping by enforcing grounded/coyote checks.
/// Works with keyboard or external (mobile) input.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class WolfController2D : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private Rigidbody2D rb;

    [SerializeField] private Transform groundCheck; // a small point under the feet
    [SerializeField] private LayerMask groundLayer; // layers considered as ground
    [SerializeField] private SpriteRenderer sprite; // optional, for flip

    [Header("Movement")] [SerializeField] private float moveSpeed = 8f; // target run speed
    [SerializeField] private float accel = 80f; // ground accel
    [SerializeField] private float decel = 90f; // ground decel
    [SerializeField] private float airAccel = 60f; // air accel
    [SerializeField] private float airDecel = 60f; // air decel
    [SerializeField] private float maxAirSpeed = 10f; // clamp X while in air

    [Header("Jump")] [SerializeField] private float jumpForce = 14f;
    [SerializeField] private float gravityScale = 3.0f; // base gravity (also set on rb)
    [SerializeField] private float fallGravityMul = 2.0f; // extra gravity when falling
    [SerializeField] private float lowJumpMul = 2.2f; // extra gravity on early release
    [SerializeField] private float coyoteTime = 0.12f; // grace time after leaving ground
    [SerializeField] private float jumpBuffer = 0.12f; // grace before landing
    [SerializeField] private float groundCheckRadius = 0.18f;

    [Header("Input")]
    [Tooltip("If true, reads Input.GetAxisRaw / GetButton. Else drive via public APIs.")]
    [SerializeField]
    private bool useBuiltInInput = true;

    [SerializeField] private string horizontalAxis = "Horizontal";
    [SerializeField] private string jumpButton = "Jump";

    // --- runtime state ---
    private float _moveInput; // -1..1
    private bool _jumpHeld; // for variable jump height
    private bool _jumpPressedThisFrame;
    private bool _isGrounded;
    private float _coyoteCounter;
    private float _jumpBufferCounter;

    private bool _facingRight = true;

    public event Action OnLeavesCollect;

    private void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;

        if (!sprite) sprite = GetComponentInChildren<SpriteRenderer>();
        if (!groundCheck)
            Debug.LogWarning($"[{nameof(WolfController2D)}] GroundCheck is not assigned on {name}.");
    }

    private void Update()
    {
        // --- INPUT ---
        if (useBuiltInInput)
        {
            _moveInput = Input.GetAxisRaw(horizontalAxis); // -1..1
            bool jumpDown = Input.GetButtonDown(jumpButton);
            _jumpHeld = Input.GetButton(jumpButton);
            if (jumpDown) _jumpPressedThisFrame = true;
        }

        // --- GROUND CHECK ---
        bool wasGrounded = _isGrounded;
        _isGrounded = IsGrounded();

        if (_isGrounded)
            _coyoteCounter = coyoteTime; // reset coyote while on ground
        else
            _coyoteCounter -= Time.deltaTime;

        // --- JUMP BUFFER ---
        if (_jumpPressedThisFrame)
            _jumpBufferCounter = jumpBuffer; // store press for a short window
        else
            _jumpBufferCounter -= Time.deltaTime;

        // --- JUMP DECISION ---
        // Only jump if we have a buffered press AND we still have coyote time (or are grounded)
        if (_jumpBufferCounter > 0f && _coyoteCounter > 0f)
        {
            DoJump();
            _jumpBufferCounter = 0f; // consume buffer so it won't retrigger
            _coyoteCounter = 0f; // consume coyote window immediately
        }

        // Variable jump height & better fall gravity
        ApplyBetterJumpGravity();

        // Sprite flip
        HandleFlip();

        // Clear one-frame flags
        _jumpPressedThisFrame = false;
    }

    private void FixedUpdate()
    {
        // Horizontal motion with accel/decel
        float targetSpeed = _moveInput * moveSpeed;
        float speedDiff = targetSpeed - rb.linearVelocity.x;

        float accelRate;
        bool hasInput = Mathf.Abs(targetSpeed) > 0.01f;

        if (_isGrounded)
            accelRate = hasInput ? accel : decel;
        else
            accelRate = hasInput ? airAccel : airDecel;

        float movement = accelRate * speedDiff;
        rb.AddForce(new Vector2(movement, 0f));

        // Clamp horizontal speed in air
        if (!_isGrounded)
        {
            float clampedX = Mathf.Clamp(rb.linearVelocity.x, -maxAirSpeed, maxAirSpeed);
            rb.linearVelocity = new Vector2(clampedX, rb.linearVelocity.y);
        }
    }

    private bool IsGrounded()
    {
        if (!groundCheck) return false;
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void DoJump()
    {
        // Reset Y velocity for consistent jump
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void ApplyBetterJumpGravity()
    {
        // Stronger gravity when falling
        if (rb.linearVelocity.y < -0.01f)
        {
            rb.linearVelocity +=
                Vector2.up * (Physics2D.gravity.y * (fallGravityMul - 1f) * Time.deltaTime * rb.gravityScale);
        }
        // Short hop if jump is released early during ascent
        else if (rb.linearVelocity.y > 0.01f && !_jumpHeld)
        {
            rb.linearVelocity +=
                Vector2.up * (Physics2D.gravity.y * (lowJumpMul - 1f) * Time.deltaTime * rb.gravityScale);
        }
    }

    private void HandleFlip()
    {
        if (_moveInput > 0.05f && !_facingRight) Flip(true);
        else if (_moveInput < -0.05f && _facingRight) Flip(false);
    }

    private void Flip(bool faceRight)
    {
        _facingRight = faceRight;

        if (sprite)
        {
            sprite.flipX = !faceRight;
        }
        else
        {
            // fallback if no SpriteRenderer provided
            Vector3 s = transform.localScale;
            s.x = Mathf.Abs(s.x) * (faceRight ? 1f : -1f);
            transform.localScale = s;
        }
    }

    // -------- External input (mobile / UI) ----------------------------------

    /// <summary>Provide horizontal input from external sources (virtual joystick). Range [-1..1]</summary>
    public void SetMoveInput(float input) => _moveInput = Mathf.Clamp(input, -1f, 1f);

    /// <summary>Call on jump button DOWN (mobile UI).</summary>
    public void JumpPressed()
    {
        _jumpPressedThisFrame = true;
        _jumpHeld = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag($"leaves"))
        {
            OnLeavesCollect?.Invoke();
            Destroy(other.gameObject);
        }
    }

    /// <summary>Call on jump button UP (mobile UI).</summary>
    public void JumpReleased() => _jumpHeld = false;

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (groundCheck)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
#endif
}