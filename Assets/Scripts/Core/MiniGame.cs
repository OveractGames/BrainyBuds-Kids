using UnityEngine;

public abstract class MiniGame : MonoBehaviour
{
    public virtual void StartGame(int miniGameIndex)
    {
        // Override this method to initialize the mini-game
        Debug.Log("Initializing mini-game: " + gameObject.name);
    }

    public virtual void EndGame()
    {
        // Override this method to end the mini-game
        Debug.Log("Ending mini-game: " + gameObject.name);
    }
}