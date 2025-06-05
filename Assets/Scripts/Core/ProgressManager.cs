using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public static void SaveProgress(string chapterId)
    {
        PlayerPrefs.SetInt($"chapter_{chapterId}", 1);
    }

    public static bool IsUnlocked(string chapterId)
    {
        return PlayerPrefs.GetInt($"chapter_{chapterId}", 0) == 1;
    }
}
