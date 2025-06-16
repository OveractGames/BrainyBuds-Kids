using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityGame;

namespace UnityGame
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private UIHomeScreen _homeScreen;
        [SerializeField] private UIChapterScreen _chapterScreen;
        [SerializeField] private UISplashScreen _splashScreen;

        public static UIManager Instance { get; private set; }
        public static UIHomeScreen HomeScreen { get; private set; }
        public static UIChapterScreen ChapterScreen { get; private set; }
        public static UISplashScreen SplashScreen { get; private set; }

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

            HomeScreen = _homeScreen;
            ChapterScreen = _chapterScreen;
            SplashScreen = _splashScreen;
        }
    }
}
