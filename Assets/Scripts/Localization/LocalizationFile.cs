using System.Collections.Generic;
using UnityEngine;

public class LocalizationFile : MonoBehaviour
{
    public List<LocalizationEntry> entries;

    public Dictionary<string, string> GetDictionary()
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();
        foreach (LocalizationEntry entry in entries)
        {
            dict[entry.key] = entry.value;
        }

        return dict;
    }

    [System.Serializable]
    public class LocalizationEntry
    {
        public string key;
        public string value;
    }
}