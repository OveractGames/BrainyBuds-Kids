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

    [Header("Animation")] public float fadeDuration = 0.5f;

    private SlideSet currentSet;
    private float slideDuration;

    private bool useImageA = true;

    public event Action OnSlideShowComplete;

    public void Hide()
    {
        ResetSlideShow();
    }

    private void ResetSlideShow()
    {
        slideImageA.color = new Color(1, 1, 1, 0);
        slideImageB.color = new Color(1, 1, 1, 0);
        slideImageA.gameObject.SetActive(false);
        slideImageB.gameObject.SetActive(false);
        audioSource.Stop();
        useImageA = true;
        StopAllCoroutines();
    }

    public void PlaySlideShow(int setIndex)
    {
        ResetSlideShow();
        if (slideShowData == null || setIndex < 0 || setIndex >= slideShowData.slideSets.Count)
        {
            Debug.LogError("Invalid SlideShowData or set index.");
            return;
        }

        currentSet = slideShowData.slideSets[setIndex];

        if (currentSet.narrationAudio == null || currentSet.slides.Count == 0)
        {
            Debug.LogError("Slide set is incomplete.");
            return;
        }

        slideDuration = currentSet.narrationAudio.length / currentSet.slides.Count;
        audioSource.clip = currentSet.narrationAudio;

        slideImageA.color = new Color(1, 1, 1, 0);
        slideImageB.color = new Color(1, 1, 1, 0);
        slideImageA.gameObject.SetActive(true);
        slideImageB.gameObject.SetActive(true);

        StartCoroutine(CrossFade());
        audioSource.Play();
    }

    private IEnumerator CrossFade()
    {
        foreach (Sprite t in currentSet.slides)
        {
            Image fadeInImage = useImageA ? slideImageA : slideImageB;
            Image fadeOutImage = useImageA ? slideImageB : slideImageA;
            fadeInImage.sprite = t;
            fadeInImage.DOFade(1f, fadeDuration);
            fadeOutImage.DOFade(0f, fadeDuration);
            yield return new WaitForSeconds(slideDuration);
            useImageA = !useImageA;
        }

        slideImageA.DOFade(0f, fadeDuration);
        slideImageB.DOFade(0f, fadeDuration);
        yield return new WaitForSeconds(fadeDuration);
        OnSlideShowComplete.Invoke();
    }
}