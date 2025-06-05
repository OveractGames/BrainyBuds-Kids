using UnityEngine;

public static class DataLoader<T>
{
    public static T Load(string path)
    {
        TextAsset file = Resources.Load<TextAsset>(path);
        return JsonUtility.FromJson<T>(file.text);
    }
}