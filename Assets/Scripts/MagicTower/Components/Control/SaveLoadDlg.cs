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
        private const string STR_UI_SAVEINFO = "str_ui_saveInfo";
        private const string STR_UI_SAVEALERT = "str_ui_saveAlert";
        private const string STR_UI_SAVESUCCESSFUL = "str_ui_saveSuccessful";
        private const string STR_UI_SAVEFAILURE = "str_ui_saveFailure";
        private const string STR_UI_DELALERT = "str_ui_delAlert";
        private const string STR_UI_DELSUCCESSFUL = "str_ui_delSuccessful";
        private const string STR_UI_DELFAILURE = "str_ui_delFailure";
        private const string STR_UI_LOADALERT = "str_ui_loadAlert";

        private void Awake() {
            list.selectedFunc = OnSelect;
        }

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
                item.GetComponent<UserData>().SetGameObject(item.Find("Selected").gameObject);
                item.GetComponent<UserData>().GetGameObject().SetActive(false);
                item.transform.Find("Information").GetComponent<Text>().text = Game.Config.StringInternational.GetValue(STR_UI_SAVENEW);
                item.GetComponent<UserData>().SetStringData(null);
                item.GetComponent<UserData>().SetStringData(0, Game.Config.StringInternational.GetValue(STR_UI_SAVEINFO, Game.Player.Level.ToString(), Game.Player.Life.ToString(), Game.Player.Attack.ToString(), Game.Player.Defense.ToString(), Game.Player.Speed.ToString(), Game.Player.Gold.ToString()));
                item.GetComponent<Button>().onClick.AddListener(() => { list.Select(item); });
            }
            foreach(var i in saveData) {
                var item = list.PushbackDefaultItem();
                item.GetComponent<UserData>().SetGameObject(item.Find("Selected").gameObject);
                item.GetComponent<UserData>().GetGameObject().SetActive(false);
                var lastTimeStr = i.Value.data.lastTime + ' ' + i.Value.data.lastTime;
                var mapData = Game.Config.GetGameMap(i.Value.data.pos.mapId - 1);
                var totalTime = System.DateTime.FromFileTime(i.Value.data.totalTime);
                var lastTime = System.DateTime.FromFileTime(i.Value.data.lastTime);
                var playerData = Game.Config.modals[i.Value.data.player.id];
                item.transform.Find("Information").GetComponent<Text>().text = Game.Config.StringInternational.GetValue(STR_UI_SAVENAME, lastTime.ToShortDateString() + ' ' + lastTime.ToShortTimeString(), Game.Config.StringInternational.GetValue(mapData.mapName, mapData.mapNameParam.ToString()), Game.Config.StringInternational.GetValue(playerData.name), totalTime.Day.ToString(), totalTime.Hour.ToString(), totalTime.Minute.ToString());
                item.GetComponent<UserData>().SetStringData(i.Key);
                item.GetComponent<UserData>().SetStringData(0, Game.Config.StringInternational.GetValue(STR_UI_SAVEINFO, i.Value.data.player.level.ToString(), i.Value.data.player.life.ToString(), i.Value.data.player.attack.ToString(), i.Value.data.player.defense.ToString(), i.Value.data.player.speed.ToString(), i.Value.data.player.gold.ToString()));
                item.GetComponent<Button>().onClick.AddListener(() => { list.Select(item); });
            }
            btnDelete.interactable = false;
            btnOK.interactable = false;
            info.text = "";
            list.Select(0);
        }

        public void Init(bool save) {
            this.save = save;
            if(change) {
                Set();
                change = false;
            } else {
                change = true;
            }
            Game.RecaptureMap();
        }

        public void OnDelete() {
            var name = list.SelectedItem.GetComponent<UserData>().GetStringData();
            Game.ShowAlert(STR_UI_DELALERT, TextAnchor.MiddleLeft, () => {
                Game.ShowAlert(Present.Manager.SaveManager.Remove(name) ? STR_UI_DELSUCCESSFUL : STR_UI_DELFAILURE, TextAnchor.MiddleCenter);
                Set();
                return false;
            });
        }

        public void OnOK() {
            var name = list.SelectedItem.GetComponent<UserData>().GetStringData();
            if(save) {
                if(name != null) {
                    Game.ShowAlert(STR_UI_SAVEALERT, TextAnchor.MiddleLeft, () => {
                        Game.ShowAlert(Game.SaveGame(name, Game.CapturedTexture) ? STR_UI_SAVESUCCESSFUL : STR_UI_SAVEFAILURE, TextAnchor.MiddleCenter);
                        Set();
                        return false;
                    });
                } else {
                    Game.ShowAlert(Game.SaveGame(System.DateTime.Now.ToFileTime().ToString(), Game.CapturedTexture) ? STR_UI_SAVESUCCESSFUL : STR_UI_SAVEFAILURE, TextAnchor.MiddleCenter);
                    Set();
                }
            } else {
                if(Game.Status == Model.EGameStatus.Start) {
                    Game.LoadGame(name);
                    Game.HideUI(UIType.SaveLoadDialog);
                } else {
                    Game.ShowAlert(STR_UI_LOADALERT, TextAnchor.MiddleLeft, () => {
                        Game.LoadGame(name);
                        Game.HideUI(UIType.SaveLoadDialog);
                        Game.HideUI(UIType.MainMenu);
                        return true;
                    });
                }
            }
        }

        public void OnCancel() {
            Game.HideUI(UIType.SaveLoadDialog);
        }

        private void OnSelect(int index, bool select) {
            list[index].GetComponent<UserData>().GetGameObject().SetActive(select);
            if(select) {
                info.text = list[index].GetComponent<UserData>().GetStringData(0);
                btnDelete.interactable = list[index].GetComponent<UserData>().GetStringData() != null;
                btnOK.interactable = true;
                captureImage.sprite = null;
                Texture2D captureTexture;
                if(list[index].GetComponent<UserData>().GetStringData() != null) {
                    captureTexture = Present.Manager.SaveManager.GetSaveCapture(list[index].GetComponent<UserData>().GetStringData());
                } else {
                    captureTexture = Game.CapturedTexture;
                }
                captureImage.sprite = Sprite.Create(captureTexture, new Rect(0, 0, captureTexture.width, captureTexture.height), Vector2.zero);
                list[index].Find("Information").GetComponent<Text>().color = Color.black;
            } else {
                list[index].Find("Information").GetComponent<Text>().color = Color.white;
            }
        }
        

        public Text title;
        public Text txtDelete;
        public Text txtOK;
        public Text txtCancel;
        public Text info;
        public Image captureImage;
        public SelectListView list;

        public Button btnDelete;
        public Button btnOK;
        public Button btnCancel;

        private bool save;
        private bool change;
    }

}
