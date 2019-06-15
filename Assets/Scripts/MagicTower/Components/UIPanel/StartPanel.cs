using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MagicTower.Present.Manager;

namespace MagicTower.Components.UIPanel {
    public class StartPanel : MonoBehaviour {
        private const string str_startNewGame = "str_ui_startNewGame";
        private const string str_loadSavedGame = "str_ui_loadSavedGame";
        private const string str_settings = "str_ui_settings";
        private const string str_readMe = "str_ui_readMe";
        private const string str_exit = "str_ui_exit";
        private const string str_dataEditor = "str_ui_dataEditor";
        private const string str_languageSwitch = "str_ui_languageSwitch";
        private const string README_CONTENT_STR_KEY = "str_ui_readme";

        private const int id_startMusic = 58;

        // Use this for initialization
        void Start() {
            Game.HideAllDialog();
            Game.Map.HideLoadingCurtain();
            Game.Status = Model.EGameStatus.Start;

            var languages = new List<string>();
            for(var i = 0; i < Game.Config.languages.Count; ++i) {
                languages.Add(Game.Config.languages[i + 1].name);
            }
            languageDrop.AddOptions(languages);

            SetUIByLanguage(Game.Config.StringInternational.LanguageId);

            if(!Game.IsDebug) {
                btnDataEditor.gameObject.SetActive(false);
            }

            AudioManager.PlayMusicLoop(id_startMusic);
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

        public void OnStartGame() => Game.LoadGame();
        
        public void OnLoadGame() => Game.ShowSaveLoadDialog(false);
        
        public void OnSettings() => Game.ShowSettings();
        
        public void OnReadMe() {
            Game.ShowInfo(Game.Config.StringInternational.GetValue(README_CONTENT_STR_KEY));
        }

        public void OnChangeLanguage() => SetUIByLanguage(languageDrop.value + 1);

        public void OnGameEditor() {
            AudioManager.StopMusic();
            Game.ShowDataEditor(); // TODO : 需要在此暂停音乐
        }

        public void OnExitGame() => Game.ExitGame();

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
        public ArmyAnt.ViewUtil.Components.DropDownBugFix languageDrop;

        [Tooltip("编辑器")]
        [Space(4)]
        public GameObject dataEditorPanel;
    }

}
