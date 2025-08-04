using Lean.Gui;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private SlideShowController slideShowController;
    [SerializeField] private MiniGamesManager miniGamesManager;

    [SerializeField] private Transform mainRoot;

    [SerializeField] private Image logoImage;
    [SerializeField] private LeanButton startButton;
    [SerializeField] private LeanButton exitButton;

    private Sprite Logo;

    private bool isLogoLoaded = false;

    private void Awake()
    {
        Logo = Resources.Load<Sprite>($"logo");
        isLogoLoaded = Logo != null;

        if (isLogoLoaded)
        {
            logoImage.sprite = Logo;
            logoImage.SetNativeSize();
            RectTransform rt = logoImage.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
            rt.localScale = Vector3.one;
        }
        else
        {
            Debug.LogError("Logo Image component is not assigned in the inspector.");
            logoImage.gameObject.SetActive(false);
        }

        startButton.OnClick.AddListener(OnStartButtonClicked);
        exitButton.OnClick.AddListener(OnExitButtonClicked);
        slideShowController.OnSlideShowComplete += OnSlideShowComplete;
    }

    private void OnSlideShowComplete()
    {
        Debug.Log("Slide show completed. Loading the first mini-game.");
        miniGamesManager.LoadMiniGame(0);
    }

    private void OnExitButtonClicked()
    {
        bool isMainRootActive = mainRoot.gameObject.activeSelf;
        if (isMainRootActive)
        {
            //back to platform screen
            return;
        }

        mainRoot.gameObject.SetActive(true);
        slideShowController.Hide();
    }

    private void OnStartButtonClicked()
    {
        mainRoot.gameObject.SetActive(false);

        if (slideShowController != null)
        {
            slideShowController.PlaySlideShow(0); // Start the slideshow with the first set
        }
        else
        {
            Debug.LogError("SlideShowController is not assigned in the inspector.");
        }
    }
}