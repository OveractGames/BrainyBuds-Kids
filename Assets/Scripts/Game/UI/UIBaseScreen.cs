using System.Collections.Generic;
using UnityEngine;

namespace UnityGame
{
    public abstract class UIBaseScreen : MonoBehaviour
    {
        [SerializeField] private RectTransform _root;

        public void Show()
        {
            ShowInternal();
            _root.gameObject.SetActive(transform);
        }

        public void Hide()
        {
            HideInternal();
            _root.gameObject.SetActive(false);
        }

        protected virtual void ShowInternal()
        {

        }

        protected virtual void HideInternal()
        {

        }
    }
}

