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
        private const string STR_UI_SAVEALERT = "str_ui_saveAlert";
        private const string STR_UI_SAVESUCCESSFUL = "str_ui_saveSuccessful";
        private const string STR_UI_SAVEFAILURE = "str_ui_saveFailure";
        private const string STR_UI_LOADALERT = "str_ui_loadAlert";

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
                var lastTimeStr = i.Value.data.lastTime + ' ' + i.Value.data.lastTime;
                var mapData = Game.Config.GetGameMap(i.Value.data.pos.mapId - 1);
                var totalTime = System.DateTime.FromFileTime(i.Value.data.totalTime);
                var lastTime = System.DateTime.FromFileTime(i.Value.data.lastTime);
                var playerData = Game.Config.modals[i.Value.data.player.id];
                item.transform.Find("Information").GetComponent<Text>().text = Game.Config.StringInternational.GetValue(STR_UI_SAVENAME, lastTime.ToShortDateString() + ' ' + lastTime.ToShortTimeString(), Game.Config.StringInternational.GetValue(mapData.mapName, mapData.mapNameParam.ToString()), totalTime.Day.ToString(), totalTime.Hour.ToString(), totalTime.Minute.ToString(), Game.Config.StringInternational.GetValue(playerData.name), i.Value.data.player.level.ToString(), i.Value.data.player.life.ToString(), i.Value.data.player.speed.ToString(), i.Value.data.player.gold.ToString(), i.Value.data.player.attack.ToString(), i.Value.data.player.defense.ToString());
                item.transform.Find("btnOK").GetComponent<Button>().onClick.AddListener(() => { OnClickSaveLoad(i.Key); });
                item.GetComponent<UserData>().SetStringData(i.Key);
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
            if(save) {
                if(name != null) {
                    Game.ShowAlert(Game.Config.StringInternational.GetValue(STR_UI_SAVEALERT), TextAnchor.MiddleLeft, () => {
                        Game.ShowTip(Game.SaveGame(name) ? STR_UI_SAVESUCCESSFUL : STR_UI_SAVEFAILURE);
                        Set();
                        return true;
                    });
                } else {
                    Game.ShowTip(Game.SaveGame(System.DateTime.Now.ToFileTime().ToString()) ? STR_UI_SAVESUCCESSFUL : STR_UI_SAVEFAILURE);
                    Set();
                }
            } else {
                if(Game.Status == Model.EGameStatus.Start) {
                    Game.LoadGame(name);
                    Game.HideUI(UIType.SaveLoadDialog);
                } else {
                    Game.ShowAlert(Game.Config.StringInternational.GetValue(STR_UI_LOADALERT), TextAnchor.MiddleLeft, () => {
                        Game.LoadGame(name);
                        Game.HideUI(UIType.SaveLoadDialog);
                        Game.HideUI(UIType.MainMenu);
                        return true;
                    });
                }
            }
        }

        public Text title;
        public Text btnCancel;
        public ListView list;
        
        private bool save;
        private bool change;
    }

}
