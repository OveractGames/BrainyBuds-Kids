using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameManager : MonoBehaviour
{
    public void Launch(string gameId)
    {
        SceneLoader.Load("MiniGame_" + gameId);
    }
}
