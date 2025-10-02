using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlideShow : MonoBehaviour
{
    [SerializeField] private Image slideImage1;
    [SerializeField] private Image slideImage2;
    [SerializeField] private AudioSource audioSource;
    private int currentSlideIndex = 0;
    private bool usingFirstImage = true;
    public static event Action OnSlideShowComplete;
    public static event Action OnSlideShowStarted;

    public static void Create(ChapterInfo chapterInfo, Transform parent)
    {
        GameObject slideShowObject = new("SlideShow");
        SlideShow slideShow = slideShowObject.AddComponent<SlideShow>();
        slideShow.Initialize(chapterInfo);
    }

    private void Initialize(ChapterInfo chapterInfo)
    {
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        gameObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        gameObject.AddComponent<GraphicRaycaster>();
        GameObject img1Obj = new("SlideImage1");
        img1Obj.transform.SetParent(transform, false);
        slideImage1 = img1Obj.AddComponent<Image>();
        SetFullScreen(slideImage1.rectTransform);
        slideImage1.canvasRenderer.SetAlpha(1f);
        GameObject img2Obj = new("SlideImage2");
        img2Obj.transform.SetParent(transform, false);
        slideImage2 = img2Obj.AddComponent<Image>();
        SetFullScreen(slideImage2.rectTransform);
        slideImage2.canvasRenderer.SetAlpha(0f);
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = chapterInfo.narrationAudio;
        audioSource.playOnAwake = false;
        if (chapterInfo.slides is { Count: > 0 })
        {
            StartCoroutine(PlaySlideshow(chapterInfo.slides));
        }

        OnSlideShowStarted?.Invoke();
    }

    private static void SetFullScreen(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private IEnumerator PlaySlideshow(List<Sprite> slides)
    {
        audioSource.Play();
        currentSlideIndex = 0;
        slideImage1.sprite = slides[currentSlideIndex];
        float totalTime = audioSource.clip.length;
        float perSlideDuration = totalTime / slides.Count;
        while (currentSlideIndex < slides.Count - 1)
        {
            yield return new WaitForSeconds(perSlideDuration);
            currentSlideIndex++;
            yield return StartCoroutine(CrossFadeToSlide(slides[currentSlideIndex]));
        }

        yield return new WaitWhile(() => audioSource.isPlaying);
        OnSlideShowComplete?.Invoke();
        Debug.Log("SlideShow complete.");
        Destroy(gameObject);
    }

    private IEnumerator CrossFadeToSlide(Sprite newSlide)
    {
        Image activeImage = usingFirstImage ? slideImage1 : slideImage2;
        Image nextImage = usingFirstImage ? slideImage2 : slideImage1;
        nextImage.sprite = newSlide;
        nextImage.canvasRenderer.SetAlpha(0f);
        nextImage.CrossFadeAlpha(1f, 1f, false);
        activeImage.CrossFadeAlpha(0f, 1f, false);
        usingFirstImage = !usingFirstImage;
        yield return new WaitForSeconds(1f);
    }
}