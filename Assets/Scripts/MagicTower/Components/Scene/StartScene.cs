using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MagicTower.Present.Manager;

namespace MagicTower.Components.Scene
{

    public class StartScene : AScene
    {
        private const string str_startNewGame = "str_ui_startNewGame";
        private const string str_loadSavedGame = "str_ui_loadSavedGame";
        private const string str_settings = "str_ui_settings";
        private const string str_readMe = "str_ui_readMe";
        private const string str_exit = "str_ui_exit";
        private const string str_dataEditor = "str_ui_dataEditor";
        private const string str_languageSwitch = "str_ui_languageSwitch";

        // Use this for initialization
        override protected async System.Threading.Tasks.Task Start()
        {
            Input.multiTouchEnabled = false;    // NOTE : 多点触摸会导致寻路出现bug, 先禁用

            await base.Start();
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

            SetUIByLanguage(Game.Config.StringInternational.LanguageId);

            if (!Game.DEBUG)
            {
                btnDataEditor.gameObject.SetActive(false);
            }

            SetLoadingPercent(1);
        }

        // Update is called once per frame
        void Update() {
            Game.SceneUpdate();
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

        private void SetUIByLanguage(int id) {
            languageDrop.value = id - 1;
            Game.Config.StringInternational.LanguageId = id;

            btnStartNewGame.transform.Find("Text").GetComponent<Text>().text = Game.Config.StringInternational.GetValue(str_startNewGame);
            btnLoadSavedGame.transform.Find("Text").GetComponent<Text>().text = Game.Config.StringInternational.GetValue(str_loadSavedGame);
            btnSettings.transform.Find("Text").GetComponent<Text>().text = Game.Config.StringInternational.GetValue(str_settings);
            btnReadMe.transform.Find("Text").GetComponent<Text>().text = Game.Config.StringInternational.GetValue(str_readMe);
            btnExit.transform.Find("Text").GetComponent<Text>().text = Game.Config.StringInternational.GetValue(str_exit);
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

        public void OnLoadGame() {
            var dlg = Control.SaveLoadDlg.ShowDialog(gameObject, true);
            dlg.transform.SetSiblingIndex(transform.childCount - 1);
        }

        public void OnSettings() {
            var dlg = Control.SettingDlg.ShowDialog(transform.gameObject);
            dlg.transform.SetSiblingIndex(transform.childCount - 1);
        }

        public void OnReadMe()
        {
            // TODO
        }

        public void OnChangeLanguage()
        {
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
        public override SceneType Type => SceneType.StartScene;

        public override void OnMapClicked(int posx, int posy)
        {
            return;
        }

        // 弹出Tips提示, 并在一定时间后消失
        public override void ShowTips(params string[] texts)
        {
            string text = "";
            for (var i = 0; i < texts.Length; ++i)
            {
                text += texts[i];
            }
            var tipbar = Control.TipBar.ShowTip();
            tipbar.transform.SetParent(transform, false);
            tipbar.SetTipText(text);
            tipbar.StartAutoRemove(200);
        }

        [Tooltip("开始新游戏按钮")]
        [Space(4)]
        public Button btnStartNewGame;
        [Tooltip("载入存档按钮")]
        [Space(4)]
        public Button btnLoadSavedGame;
        [Tooltip("设置按钮")]
        [Space(4)]
        public Button btnSettings;
        [Tooltip("作者信息按钮")]
        [Space(4)]
        public Button btnReadMe;
        [Tooltip("退出游戏按钮")]
        [Space(4)]
        public Button btnExit;

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