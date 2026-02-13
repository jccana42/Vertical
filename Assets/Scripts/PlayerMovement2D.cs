using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Jump")]
    [SerializeField] private float jumpVelocity = 9f;
    [SerializeField, Range(0f, 1f)] private float jumpCutMultiplier = 0.5f;
    [SerializeField] private float coyoteTime = 0.08f;
    [SerializeField] private float jumpBufferTime = 0.10f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;

    private float inputX;
    private bool isGrounded;

    private float coyoteTimer;
    private float jumpBufferTimer;

    private bool jumpHeld;
    private bool jumpCutUsed;

    // ?? Esto lo leerá PlayerAnimator2D
    public bool JumpHeld => jumpHeld;
    public bool IsGrounded => isGrounded;
    public float XVel => rb != null ? rb.linearVelocity.x : 0f;
    public float YVel => rb != null ? rb.linearVelocity.y : 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (groundCheck == null)
            Debug.LogWarning("GroundCheck NO asignado. Arrastra el hijo GroundCheck al campo del script.");
    }

    private void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");

        isGrounded = CheckGrounded();

        if (isGrounded)
        {
            coyoteTimer = coyoteTime;
            jumpCutUsed = false;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        // Buffer de salto
        if (Input.GetButtonDown("Jump"))
            jumpBufferTimer = jumpBufferTime;

        jumpBufferTimer -= Time.deltaTime;

        // Mantener salto
        jumpHeld = Input.GetButton("Jump");
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(inputX * moveSpeed, rb.linearVelocity.y);
        HandleJump();
    }

    private void HandleJump()
    {
        // Salto si: buffer activo + grounded o coyote
        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);
            jumpCutUsed = false;
        }

        // Salto variable: al soltar durante subida, recorte 1 vez
        if (!jumpHeld && rb.linearVelocity.y > 0f && !jumpCutUsed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
            jumpCutUsed = true;
        }
    }

    private bool CheckGrounded()
    {
        if (groundCheck == null) return false;

        return Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
