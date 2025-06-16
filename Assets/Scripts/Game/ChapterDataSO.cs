using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityGame
{
    [Serializable]
    public class ChapterDefinition
    {
        public int index;
        public string name;
        public Sprite icon;
        public List<ChapterLevelDefinition> levels = new List<ChapterLevelDefinition>();
    }


    [Serializable]
    public class ChapterLevelDefinition
    {
        public int index;
        public string name;
        public Sprite icon;
    }

    
    [CreateAssetMenu(fileName = "ChaptersData", menuName = "--Overact Games--/Chapters Data", order = 1)]
    public class ChapterDataSO : ScriptableObject
    {
        public List<ChapterDefinition> chapters = new List<ChapterDefinition>();
    }
}

