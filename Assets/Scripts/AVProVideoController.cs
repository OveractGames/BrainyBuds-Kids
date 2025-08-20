using System.Collections.Generic;
using System.Linq;
using RenderHeads.Media.AVProVideo;
using UnityEngine;
using UnityEngine.UI;

public class AVProVideoController : MonoBehaviour
{
    public MediaPlayer mediaPlayer;
    public DisplayUGUI mediaDisplay;

    public Slider videoSeekSlider;
    public Slider audioVolumeSlider;
    public Toggle autoStartToggle;
    public Toggle muteToggle;

    [SerializeField] private Button playButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button rewindButton;
    [SerializeField] private Button continueButton;

    [SerializeField] private float maxInactiveTime = 5f;

    public RectTransform bufferedSliderRect;

    public MediaPlayer.FileLocation location = MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder;

    private bool wasPlayingOnScrub = false;
    private float cachedSeekSliderValue = -1f;
    private float cachedVolumeSliderValue = -1f;
    private Image bufferedSliderImage;

    [SerializeField] float moveThreshold = 0.5f;
    private Vector3 _lastMousePos;

    private List<GameObject> _controls = new List<GameObject>();

    private void Awake()
    {
        if (mediaPlayer != null)
        {
            Debug.Log("MediaPlayer is set in Awake.");
            mediaPlayer.Events.AddListener(OnVideoEvent);
        }

        _lastMousePos = Input.mousePosition;
    }

    public static string videoFileName = "logo.mp4";

    private void Start()
    {
        _controls = new List<GameObject>
        {
            playButton?.gameObject,
            pauseButton?.gameObject,
            rewindButton?.gameObject,
            continueButton?.gameObject,
            videoSeekSlider?.gameObject,
            audioVolumeSlider?.gameObject,
            autoStartToggle?.gameObject,
            muteToggle?.gameObject,
        };
        if (mediaDisplay != null && mediaPlayer != null)
        {
            mediaDisplay.CurrentMediaPlayer = mediaPlayer;
        }

        if (bufferedSliderRect != null)
        {
            bufferedSliderImage = bufferedSliderRect.GetComponent<Image>();
        }

        audioVolumeSlider.onValueChanged.AddListener(OnAudioVolumeSlider);
        if (audioVolumeSlider != null && mediaPlayer != null && mediaPlayer.Control != null)
        {
            float v = Mathf.Clamp01(mediaPlayer.Control.GetVolume());
            cachedVolumeSliderValue = v;
            audioVolumeSlider.value = v;
        }

        if (autoStartToggle != null && mediaPlayer != null)
        {
            autoStartToggle.isOn = mediaPlayer.m_AutoStart;
        }

        if (muteToggle != null && mediaPlayer != null && mediaPlayer.Control != null)
        {
            muteToggle.isOn = mediaPlayer.Control.IsMuted();
        }

        playButton?.onClick.AddListener(OnPlayButton);
        pauseButton?.onClick.AddListener(OnPauseButton);
        rewindButton?.onClick.AddListener(OnRewindButton);

        OnOpenVideoFile(videoFileName);
    }

    private void OnDestroy()
    {
        if (mediaPlayer != null)
        {
            mediaPlayer.Events.RemoveListener(OnVideoEvent);
        }
    }

    private bool HasValidDuration()
    {
        return mediaPlayer && mediaPlayer.Info != null && mediaPlayer.Info.GetDurationMs() > 0f;
    }

    private bool _isFinished = false;

    private float _mouseInactiveTime = 0f;

    private bool _controlsVisible = true;

    private void Update()
    {
        if (_isFinished)
        {
            if (!continueButton.gameObject.activeSelf)
            {
                continueButton.gameObject.SetActive(true);
            }

            return;
        }

        if (mediaPlayer && HasValidDuration())
        {
            float timeMs = mediaPlayer.Control.GetCurrentTimeMs();
            float durMs = mediaPlayer.Info.GetDurationMs();
            float t = Mathf.Clamp01(durMs > 0f ? timeMs / durMs : 0f);

            cachedSeekSliderValue = t;
            if (videoSeekSlider)
            {
                videoSeekSlider.value = t;
            }

            if (bufferedSliderRect && mediaPlayer.Control.IsBuffering())
            {
                float t1 = 0f;
                float t2 = mediaPlayer.Control.GetBufferingProgress();
                if (t2 <= 0f && mediaPlayer.Control.GetBufferedTimeRangeCount() > 0)
                {
                    mediaPlayer.Control.GetBufferedTimeRange(0, ref t1, ref t2);
                    float inv = 1f / Mathf.Max(1f, durMs);
                    t1 *= inv;
                    t2 *= inv;
                }

                Vector2 min = Vector2.zero;
                Vector2 max = Vector2.one;

                if (bufferedSliderImage && bufferedSliderImage.type == Image.Type.Filled)
                {
                    bufferedSliderImage.fillAmount = t;
                }
                else
                {
                    min.x = Mathf.Clamp01(t1);
                    max.x = Mathf.Clamp01(t2);
                }

                bufferedSliderRect.anchorMin = min;
                bufferedSliderRect.anchorMax = max;
            }

            if (videoSeekSlider.value >= 0.9900f)
            {
                _isFinished = true;
                if (mediaPlayer.Control != null)
                {
                    mediaPlayer.Control.Pause();
                    mediaPlayer.Control.Seek(0f);
                }

                ShowControls(false);
                Debug.Log("Video playback finished.");
            }
        }

        if (_controlsVisible)
        {
            _mouseInactiveTime += Time.deltaTime;
            if (_mouseInactiveTime >= maxInactiveTime)
            {
                ShowControls(false);
            }
        }

        Vector3 delta = Input.mousePosition - _lastMousePos;
        if (delta.sqrMagnitude >= moveThreshold * moveThreshold)
        {
            _mouseInactiveTime = 0f;
            if (!_controlsVisible)
            {
                ShowControls(true);
            }
        }

        _lastMousePos = Input.mousePosition;
    }

