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
        void Start() {
            if (change) {
                Set();
                change = false;
            } else {
                change = true;
            }
        }

        private void Set() {
            list.Clear();
            var saveData = Present.Manager.SaveManager.ListAll();
            if(save) {
                var item = list.PushbackDefaultItem();
                item.transform.Find("Information").GetComponent<Text>().text = Game.Config.StringInternational.GetValue(STR_UI_SAVENEW);
                item.transform.Find("btnOK").GetComponent<Button>().onClick.AddListener(() => { OnClickSaveLoad(); });
            }
            foreach(var i in saveData) {
                var item = list.PushbackDefaultItem();
                var lastTimeStr = i.Value.lastTime.ToShortDateString() + ' ' + i.Value.lastTime.ToShortTimeString();
                var mapData = Game.Config.GetGameMap(i.Value.currentMapId);
                var totalTime = System.DateTime.FromFileTime(i.Value.totalTime);
                var playerData = Game.Config.modals[i.Value.playerData.id];
                item.transform.Find("Information").GetComponent<Text>().text = Game.Config.StringInternational.GetValue(STR_UI_SAVENAME, i.Value.lastTime.ToShortDateString() + ' ' + i.Value.lastTime.ToShortTimeString(), Game.Config.StringInternational.GetValue(mapData.mapName, mapData.mapNameParam.ToString()), totalTime.Day.ToString(), totalTime.Hour.ToString(), totalTime.Minute.ToString(), Game.Config.StringInternational.GetValue(playerData.name), i.Value.playerData.level.ToString(), i.Value.playerData.life.ToString(), i.Value.playerData.speed.ToString(), i.Value.playerData.gold.ToString(), i.Value.playerData.attack.ToString(), i.Value.playerData.defense.ToString());
                item.transform.Find("btnOK").GetComponent<Button>().onClick.AddListener(() => { OnClickSaveLoad(i.Key); });
            }
        }

        public void Init(bool save) {
            this.save = save;
            if(change) {
                Set();
                change = false;
            } else {
                change = true;
            }
        }

        public void OnCancel() {
            Game.HideUI(UIType.SaveLoadDialog);
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
