using UnityEngine;
using System.Collections;

public abstract class AScene : MonoBehaviour
{
    public enum SceneType : byte
    {
        None,
        StartScene,
        MainScene,
        DataEditorScene,
    }

    public abstract SceneType Type { get; }

    // Use this for initialization
    virtual protected System.Threading.Tasks.Task Start()
    {
        Game.Initial();
        Game.CurrentScene = this;
        return null;
    }

    public void BackToStartScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("StartScene");
    }

    abstract public void OnMapClicked(int posx, int posy);
}
