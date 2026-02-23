using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerEnemyInteraction2D : MonoBehaviour
{
    [Header("Stomp")]
    [SerializeField] private float stompTolerance = 0.06f; // margen para considerar "por encima"
    [SerializeField] private float bounceVelocity = 6f;    // rebote al matar

    [Header("Damage")]
    [SerializeField] private int contactDamage = 1;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = true;

    private Rigidbody2D rb;
    private Collider2D playerCol;
    private PlayerHealth health;
    private PlayerAudio playerAudio;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCol = GetComponent<Collider2D>();
        health = GetComponent<PlayerHealth>();
        playerAudio = GetComponent<PlayerAudio>();

        if (debugLogs)
        {
            Debug.Log($"[PlayerEnemyInteraction2D] Awake en {name} | " +
                      $"RB={(rb != null)} COL={(playerCol != null)} Health={(health != null)} Audio={(playerAudio != null)}");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision == null || collision.collider == null) return;

        if (debugLogs)
        {
            Debug.Log($"[PlayerEnemyInteraction2D] HIT: {collision.collider.name} " +
                      $"Tag={collision.collider.tag} Layer={LayerMask.LayerToName(collision.collider.gameObject.layer)} " +
                      $"v=({rb.linearVelocity.x:0.00},{rb.linearVelocity.y:0.00})");
        }

        // Solo enemigos por TAG
        if (!collision.collider.CompareTag("Enemy"))
        {
            if (debugLogs) Debug.Log("[PlayerEnemyInteraction2D] Ignorado: no tiene Tag Enemy.");
            return;
        }

        var enemyCol = collision.collider;
        var enemyKillable = enemyCol.GetComponentInParent<EnemyKillable>();
        bool canKill = enemyKillable != null;

        bool stomp = IsStomp(enemyCol);

        if (debugLogs)
            Debug.Log($"[PlayerEnemyInteraction2D] stomp={stomp} killable={canKill} healthNull={(health == null)} audioNull={(playerAudio == null)}");

        if (stomp && canKill)
        {
            if (debugLogs) Debug.Log("[PlayerEnemyInteraction2D] STOMP -> enemigo eliminado");

            // SONIDO: pisar enemigo
            playerAudio?.PlayStompSound();

            // Eliminar enemigo
            enemyKillable.Die();

            // Rebote
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceVelocity);
        }
        else
        {
            if (debugLogs) Debug.Log($"[PlayerEnemyInteraction2D] CONTACT DAMAGE -> {contactDamage}");

            if (health == null)
            {
                Debug.LogError("[PlayerEnemyInteraction2D] ERROR: PlayerHealth es NULL. " +
                               "Pon PlayerHealth en el MISMO GameObject que este script.");
                return;
            }

            int before = health.CurrentHealth;
            bool applied = health.Damage(contactDamage);
            int after = health.CurrentHealth;

            if (debugLogs)
                Debug.Log($"[PlayerEnemyInteraction2D] Damage(applied={applied}) {before} -> {after}");
        }
    }

    private bool IsStomp(Collider2D enemyCol)
    {
        // si vas subiendo, no es stomp
        if (rb.linearVelocity.y > 0.1f) return false;

        float playerBottom = playerCol.bounds.min.y;
        float enemyTop = enemyCol.bounds.max.y;

        bool stomp = playerBottom >= (enemyTop - stompTolerance);

        if (debugLogs)
            Debug.Log($"[PlayerEnemyInteraction2D] IsStomp? playerBottom={playerBottom:0.000} enemyTop={enemyTop:0.000} tol={stompTolerance:0.000} => {stomp}");

        return stomp;
    }
}