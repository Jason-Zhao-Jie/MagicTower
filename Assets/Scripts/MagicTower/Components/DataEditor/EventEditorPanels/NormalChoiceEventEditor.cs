using UnityEngine;
using UnityEngine.UI;
using ArmyAnt.ViewUtil.Components;

namespace MagicTower.Components.DataEditor.EventEditorPanels
{
    // param0: choiceDataIndex

    public class NormalChoiceEventEditor : MonoBehaviour
    {
        void Start()
        {
            choiceList.selectedFunc = OnSelect;
            var c = GetComponent<Common_EventEditor>();
            long selectedId = 0;
            if(c.parent.SelectedKey.Length > 0) {
                selectedId = c.parent.SelectedKey[0];
            }
            foreach (var i in Game.Config.choices)
            {
                var item = choiceList.PushbackDefaultItem<DefaultSelectableElement>();
                item.text.text = Game.Config.StringInternational.GetValue(i.Value.title);
                item.AddOnclickEvent(() => { choiceList.Select(item.GetComponent<RectTransform>()); });
                var data = item.gameObject.AddComponent<UserData>();
                data.SetIntegerData(i.Value.id);
                if(i.Value.id == selectedId) {
                    choiceList.Select(item.GetComponent<RectTransform>());
                }
            }
        }

        public void OnSelect(int index, bool select) {
            if(select) {
                var id = choiceList.SelectedItem.GetComponent<UserData>().GetIntegerData();
                GetComponent<Common_EventEditor>().parent.SelectedKey = new long[] { id };
                choiceTitleText.text = Game.Config.StringInternational.GetValue(Game.Config.choices[id].title);
                choiceChosenList.Clear();
                foreach(var i in Game.Config.choices[id].data) {
                    choiceChosenList.PushbackDefaultItem<DefaultSelectableElement>().text.text = Game.Config.StringInternational.GetValue(i.content);
                }
            }
        }

        public SelectListView choiceList;
        public Text choiceTitleText;
        public SelectListView choiceChosenList;
    }

}
