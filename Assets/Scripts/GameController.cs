using Lean.Gui;
using UnityEngine;
using UnityEngine.Serialization;

public class GameController : MonoBehaviour
{
    [SerializeField] private SlideShowController slideShowController;

    [FormerlySerializedAs("miniGamesManager")] [SerializeField]
    private FunGameManager funGameManager;

    [SerializeField] private Quiz quiz;

    [SerializeField] private MainScreen mainScreen;
    [SerializeField] private SlideShowScreen slideShowScreen;
    [SerializeField] private QuizScreen quizScreen;
    [SerializeField] private FunGameScreen funGameScreen;
    [SerializeField] private FinishScreen finishScreen;

    [SerializeField] private LeanButton startButton;
    [SerializeField] private LeanButton exitButton;

    private void Awake()
    {
        startButton.OnClick.AddListener(() => PlaySlideShow(0));
        exitButton.OnClick.AddListener(OnExitButtonClicked);
        slideShowController.OnSlideShowComplete += OnSlideShowComplete;
        quiz.OnQuizCompleted += OnQuizCompleted;
        finishScreen.OnFinish += OnContinueClick;
        funGameManager.OnFunGameFinished += OnFinish;
        mainScreen.Show();
    }

    private void OnContinueClick()
    {
        // This method is called when the continue button is clicked on the finish screen.
        mainScreen.Show();
    }

    private void OnFinish()
    {
        Debug.Log("Game finished. Showing finish screen.");
        finishScreen.Show();
    }

    private void OnQuizCompleted()
    {
        quiz.HideQuiz();
        if (!quiz.HaveMoreQuizzes())
        {
            Debug.Log("No more quizzes available. Loading the mini-game.");
            quizScreen.Hide();
            funGameManager.LoadFunGame();
            return;
        }

        quizScreen.Hide();
        PlaySlideShow(1);
    }

    private void OnSlideShowComplete()
    {
        Debug.Log("Slide show completed. Loading the quiz.");
        quizScreen.Show();
        slideShowScreen.Hide();
        quiz.Init();
    }

    private void OnExitButtonClicked()
    {
        bool isMainRootActive = mainScreen.IsVisible;
        if (isMainRootActive)
        {
            return;
        }

        mainScreen.Show();
        slideShowController.Hide();
    }

    private void PlaySlideShow(int index)
    {
        slideShowScreen.Show();
        mainScreen.Hide();

        if (slideShowController)
        {
            slideShowController.PlaySlideShow(index);
        }
        else
        {
            Debug.LogError("SlideShowController is not assigned in the inspector.");
        }
    }
}