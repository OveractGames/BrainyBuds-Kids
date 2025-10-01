using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Slider progressBar;

    public bool IsFullyVisible { get; private set; } = true; // always true if no fade-in

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
        IsFullyVisible = visible; // set true immediately (or animate if needed)
    }

    public void SetProgress(float progress)
    {
        if (progressBar != null)
            progressBar.value = progress;
    }

    public void BeginFadeOut(Action onComplete)
    {
        // If you don’t need fade-out, just destroy instantly
        onComplete?.Invoke();
    }
}