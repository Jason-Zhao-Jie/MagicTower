using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArmyAnt.ViewUtil.Components;

namespace MagicTower.Components.Control {
    public class InfoDlg : MonoBehaviour {
        private const string STR_UI_CLOSE = "str_ui_close";

        private void Awake() {
            foreach(var i in Game.Config.infos) {
                var item = titleList.PushbackDefaultItem<DefaultSelectableElement>();
                item.text.text = Game.Config.StringInternational.GetValue(i.title);
                item.AddOnclickEvent(() => { titleList.Select(item.GetComponent<RectTransform>()); });
            }
            titleList.selectedFunc = OnItemClick;
            btnText.text = Game.Config.StringInternational.GetValue(STR_UI_CLOSE);
        }

        private void OnEnable() {
            lastStatus = Game.Status;
            if(Game.Status != Model.EGameStatus.Start) {
                Game.Status = Model.EGameStatus.InEditorDialog;
            }
        }

        private void OnItemClick(int index, bool selected) {
            titleList[index].GetComponent<DefaultSelectableElement>().Selected = selected;
            if(selected) {
                content.text = Game.Config.StringInternational.GetValue(Game.Config.infos[index].content);
            }
        }

        public void OnBtnClick() {
            Game.Status = lastStatus;
            Game.HideUI(UIType.InfoDialog);
        }

        public SelectListView titleList;
        public Text content;
        public Button btn;
        public Text btnText;

        private Model.EGameStatus lastStatus;
    }

}
