using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoadWatcher
{
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

    private static HookAPI _api;
    private static Runner _runner;

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

        public void HideWhenReady()
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
                });
            }
        }

        public void Track(AsyncOperation op)
        {
            if (op == null)
            {
                return;
            }

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
        private readonly LoadingScreen _prefab;
        private readonly float _delay;
        private readonly float _minVisible;
        private readonly Runner _runner;

        public HookAPI(LoadingScreen prefab, float delay, float minVisible, Runner runner)
        {
            _prefab = prefab;
            _delay = delay;
            _minVisible = minVisible;
            _runner = runner;
        }

        protected override AsyncOperation LoadSceneAsyncByNameOrIndex(
            string sceneName, int sceneBuildIndex, LoadSceneParameters parameters, bool mustCompleteNextFrame)
        {
            _runner.Show(_prefab, _minVisible, _delay);
            AsyncOperation op =
                base.LoadSceneAsyncByNameOrIndex(sceneName, sceneBuildIndex, parameters, mustCompleteNextFrame);

            if (op != null)
            {
                _runner.Track(op);
            }

            return op;
        }
    }
}