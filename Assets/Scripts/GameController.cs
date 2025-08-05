using Lean.Gui;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private SlideShowController slideShowController;
    [SerializeField] private MiniGamesManager miniGamesManager;
    [SerializeField] private Quiz quiz;

    [SerializeField] private Transform mainRoot;

    [SerializeField] private LeanButton startButton;
    [SerializeField] private LeanButton exitButton;

    private void Awake()
    {
        startButton.OnClick.AddListener( () => PlaySlideShow(0));
        exitButton.OnClick.AddListener(OnExitButtonClicked);
        slideShowController.OnSlideShowComplete += OnSlideShowComplete;
        quiz.OnQuizCompleted += OnQuizCompleted;
    }

    private void OnQuizCompleted()
    {
        quiz.HideQuiz();
        PlaySlideShow(1);
    }

    private void OnSlideShowComplete()
    {
        Debug.Log("Slide show completed. Loading the quiz.");
        quiz.gameObject.SetActive(true);
        quiz.Init();
    }

    private void OnExitButtonClicked()
    {
        bool isMainRootActive = mainRoot.gameObject.activeSelf;
        if (isMainRootActive)
        {
            return;
        }

        mainRoot.gameObject.SetActive(true);
        slideShowController.Hide();
    }

    private void PlaySlideShow(int index)
    {
        mainRoot.gameObject.SetActive(false);

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