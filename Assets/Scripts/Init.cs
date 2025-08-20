using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Init : MonoBehaviour
{
    [SerializeField] private Button testButton;
    [SerializeField] private LoadingScreen loadingScreen;
    [SerializeField, Min(0f)] private float destroyDelaySeconds = 0.5f;

    [SerializeField, Min(0f)] private float minVisibleSeconds = 0.25f;
    // This script is used to initialize the game or application.
    // It can be used to set up initial configurations, load resources, etc.

    private void Start()
    {
        Debug.Log("Initialization complete.");
        AVProVideoController.videoFileName = "cap1part1.mp4";
        SceneLoadWatcher.Install(loadingScreen, destroyDelaySeconds, minVisibleSeconds);
        testButton.onClick.AddListener(OnTestButtonClick);
    }

    private void OnTestButtonClick()
    {
        SceneManager.LoadScene("Interface");
    }
}