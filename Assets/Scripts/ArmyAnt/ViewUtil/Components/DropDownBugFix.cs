using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ArmyAnt.ViewUtil.Components {
    public class DropDownBugFix : Dropdown {

        protected override GameObject CreateDropdownList(GameObject template) {
            var ret = base.CreateDropdownList(template);
            ret.GetComponent<Canvas>().sortingLayerName = SortingLayer;
            return ret;
        }

        [Attribute.SortingLayerSetting]
        public string SortingLayer;
    }

}
