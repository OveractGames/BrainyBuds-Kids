//using UnityEditor;
//using UnityEngine;
//using System.Collections.Generic;
//using UnityGame;

//public class GameDataEditor : EditorWindow
//{
//    private static int chapterIndex;
//    private static GameFlowEditor.ChapterData chapterData;


//    [MenuItem("Custom Tools/Game Data Editor")]
//    public static void ShowWindow()
//    {
//        var window = GetWindow<GameDataEditor>("Game Data Editor");
//        window.Load();
//        window.Show();
//    }

//    public void Load()
//    {
//        chapterIndex = index;
//        chapterData = data;
//        ChapterEditorWindow window = GetWindow<ChapterEditorWindow>($"Chapter {index + 1} Editor");
//        window.minSize = new Vector2(500, 400);
//        window.Show();
//    }

//    private List<ChapterData> _

//    void OnGUI()
//    {
//        GUILayout.Label($"Chapter {chapterIndex + 1} Configuration", EditorStyles.boldLabel);

//        int numGames = Mathf.Max(0, EditorGUILayout.IntField("Number of Games", chapterData.games.Count));

//        while (chapterData.games.Count < numGames)
//            chapterData.games.Add(new GameFlowEditor.GameData());
//        while (chapterData.games.Count > numGames)
//            chapterData.games.RemoveAt(chapterData.games.Count - 1);

//        GUILayout.Space(10);

//        for (int i = 0; i < chapterData.games.Count; i++)
//        {
//            GUILayout.BeginVertical("box");
//            GUILayout.Label($"Game {i + 1} Illustrations", EditorStyles.boldLabel);

//            GameFlowEditor.GameData game = chapterData.games[i];

//            int numIllustrations = Mathf.Max(0, EditorGUILayout.IntField("Number of Illustrations", game.illustrations.Count));

//            while (game.illustrations.Count < numIllustrations)
//                game.illustrations.Add(null);
//            while (game.illustrations.Count > numIllustrations)
//                game.illustrations.RemoveAt(game.illustrations.Count - 1);

//            for (int j = 0; j < game.illustrations.Count; j++)
//            {
//                game.illustrations[j] = (Texture2D)EditorGUILayout.ObjectField($"Illustration {j + 1}", game.illustrations[j], typeof(Texture2D), false);
//            }

//            GUILayout.EndVertical();
//            GUILayout.Space(10);
//        }

//        GUILayout.FlexibleSpace();

//        if (GUILayout.Button("Save Chapter"))
//        {
//            Close();
//        }
//    }
//}
