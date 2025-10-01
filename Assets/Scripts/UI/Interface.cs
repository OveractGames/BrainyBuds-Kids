using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Interface : MonoBehaviour
{
    [SerializeField] private GameData gameData;
    [SerializeField] private Transform chaptersContainer;
    [SerializeField] private ChapterCard[] chapterCards;

    public static int clickedChapterIndex;

    private void Start()
    {
        CreateChapterCards();
    }

    private void CreateChapterCards()
    {
        if (chapterCards.Length > gameData.Cards.Length)
        {
            Debug.LogError("Mismatch between chapter cards and game data cards.");
            return;
        }

        for (int index = 0; index < chapterCards.Length; index++)
        {
            CardData chapter = gameData.Cards[index];
            chapter.cardID = index;
            ChapterCard chapterCard = chapterCards[index];
            chapterCard.Initialize(chapter, OnChapterCardClicked);
        }
    }

    private static void OnChapterCardClicked(int chapterIndex)
    {
        clickedChapterIndex = chapterIndex;
        SceneLoadWatcher.LoadScene("ChapterInterface");
    }
}