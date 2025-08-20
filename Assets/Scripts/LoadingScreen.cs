using System;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private UnityEngine.UI.Slider progressBar;

    public void SetVisible(bool v) => gameObject.SetActive(v);

    public void SetProgress(float p)
    {
        if (progressBar != null)
            progressBar.value = Mathf.Clamp01(p / 0.9f); // normalize 0..0.9 to 0..1
    }

    public void BeginFadeOut(System.Action onDone)
    {
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