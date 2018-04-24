using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScene : MonoBehaviour
{
    public static StartScene instance = null; 
    // Use this for initialization
    void Start()
    {
        instance = this;
        Input.multiTouchEnabled = true;

        Initializationer.InitBases(GetComponent<RectTransform>().rect.size);
        SetMapLoadingPercent(1);
        AudioController.instance.MusicSource = GetComponent<AudioSource>();
        AudioController.instance.ClearSoundSource();
        AudioController.instance.AddSoundSource(GameObject.Find("Main Camera").GetComponent<AudioSource>());
    }

    private void OnDestroy()
    {
        instance = null;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetMapLoadingPercent(double percent)
    {
        this.percent = percent;
        if(1-percent <= double.Epsilon)
        {
            loadedOK = true;
            //PlatformUIManager.ShowMessageBox("游戏数据载入完毕");
        }
    }

    public void OnStartGame()
    {
        if (loadedOK)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
        }
    }

    public void OnLoadGame()
    {

    }

    public void OnReadMe()
    {

    }

    public void OnGameEditor()
    {
        if (loadedOK)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("DataEditorScene");
        }
    }

    public void OnExitGame()
    {
        Application.Quit();
    }

    private double percent = 0;
    private bool loadedOK = false;
}
