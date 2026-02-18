// Assets/Scripts/VirtualJoystick.cs
using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("UI Refs")]
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;

    [Header("Settings")]
    [Tooltip("0 = usa tamaño del background automáticamente")]
    [SerializeField] private float handleRange = 0f;

    [Header("Axis Lock")]
    [Tooltip("Si está activado, el joystick solo produce movimiento horizontal (Y siempre 0).")]
    [SerializeField] private bool lockToX = true;

    public Vector2 InputVector { get; private set; }

    private Canvas _canvas;
    private Camera _uiCamera;

    private void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
        if (_canvas == null)
        {
            Debug.LogError("VirtualJoystick: Debe estar dentro de un Canvas.");
            enabled = false;
            return;
        }

        _uiCamera = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;

        if (background == null)
            background = GetComponent<RectTransform>();

        if (background == null)
        {
            Debug.LogError("VirtualJoystick: No hay RectTransform en el Joystick. Debe ser un objeto UI.");
            enabled = false;
            return;
        }

        if (handle == null)
        {
            Debug.LogError("VirtualJoystick: Handle no asignado (RectTransform).");
            enabled = false;
            return;
        }

        ResetStick();
    }

    public void OnPointerDown(PointerEventData eventData) => OnDrag(eventData);

    public void OnDrag(PointerEventData eventData)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                background, eventData.position, _uiCamera, out var localPoint))
            return;

        float range = handleRange;
        if (range <= 0f)
            range = Mathf.Min(background.rect.width, background.rect.height) * 0.5f;

        // Si bloqueamos a X, ignoramos Y desde el principio
        if (lockToX)
            localPoint = new Vector2(localPoint.x, 0f);

        // Clamp dentro del rango
        Vector2 clamped = Vector2.ClampMagnitude(localPoint, range);

        // Si bloqueamos a X, aseguramos Y=0 también tras el clamp
        if (lockToX)
            clamped = new Vector2(clamped.x, 0f);

        // Mueve el handle (requiere que handle sea hijo de background)
        handle.anchoredPosition = clamped;

        // Vector normalizado -1..1
        InputVector = clamped / range;

        // Seguridad: en modo X-only, InputVector.y siempre 0
        if (lockToX)
            InputVector = new Vector2(InputVector.x, 0f);
    }

    public void OnPointerUp(PointerEventData eventData) => ResetStick();

    private void ResetStick()
    {
        InputVector = Vector2.zero;
        if (handle != null) handle.anchoredPosition = Vector2.zero;
    }
}
