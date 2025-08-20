using UnityEngine;

public class TestVideoService : MonoBehaviour
{
    private StreamingAssetsVideoService videoService;

    void Start()
    {
        videoService = new StreamingAssetsVideoService();

        // Play intro.mp4 from StreamingAssets
        videoService.CreateFullscreenUGUIDisplay();
        videoService.OpenFromStreamingAssets("logo.mp4", true);

        Debug.Log("Video should be playing now.");
    }

    void OnDestroy()
    {
        videoService?.Dispose();
    }
}
