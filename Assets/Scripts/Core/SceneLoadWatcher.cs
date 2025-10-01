using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoadWatcher
{
    private static HookAPI _api;
    private static Runner _runner;

    private static string _pendingSceneName;
    private static LoadSceneMode _pendingMode;

    public static void Install(LoadingScreen loadingPrefab, float destroyDelaySeconds = 0.5f,
        float minVisibleSeconds = 0.25f)
    {
        if (loadingPrefab == null)
        {
            Debug.LogError("[SceneLoadWatcher] LoadingScreen prefab is null.");
            return;
        }

        if (SceneManagerAPI.overrideAPI != null)
        {
            Debug.LogWarning("[SceneLoadWatcher] Another overrideAPI is already set. " +
                             "Cannot chain safely; watcher not installed.");
            return;
        }

        _runner = EnsureRunner();
        _api = new HookAPI(loadingPrefab, destroyDelaySeconds, minVisibleSeconds, _runner);
        SceneManagerAPI.overrideAPI = _api;
    }

    private static bool _isLoading = false;

    public static void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        if (_runner == null)
        {
            Debug.LogError("[SceneLoadWatcher] Runner not initialized. Did you call Install()?");
            return;
        }

        if (_isLoading)
        {
            Debug.LogWarning($"[SceneLoadWatcher] Already loading a scene ({_pendingSceneName}). Ignoring request for {sceneName}.");
            return;
        }

        Debug.Log("Is loading scene: " + sceneName);
        _isLoading = true;
        _pendingSceneName = sceneName;
        _pendingMode = mode;
        _runner.BeginSceneLoadRequest();
    }

    private static Runner EnsureRunner()
    {
        GameObject go = new("SceneLoadWatcher_Runner");
        Object.DontDestroyOnLoad(go);
        return go.AddComponent<Runner>();
    }

    private class Runner : MonoBehaviour
    {
        private readonly List<AsyncOperation> _ops = new();
        private LoadingScreen _instance;
        private float _shownAt;
        private float _minVisible;
        private float _destroyDelay;

        public void Show(LoadingScreen prefab, float minVisibleSeconds, float destroyDelay)
        {
            _minVisible = minVisibleSeconds;
            _destroyDelay = destroyDelay;

            if (_instance == null)
            {
                _instance = Instantiate(prefab);
                DontDestroyOnLoad(_instance.gameObject);
                _instance.SetVisible(true);
                _shownAt = Time.unscaledTime;
            }
        }

        public void BeginSceneLoadRequest()
        {
            if (_instance == null)
            {
                Debug.LogWarning("[SceneLoadWatcher] Loading screen not visible yet. Forcing show.");
                Show(_api.Prefab, _api.MinVisible, _api.Delay);
            }

            StartCoroutine(WaitForVisibleThenLoad());
        }

        private IEnumerator WaitForVisibleThenLoad()
        {
            yield return null;
            while (_instance && !_instance.IsFullyVisible)
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.25f);

            if (!string.IsNullOrEmpty(_pendingSceneName))
            {
                AsyncOperation op = SceneManager.LoadSceneAsync(_pendingSceneName, _pendingMode);
                if (op != null)
                    Track(op);

                _pendingSceneName = null;
            }
        }

        private void HideWhenReady()
        {
            StartCoroutine(HideAfter());
        }

        private IEnumerator HideAfter()
        {
            var minEnd = _shownAt + _minVisible;
            while (Time.unscaledTime < minEnd)
            {
                yield return null;
            }

            var endAt = Time.unscaledTime + _destroyDelay;
            while (Time.unscaledTime < endAt)
            {
                yield return null;
            }

            if (_instance)
            {
                _instance.BeginFadeOut(() =>
                {
                    if (_instance)
                    {
                        Destroy(_instance.gameObject);
                        _instance = null;
                    }

                    Debug.Log("Scene load complete.");
                    _isLoading = false;
                });
            }
        }

        public void Track(AsyncOperation op)
        {
            if (op == null)
                return;

            _ops.Add(op);
            StartCoroutine(TrackRoutine(op));
        }

        private IEnumerator TrackRoutine(AsyncOperation op)
        {
            while (!op.isDone)
            {
                if (_instance)
                {
                    _instance.SetProgress(op.progress);
                    yield return null;
                }
            }

            _ops.Remove(op);
            if (_ops.Count == 0)
            {
                HideWhenReady();
            }
        }
    }

    private class HookAPI : SceneManagerAPI
    {
        public LoadingScreen Prefab { get; }
        public float Delay { get; }
        public float MinVisible { get; }
        private readonly Runner _runner;

        public HookAPI(LoadingScreen prefab, float delay, float minVisible, Runner runner)
        {
            Prefab = prefab;
            Delay = delay;
            MinVisible = minVisible;
            _runner = runner;
        }

        protected override AsyncOperation LoadSceneAsyncByNameOrIndex(
            string sceneName, int sceneBuildIndex, LoadSceneParameters parameters, bool mustCompleteNextFrame)
        {
            _runner.Show(Prefab, MinVisible, Delay);
            AsyncOperation op =
                base.LoadSceneAsyncByNameOrIndex(sceneName, sceneBuildIndex, parameters, mustCompleteNextFrame);

            if (op != null)
                _runner.Track(op);

            return op;
        }
    }
}