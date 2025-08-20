using UnityEngine;

public class GlobalController : MonoBehaviour
{
    [SerializeField] private LoadingScreen loadingScreen;
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    
}
