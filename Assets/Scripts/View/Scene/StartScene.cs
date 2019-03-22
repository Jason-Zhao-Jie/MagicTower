using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScene : MonoBehaviour
{
    private const string str_startNewGame = "str_ui_startNewGame";
    private const string str_loadSavedGame = "str_ui_loadSavedGame";
    private const string str_readMe = "str_ui_readMe";
    private const string str_dataEditor = "str_ui_dataEditor";

    // Use this for initialization
    void Start() {
        Input.multiTouchEnabled = false;    // NOTE : 多点触摸会导致寻路出现bug, 先禁用

        Game.Initial();
        Game.Map = null;
        Game.Player = null;

        Game.Managers.Audio.MusicSource = GetComponent<AudioSource>();
        Game.Managers.Audio.ClearSoundSource();
        Game.Managers.Audio.AddSoundSource(GameObject.Find("Main Camera").GetComponent<AudioSource>());

        btnStartNewGame.transform.Find("Text").GetComponent<Text>().text = Game.Config.StringInternational.GetValue(str_startNewGame);
        btnLoadSavedGame.transform.Find("Text").GetComponent<Text>().text = Game.Config.StringInternational.GetValue(str_loadSavedGame);
        btnReadMe.transform.Find("Text").GetComponent<Text>().text = Game.Config.StringInternational.GetValue(str_readMe);
        btnDataEditor.transform.Find("Text").GetComponent<Text>().text = Game.Config.StringInternational.GetValue(str_dataEditor);
        if (!Game.DEBUG)
        {
            btnDataEditor.gameObject.SetActive(false);
        }

        SetLoadingPercent(1);
    }

    private void OnDestroy() {
        Game.ObjPool.ClearAll();
    }

    // Update is called once per frame
    void Update() {

    }

    public void SetLoadingPercent(double percent) {
        this.percent = percent;
        if (1 - percent <= double.Epsilon) {
            loadedOK = true;
            //PlatformUIManager.ShowMessageBox("游戏数据载入完毕");
        }
    }

    public void OnStartGame() {
        if (loadedOK) {
            Game.CurrentSaveName = "";
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
        }
    }

    public void OnLoadGame() {

    }

    public void OnReadMe() {

    }

    public void OnGameEditor() {
        if (loadedOK) {
            UnityEngine.SceneManagement.SceneManager.LoadScene("DataEditorScene");
        }
    }

    public void OnExitGame() {
        Game.ExitGame();
    }

    [Tooltip("开始新游戏按钮")]
    [Space(4)]
    public Button btnStartNewGame;
    [Tooltip("载入存档按钮")]
    [Space(4)]
    public Button btnLoadSavedGame;
    [Tooltip("作者信息按钮")]
    [Space(4)]
    public Button btnReadMe;

    [Tooltip("编辑器按钮")]
    [Space(4)]
    public Button btnDataEditor;

    private double percent = 0;
    private bool loadedOK = false;
}
