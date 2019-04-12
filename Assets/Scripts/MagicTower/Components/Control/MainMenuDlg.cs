using ArmyAnt.ViewUtil;
using UnityEngine;
using UnityEngine.UI;

namespace MagicTower.Components.Control {

    public class MainMenuDlg : ObjectPool.AViewUnit {
        private const string PREFAB_DIR = "MainMenuDlg";
        private const int PREFAB_ID = 8;

        private const string RESUME_STR_KEY = "str_ui_resume";
        private const string SAVEGAME_STR_KEY = "str_ui_savegame";
        private const string LOADGAME_STR_KEY = "str_ui_loadSavedGame";
        private const string SETTINGS_STR_KEY = "str_ui_settings";
        private const string EXIT_STR_KEY = "str_ui_exit";
        private const string ALERT_EXIT_STR_KEY = "str_ui_alertExit";

        public static MainMenuDlg ShowDialog(Scene.MainScene parent) {
            // 弹出战斗框
            var ret = Game.ObjPool.GetAnElement<MainMenuDlg>(PREFAB_ID, ObjectPool.ElementType.Dialog, GetResourcePath());
            ret.parent = parent;
            ret.transform.SetParent(parent.dialogCanvas.transform, false);
            ret.transform.SetSiblingIndex(1);
            ret.transform.localPosition = new Vector3(0, 0, ret.transform.localPosition.z);
            ret.lastStatus = Game.Status;
            Game.Status = Model.EGameStatus.OnDialog;
            return ret;
        }

        public override string ResourcePath => Model.Dirs.DIALOG_DIR + PREFAB_DIR;
        public static string GetResourcePath() => Model.Dirs.DIALOG_DIR + PREFAB_DIR;

        public override ObjectPool.ElementType GetPoolTypeId() {
            return ObjectPool.ElementType.Dialog;
        }

        public override bool OnCreate(ObjectPool.ElementType tid, int elemId, string resourcePath) {
            return true;
        }

        public override void OnReuse(ObjectPool.ElementType tid, int elemId) {

        }

        public override bool OnUnuse(ObjectPool.ElementType tid, int elemId) {
            Game.Status = lastStatus;
            return true;
        }

        public override bool RecycleSelf() {
            return Game.ObjPoolRecycleSelf(this);
        }

        // Start is called before the first frame update
        void Awake() {
            btnResumeText.text = Game.Config.StringInternational.GetValue(RESUME_STR_KEY);
            btnSaveGameText.text = Game.Config.StringInternational.GetValue(SAVEGAME_STR_KEY);
            btnLoadGameText.text = Game.Config.StringInternational.GetValue(LOADGAME_STR_KEY);
            btnSettingsText.text = Game.Config.StringInternational.GetValue(SETTINGS_STR_KEY);
            btnExitText.text = Game.Config.StringInternational.GetValue(EXIT_STR_KEY);
        }

        // Update is called once per frame
        void Update() {

        }

        public void OnResume() {
            RecycleSelf();
        }

        public void OnSaveGame() {
            var dlg = SaveLoadDlg.ShowDialog(transform.parent.gameObject, true);
            dlg.transform.SetSiblingIndex(2);
        }

        public void OnLoadGame() {
            var dlg = SaveLoadDlg.ShowDialog(transform.parent.gameObject, false);
            dlg.transform.SetSiblingIndex(2);
        }

        public void OnSettings() {
            var dlg = SettingDlg.ShowDialog(transform.parent.gameObject);
            dlg.transform.SetSiblingIndex(2);
        }

        public void OnExit() {
            AlertDlg.ShowDialog(transform.parent, Game.Config.StringInternational.GetValue(ALERT_EXIT_STR_KEY), TextAnchor.MiddleCenter, () => {
                Game.CurrentScene.BackToStartScene();
                return false;
            });
        }

        public Text btnResumeText;
        public Text btnSaveGameText;
        public Text btnLoadGameText;
        public Text btnSettingsText;
        public Text btnExitText;

        private Scene.MainScene parent = null;
        private Model.EGameStatus lastStatus;
    }

}
