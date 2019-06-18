using ArmyAnt.ViewUtil;
using UnityEngine;
using UnityEngine.UI;

namespace MagicTower.Components.Control {

    public class MainMenuDlg : MonoBehaviour {
        private const string RESUME_STR_KEY = "str_ui_resume";
        private const string SAVEGAME_STR_KEY = "str_ui_savegame";
        private const string LOADGAME_STR_KEY = "str_ui_loadSavedGame";
        private const string SETTINGS_STR_KEY = "str_ui_settings";
        private const string EXIT_STR_KEY = "str_ui_exit";
        private const string ALERT_EXIT_STR_KEY = "str_ui_alertExit";

        // Start is called before the first frame update
        void Awake() {
            btnResumeText.text = Game.Config.StringInternational.GetValue(RESUME_STR_KEY);
            btnSaveGameText.text = Game.Config.StringInternational.GetValue(SAVEGAME_STR_KEY);
            btnLoadGameText.text = Game.Config.StringInternational.GetValue(LOADGAME_STR_KEY);
            btnSettingsText.text = Game.Config.StringInternational.GetValue(SETTINGS_STR_KEY);
            btnExitText.text = Game.Config.StringInternational.GetValue(EXIT_STR_KEY);
        }

        void Start() {
            lastStatus = Game.Status;
            Game.Status = Model.EGameStatus.OnDialog;
            Game.GamePaused = true;
        }

        public void OnResume() {
            Game.Status = lastStatus;
            Game.GamePaused = false;
            Game.HideUI(UIType.MainMenu);
        }

        public void OnSaveGame() {
            Game.ShowSaveLoadDialog(true);
        }

        public void OnLoadGame() {
            Game.ShowSaveLoadDialog(false);
        }

        public void OnSettings() {
            Game.ShowSettings();
        }

        public void OnExit() {
            Game.ShowAlert(ALERT_EXIT_STR_KEY, TextAnchor.MiddleCenter, () => {
                Game.StopAndBackToStart();
                Game.HideUI(UIType.MainMenu);
                return false;
            });
        }

        public Text btnResumeText;
        public Text btnSaveGameText;
        public Text btnLoadGameText;
        public Text btnSettingsText;
        public Text btnExitText;
        private Model.EGameStatus lastStatus;
    }

}
