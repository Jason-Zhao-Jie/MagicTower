using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace MagicTower.Components.DataEditor.EventEditorPanels
{
    public class NormalBattleEventEditor : MonoBehaviour
    {
        // param0: canFail, param1: selfUuid

        // todo: Battle 事件目前非常简陋, 后续需要根据具体需求, 添加内容, 比如胜利事件失败事件, 暂停条件事件, enemyUuid(考虑在无地图敌人处空挥的情况)

        private void Awake() {
            var opt = new List<Dropdown.OptionData>();
            opt.Add(new Dropdown.OptionData("self", null));
            foreach(var i in Game.Config.modals) {
                opt.Add(new Dropdown.OptionData(i.Value.id + " " + Game.Config.StringInternational.GetValue(i.Value.name), Game.GetMods(i.Value.prefabPath)[0]));
            }
            self.AddOptions(opt);

            var parent = GetComponent<Common_EventEditor>().parent;
            canFailToggle.isOn = parent.SelectedKey != null && parent.SelectedKey.Length > 0 && parent.SelectedKey[0] != 0;
            if(parent.SelectedKey != null && parent.SelectedKey.Length > 1) {
                self.value = Convert.ToInt32(parent.SelectedKey[1]);
            } else {
                self.value = 0;
            }
        }

        public void OnChangeData() {
            var parent = GetComponent<Common_EventEditor>().parent;
            parent.SelectedKey = new long[] {
                canFailToggle.isOn?1:0,
                self.value
            };
        }

        public Toggle canFailToggle;
        public Dropdown self;
    }

}
