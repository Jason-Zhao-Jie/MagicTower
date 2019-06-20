using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MagicTower.Components.Control {

    public class InfoDlg : MonoBehaviour {
        private void Awake() {
            foreach(var i in Game.Config.infos) {
                var item = titleList.PushbackDefaultItem();
                item.Find("Text").GetComponent<Text>().text = Game.Config.StringInternational.GetValue(i.title);
                item.Find("Text").GetComponent<Text>().color = new Color(0.7f, 0.8f, 0.9f);
                item.GetComponent<Button>().onClick.AddListener(() => { titleList.Select(item); });
            }
            titleList.selectedFunc = OnItemClick;
        }

        private void OnEnable() {
            lastStatus = Game.Status;
            if(Game.Status != Model.EGameStatus.Start) {
                Game.Status = Model.EGameStatus.InEditorDialog;
            }
        }

        private void OnItemClick(int index, bool selected) {
            titleList[index].GetComponent<Button>().interactable = !selected;
            if(selected) {
                titleList[index].Find("Text").GetComponent<Text>().color = new Color(0.1f, 0.2f, 0.3f);
                content.text = Game.Config.StringInternational.GetValue(Game.Config.infos[index].content);
            } else {
                titleList[index].Find("Text").GetComponent<Text>().color = new Color(0.7f, 0.8f, 0.9f);

            }
        }

        public void OnBtnClick() {
            Game.Status = lastStatus;
            Game.HideUI(UIType.InfoDialog);
        }

        public ArmyAnt.ViewUtil.Components.SelectListView titleList;
        public Text content;
        public Button btn;
        public Text btnText;

        private Model.EGameStatus lastStatus;
    }

}
