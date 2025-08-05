using System;
using Lean.Gui;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClickableQuizAnswer : MonoBehaviour
{
    [SerializeField] private TMP_Text answerText;
    [SerializeField] private LeanButton answerButton;
    [SerializeField] private Image answerImage;

    private bool isCorrectAnswer;

    public event Action<bool, ClickableQuizAnswer> OnAnswerClick;

    private bool _canClick = true;

    public void Initialize(string text, bool isCorrect)
    {
        if (!answerText)
        {
            answerText = GetComponentInChildren<TMP_Text>();
        }

        if (!answerText)
        {
            Debug.LogError("Answer Text component is not assigned in the inspector.");
            return;
        }

        answerText.text = text;
        isCorrectAnswer = isCorrect;

        if (!answerButton)
        {
            answerButton = GetComponentInChildren<LeanButton>();
        }

        if (!answerButton)
        {
            Debug.LogError("Answer Button component is not assigned in the inspector.");
            return;
        }

        answerButton.OnClick.AddListener(OnAnswerClicked);
    }

    public void LockEvents()
    {
        _canClick = false;
    }

    public void SetSprite(Sprite sp)
    {
        if (!answerButton)
        {
            Debug.LogError("Answer Button component is not assigned in the inspector.");
            return;
        }

        enabled = false;
        _canClick = false;
        answerImage.sprite = sp;
    }

    private void OnAnswerClicked()
    {
        if (!_canClick)
        {
            return;
        }

        OnAnswerClick?.Invoke(isCorrectAnswer, this);
    }
}