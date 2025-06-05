using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public string SelectedLanguage { get; set; }
    public string CurrentChapterId { get; set; }
    public UserProfile CurrentUser { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

public abstract class UserProfile
{
    public string username;
    public string avatar;
    public string language;
    public int lastChapter;
}