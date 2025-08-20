using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Interface : MonoBehaviour
{
    public Button testButton;
    private void Start()
    {
        testButton.onClick.AddListener(OnTestButtonClick);
    }

    private void OnTestButtonClick()
    {
        SceneManager.LoadScene("Init");
    }
}
