using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource jumpAudio;
    [SerializeField] private AudioSource stompAudio;

    private bool jumpSoundUsed;

    // ===== SALTO =====
    public void TryPlayJumpSound()
    {
        if (jumpSoundUsed) return;
        jumpSoundUsed = true;

        if (jumpAudio == null) return;

        jumpAudio.pitch = Random.Range(0.95f, 1.05f);
        jumpAudio.Play();
    }

    public void ResetJumpSound()
    {
        jumpSoundUsed = false;
    }

    // ===== STOMP (pisar enemigo) =====
    public void PlayStompSound()
    {
        if (stompAudio == null) return;

        stompAudio.pitch = Random.Range(0.95f, 1.05f);
        stompAudio.Play();
    }
}