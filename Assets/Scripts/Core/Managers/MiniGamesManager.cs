using System;
using UnityEngine;

public class MiniGamesManager : MonoBehaviour
{
    [Header("Mini Games")] public MiniGame[] miniGamePrefabs;

    [SerializeField] private int _miniGameIndex;

    public void LoadMiniGame(int miniGameIndex)
    {
        if (miniGameIndex < 0 || miniGameIndex >= miniGamePrefabs.Length)
        {
            Debug.LogError("Invalid mini-game index: " + miniGameIndex);
            return;
        }

        _miniGameIndex = miniGameIndex;
        MiniGame selectedMiniGame = Instantiate(miniGamePrefabs[miniGameIndex]);
        selectedMiniGame.StartGame(miniGameIndex);
    }
}