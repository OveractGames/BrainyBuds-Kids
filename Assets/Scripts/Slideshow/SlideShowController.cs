using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class SlideShowController : MonoBehaviour
{
    [Header("SlideShow Sources")]
    public SlideShowData slideShowData;

    [Header("UI & Audio")]
    public Image slideImageA;
    public Image slideImageB;
    public AudioSource audioSource;

    [Header("Crossfade")]
    public float fadeDuration = 0.5f;

    [Header("Ken Burns (Zoom/Pan)")]
    [Tooltip("Start scale for each slide (e.g., 0.98)")]
    public float zoomFrom = 0.98f;
    [Tooltip("End scale for each slide (e.g., 1.06)")]
    public float zoomTo = 1.06f;
    [Tooltip("Swap from→to every slide for variety")]
    public bool alternateZoomDirection = true;

    private SlideSet currentSet;
    private float perSlideDuration;
    private bool useImageA = true;
    
    [Header("Ken Burns (Zoom/Pan)")]
    [Min(1f)] public float maxScale = 1.08f;      // upper bound; min is clamped to 1
    public bool randomizeDirection = true;        // random in/out per slide
    public bool enablePan = true;
    public Vector2 panMax = new Vector2(30f, 18f);


    public event Action OnSlideShowComplete;

    // --- Public API ----------------------------------------------------------

    public void Hide() => ResetSlideShow();

    public void PlaySlideShow(int setIndex)
    {
        ResetSlideShow();

        if (!slideShowData || setIndex < 0 || setIndex >= slideShowData.slideSets.Count)
        {
            Debug.LogError("Invalid SlideShowData or set index.");
            return;
        }

        currentSet = slideShowData.slideSets[setIndex];

        if (currentSet.narrationAudio == null || currentSet.slides == null || currentSet.slides.Count == 0)
        {
            Debug.LogError("Slide set is incomplete.");
            return;
        }

        perSlideDuration = currentSet.narrationAudio.length / currentSet.slides.Count;
        audioSource.clip = currentSet.narrationAudio;

        PrepareImage(slideImageA);
        PrepareImage(slideImageB);
        slideImageA.gameObject.SetActive(true);
        slideImageB.gameObject.SetActive(true);

        StartCoroutine(CrossFadeKenBurns());
        audioSource.Play();
    }

    // --- Internals -----------------------------------------------------------

    private void PrepareImage(Image img)
    {
        if (!img) return;
        img.color = new Color(1, 1, 1, 0);
        var rt = img.rectTransform;
        rt.localScale = Vector3.one;
        rt.anchoredPosition = Vector2.zero;
        img.DOKill(); // ensure no leftover tweens
        rt.DOKill();
    }

    private void ResetSlideShow()
    {
        StopAllCoroutines();
        if (audioSource) audioSource.Stop();

        PrepareImage(slideImageA);
        PrepareImage(slideImageB);

        if (slideImageA) slideImageA.gameObject.SetActive(false);
        if (slideImageB) slideImageB.gameObject.SetActive(false);

        useImageA = true;
    }

private IEnumerator CrossFadeKenBurns()
{
    foreach (var sprite in currentSet.slides)
    {
        Image inImg  = useImageA ? slideImageA : slideImageB;
        Image outImg = useImageA ? slideImageB : slideImageA;

        // Kill any running tweens on both images
        inImg.DOKill();  inImg.rectTransform.DOKill();
        outImg.DOKill(); outImg.rectTransform.DOKill();

        // Set incoming sprite & initial state
        inImg.sprite = sprite;
        inImg.color  = new Color(1, 1, 1, 0);

        // Decide zoom direction (never < 1)
        bool zoomOut = randomizeDirection ? (UnityEngine.Random.value < 0.5f) : false;
        float startScale = zoomOut ? Mathf.Max(1f, maxScale) : 1f;   // max→1  or  1→max
        float endScale   = zoomOut ? 1f : Mathf.Max(1f, maxScale);

        // Optional gentle pan
        Vector2 pan = Vector2.zero;
        if (enablePan)
        {
            pan = new Vector2(
                UnityEngine.Random.Range(-panMax.x, panMax.x),
                UnityEngine.Random.Range(-panMax.y, panMax.y)
            );
        }

        // Prepare transforms
        var rtIn  = inImg.rectTransform;
        var rtOut = outImg.rectTransform;

        rtIn.localScale       = Vector3.one * startScale;
        rtIn.anchoredPosition = -pan * 0.5f; // start opposite so it travels toward +pan

        // Crossfade
        inImg.DOFade(1f, fadeDuration).SetEase(Ease.InOutSine);
        outImg.DOFade(0f, fadeDuration).SetEase(Ease.InOutSine);

        // Ken Burns on incoming image across full display
        rtIn.DOScale(endScale, perSlideDuration).SetEase(Ease.Linear);
        rtIn.DOAnchorPos(pan * 0.5f, perSlideDuration).SetEase(Ease.Linear);

        // Optional subtle settle on the outgoing image
        rtOut.DOScale(Mathf.Max(1f, maxScale * 0.99f), fadeDuration).SetEase(Ease.InOutSine);

        // Wait for the slide's time
        yield return new WaitForSeconds(perSlideDuration);
        useImageA = !useImageA;
    }

    // Fade out at the end
    slideImageA.DOFade(0f, fadeDuration);
    slideImageB.DOFade(0f, fadeDuration);
    slideImageA.rectTransform.DOScale(1.02f, fadeDuration);
    slideImageB.rectTransform.DOScale(1.02f, fadeDuration);

    yield return new WaitForSeconds(fadeDuration);
    OnSlideShowComplete?.Invoke();
}

}
