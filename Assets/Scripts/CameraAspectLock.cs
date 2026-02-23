using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraAspectLock : MonoBehaviour
{
    [Header("Target aspect (16:9 por defecto)")]
    [SerializeField] private float targetAspect = 16f / 9f;

    [Header("Bar color")]
    [SerializeField] private Color barColor = Color.black;

    private Camera cam;
    private int lastW, lastH;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        cam.backgroundColor = barColor;

        Sanitize();
        Apply();
        CacheSize();
    }

    private void OnValidate()
    {
        cam = GetComponent<Camera>();
        if (cam != null) cam.backgroundColor = barColor;

        Sanitize();
        Apply();
        CacheSize();
    }

    private void Update()
    {
        // Solo recalcular si cambia la resolución
        if (Screen.width != lastW || Screen.height != lastH)
        {
            Sanitize();
            Apply();
            CacheSize();
        }
    }

    private void CacheSize()
    {
        lastW = Screen.width;
        lastH = Screen.height;
    }

    private void Sanitize()
    {
        // Evita valores absurdos que provocan rects 0 y pantalla negra
        if (targetAspect < 0.5f || targetAspect > 5f)
            targetAspect = 16f / 9f;

        if (Screen.width <= 0 || Screen.height <= 0)
        {
            // En editor a veces puede dar frames raros; no hagas nada
            return;
        }
    }

    private void Apply()
    {
        if (cam == null) return;
        if (Screen.width <= 0 || Screen.height <= 0) return;

        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        Rect rect = cam.rect;

        if (scaleHeight < 1f)
        {
            // Letterbox (arriba/abajo)
            rect.width = 1f;
            rect.height = Mathf.Max(0.01f, scaleHeight);
            rect.x = 0f;
            rect.y = (1f - rect.height) * 0.5f;
        }
        else
        {
            // Pillarbox (laterales)
            float scaleWidth = 1f / scaleHeight;
            rect.width = Mathf.Max(0.01f, scaleWidth);
            rect.height = 1f;
            rect.x = (1f - rect.width) * 0.5f;
            rect.y = 0f;
        }

        cam.rect = rect;
        cam.backgroundColor = barColor;
    }
}
