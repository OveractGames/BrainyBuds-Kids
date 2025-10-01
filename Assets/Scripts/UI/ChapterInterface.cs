using System;
using UnityEngine;
using UnityEngine.UI;

public class ChapterInterface : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private GameData gameData;
    [SerializeField] private ChapterInterfaceElement[] chapterElements;

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
            ChapterInterfaceElement chapterElement = chapterElements[i];
            chapterElement.Initialize(chapterInfo);
        }

        backButton.onClick.AddListener(OnBackButtonClicked);
    }

    private static void OnBackButtonClicked()
    {
        SceneLoadWatcher.LoadScene("Interface");
    }
}