    private void ShowControls(bool state)
    {
        _controlsVisible = state;
        foreach (GameObject control in _controls.Where(control => control))
        {
            control.SetActive(state);
        }
    }

    public void OnPlayButton()
    {
        Debug.Log("OnPlayButton called");
        if (mediaPlayer != null && mediaPlayer.Control != null)
        {
            mediaPlayer.Control.Play();
        }
    }

    public void OnPauseButton()
    {
        if (mediaPlayer != null && mediaPlayer.Control != null) mediaPlayer.Control.Pause();
    }

    public void OnRewindButton()
    {
        if (mediaPlayer != null && mediaPlayer.Control != null)
        {
            mediaPlayer.Control.Rewind();
            mediaPlayer.Control.Pause();
        }
    }

    public void OnAutoStartChange()
    {
        if (mediaPlayer != null && autoStartToggle != null && autoStartToggle.enabled)
            mediaPlayer.m_AutoStart = autoStartToggle.isOn;
    }

    public void OnMuteChange()
    {
        if (mediaPlayer != null && mediaPlayer.Control != null && muteToggle != null)
        {
            mediaPlayer.Control.MuteAudio(muteToggle.isOn);
        }
    }

    public void OnAudioVolumeSlider(float value)
    {
        Debug.Log($"OnAudioVolumeSlider called with value: {value}");
        if (mediaPlayer == null || mediaPlayer.Control == null)
            return;

        if (!Mathf.Approximately(value, cachedVolumeSliderValue))
        {
            cachedVolumeSliderValue = value;
            mediaPlayer.Control.SetVolume(cachedVolumeSliderValue);
        }
    }

    public void OnVideoSliderDown()
    {
        if (mediaPlayer != null && mediaPlayer.Control != null)
        {
            wasPlayingOnScrub = mediaPlayer.Control.IsPlaying();
            if (wasPlayingOnScrub)
            {
                mediaPlayer.Control.Pause();
            }

            OnVideoSeekSlider();
        }
    }

    public void OnVideoSeekSlider()
    {
        if (mediaPlayer == null || mediaPlayer.Control == null || videoSeekSlider == null)
        {
            return;
        }

        if (!HasValidDuration())
        {
            return;
        }

        if (!Mathf.Approximately(videoSeekSlider.value, cachedSeekSliderValue))
        {
            float durMs = mediaPlayer.Info.GetDurationMs();
            mediaPlayer.Control.Seek(videoSeekSlider.value * durMs);
        }
    }

    public void OnVideoSliderUp()
    {
        if (mediaPlayer != null && mediaPlayer.Control != null && wasPlayingOnScrub)
        {
            mediaPlayer.Control.Play();
            wasPlayingOnScrub = false;
        }
    }

    private void OnOpenVideoFile(string fileName)
    {
        if (mediaPlayer == null) return;
        if (string.IsNullOrEmpty(fileName))
        {
            mediaPlayer.CloseVideo();
            return;
        }

        bool autoStart = autoStartToggle != null ? autoStartToggle.isOn : mediaPlayer.m_AutoStart;

        mediaPlayer.OpenVideoFromFile(
            MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder,
            fileName,
            autoStart
        );
    }


    private void OnVideoEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode errorCode)
    {
        Debug.Log($"OnVideoEvent called with type: {et}, errorCode: {errorCode}");
        switch (et)
        {
            case MediaPlayerEvent.EventType.ReadyToPlay:
                mediaPlayer.Control?.SetLooping(false);
                break;
            case MediaPlayerEvent.EventType.FinishedPlaying:
                Debug.Log("Video finished playing.");
                if (mediaPlayer.Control != null)
                {
                    mediaPlayer.Control.Pause();
                    // mediaPlayer.Control.Seek(0f);
                }

                break;
        }
    }
}