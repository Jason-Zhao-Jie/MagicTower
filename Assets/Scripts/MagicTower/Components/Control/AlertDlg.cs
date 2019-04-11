using ArmyAnt.ViewUtil;
using UnityEngine;
using UnityEngine.UI;

namespace MagicTower.Components.Control {

    public class AlertDlg : ObjectPool.AViewUnit {
        private const string PREFAB_DIR = "AlertDlg";
        private const int PREFAB_ID = 7;

        public static AlertDlg ShowDialog(Transform parent, string contentStr,Model.EmptyBoolCallBack leftCallback, string leftStr = "OK", Model.EmptyBoolCallBack rightCallback = null, string rightStr = "Cancel") {
            // 弹出战斗框
            var ret = Game.ObjPool.GetAnElement<AlertDlg>(PREFAB_ID, ObjectPool.ElementType.Dialog, GetResourcePath());
            // 设定信息
            ret.contentStr = contentStr;
            ret.leftCallback = leftCallback;
            ret.leftStr = leftStr;
            ret.rightCallback = rightCallback;
            ret.rightStr = rightStr;
            if (ret.isActiveAndEnabled) {
                ret.content.text = contentStr;
                ret.leftText.text = leftStr;
                ret.rightText.text = rightStr;
            }
            ret.showed = true;
            ret.transform.SetParent(parent, false);
            ret.transform.SetSiblingIndex(3);
            ret.transform.localPosition = new Vector3(0, 0, ret.transform.localPosition.z);
            ret.lastStatus = Game.Status;
            Game.Status = Model.EGameStatus.InEditorDialog;
            return ret;
        }
        public static AlertDlg ShowDialog(Transform parent, string contentStr, TextAnchor contentAlignment, Model.EmptyBoolCallBack leftCallback, string leftStr = "OK", Model.EmptyBoolCallBack rightCallback = null, string rightStr = "Cancel") {
            var ret = ShowDialog(parent, contentStr, leftCallback, leftStr, rightCallback, rightStr);
            ret.content.alignment = contentAlignment;
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
            showed = false;
            Game.Status = lastStatus;
            return true;
        }

        public override bool RecycleSelf() {
            return Game.ObjPoolRecycleSelf(this);
        }

        // Start is called before the first frame update
        void Start() {
            if (showed) {
                content.text = contentStr;
                leftText.text = leftStr;
                rightText.text = rightStr;
            }
        }

        // Update is called once per frame
        void Update() {

        }

        public void OnLeftClick() {
            if (leftCallback == null || leftCallback()) {
                RecycleSelf();
            }
        }

        public void OnRightClick() {
            if (rightCallback == null || rightCallback()) {
                RecycleSelf();
            }
        }

        public Text content;
        public Button left;
        public Text leftText;
        public Button right;
        public Text rightText;

        private bool showed = false;
        private string contentStr;
        private Model.EmptyBoolCallBack leftCallback;
        private string leftStr;
        private Model.EmptyBoolCallBack rightCallback;
        private string rightStr;
        private Model.EGameStatus lastStatus;
    }

}
