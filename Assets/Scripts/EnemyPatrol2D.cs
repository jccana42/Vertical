using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyPatrol2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Edge Check")]
    [SerializeField] private Transform edgeCheck;     // hijo "EdgeCheck"
    [SerializeField] private float checkRadius = 0.12f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Facing")]
    [Tooltip("Marca esto si tu sprite por defecto mira a la IZQUIERDA.")]
    [SerializeField] private bool spriteFacesLeftByDefault = true;

    [Header("Anti Jitter")]
    [SerializeField] private float flipCooldown = 0.15f; // evita flip en bucle en el borde

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private int dir = -1; // -1 izquierda, +1 derecha
    private float flipTimer;

    private Vector3 edgeCheckLocalBase; // guardamos su posición local "base"

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr == null) sr = GetComponent<SpriteRenderer>();

        if (edgeCheck == null)
            Debug.LogWarning("EdgeCheck no asignado. Crea un hijo 'EdgeCheck' y arrástralo aquí.");

        if (edgeCheck != null)
            edgeCheckLocalBase = edgeCheck.localPosition;
    }

    private void FixedUpdate()
    {
        // cooldown de giro
        if (flipTimer > 0f) flipTimer -= Time.fixedDeltaTime;

        // 1) mover
        rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);

        // 2) colocar EdgeCheck en el lado hacia donde vamos
        UpdateEdgeCheckSide();

        // 3) comprobar suelo delante
        if (edgeCheck != null && flipTimer <= 0f)
        {
            bool hasGroundAhead = Physics2D.OverlapCircle(edgeCheck.position, checkRadius, groundLayer);
            if (!hasGroundAhead)
                Flip();
        }

        // 4) flip visual
        ApplyVisualFlip();
    }

    private void UpdateEdgeCheckSide()
    {
        if (edgeCheck == null) return;

        // edgeCheckLocalBase.x debe ser el "adelante" en valor absoluto
        float x = Mathf.Abs(edgeCheckLocalBase.x);
        edgeCheck.localPosition = new Vector3(x * dir, edgeCheckLocalBase.y, edgeCheckLocalBase.z);
    }

    private void Flip()
    {
        dir *= -1;
        flipTimer = flipCooldown;

        // empujón mínimo para alejarse del borde y no re-flippear instantáneo
        rb.position += new Vector2(dir * 0.03f, 0f);
    }

    private void ApplyVisualFlip()
    {
        if (sr == null) return;

        // Si el sprite base mira a la izquierda:
        // dir>0 (derecha) => flipX=true
        // dir<0 (izquierda) => flipX=false
        if (dir > 0)
            sr.flipX = spriteFacesLeftByDefault ? true : false;
        else
            sr.flipX = spriteFacesLeftByDefault ? false : true;
    }

    private void OnDrawGizmosSelected()
    {
        if (edgeCheck == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(edgeCheck.position, checkRadius);
    }
}
