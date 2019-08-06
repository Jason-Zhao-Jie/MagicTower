using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArmyAnt.Algorithm;
using ArmyAnt.ViewUtil.Components;

namespace MagicTower.Components.DataEditor {
    public class MonstersMakerPanel : MonoBehaviour {
        private void Awake() {
            // 设定武器下拉框的内容
            var weaponStrings = new List<string>(Game.Config.weapons.Count);
            foreach(var i in Game.Config.weapons) {
                weaponStrings.Add(i.Value.id + "." + Game.Config.StringInternational.GetValue(i.Value.name));
            }
            weaponSelect.AddOptions(weaponStrings);
            // 设定列表
            monstersList.selectedFunc = OnMonstersListSelected;
            foreach(var i in Game.Config.modals) {
                if(i.Value.typeId == (int)Unit.ModalType.Monster) {
                    var item = monstersList.PushbackDefaultItem<MonstersSelectItem>();
                    item.text.text = i.Value.id + " " + Game.Config.StringInternational.GetValue(i.Value.name);
                    item.image.sprite = Game.GetMods(i.Value.prefabPath)[0];
                    item.AddOnclickEvent(() => { monstersList.Select(item.GetComponent<RectTransform>()); });
                    var data = item.gameObject.AddComponent<UserData>();
                    data.SetIntegerData(i.Value.id);
                }
            }
        }

        private void OnMonstersListSelected(int index, bool select) {
            var item = monstersList[index].GetComponent<MonstersSelectItem>();
            item.Selected = select;
            if(select) {
                var ud = item.GetComponent<UserData>();
                var id = ud.GetIntegerData();
                if(!Game.Config.monsters.ContainsKey(id)) {
                    Game.Config.monsters.Add(id, new Model.MonsterData() { id = id, level = 1, exp = 0, life = 1, attack = 1, speed = 1, critical = 1, gold = 1, special = new int[0], weaponId = 1 });
                }
                var data = Game.Config.monsters[id];
                idText.text = "ID: " + data.id;
                levelInput.text = data.level.ToString();
                expInput.text = data.exp.ToString();
                lifeInput.text = data.life.ToString();
                attackInput.text = data.attack.ToString();
                defenseInput.text = data.defense.ToString();
                speedInput.text = data.speed.ToString();
                criticalInput.text = data.critical.ToString();
                goldInput.text = data.gold.ToString();
                weaponSelect.value = data.weaponId;
            } else {
                SaveValues();
            }
        }

        public void SaveValues() {
            var ud = monstersList.SelectedItem.GetComponent<UserData>();
            var id = ud.GetIntegerData();
            Game.Config.monsters[id].level = System.Convert.ToInt32(levelInput.text);
            Game.Config.monsters[id].exp = System.Convert.ToInt32(expInput.text);
            Game.Config.monsters[id].life = System.Convert.ToInt32(lifeInput.text);
            Game.Config.monsters[id].attack = System.Convert.ToInt32(attackInput.text);
            Game.Config.monsters[id].defense = System.Convert.ToInt32(defenseInput.text);
            Game.Config.monsters[id].speed = System.Convert.ToInt32(speedInput.text);
            Game.Config.monsters[id].critical = System.Convert.ToDouble(criticalInput.text);
            Game.Config.monsters[id].gold = System.Convert.ToInt32(goldInput.text);
            Game.Config.monsters[id].weaponId = weaponSelect.value;
        }

        public void OnEditSpecials() {
            // TODO
            SaveValues();
        }
        public void OnSaveExit() {
            SaveValues();
            Game.ShowDataEditor();
            Destroy(gameObject);
        }

        public SelectListView monstersList;
        public Text idText;
        public InputField levelInput;
        public InputField expInput;
        public InputField lifeInput;
        public InputField attackInput;
        public InputField defenseInput;
        public InputField speedInput;
        public InputField criticalInput;
        public InputField goldInput;
        public Dropdown weaponSelect;
    }
}
