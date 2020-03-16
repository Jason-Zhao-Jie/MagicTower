using UnityEngine;
using UnityEngine.UI;
using ArmyAnt.ViewUtil.Components;

namespace MagicTower.Components.DataEditor.EventEditorPanels
{
    // param0: choiceDataIndex

    public class NormalChoiceEventEditor : MonoBehaviour
    {
        void Awake()
        {
            choiceList.selectedFunc = OnSelect;
            foreach (var i in Game.Config.choices)
            {
                var item = choiceList.PushbackDefaultItem<DefaultSelectableElement>();
                item.text.text = Game.Config.StringInternational.GetValue(i.Value.title);
                var data = item.gameObject.AddComponent<UserData>();
                data.SetIntegerData(i.Value.id);
            }
        }

        public void OnSelect(int index, bool select)
        {
            var id = choiceList.SelectedItem.GetComponent<UserData>().GetIntegerData();
            GetComponent<Common_EventEditor>().parent.SelectedKey[0] = id;
            choiceTitleText.text = Game.Config.StringInternational.GetValue(Game.Config.choices[id].title);
            foreach(var i in Game.Config.choices[id].data)
            {
                choiceChosenList.PushbackDefaultItem<DefaultSelectableElement>().text.text = Game.Config.StringInternational.GetValue(i.content);
            }
        }

        public SelectListView choiceList;
        public Text choiceTitleText;
        public SelectListView choiceChosenList;
    }

}
