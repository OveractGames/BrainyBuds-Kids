using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnityGame
{
    public class UIChapterLevelButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _name;
        [SerializeField] private Image _icon;
        private Action<UIChapterLevelButton> _clicked;

        public ChapterLevelDefinition Definition {  get; private set; }

        private void Start()
        {
            _button.onClick.AddListener(OnPlayClicked);
        }

        private void OnPlayClicked()
        {
            _clicked?.Invoke(this);
        }

        public void Init(ChapterLevelDefinition levelDef, Action<UIChapterLevelButton> clicked)
        {
            _clicked = clicked;
            Definition = levelDef;
            _icon.sprite = levelDef.icon;
            _name.text = levelDef.name;
        }
    }
}

