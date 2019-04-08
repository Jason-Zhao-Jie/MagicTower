using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArmyAnt.ViewUtil;

namespace MagicTower.Components.Control {
    public class SettingDlg : ObjectPool.AViewUnit {
        public const string PREFAB_DIR = "SettingDlg";
        public const int PREFAB_ID = 9;

        public static SettingDlg ShowDialog(GameObject parent) {
            // 弹出战斗框
            var ret = Game.ObjPool.GetAnElement<SettingDlg>(PREFAB_ID, ObjectPool.ElementType.Dialog, GetResourcePath());
            ret.transform.SetParent(parent.transform, false);
            ret.transform.SetSiblingIndex(2);
            ret.transform.localPosition = new Vector3(0, 0, ret.transform.localPosition.z);
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

        public void OnOK() {
            // TODO: save settings
            RecycleSelf();
        }

        public void OnCancel() {
            RecycleSelf();
        }

        public ScrollRect content;
        public Button btnOK;
        public Text txtOK;
        public Button btnCancel;
        public Text txtCancel;
    }

}
