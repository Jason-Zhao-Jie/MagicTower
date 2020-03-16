using System.Collections;
using System.Collections.Generic;
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
                item.text.text = i.Value.title;
            }
        }

        public void OnSelect(int index, bool select)
        {

        }

        public SelectListView choiceList;
        public Text choiceTitleText;
        public SelectListView choiceChosenList;
    }

}
