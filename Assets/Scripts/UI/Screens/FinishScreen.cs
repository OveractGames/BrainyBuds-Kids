using System;
using Lean.Gui;
using UnityEngine;

public class FinishScreen : AbstractScreen
{
    [SerializeField] private LeanButton finishButton;
    public event Action OnFinish;

    public override void Hide()
    {
        base.Hide();
    }

    public override void Show()
    {
        finishButton.OnClick.AddListener(() =>
        {
            OnFinish?.Invoke();
            Hide();
        });
        base.Show();
    }
}