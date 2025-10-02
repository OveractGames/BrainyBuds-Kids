using System;
using UnityEngine;
using UnityEngine.UI;

public class ChapterInterface : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private GameData gameData;
    [SerializeField] private ChapterInterfaceElement[] chapterElements;
    [SerializeField] private Canvas chaperCanvas;

    private void Start()
    {
        if (gameData == null)
        {
            Debug.LogError("GameData reference is missing in ChapterInterface.");
            return;
        }

        Debug.Log($"Current Chapter: {Interface.clickedChapterIndex}");

        if (chapterElements.Length > gameData.Cards[Interface.clickedChapterIndex].ChapterInfos.Count)
        {
            Debug.LogError("Mismatch between chapter elements and game data chapters.");
            return;
        }

        for (int i = 0; i < chapterElements.Length; i++)
        {
            ChapterInfo chapterInfo = gameData.Cards[Interface.clickedChapterIndex].ChapterInfos[i];
            Debug.Log(chapterInfo.elementType);
            ChapterInterfaceElement chapterElement = chapterElements[i];
            chapterElement.Initialize(chapterInfo);
        }

        backButton.onClick.AddListener(OnBackButtonClicked);
        SlideShow.OnSlideShowStarted += OnSlideShowStarted;
        SlideShow.OnSlideShowComplete += OnSlideShowComplete;
    }

    private void OnSlideShowComplete()
    {
        Debug.Log("SlideShow completed, enabling chapter canvas.");
        //chaperCanvas.enabled = true;
        SlideShow.OnSlideShowStarted -= OnSlideShowStarted;
        SlideShow.OnSlideShowComplete -= OnSlideShowComplete;
    }

    private void OnSlideShowStarted()
    {
        Debug.Log("SlideShow started, disabling chapter canvas.");
        //chaperCanvas.enabled = false;
        SlideShow.OnSlideShowStarted += OnSlideShowStarted;
        SlideShow.OnSlideShowComplete += OnSlideShowComplete;
    }

    private static void OnBackButtonClicked()
    {
        SceneLoadWatcher.LoadScene("Interface");
    }
}