using UnityEngine;

public class TestGameScript : AbstractGame
{
    protected override void OnBackButtonClicked()
    {
        SceneLoadWatcher.LoadScene("ChapterInterface");
    }

    public override void StartGame()
    {
        throw new System.NotImplementedException();
    }

    public override void EndGame()
    {
        throw new System.NotImplementedException();
    }
}