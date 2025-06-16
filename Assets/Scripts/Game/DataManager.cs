using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityGame
{
    public class PlayerProfile
    {
        public string username;
        public string avatar;
        public string language;
        public List<int> unlockedChapters = new();
    }

    public class DataManager
    {
        private PlayerProfile _playerProfile;
        private ChapterDataSO _chapterData;

        public void LoadData()
        {
            if (PlayerPrefs.HasKey("player_progress"))
            {
                string json = PlayerPrefs.GetString("player_progress", null);
                _playerProfile = JsonUtility.FromJson<PlayerProfile>(json);
            }
            else
            {
                _playerProfile = new PlayerProfile()
                {
                    
                };
            }

            _chapterData = LoadScriptableObject<ChapterDataSO>("ChaptersData");
        }

        public void SaveData()
        {
            if(_playerProfile == null)
            {
                Debug.LogError("[DataManager] SaveData failed. Reason:PlayerProfile is NULL");
                return;
            }
            string json = JsonUtility.ToJson(_playerProfile, true);
            PlayerPrefs.GetString("player_progress", json);
        }

        public ChapterDataSO GetChapterData()
        {
            return _chapterData;
        }

        public void UnlockChapter(int chapterIndex)
        {
            if (_playerProfile == null)
            {
                Debug.LogError("[DataManager] UnlockChapter failed. Reason:PlayerProfile is NULL");
                return;
            }

            if (!IsChapterUnlocked(chapterIndex))
                _playerProfile.unlockedChapters.Add(chapterIndex);
        }

        public bool IsChapterUnlocked(int chapterIndex)
        {
            if (_playerProfile == null)
            {
                Debug.LogError("[DataManager] IsChapterUnlocked failed. Reason:PlayerProfile is NULL");
                return false;
            }
            return _playerProfile.unlockedChapters.Find(e => e == chapterIndex) != -1;
        }

        public static T LoadTextFile<T>(string path) where T: class
        {
            TextAsset file = Resources.Load<TextAsset>(path);
            if (file == null)
            {
                Debug.LogError($"[DataManager] LoadTextFile failed. Reason: File with path '{path}' not found!");
                return default;
            }
            return JsonUtility.FromJson<T>(file.text);
        }

        private T LoadScriptableObject<T>(string path) where T : ScriptableObject
        {
            ScriptableObject file = Resources.Load<ScriptableObject>(path);
            if (file == null)
            {
                Debug.LogError($"[DataManager] LoadScriptableObject failed. Reason: File with path '{path}' not found!");
                return default;
            }
            return (T)file;
        }
    }
}

