using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroCutsceneController : MonoBehaviour
{
    [Header("UI Refs")]
    public RectTransform imageART;
    public CanvasGroup imageACG;

    public RectTransform imageBRT;
    public CanvasGroup imageBCG;

    public RectTransform fadeBlackRT;
    public CanvasGroup fadeBlackCG;

    public Button btnSkip;

    [Header("Sprites (A..F)")]
    public Sprite[] slides = new Sprite[6];

    [Header("Timings")]
    public float holdTime = 1.6f;
    public float crossFadeTime = 1.2f;

    public float finalZoomTime = 2.0f;
    public float finalZoomScale = 1.35f;
    public float finalFadeToBlackTime = 1.2f;

    [Header("Extra Fades")]
    public float startFadeInTime = 1.0f;   // Fade-in inicial (negro -> imagen)
    public float skipFadeOutTime = 0.6f;   // Fade-out al hacer Skip (imagen -> negro)

    [Header("Final Zoom Focus (child of the active image)")]
    public RectTransform planeFocus;

    [Header("Next Scene")]
    public string nextSceneName = "Level_01";

    // Internals
    private Image _imgA;
    private Image _imgB;
    private Coroutine _mainRoutine;
    private bool _skipping;

    void Awake()
    {
        if (imageART) _imgA = imageART.GetComponent<Image>();
        if (imageBRT) _imgB = imageBRT.GetComponent<Image>();

        if (btnSkip)
        {
            btnSkip.onClick.RemoveListener(Skip);
            btnSkip.onClick.AddListener(Skip);
        }
    }

    void Start()
    {
        // Validaciones básicas
        if (!_imgA || !_imgB || !imageACG || !imageBCG || !fadeBlackCG)
        {
            Debug.LogError("[IntroCutsceneController] Faltan referencias (Image/CanvasGroup/FadeBlack). Revisa el Inspector.");
            return;
        }

        _mainRoutine = StartCoroutine(MainRoutine());
    }

    public void Skip()
    {
        if (_skipping) return;
        _skipping = true;

        if (btnSkip) btnSkip.interactable = false;

        // Para SOLO la rutina principal (no mates todas las coroutines)
        if (_mainRoutine != null) StopCoroutine(_mainRoutine);

        StartCoroutine(SkipRoutine());
    }

    private IEnumerator MainRoutine()
    {
        // Estado inicial
        imageACG.alpha = 1f;
        imageBCG.alpha = 0f;

        // Negro por encima al inicio
        fadeBlackCG.alpha = 1f;
        fadeBlackCG.blocksRaycasts = true;
        fadeBlackCG.interactable = false;

        // Primera slide en A
        if (slides != null && slides.Length > 0 && slides[0] != null)
            _imgA.sprite = slides[0];

        // Asegura escala/posición base
        ResetRect(imageART);
        ResetRect(imageBRT);

        // Fade-in inicial (negro -> imagen)
        yield return FadeCanvasGroup(fadeBlackCG, 1f, 0f, startFadeInTime);
        fadeBlackCG.blocksRaycasts = false;

        // Reproducir slides 0..N-1 con crossfade A<->B
        int count = (slides == null) ? 0 : slides.Length;
        count = Mathf.Clamp(count, 0, 999);

        // Si no hay sprites, sal igualmente
        if (count == 0)
        {
            yield return EndAndLoad();
            yield break;
        }

        RectTransform activeRT = imageART;
        CanvasGroup activeCG = imageACG;
        Image activeImg = _imgA;

        RectTransform nextRT = imageBRT;
        CanvasGroup nextCG = imageBCG;
        Image nextImg = _imgB;

        for (int i = 0; i < count; i++)
        {
            // Mantener en pantalla
            yield return new WaitForSeconds(holdTime);

            // Si hay siguiente, crossfade
            if (i < count - 1)
            {
                if (slides[i + 1] != null)
                    nextImg.sprite = slides[i + 1];

                // Prepara next
                ResetRect(nextRT);
                nextCG.alpha = 0f;

                // Crossfade
                yield return CrossFade(activeCG, nextCG, crossFadeTime);

                // Swap (active <-> next)
                (activeRT, nextRT) = (nextRT, activeRT);
                (activeCG, nextCG) = (nextCG, activeCG);
                (activeImg, nextImg) = (nextImg, activeImg);
            }
        }

        // Zoom final sobre el avión (planeFocus)
        // Importante: planeFocus debe ser HIJO de la imagen activa en ese momento
        if (planeFocus != null && finalZoomTime > 0f && finalZoomScale > 0f)
        {
            // Si planeFocus no es hijo del activeRT, el “centrado” puede fallar.
            yield return ZoomToFocus(activeRT, planeFocus, finalZoomScale, finalZoomTime);
        }

        // Fade-out a negro al terminar y carga escena
        yield return EndAndLoad();
    }

    private IEnumerator SkipRoutine()
    {
        // Bloquea clicks durante el fade
        fadeBlackCG.blocksRaycasts = true;

        // Fade-out a negro desde el estado actual
        yield return FadeCanvasGroup(fadeBlackCG, fadeBlackCG.alpha, 1f, skipFadeOutTime);

        SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator EndAndLoad()
    {
        fadeBlackCG.blocksRaycasts = true;
        yield return FadeCanvasGroup(fadeBlackCG, fadeBlackCG.alpha, 1f, finalFadeToBlackTime);
        SceneManager.LoadScene(nextSceneName);
    }

    // -------- Helpers --------

    private static void ResetRect(RectTransform rt)
    {
        if (!rt) return;
        rt.localScale = Vector3.one;
        rt.anchoredPosition = Vector2.zero;
        rt.localRotation = Quaternion.identity;
    }

    private static IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float time)
    {
        if (!cg) yield break;

        if (time <= 0f)
        {
            cg.alpha = to;
            yield break;
        }

        cg.alpha = from;
        float t = 0f;

        while (t < time)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, t / time);
            yield return null;
        }

        cg.alpha = to;
    }

    private static IEnumerator CrossFade(CanvasGroup fromCG, CanvasGroup toCG, float time)
    {
        if (!fromCG || !toCG)
            yield break;

        if (time <= 0f)
        {
            fromCG.alpha = 0f;
            toCG.alpha = 1f;
            yield break;
        }

        float t = 0f;
        float fromStart = fromCG.alpha;
        float toStart = toCG.alpha;

        while (t < time)
        {
            t += Time.deltaTime;
            float k = t / time;
            fromCG.alpha = Mathf.Lerp(fromStart, 0f, k);
            toCG.alpha = Mathf.Lerp(toStart, 1f, k);
            yield return null;
        }

        fromCG.alpha = 0f;
        toCG.alpha = 1f;
    }

    private IEnumerator ZoomToFocus(RectTransform imageRT, RectTransform focusRT, float targetScale, float time)
    {
        if (!imageRT || !focusRT) yield break;

        // Canvas para encontrar el “centro” real en UI
        Canvas canvas = imageRT.GetComponentInParent<Canvas>();
        RectTransform canvasRT = canvas ? canvas.transform as RectTransform : null;

        Vector3 startScale = imageRT.localScale;
        Vector3 endScale = Vector3.one * targetScale;

        float t = 0f;

        while (t < time)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / time);

            imageRT.localScale = Vector3.Lerp(startScale, endScale, k);

            // Intento de mantener el focus en el centro del canvas/pantalla
            Vector3 centerWorld;
            if (canvasRT != null)
                centerWorld = canvasRT.TransformPoint(canvasRT.rect.center);
            else
                centerWorld = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);

            Vector3 delta = centerWorld - focusRT.position;
            imageRT.position += delta;

            yield return null;
        }

        imageRT.localScale = endScale;
    }
}