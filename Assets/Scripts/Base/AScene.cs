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
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BackToStartScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("StartScene");
    }

    abstract public void OnMapClicked(int posx, int posy);
}
