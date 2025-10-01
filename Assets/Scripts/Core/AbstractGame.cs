using UnityEngine;
using UnityEngine.UI;

public abstract class AbstractGame : MonoBehaviour
{
    [SerializeField] private Button backButton;

    protected virtual void Start()
    {
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
        }
    }

    protected abstract void OnBackButtonClicked();
    public abstract void StartGame();
    public abstract void EndGame();
}