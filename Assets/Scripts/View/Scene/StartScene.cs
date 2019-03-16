using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScene : MonoBehaviour {
    public static StartScene instance = null;
    // Use this for initialization
    void Start() {
        instance = this;
        Input.multiTouchEnabled = false;    // Note : 多点触摸会导致寻路出现bug, 先禁用

        Game.Initial(GetComponent<RectTransform>().rect.size);
        Game.Map.SetStartData();
        SetLoadingPercent(1);
        Game.Controller.Audio.MusicSource = GetComponent<AudioSource>();
        Game.Controller.Audio.ClearSoundSource();
        Game.Controller.Audio.AddSoundSource(GameObject.Find("Main Camera").GetComponent<AudioSource>());
    }

    private void OnDestroy() {
        instance = null;
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
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private double percent = 0;
    private bool loadedOK = false;
}
