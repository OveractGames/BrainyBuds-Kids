using System;
using RenderHeads.Media.AVProVideo;
using UnityEngine;
using UnityEngine.UI;

public class StreamingAssetsVideoService: IDisposable
{
    public MediaPlayer MediaPlayer { get; private set; }
    private GameObject mediaPlayerGO;

    public StreamingAssetsVideoService(string name = "MediaPlayer (Service)")
    {
        // Create hidden GameObject to hold MediaPlayer
        mediaPlayerGO = new GameObject(name);
        MediaPlayer = mediaPlayerGO.AddComponent<MediaPlayer>();
        UnityEngine.Object.DontDestroyOnLoad(mediaPlayerGO);  // optional: persist across scenes
    }

    public void OpenFromStreamingAssets(string relativePath, bool autoPlay = true)
    {
        if (MediaPlayer == null)
            throw new InvalidOperationException("MediaPlayer not created.");

        MediaPlayer.OpenVideoFromFile(
            MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder,
            relativePath,
            autoPlay
        );
    }

    public void Play()  => MediaPlayer?.Control?.Play();
    public void Pause() => MediaPlayer?.Control?.Pause();
    public void Stop()  => MediaPlayer?.Control?.Stop();

    public void Dispose()
    {
        if (MediaPlayer != null)
        {
            MediaPlayer.CloseVideo();
        }
        if (mediaPlayerGO != null)
        {
            UnityEngine.Object.Destroy(mediaPlayerGO);
        }
        MediaPlayer = null;
        mediaPlayerGO = null;
    }
    
    public GameObject CreateFullscreenUGUIDisplay(Camera uiCamera = null)
    {
        // Canvas
        var canvasGO = new GameObject("AVPro Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        if (uiCamera != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = uiCamera;
        }

        var scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // DisplayUGUI (this is the RawImage replacement)
        var imageGO = new GameObject("AVPro DisplayUGUI", typeof(RectTransform), typeof(DisplayUGUI));
        imageGO.transform.SetParent(canvasGO.transform, false);

        var rt = imageGO.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        var display = imageGO.GetComponent<DisplayUGUI>();
        display._mediaPlayer = MediaPlayer;                // hook your MediaPlayer
       // display._scaleMode  = DisplayUGUI.ScaleMode.ScaleToFit;
       // display._color      = Color.white;

        return canvasGO;
    }


}
