using System;
using UnityEngine;

public class Quiz : MonoBehaviour
{
    [SerializeField] private ClickableQuizAnswer[] quizAnswers;
    [SerializeField] private QuizSet quizSet;

    private int currentQuizIndex = 0;

    private void Start()
    {
        if (quizSet == null || quizAnswers == null || quizAnswers.Length == 0)
        {
            Debug.LogError("QuizSet or QuizAnswers not set up correctly.");
            return;
        }
        LoadQuiz(currentQuizIndex);
    }

    private void LoadQuiz(int i)
    {
        QuizData quizData = quizSet.GetQuizByIndex(i);
        if (quizData == null)
        {
            Debug.LogError("Quiz data not found for index: " + i);
            return;
        }

        QuizQuestion question = quizData.GetRandomQuestion();
        QuizAnswer[] randomizedAnswers = RandomizeAnswers(question);
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
            (randomizedAnswers[i], randomizedAnswers[randomIndex]) = (randomizedAnswers[randomIndex], randomizedAnswers[i]);
        }
        return randomizedAnswers;
    }

    private void HandleAnswerClick(bool isCorrect)
    {
        if (isCorrect)
        {
            Debug.Log("Correct answer!");
            currentQuizIndex++;
            if (currentQuizIndex < quizSet.quizzes.Length)
            {
                LoadQuiz(currentQuizIndex);
            }
            else
            {
                Debug.Log("Quiz completed!");
                // Trigger any end of quiz logic here
            }
        }
        else
        {
            Debug.Log("Incorrect answer, try again.");
            // Optionally, you can reset the quiz or provide feedback
        }
    }
}