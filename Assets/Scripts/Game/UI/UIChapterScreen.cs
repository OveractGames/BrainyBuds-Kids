using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnityGame
{
    public class UIChapterScreen : UIBaseScreen
    {
       // [SerializeField] private UIHomeScreen _homeScreen;
       [SerializeField] private UIChapterLevelButton _chapterLevelPrefab;
        [SerializeField] private RectTransform _chapterLevelsContainer;
        [SerializeField] private Button _back;
        private List<UIChapterLevelButton> _spawnedSlots = new();

        private int _chapterIndex;

        private void Start()
        {
            _back.onClick.AddListener(OnBackClicked);
        }

        private void OnBackClicked()
        {
            Hide();
            UIManager.HomeScreen.Show();
        }

        private void OnChapterClicked(UIChapterLevelButton level)
        {
            GameManager.Instance.StartLevel(_chapterIndex, level.Definition.index, () =>
            {
                Hide();
            });
        }

        public void Show(int chapterIndex)
        {
            _chapterIndex = chapterIndex;

            var chapterData = GameManager.Instance.GetChapterData(chapterIndex);

            int index = 0;
            foreach (var elem in chapterData.levels)
            {
                UIChapterLevelButton chapterLevelSlot = Instantiate(_chapterLevelPrefab, _chapterLevelsContainer);
                chapterLevelSlot.Init(elem, OnChapterClicked);
                _spawnedSlots.Add(chapterLevelSlot);
                ++index;
            }

            base.Show();
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

