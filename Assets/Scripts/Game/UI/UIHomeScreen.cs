using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityGame
{
    public class UIHomeScreen : UIBaseScreen
    {
        [SerializeField] private UIChapterButton _chapterButtonPrefab;
        [SerializeField] private RectTransform _chaptersContainer;
        private List<UIChapterButton> _spawnedSlots = new();

        private void OnChapterClicked(UIChapterButton chapter)
        {
            Hide();

            UIManager.ChapterScreen.Show(chapter.Definition.index);
        }

        protected override void ShowInternal()
        {
            foreach(var chapterData in GameManager.Instance.GetAllChaptersData())
            {
                UIChapterButton chapterSlot = Instantiate(_chapterButtonPrefab, _chaptersContainer);
                chapterSlot.Init(chapterData, OnChapterClicked);
                _spawnedSlots.Add(chapterSlot);
            }
        }

        protected override void HideInternal()
        {
            foreach (var slot in _spawnedSlots)
            {
                Destroy(slot.gameObject);
            }
            _spawnedSlots.Clear();
        }
    }
}

