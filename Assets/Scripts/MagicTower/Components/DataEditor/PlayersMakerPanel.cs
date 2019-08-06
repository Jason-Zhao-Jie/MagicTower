using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArmyAnt.Algorithm;
using ArmyAnt.ViewUtil.Components;

namespace MagicTower.Components.DataEditor {
    public class PlayersMakerPanel : MonoBehaviour {
        private void Awake() {
            // 设定武器下拉框的内容
            var weaponStrings = new List<string>(Game.Config.weapons.Count);
            foreach(var i in Game.Config.weapons) {
                weaponStrings.Add(i.Value.id + "." + Game.Config.StringInternational.GetValue(i.Value.name));
            }
            weaponSelect.AddOptions(weaponStrings);
            // 设定列表
            playersList.selectedFunc = OnPlayersListSelected;
            foreach(var i in Game.Config.modals) {
                if(i.Value.typeId == (int)Unit.ModalType.Player) {
                    var item = playersList.PushbackDefaultItem<MonstersSelectItem>();
                    item.text.text = i.Value.id + " " + Game.Config.StringInternational.GetValue(i.Value.name);
                    item.image.sprite = Game.GetMods(i.Value.prefabPath)[0];
                    item.AddOnclickEvent(() => { playersList.Select(item.GetComponent<RectTransform>()); });
                    var data = item.gameObject.AddComponent<UserData>();
                    data.SetIntegerData(i.Value.id);
                }
            }
        }

        private void OnPlayersListSelected(int index, bool select) {
            var item = playersList[index].GetComponent<MonstersSelectItem>();
            item.Selected = select;
            if(select) {
                var ud = item.GetComponent<UserData>();
                var id = ud.GetIntegerData();
                if(!Game.Config.players.ContainsKey(id)) {
                    Game.Config.players.Add(id, new Model.PlayerData() { id = id, level = 1, exp = 0, life = 1, attack = 1, defense = 1, speed = 1, critical = 1, gold = 1, weaponId = 1, yellowKey = 0, blueKey = 0, redKey = 0, greenKey = 0 });
                }
                var data = Game.Config.players[id];
                idText.text = "ID: " + data.id;
                levelInput.text = data.level.ToString();
                expInput.text = data.exp.ToString();
                lifeInput.text = data.life.ToString();
                attackInput.text = data.attack.ToString();
                defenseInput.text = data.defense.ToString();
                speedInput.text = data.speed.ToString();
                criticalInput.text = data.critical.ToString();
                goldInput.text = data.gold.ToString();
                yellowKeyNumInput.text = data.yellowKey.ToString();
                blueKeyNumInput.text = data.blueKey.ToString();
                redKeyNumInput.text = data.redKey.ToString();
                greenKeyNumInput.text = data.greenKey.ToString();
                weaponSelect.value = data.weaponId - 1;
            } else {
                SaveValues();
            }
        }

        public void SaveValues() {
            var ud = playersList.SelectedItem.GetComponent<UserData>();
            var id = ud.GetIntegerData();
            Game.Config.players[id] = new Model.PlayerData() {
                id = id,
                level = System.Convert.ToInt32(levelInput.text),
                exp = System.Convert.ToInt32(expInput.text),
                life = System.Convert.ToInt32(lifeInput.text),
                attack = System.Convert.ToInt32(attackInput.text),
                defense = System.Convert.ToInt32(defenseInput.text),
                speed = System.Convert.ToInt32(speedInput.text),
                critical = System.Convert.ToDouble(criticalInput.text),
                gold = System.Convert.ToInt32(goldInput.text),
                weaponId = weaponSelect.value + 1,
                yellowKey = System.Convert.ToInt32(yellowKeyNumInput.text),
                blueKey = System.Convert.ToInt32(blueKeyNumInput.text),
                redKey = System.Convert.ToInt32(redKeyNumInput.text),
                greenKey = System.Convert.ToInt32(greenKeyNumInput.text),
            };
        }

        public void OnSaveExit() {
            SaveValues();
            Game.ShowDataEditor();
            Destroy(gameObject);
        }

        public SelectListView playersList;
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
        public InputField yellowKeyNumInput;
        public InputField blueKeyNumInput;
        public InputField redKeyNumInput;
        public InputField greenKeyNumInput;
    }
}
