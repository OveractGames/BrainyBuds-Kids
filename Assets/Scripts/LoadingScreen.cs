using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private SpriteRenderer backgroundImage;
    [SerializeField] private GameObject animationObject;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private UnityEngine.UI.Slider progressBar;

    public void SetVisible(bool v)
    {
        gameObject.SetActive(v);
        StartCoroutine(ShowDelayed(0.1f));
    }

    private IEnumerator ShowDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        animationObject.SetActive(true);
    }

    private void OnDestroy()
    {
        backgroundImage.DOKill();
    }

    public void SetProgress(float p)
    {
        if (progressBar)
            progressBar.value = Mathf.Clamp01(p / 0.9f); // normalize 0..0.9 to 0..1
    }

    public void BeginFadeOut(System.Action onDone)
    {
        backgroundImage.DOFade(0f, 0.25f).SetEase(Ease.Linear);
        StartCoroutine(FadeOutRoutine(onDone));
    }

    private System.Collections.IEnumerator FadeOutRoutine(System.Action onDone)
    {
        float t = 0f;
        while (t < 0.2f) // quick fade
        {
            t += Time.unscaledDeltaTime;
            if (canvasGroup) canvasGroup.alpha = 1f - (t / 0.2f);
            yield return null;
        }

        onDone?.Invoke();
    }
}