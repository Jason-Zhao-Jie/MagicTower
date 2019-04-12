using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArmyAnt.ViewUtil;
using ArmyAnt.ViewUtil.Components;

namespace MagicTower.Components.Control {

    public class SaveLoadDlg : ObjectPool.AViewUnit {
        private const string PREFAB_DIR = "SaveLoadDlg";
        private const int PREFAB_ID = 10;

        public static SaveLoadDlg ShowDialog(GameObject parent, bool save) {
            // 弹出战斗框
            var ret = Game.ObjPool.GetAnElement<SaveLoadDlg>(PREFAB_ID, ObjectPool.ElementType.Dialog, GetResourcePath());
            ret.transform.SetParent(parent.transform, false);
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

        public void OnCancel() {
            RecycleSelf();
        }

        public Text title;
        public Text btnCancel;
        public ListView list;
    }

}
