using UnityEngine;

public class EnemyKillable : MonoBehaviour
{
    [Header("Death VFX")]
    [SerializeField] private GameObject deathVfxPrefab; // aquí arrastras Peludo_explode prefab
    [SerializeField] private Vector3 vfxOffset = Vector3.zero;

    private bool isDead;

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        // Spawn explosión
        if (deathVfxPrefab != null)
        {
            Instantiate(deathVfxPrefab, transform.position + vfxOffset, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}