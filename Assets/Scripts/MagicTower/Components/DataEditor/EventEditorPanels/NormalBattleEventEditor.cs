using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace MagicTower.Components.DataEditor.EventEditorPanels
{
    public class NormalBattleEventEditor : MonoBehaviour
    {
        // param0: canFail, param1: selfUuid

        private void Awake()
        {

        }

        public Toggle canFailToggle;
        public Dropdown self;
    }

}
