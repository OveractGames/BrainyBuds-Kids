using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    private Dictionary<string, string> localizedTexts;

    public void LoadLanguage(string lang)
    {
        localizedTexts = DataLoader<LocalizationFile>.Load($"Lang/{lang}").GetDictionary();
    }

    public string Get(string key) => localizedTexts.ContainsKey(key) ? localizedTexts[key] : key;
}
