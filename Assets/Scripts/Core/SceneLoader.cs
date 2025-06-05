using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static void Load(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
