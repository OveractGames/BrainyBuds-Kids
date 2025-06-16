using System.Collections.Generic;
using UnityEngine;

namespace UnityGame
{
    public class GameInitializer : MonoBehaviour
    {
        [SerializeField] private UISplashScreen _splashScreen;
        [SerializeField] private UIHomeScreen _homeScreen;

        private void Start()
        {
            UIManager.SplashScreen.Show();

            GameManager.Instance.Init();

            UIManager.SplashScreen.Hide();


            UIManager.HomeScreen.Show();
        }
    }
}

