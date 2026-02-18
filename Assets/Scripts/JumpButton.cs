using UnityEngine;
using UnityEngine.EventSystems;

public class JumpButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool IsHeld { get; private set; }
    public bool WasPressedThisFrame { get; private set; }

    private void LateUpdate()
    {
        // se consume cada frame
        WasPressedThisFrame = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsHeld = true;
        WasPressedThisFrame = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsHeld = false;
    }
}
