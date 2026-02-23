using UnityEngine;
using System.Collections;

public class SceneFadeIn : MonoBehaviour
{
    public CanvasGroup cg;
    public float fadeInTime = 1.0f;

    void Reset()
    {
        cg = GetComponent<CanvasGroup>();
    }

    IEnumerator Start()
    {
        if (!cg) cg = GetComponent<CanvasGroup>();
        cg.alpha = 1f;

        float t = 0f;
        while (t < fadeInTime)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, t / fadeInTime);
            yield return null;
        }
        cg.alpha = 0f;

        // Opcional: destruir el fader para que no bloquee clicks
        Destroy(gameObject);
    }
}