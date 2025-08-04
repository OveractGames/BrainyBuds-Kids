using System;
using Lean.Gui;
using TMPro;
using UnityEngine;

public class ClickableQuizAnswer : MonoBehaviour
{
    [SerializeField] private TMP_Text answerText;
    [SerializeField] private LeanButton answerButton;

    private bool isCorrectAnswer;

    public event Action<bool> OnAnswerClick;

    public void Initialize(string text, bool isCorrect)
    {
        if (!answerText)
        {
            answerText = GetComponentInChildren<TMP_Text>();
        }

        if (answerText == null)
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

        if (answerButton == null)
        {
            Debug.LogError("Answer Button component is not assigned in the inspector.");
            return;
        }

        answerButton.OnClick.AddListener(OnAnswerClicked);
    }

    private void OnAnswerClicked()
    {
        OnAnswerClick?.Invoke(isCorrectAnswer);
    }
}