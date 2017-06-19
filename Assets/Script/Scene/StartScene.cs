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

        if (DataCenter.instance == null)
        {
            LoadData();
        }
        AudioController.instance.MusicSource = GetComponent<AudioSource>();
        AudioController.instance.SoundSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        SetMapLoadingPercent(1);
    }

    private void OnDestroy()
    {
        instance = null;
    }

    void LoadData()
    {
        DataCenter.instance = new DataCenter();
        DataCenter.instance.LoadData();
        InputController.instance = new InputController();
        InputController.instance.Init();
        EventManager.instance = new EventManager();
        EventManager.instance.InitEvents();
        AudioController.instance = new AudioController();
        MapManager.instance = new MapManager();
        PlayerController.instance = new PlayerController();
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
            PlatformUIManager.ShowMessageBox("游戏数据载入完毕");
        }
    }

    public void OnStartGame()
    {
        if (loadedOK)
        {
            MapManager.instance.SetData();
            PlayerController.instance.SetPlayerInfo(62);
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
            MapManager.instance.SetData();
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
