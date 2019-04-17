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
        private const string STR_UI_SAVENEW = "str_ui_saveNew";
        private const string STR_UI_SAVENAME = "str_ui_saveName";

        public static SaveLoadDlg ShowDialog(GameObject parent, bool save) {
            // 弹出战斗框
            var ret = Game.ObjPool.GetAnElement<SaveLoadDlg>(PREFAB_ID, ObjectPool.ElementType.Dialog, GetResourcePath());
            ret.transform.SetParent(parent.transform, false);
            ret.transform.localPosition = new Vector3(0, 0, ret.transform.localPosition.z);
            ret.save = save;
            ret.change = true;
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
        async System.Threading.Tasks.Task Update() {
            if (change) {
                list.Clear();
                var saveData = await Present.Manager.SaveManager.ListAll();
                if (save) {
                    var item = list.PushbackDefaultItem();
                    item.transform.Find("Information").GetComponent<Text>().text = Game.Config.StringInternational.GetValue(STR_UI_SAVENEW);    // TODO : 在配置里设定这些字符串
                    item.transform.Find("btnOK").GetComponent<Button>().onClick.AddListener(() => { OnClickSaveLoad(); });
                }
                foreach(var i in saveData) {
                    var item = list.PushbackDefaultItem();
                    item.transform.Find("Information").GetComponent<Text>().text = Game.Config.StringInternational.GetValue(STR_UI_SAVENAME, i.Key);    // TODO : 完善显示
                    item.transform.Find("btnOK").GetComponent<Button>().onClick.AddListener(() => { OnClickSaveLoad(i.Key); });
                }
                change = false;
            }
        }

        public void OnCancel() {
            if (!change) {
                RecycleSelf();
            }
        }

        public void OnClickSaveLoad(string name = null) {
            if (!change) {
                // TODO
            }
        }

        public Text title;
        public Text btnCancel;
        public ListView list;
        
        private bool save;
        private bool change;
    }

}
