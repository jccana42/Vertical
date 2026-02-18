using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraAspectLock : MonoBehaviour
{
    [Header("Target aspect (16:9 por defecto)")]
    [SerializeField] private float targetAspect = 16f / 9f;

    [Header("Bar color")]
    [SerializeField] private Color barColor = Color.black;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        cam.backgroundColor = barColor;
    }

    private void Start()
    {
        Apply();
    }

    private void OnValidate()
    {
        if (targetAspect <= 0.01f) targetAspect = 16f / 9f;
        if (cam == null) cam = GetComponent<Camera>();
        if (cam != null) cam.backgroundColor = barColor;
        Apply();
    }

    private void Apply()
    {
        if (cam == null) return;

        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1f)
        {
            // Barras arriba/abajo (letterbox)
            Rect rect = cam.rect;
            rect.width = 1f;
            rect.height = scaleHeight;
            rect.x = 0f;
            rect.y = (1f - scaleHeight) / 2f;
            cam.rect = rect;
        }
        else
        {
            // Barras laterales (pillarbox)
            float scaleWidth = 1f / scaleHeight;

            Rect rect = cam.rect;
            rect.width = scaleWidth;
            rect.height = 1f;
            rect.x = (1f - scaleWidth) / 2f;
            rect.y = 0f;
            cam.rect = rect;
        }
    }

    private void Update()
    {
        // Si el usuario rota pantalla o cambia resolución, se reajusta
        Apply();
    }
}
