using ArmyAnt.ViewUtil;
using UnityEngine;
using UnityEngine.UI;

namespace MagicTower.Components.Control {

    public class MainMenuDlg : ObjectPool.AViewUnit {
        public const string PREFAB_DIR = "MainMenuDlg";
        public const int PREFAB_ID = 8;

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
        void Start() {
        }

        // Update is called once per frame
        void Update() {

        }

        public void OnResume() {
            RecycleSelf();
        }

        public void OnSaveGame() {

        }

        public void OnLoadGame() {

        }

        public void OnSettings() {
            SettingDlg.ShowDialog(transform.parent.gameObject);
        }

        public void OnExit() {
            Game.CurrentScene.BackToStartScene();
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
