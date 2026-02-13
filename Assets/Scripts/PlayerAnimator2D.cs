using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerAnimator2D : MonoBehaviour
{
    [Header("Facing")]
    [Tooltip("Marca esto si tu sprite/animación por defecto mira a la IZQUIERDA.")]
    [SerializeField] private bool spriteFacesLeftByDefault = true;

    private Animator anim;
    private SpriteRenderer sr;
    private PlayerMovement2D move;

    private void Awake()
    {
        anim = GetComponent<Animator>();

        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr == null) sr = GetComponent<SpriteRenderer>();

        move = GetComponent<PlayerMovement2D>();
        if (move == null)
            Debug.LogWarning("No encuentro PlayerMovement2D en el Player. Añádelo al mismo GameObject.");
    }

    private void Update()
    {
        if (move == null) return;

        float xVel = move.XVel;
        float yVel = move.YVel;

        anim.SetBool("Grounded", move.IsGrounded);
        anim.SetFloat("Speed", Mathf.Abs(xVel));
        anim.SetFloat("YVel", yVel);

        // ✅ Necesitas tener este parámetro creado en el Animator (Bool JumpHeld)
        anim.SetBool("JumpHeld", move.JumpHeld);

        // Flip estable
        if (sr != null)
        {
            if (xVel > 0.01f)
                sr.flipX = spriteFacesLeftByDefault ? true : false;
            else if (xVel < -0.01f)
                sr.flipX = spriteFacesLeftByDefault ? false : true;
        }
    }
}
