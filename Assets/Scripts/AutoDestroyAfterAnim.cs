using UnityEngine;

public class AutoDestroyAfterAnim : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string stateName = "Peludo_explode"; // nombre del estado/clip en el Animator
    [SerializeField] private float fallbackSeconds = 0.5f;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Start()
    {
        float t = fallbackSeconds;

        if (animator != null)
        {
            // Espera 1 frame para que el Animator entre al estado
            StartCoroutine(DestroyNextFrame());
            return;
        }

        Destroy(gameObject, t);
    }

    private System.Collections.IEnumerator DestroyNextFrame()
    {
        yield return null;

        float t = fallbackSeconds;

        if (animator != null)
        {
            var st = animator.GetCurrentAnimatorStateInfo(0);
            // Si está en un estado, usamos su duración
            t = Mathf.Max(0.05f, st.length);
        }

        Destroy(gameObject, t);
    }
}