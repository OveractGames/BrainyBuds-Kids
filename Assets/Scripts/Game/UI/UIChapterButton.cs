using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnityGame
{
    public class UIChapterButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _name;
        [SerializeField] private Image _icon;
        private Action<UIChapterButton> _clicked;

        public ChapterDefinition Definition { get; private set; }

        private void Start()
        {
            _button.onClick.AddListener(OnClicked);
        }

        private void OnClicked()
        {
            _clicked?.Invoke(this);
        }

        public void Init(ChapterDefinition chapter, Action<UIChapterButton> clicked)
        {
            _clicked = clicked;
            Definition = chapter;
            _icon.sprite = chapter.icon;
            _name.text = chapter.name;
        }
    }
}

