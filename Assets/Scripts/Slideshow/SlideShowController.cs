using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class SlideShowController : MonoBehaviour
{
    [Header("SlideShow Sources")] public SlideShowData slideShowData;

    [Header("UI & Audio")] public Image slideImageA;
    public Image slideImageB;
    public AudioSource audioSource;

    [Header("Crossfade")] public float fadeDuration = 0.5f;

    private SlideSet currentSet;
    private float perSlideDuration;
    private bool useImageA = true;

    [Header("Ken Burns (Zoom/Pan)")] [Min(1f)]
    public float maxScale = 1.08f;

    public bool randomizeDirection = true;
    public bool enablePan = true;
    public Vector2 panMax = new Vector2(30f, 18f);

    public event Action OnSlideShowComplete;

    public void Start()
    {
        Create(0, "cap1game1");
    }

    public void Hide() => ResetSlideShow();

    public void Create(int storyIndex, string gameSceneName)
    {
        ResetSlideShow();

        if (!slideShowData || storyIndex < 0 || storyIndex >= slideShowData.slideSets.Count)
        {
            Debug.LogError("Invalid SlideShowData or set index.");
            return;
        }

        currentSet = slideShowData.slideSets[storyIndex];

        if (!currentSet.narrationAudio || currentSet.slides == null || currentSet.slides.Count == 0)
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

    private static void PrepareImage(Image img)
    {
        if (!img)
        {
            return;
        }

        img.color = new Color(1, 1, 1, 0);
        RectTransform rt = img.rectTransform;
        rt.localScale = Vector3.one;
        rt.anchoredPosition = Vector2.zero;
        img.DOKill();
        rt.DOKill();
    }

    private void ResetSlideShow()
    {
        StopAllCoroutines();
        if (audioSource)
        {
            audioSource.Stop();
        }

        PrepareImage(slideImageA);
        PrepareImage(slideImageB);
        if (slideImageA)
        {
            slideImageA.gameObject.SetActive(false);
        }

        if (slideImageB)
        {
            slideImageB.gameObject.SetActive(false);
        }

        useImageA = true;
    }

    private IEnumerator CrossFadeKenBurns()
    {
        foreach (Sprite sprite in currentSet.slides)
        {
            Image inImg = useImageA ? slideImageA : slideImageB;
            Image outImg = useImageA ? slideImageB : slideImageA;

            inImg.DOKill();
            inImg.rectTransform.DOKill();
            outImg.DOKill();
            outImg.rectTransform.DOKill();

            inImg.sprite = sprite;
            inImg.color = new Color(1, 1, 1, 0);

            bool zoomOut = randomizeDirection && (UnityEngine.Random.value < 0.5f);
            float startScale = zoomOut ? Mathf.Max(1f, maxScale) : 1f;
            float endScale = zoomOut ? 1f : Mathf.Max(1f, maxScale);

            Vector2 pan = Vector2.zero;
            if (enablePan)
            {
                pan = new Vector2(
                    UnityEngine.Random.Range(-panMax.x, panMax.x),
                    UnityEngine.Random.Range(-panMax.y, panMax.y)
                );
            }

            RectTransform rtIn = inImg.rectTransform;
            RectTransform rtOut = outImg.rectTransform;

            rtIn.localScale = Vector3.one * startScale;
            rtIn.anchoredPosition = -pan * 0.5f;

            inImg.DOFade(1f, fadeDuration).SetEase(Ease.InOutSine);
            outImg.DOFade(0f, fadeDuration).SetEase(Ease.InOutSine);

            rtIn.DOScale(endScale, perSlideDuration).SetEase(Ease.Linear);
            rtIn.DOAnchorPos(pan * 0.5f, perSlideDuration).SetEase(Ease.Linear);

            rtOut.DOScale(Mathf.Max(1f, maxScale * 0.99f), fadeDuration).SetEase(Ease.InOutSine);

            yield return new WaitForSeconds(perSlideDuration);
            useImageA = !useImageA;
        }

        slideImageA.DOFade(0f, fadeDuration);
        slideImageB.DOFade(0f, fadeDuration);
        slideImageA.rectTransform.DOScale(1.02f, fadeDuration);
        slideImageB.rectTransform.DOScale(1.02f, fadeDuration);

        yield return new WaitForSeconds(fadeDuration);
        OnSlideShowComplete?.Invoke();
    }
}