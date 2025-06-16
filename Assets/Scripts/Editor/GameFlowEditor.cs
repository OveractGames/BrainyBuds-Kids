using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class GameFlowEditor : EditorWindow
{
    private Texture2D introIllustration;
    private AudioClip introVoiceOver;
    private int numChapters = 1;

    private readonly List<ChapterData> chapters = new List<ChapterData>();

   // [MenuItem("Custom Tools/Game Flow Editor")]
    public static void ShowWindow()
    {
        GetWindow<GameFlowEditor>("Game Flow Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Intro Settings", EditorStyles.boldLabel);
        introIllustration =
            (Texture2D)EditorGUILayout.ObjectField("Intro Illustration", introIllustration, typeof(Texture2D), false);
        introVoiceOver = (AudioClip)EditorGUILayout.ObjectField("Voice Over", introVoiceOver, typeof(AudioClip), false);

        GUILayout.Space(10);
        GUILayout.Label("Chapters", EditorStyles.boldLabel);
        numChapters = EditorGUILayout.IntSlider("Number of Chapters", numChapters, 1, 10);

        while (chapters.Count < numChapters) chapters.Add(new ChapterData());
        while (chapters.Count > numChapters) chapters.RemoveAt(chapters.Count - 1);

        for (int i = 0; i < numChapters; i++)
        {
            if (GUILayout.Button($"Edit Chapter {i + 1}"))
            {
                ChapterEditorWindow.ShowWindow(i, chapters[i]);
            }
        }

        GUILayout.Space(20);
        if (GUILayout.Button("Create Game Structure"))
        {
            CreateGameStructure();
        }
    }

    void CreateGameStructure()
    {
        if (!introIllustration || !introVoiceOver)
        {
            EditorUtility.DisplayDialog("Missing Data", "Intro illustration or voice-over is missing.", "OK");
            return;
        }

        const string basePath = "Assets/GameContent";
        Directory.CreateDirectory(basePath + "/Intro");
        AssetDatabase.CreateAsset(Object.Instantiate(introIllustration), basePath + "/Intro/IntroIllustration.asset");
        AssetDatabase.CreateAsset(Object.Instantiate(introVoiceOver), basePath + "/Intro/IntroVoice.asset");

        for (int i = 0; i < chapters.Count; i++)
        {
            string capPath = $"{basePath}/Cap{i + 1}";
            Directory.CreateDirectory(capPath);
            ChapterData chapter = chapters[i];

            for (int j = 0; j < chapter.games.Count; j++)
            {
                string gamePath = $"{capPath}/Game{j + 1}";
                Directory.CreateDirectory(gamePath);

                foreach (Texture2D tex in chapter.games[j].illustrations)
                {
                    AssetDatabase.CreateAsset(Object.Instantiate(tex),
                        $"{gamePath}/Illustration_{Random.Range(1000, 9999)}.asset");
                }

                Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
                EditorSceneManager.SaveScene(scene, $"{gamePath}/Game{j + 1}Scene.unity");
            }
        }

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Success", "Game structure created successfully.", "OK");
    }

    [System.Serializable]
    public class ChapterData
    {
        public List<GameData> games = new List<GameData>();
    }

    [System.Serializable]
    public class GameData
    {
        public List<Texture2D> illustrations = new List<Texture2D>();
    }
}