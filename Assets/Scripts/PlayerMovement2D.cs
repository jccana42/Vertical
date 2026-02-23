using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5.3f;
    [SerializeField] private float accel = 25f;
    [SerializeField] private float decel = 35f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float coyoteTime = 0.10f;
    [SerializeField] private float jumpBuffer = 0.10f;
    [SerializeField] private float jumpCutMultiplier = 0.5f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.12f;
    [SerializeField] private LayerMask groundMask;

    [Header("Input")]
    [SerializeField] private bool useMobileInput = true;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool spriteFacesLeftByDefault = true;

    [Header("Audio (opcional)")]
    [SerializeField] private PlayerAudio playerAudio;

    private Rigidbody2D rb;

    private float moveX;
    private float coyoteTimer;
    private float jumpBufferTimer;

    private bool wasGrounded;

    // ====== API p�blica para Animator ======
    public bool IsGrounded { get; private set; }
    public bool JumpHeld { get; private set; }
    public bool JumpPressed { get; private set; }
    public float XVel => rb != null ? rb.linearVelocity.x : 0f;
    public float YVel => rb != null ? rb.linearVelocity.y : 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (groundCheck == null)
            groundCheck = transform;

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (playerAudio == null)
            playerAudio = GetComponent<PlayerAudio>();
    }

    private void Update()
    {
        // ===== INPUT =====
        if (useMobileInput)
        {
            moveX = MobileInput.Horizontal;
            JumpHeld = MobileInput.JumpHeld;

            if (MobileInput.JumpDown)
                jumpBufferTimer = jumpBuffer;
            else
                jumpBufferTimer -= Time.deltaTime;
        }
        else
        {
            moveX = Input.GetAxisRaw("Horizontal");
            JumpHeld = Input.GetButton("Jump");

            if (Input.GetButtonDown("Jump"))
                jumpBufferTimer = jumpBuffer;
            else
                jumpBufferTimer -= Time.deltaTime;
        }

        // ===== GROUND CHECK =====
        IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask);

        // Detectar aterrizaje (para reset de sonido de salto)
        if (!wasGrounded && IsGrounded)
        {
            playerAudio?.ResetJumpSound();
        }
        wasGrounded = IsGrounded;

        // Coyote
        if (IsGrounded)
            coyoteTimer = coyoteTime;
        else
            coyoteTimer -= Time.deltaTime;

        // Buffer para animator
        JumpPressed = jumpBufferTimer > 0f;

        // ===== EJECUTAR SALTO =====
        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            DoJump();
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
        }

        // ===== SALTO VARIABLE (si sueltas antes, salto m�s corto) =====
        if (!JumpHeld && rb.linearVelocity.y > 0.01f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
        }

        // ===== FLIP =====
        if (spriteRenderer != null && Mathf.Abs(moveX) > 0.01f)
        {
            spriteRenderer.flipX = spriteFacesLeftByDefault ? (moveX > 0f) : (moveX < 0f);
        }
    }

    private void FixedUpdate()
    {
        // Movimiento estable para m�vil: velocity directa con suavizado
        float targetX = moveX * moveSpeed;

        float rate = Mathf.Abs(targetX) > 0.01f ? accel : decel;
        float newX = Mathf.MoveTowards(rb.linearVelocity.x, targetX, rate * Time.fixedDeltaTime);

        rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);
    }

    private void DoJump()
    {
        // Reset vertical para salto consistente
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        // Sonido de salto (una vez por salto)
        playerAudio?.TryPlayJumpSound();
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}