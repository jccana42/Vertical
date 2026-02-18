// Assets/Scripts/MobileInput.cs
using UnityEngine;

public class MobileInput : MonoBehaviour
{
    public static float Horizontal { get; private set; }
    public static bool JumpDown { get; private set; }
    public static bool JumpHeld { get; private set; }

    [Header("References")]
    [SerializeField] private VirtualJoystick joystick;
    [SerializeField] private JumpButton jumpButton;

    [Header("Debug")]
    [SerializeField] private bool logHorizontal = false;

    private void Update()
    {
        // Horizontal (joystick)
        Horizontal = joystick != null ? joystick.InputVector.x : 0f;

        // Jump (botón)
        if (jumpButton != null)
        {
            JumpDown = jumpButton.WasPressedThisFrame;
            JumpHeld = jumpButton.IsHeld;
        }
        else
        {
            JumpDown = false;
            JumpHeld = false;
        }

        // Debug opcional
        if (logHorizontal && joystick != null)
        {
            Debug.Log($"[MobileInput] Horizontal={Horizontal:0.00}  InputVector={joystick.InputVector}");
        }
    }

    // Útil si quieres asignar referencias por código desde otro script
    public void SetJoystick(VirtualJoystick vj) => joystick = vj;
    public void SetJumpButton(JumpButton jb) => jumpButton = jb;
}
