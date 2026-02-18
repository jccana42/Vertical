using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint; // arrastra un Empty "RespawnPoint"
    private PlayerHealth health;
    private Rigidbody2D rb;

    private void Awake()
    {
        health = GetComponent<PlayerHealth>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        if (health != null) health.OnDied += Respawn;
    }

    private void OnDisable()
    {
        if (health != null) health.OnDied -= Respawn;
    }

    private void Respawn()
    {
        if (respawnPoint == null)
        {
            // fallback: al origen
            transform.position = Vector3.zero;
        }
        else
        {
            transform.position = respawnPoint.position;
        }

        if (rb != null) rb.linearVelocity = Vector2.zero;

        health.ResetHealth();
    }
}
