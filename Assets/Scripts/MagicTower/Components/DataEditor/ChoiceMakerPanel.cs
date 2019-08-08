using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArmyAnt.Algorithm;
using ArmyAnt.ViewUtil.Components;

namespace MagicTower.Components.DataEditor {
    public class ChoiceMakerPanel : MonoBehaviour {
        private void Awake() {
            
        }

        public SelectListView choiceList;
        public SelectListView choiceDataList;
        public SelectListView eventDataList;

        public InputField idInput;
        public Dropdown speakerTypeSelect;
        public MonstersSelectItem speakerModal;
        public GameObject speakerModalPanel;
        public Text titleStringText;
        public Text tailStringText;
        public Toggle canonToggle;
        public Text contentStringText;
        public Dropdown contentTypeSelect;
        public Text contentDataText;
        public Toggle willClose;
        public Dropdown eventTypeSelect;

        public Text btnSaveApplyText;
        public GameObject stringSelector;
    }
}
