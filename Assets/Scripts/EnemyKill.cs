using UnityEngine;

public class EnemyKillable : MonoBehaviour
{
    public void Die()
    {
        // Aquí luego puedes meter animación/partículas/sonido.
        Destroy(gameObject);
    }
}
