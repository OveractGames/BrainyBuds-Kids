using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Scriptable Objects/GameData")]
public class GameData : ScriptableObject
{
    [SerializeField] private CardData[] cards;
    public CardData[] Cards => cards;
}

[System.Serializable]
public class CardData
{
    public int cardID;
    public string cardName;
    public Sprite cardImage;
    [SerializeField] private List<ChapterInfo> chapterInfos;

    public List<ChapterInfo> ChapterInfos => chapterInfos;
}

[System.Serializable]
public class ChapterInfo
{
    public ElementType elementType;
    public AudioClip narrationAudio;
    public List<Sprite> slides;
    public string gameName;
}

public enum ElementType
{
    SlideShow = 0,
    GAME = 1
}