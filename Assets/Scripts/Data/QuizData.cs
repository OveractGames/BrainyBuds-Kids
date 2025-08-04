using UnityEngine;

[CreateAssetMenu(fileName = "QuizData", menuName = "Scriptable Objects/QuizData")]
public class QuizData : ScriptableObject
{
    public QuizQuestion[] questions;

    public QuizQuestion GetRandomQuestion()
    {
        if (questions == null || questions.Length == 0) return null;
        return questions[Random.Range(0, questions.Length)];
    }
}

[System.Serializable]
public class QuizQuestion
{
    public int questionID;
    [TextArea] public string questionText;
    public QuizAnswer[] answers;

    public int GetCorrectAnswerIndex()
    {
        for (int i = 0; i < answers.Length; i++)
        {
            if (answers[i].isCorrect)
                return i;
        }

        return -1;
    }
}

[System.Serializable]
public class QuizAnswer
{
    public string answerText;
    public bool isCorrect;
}