using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityGame;


public enum GameState
{
    None = 0,
    Intro = 1,
    Gameplay = 2,
    OutroGood = 3,
    OutroBad = 4,
}

public class GameManager : MonoBehaviour
{
    private DataManager _dataManager;
    private int _currChapterIndex = -1;
    private int _currChapterLevelIndex = -1;
    private bool _running;
    private GameLevelData _gameLevelData = null;

    public static GameManager Instance { get; private set; }
    public string SelectedLanguage { get; private set; }
    public string CurrentChapterId { get; private set; }
    public PlayerProfile CurrentUser { get; private set; }

    public static Action OnReadyToStartGame;
    public static Action<GameState> StateChanged;
    public static bool CanStartGame { get; private set; }

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
    }

    public void Init()
    {
        _dataManager = new DataManager();
        _dataManager.LoadData();
    }

    public void StartNextLevel()
    {
       

        ChapterDataSO data = _dataManager.GetChapterData();

        var currChapter = data.chapters[_currChapterIndex];

        if(currChapter.levels.Count > _currChapterLevelIndex + 1)
        {
            Debug.Log($"[GameManager] Starting Next LEVEL. Chapter:{_currChapterIndex}; Level:{_currChapterLevelIndex + 1}");
            var nextLevel = currChapter.levels[_currChapterLevelIndex + 1];
            FinishGame(() =>
            {
                StartLevel(_currChapterIndex, _currChapterLevelIndex + 1, null);
            }, false);
        }
        else
        {
            if (data.chapters.Count > _currChapterIndex + 1)
            {
                Debug.Log($"[GameManager] Starting Next CHAPTER. NextChapter:{_currChapterIndex + 1}; NextLevel:{0}");
                var nextChapter = data.chapters.Find(e => e.index == _currChapterIndex + 1);
                FinishGame(() =>
                {
                    StartLevel(_currChapterIndex + 1, 0, null);
                }, false);
            }
            else
            {
                Debug.LogError($"[GameManager] ALL CHAPTERS DONE. Implement logic.");
            }
        }
    }

    public void RestartLevel()
    {
        FinishGame(()=>
        {
            StartLevel(_currChapterIndex, _currChapterLevelIndex, null);
        }, false);

    }

    public async void StartLevel(int chapterIndex, int chapterLevelIndex, Action started)
    {
        if (_running)
        {
            Debug.LogError($"[GameManager] StartGame failed. Reason: A game already started");
            return;
        }

        _running = true;

        Debug.Log($"[GameManager] StartGame called 1");

        CanStartGame = false;

        _currChapterIndex = chapterIndex;
        _currChapterLevelIndex = chapterLevelIndex;
        var op = SceneManager.LoadSceneAsync(GetSceneName(chapterIndex, chapterLevelIndex), LoadSceneMode.Additive);
        while (!op.isDone)
        {
            await Task.Yield();
        }
        Debug.Log($"[GameManager] StartGame called 2. Scene Loaded.");

        Scene scene = SceneManager.GetSceneByName(GetSceneName(chapterIndex, chapterLevelIndex));
        _gameLevelData = null;
        foreach(var elem in scene.GetRootGameObjects())
        {
            _gameLevelData = elem.GetComponent<GameLevelData>();
            if (_gameLevelData != null)
            {
                break;
            }
        }
        Debug.Log($"[GameManager] StartGame called 3. Got GameLevelData.");

        if (_gameLevelData == null)
        {
            Debug.LogError($"[GameManager] StartGame failed. Reason: GameLevelData not found!");
            return;
        }

        if(_gameLevelData.intro == null) Debug.LogError($"[GameManager] StartGame failed. Reason: gameLevelData.intro is NULL!");
        if(_gameLevelData.gameplay == null) Debug.LogError($"[GameManager] StartGame failed. Reason: gameLevelData.gameplay is NULL!");
        if(_gameLevelData.outroBad == null) Debug.LogError($"[GameManager] StartGame failed. Reason: gameLevelData.outroBad is NULL!");
        if(_gameLevelData.outroGood == null) Debug.LogError($"[GameManager] StartGame failed. Reason: gameLevelData.outroGood is NULL!");

        _gameLevelData.intro.gameObject.SetActive(false);
        _gameLevelData.gameplay.gameObject.SetActive(false);
        _gameLevelData.outroBad.gameObject.SetActive(false);
        _gameLevelData.outroGood.gameObject.SetActive(false);

        // RunIntro();

        Debug.Log($"[GameManager] StartGame called 4. Ready To Start");

        CanStartGame = true;
        OnReadyToStartGame?.Invoke();

        started?.Invoke();
    }

    public async void FinishGame(Action finished, bool goHomeScreen = true)
    {
        if (!_running)
        {
            Debug.LogError($"[GameManager] FinishGame failed. Reason: No Game is running now");
            return;
        }
        
        var op = SceneManager.UnloadSceneAsync(GetSceneName(_currChapterIndex, _currChapterLevelIndex));

        _running = false;

        if(goHomeScreen)
            UIManager.HomeScreen.Show();

        //var op = SceneManager.LoadSceneAsync("BootScene", LoadSceneMode.Single);
        while (!op.isDone)
        {
            await Task.Yield();
        }

        finished?.Invoke();
    }

    private string GetSceneName(int chapterIndex, int chapterLevelIndex)
    {
        return $"Scene_Cap{(chapterIndex+1)}_Level{(chapterLevelIndex+1)}";
    }

    public void RunIntro()
    {
        _gameLevelData.intro.gameObject.SetActive(true);
        _gameLevelData.gameplay.gameObject.SetActive(false);
        _gameLevelData.outroBad.gameObject.SetActive(false);
        _gameLevelData.outroGood.gameObject.SetActive(false);
        _gameLevelData.intro.Show(_currChapterIndex, _currChapterLevelIndex);

        StateChanged?.Invoke(GameState.Intro);
    }

    public void RunOutro(bool passedLevel)
    {
        _gameLevelData.intro.gameObject.SetActive(false);
        _gameLevelData.gameplay.gameObject.SetActive(false);
        _gameLevelData.outroBad.gameObject.SetActive(!passedLevel);
        _gameLevelData.outroGood.gameObject.SetActive(passedLevel);
        if (passedLevel)
            _gameLevelData.outroGood.Show(_currChapterIndex, _currChapterLevelIndex);
        else
            _gameLevelData.outroBad.Show(_currChapterIndex, _currChapterLevelIndex);

        StateChanged?.Invoke(passedLevel ? GameState.OutroGood : GameState.OutroBad);
    }

    public void RunGameplay()
    {
        _gameLevelData.intro.gameObject.SetActive(false);
        _gameLevelData.gameplay.gameObject.SetActive(true);
        _gameLevelData.outroBad.gameObject.SetActive(false);
        _gameLevelData.outroGood.gameObject.SetActive(false);
        _gameLevelData.gameplay.Show(_currChapterIndex, _currChapterLevelIndex);

        StateChanged?.Invoke(GameState.Gameplay);
    }

    public ChapterDefinition GetChapterData(int chapterIdx)
    {
        return _dataManager.GetChapterData().chapters.Find(e=>e.index == chapterIdx);
    }

    public List<ChapterDefinition> GetAllChaptersData()
    {
        return _dataManager.GetChapterData().chapters;
    }
}