using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarrativeManager : MonoBehaviour
{
    private List<Slide> slides;
    private int currentIndex;

    public void Init(string chapterId)
    {
        slides = DataLoader<SlideList>.Load("Chapters/" + chapterId).slides;
        currentIndex = 0;
        ShowSlide();
    }

    public void Next()
    {
        /* Next slide logic */
    }

    public void Previous()
    {
        /* Previous slide logic */
    }

    private void ShowSlide()
    {
        /* Update UI */
    }
}

[System.Serializable]
public class Slide
{
    public string character;
    public string text;
    public string image;
    public string voice;
}

[System.Serializable]
public class SlideList
{
    public List<Slide> slides;
}