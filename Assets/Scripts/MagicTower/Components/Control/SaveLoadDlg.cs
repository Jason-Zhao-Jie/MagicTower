using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArmyAnt.ViewUtil;
using ArmyAnt.ViewUtil.Components;

namespace MagicTower.Components.Control {

    public class SaveLoadDlg : MonoBehaviour { 
        private const string STR_UI_SAVENEW = "str_ui_saveNew";
        private const string STR_UI_SAVENAME = "str_ui_saveName";

        // Update is called once per frame
        void Update() {
            if (change) {
                list.Clear();
                var saveData = Present.Manager.SaveManager.ListAll();
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

        public void Init(bool save) {
            this.save = save;
            change = true;
        }

        public void OnCancel() {
            if (!change) {
                Game.HideUI(UIType.SaveLoadDialog);
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
