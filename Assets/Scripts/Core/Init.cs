using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Init : MonoBehaviour
{
    [SerializeField] private LoadingScreen loadingScreen;
    [SerializeField, Min(0f)] private float destroyDelaySeconds = 0.5f;
    [SerializeField, Min(0f)] private float minVisibleSeconds = 0.25f;

    private IEnumerator Start()
    {
        SceneLoadWatcher.Install(loadingScreen, destroyDelaySeconds, minVisibleSeconds);
        yield return new WaitForSeconds(0.5f);
        SceneLoadWatcher.LoadScene("Interface");
    }
}