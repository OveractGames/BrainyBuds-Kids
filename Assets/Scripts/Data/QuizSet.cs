using UnityEngine;

[CreateAssetMenu(fileName = "QuizSet", menuName = "Scriptable Objects/QuizSet")]
public class QuizSet : ScriptableObject
{
    public QuizData[] quizzes;
    
    public QuizData GetQuizByIndex(int index)
    {
        if (index < 0 || index >= quizzes.Length) return null;
        return quizzes[index];
    }
    
    public QuizData GetRandomQuiz()
    {
        if (quizzes == null || quizzes.Length == 0) return null;
        return quizzes[Random.Range(0, quizzes.Length)];
    }
}
