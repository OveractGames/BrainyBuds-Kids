using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnityGame
{
    public class UIOutro : MonoBehaviour
    {
        [SerializeField] private RectTransform _content;
        [SerializeField] private Button _reload;
        [SerializeField] private Button _continue;
        [SerializeField] private bool _isGood;
        [SerializeField] private TMP_Text _header;
        private int _chapterIndex;
        private int _chapterLevelIndex;

        private void Start()
        {
            _reload.onClick.AddListener(OnReloadClicked);
            _continue.onClick.AddListener(OnContinueClicked);
        }

        private void OnReloadClicked()
        {
            GameManager.Instance.RestartLevel();
        }

        private void OnContinueClicked()
        {
            GameManager.Instance.StartNextLevel();
        }

        public void Show(int chapterIndex, int chapterLevelIndex)
        {
            _chapterIndex = chapterIndex;
            _chapterLevelIndex = chapterLevelIndex;
            _header.text = $"Cap {chapterIndex}, Level {chapterLevelIndex}\r\nOutro ({(_isGood ? "GOOD" : "BAD")})";

            _content.gameObject.SetActive(transform);
        }

        public void Hide()
        {
            _content.gameObject.SetActive(false);
        }
    }
}

