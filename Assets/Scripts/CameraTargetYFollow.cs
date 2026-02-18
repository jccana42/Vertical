using UnityEngine;

public class CameraTargetYFollow : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform player;

    [Header("Fixed X/Z")]
    [SerializeField] private float fixedX = 0f;
    [SerializeField] private float fixedZ = -10f;

    [Header("Start behavior")]
    [Tooltip("Y inicial del target de cámara (pon aquí la altura con la que quieres empezar, abajo).")]
    [SerializeField] private float startY = -2.3f;

    [Tooltip("Cuánto tiene que subir el player (en unidades Unity) para que la cámara empiece a seguir en Y.")]
    [SerializeField] private float engageDeltaY = 1.0f;

    [Tooltip("Suavizado del enganche (0 = instantáneo).")]
    [SerializeField] private float engageSmooth = 0f;

    private bool engaged;
    private float engageY;
    private float currentY;

    private void Start()
    {
        currentY = startY;

        if (player != null)
            engageY = player.position.y + engageDeltaY;
    }

    private void LateUpdate()
    {
        if (!player) return;

        float py = player.position.y;

        // Engancha cuando el player suba lo suficiente
        if (!engaged && py >= engageY)
            engaged = true;

        float targetY = engaged ? py : startY;

        // Suavizado opcional
        if (engageSmooth > 0f)
            currentY = Mathf.Lerp(currentY, targetY, Time.deltaTime * engageSmooth);
        else
            currentY = targetY;

        transform.position = new Vector3(fixedX, currentY, fixedZ);
    }
}
