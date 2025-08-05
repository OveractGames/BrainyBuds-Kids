using System;
using TMPro;
using UnityEngine;

public class Quiz : MonoBehaviour
{
    [SerializeField] private ClickableQuizAnswer[] quizAnswers;
    [SerializeField] private QuizSet quizSet;
    [SerializeField] private TMP_Text questionText;

    [SerializeField] private Sprite correctAnswerSprite;
    [SerializeField] private Sprite incorrectAnswerSprite;

    private int currentQuizIndex = 0;

    public event Action OnQuizCompleted;

    public void Init()
    {
        if (!quizSet || quizAnswers == null || quizAnswers.Length == 0)
        {
            Debug.LogError("QuizSet or QuizAnswers not set up correctly.");
            return;
        }

        LoadQuiz(currentQuizIndex);
    }

    public void HideQuiz()
    {
        foreach (ClickableQuizAnswer quizAnswer in quizAnswers)
        {
            quizAnswer.OnAnswerClick -= HandleAnswerClick;
        }

        gameObject.SetActive(false);
    }

    private void LoadQuiz(int i)
    {
        QuizData quizData = quizSet.GetQuizByIndex(i);
        if (!quizData)
        {
            Debug.LogError("Quiz data not found for index: " + i);
            return;
        }

        QuizQuestion question = quizData.GetRandomQuestion();
        QuizAnswer[] randomizedAnswers = RandomizeAnswers(question);
        questionText.text = question.questionText;
        for (int j = 0; j < quizAnswers.Length; j++)
        {
            quizAnswers[j].Initialize(randomizedAnswers[j].answerText, randomizedAnswers[j].isCorrect);
            quizAnswers[j].OnAnswerClick += HandleAnswerClick;
        }
    }

    private static QuizAnswer[] RandomizeAnswers(QuizQuestion question)
    {
        QuizAnswer[] randomizedAnswers = new QuizAnswer[question.answers.Length];
        Array.Copy(question.answers, randomizedAnswers, question.answers.Length);
        for (int i = 0; i < randomizedAnswers.Length; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, randomizedAnswers.Length);
            (randomizedAnswers[i], randomizedAnswers[randomIndex]) =
                (randomizedAnswers[randomIndex], randomizedAnswers[i]);
        }

        return randomizedAnswers;
    }

    private void HandleAnswerClick(bool isCorrect, ClickableQuizAnswer target)
    {
        target.SetSprite(isCorrect ? correctAnswerSprite : incorrectAnswerSprite);
        if (isCorrect)
        {
            Debug.Log("Quiz completed!");
            foreach (ClickableQuizAnswer quizAnswer in quizAnswers)
            {
                quizAnswer.LockEvents();
            }

            currentQuizIndex++;
            OnQuizCompleted?.Invoke();
        }
        else
        {
            Debug.Log("Incorrect answer, try again.");
        }
    }
}