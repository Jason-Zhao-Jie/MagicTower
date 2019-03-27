using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MagicTower.Present.Manager;

namespace MagicTower.Components.Scene
{

    public class StartScene : MonoBehaviour
    {
        private const string str_startNewGame = "str_ui_startNewGame";
        private const string str_loadSavedGame = "str_ui_loadSavedGame";
        private const string str_readMe = "str_ui_readMe";
        private const string str_dataEditor = "str_ui_dataEditor";
        private const string str_languageSwitch = "str_ui_languageSwitch";

        private const int default_language_id = 2;
        private const string language_pref_key = "language";

        // Use this for initialization
        void Start()
        {
            Input.multiTouchEnabled = false;    // NOTE : 多点触摸会导致寻路出现bug, 先禁用

            Game.Initial();
            Game.Map = null;
            Game.Player = null;

            AudioManager.MusicSource = GetComponent<AudioSource>();
            AudioManager.ClearSoundSource();
            AudioManager.AddSoundSource(GameObject.Find("Main Camera").GetComponent<AudioSource>());

            var languages = new List<string>();
            for (var i = 0; i < Game.Config.languages.Count; ++i)
            {
                languages.Add(Game.Config.languages[i + 1].name);
            }
            languageDrop.AddOptions(languages);

            var id = default_language_id;
            if (PlayerPrefs.HasKey(language_pref_key))
            {
                id = PlayerPrefs.GetInt(language_pref_key);
                languageDrop.value = id - 1;
                SetUIByLanguage(id);
            }

            if (!Game.DEBUG)
            {
                btnDataEditor.gameObject.SetActive(false);
            }

            SetLoadingPercent(1);
        }

        private void OnDestroy()
        {
            Game.ObjPool.ClearAll();
        }

        // Update is called once per frame
        void Update() {
            InputManager.UpdateScene();
        }

        public void SetLoadingPercent(double percent)
        {
            this.percent = percent;
            if (1 - percent <= double.Epsilon)
            {
                loadedOK = true;
                //PlatformUIManager.ShowMessageBox("游戏数据载入完毕");
            }
        }

        private void SetUIByLanguage(int id)
        {
            Game.Config.StringInternational.SetLanguageById(id);

            btnStartNewGame.transform.Find("Text").GetComponent<Text>().text = Game.Config.StringInternational.GetValue(str_startNewGame);
            btnLoadSavedGame.transform.Find("Text").GetComponent<Text>().text = Game.Config.StringInternational.GetValue(str_loadSavedGame);
            btnReadMe.transform.Find("Text").GetComponent<Text>().text = Game.Config.StringInternational.GetValue(str_readMe);
            btnDataEditor.transform.Find("Text").GetComponent<Text>().text = Game.Config.StringInternational.GetValue(str_dataEditor);
            btnSwitchLanguage.transform.Find("Text").GetComponent<Text>().text = Game.Config.StringInternational.GetValue(str_languageSwitch);
        }

        public void OnStartGame()
        {
            if (loadedOK)
            {
                Game.CurrentSaveName = "";
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
            }
        }

        public void OnLoadGame()
        {

        }

        public void OnReadMe()
        {

        }

        public void OnChangeLanguage()
        {
            PlayerPrefs.SetInt(language_pref_key, languageDrop.value + 1);
            SetUIByLanguage(languageDrop.value + 1);
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

        [Tooltip("切换语言按钮")]
        [Space(4)]
        public Button btnSwitchLanguage;
        [Tooltip("语言选择下拉框")]
        [Space(4)]
        public Dropdown languageDrop;

        private double percent = 0;
        private bool loadedOK = false;
    }

}