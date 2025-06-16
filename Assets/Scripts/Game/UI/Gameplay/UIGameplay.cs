using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UnityGame
{
    public class UIGameplay : MonoBehaviour
    {
        [SerializeField] private RectTransform _content;
        [SerializeField] private TMP_Text _header;
        private int _chapterIndex;
        private int _chapterLevelIndex;

        public void Show(int chapterIndex, int chapterLevelIndex)
        {
            _chapterIndex = chapterIndex;
            _chapterLevelIndex = chapterLevelIndex;
            _header.text = $"Cap {chapterIndex}, Level {chapterLevelIndex}\r\nGameplay";
            _content.gameObject.SetActive(transform);
        }

        public void Hide()
        {
            _content.gameObject.SetActive(false);
        }
    }
}

