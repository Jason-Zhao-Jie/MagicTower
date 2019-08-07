using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArmyAnt.Algorithm;
using ArmyAnt.ViewUtil.Components;

namespace MagicTower.Components.DataEditor {
    public class WeaponSetterPanel : MonoBehaviour {
        private void Awake() {
            // 设定audio下拉框的内容
            var audioStrings = new List<string>(Game.Config.audios.Count);
            foreach(var i in Game.Config.audios) {
                audioStrings.Add(i.Value.id + "." + i.Value.path);
            }
            normalHitAudioSelect.AddOptions(audioStrings);
            criticalHitAudioSelect.AddOptions(audioStrings);
            // 设定列表
            weaponList.selectedFunc = OnWeaponListSelected;
            foreach(var i in Game.Config.weapons) {
                var item = weaponList.PushbackDefaultItem<MonstersSelectItem>();
                item.text.text = i.Value.id + " " + Game.Config.StringInternational.GetValue(i.Value.name);
                item.image.sprite = Game.GetMods(i.Value.prefabPath)[0];
                item.AddOnclickEvent(() => { weaponList.Select(item.GetComponent<RectTransform>()); });
                var data = item.gameObject.AddComponent<UserData>();
                data.SetIntegerData(i.Value.id);
            }
            spriteList.selectedFunc = OnSpriteSelected;
            foreach(var i in Game.GetMods()) {
                var item = spriteList.PushbackDefaultItem<Image>();
                item.sprite = i.Value[0];
                var btn = item.gameObject.AddComponent<Button>();
                btn.onClick.AddListener(() => { spriteList.Select(item.GetComponent<RectTransform>()); });
                var data = item.gameObject.AddComponent<UserData>();
                data.SetStringData(i.Key);
            }
        }

        private void OnWeaponListSelected(int index, bool select) {
            var item = weaponList[index].GetComponent<MonstersSelectItem>();
            item.Selected = select;
            if(select) {
                var ud = item.GetComponent<UserData>();
                var id = ud.GetIntegerData();
                var data = Game.Config.weapons[id];
                idText.text = "ID: " + data.id;
                nameBtnText.text = data.name;
                normalHitSpriteNameText.text = data.prefabPath;
                criticalHitSpriteNameText.text = data.critPrefabPath;
                normalHitSprite.sprite = Game.GetMods(data.prefabPath)[0];
                criticalHitSprite.sprite = Game.GetMods(data.critPrefabPath)[0];
                autoChanging = true;
                normalHitAudioSelect.value = data.audioId - 1;
                autoChanging = true;
                criticalHitAudioSelect.value = data.critAudioId - 1;
                selectedSprite.sprite = normalHitSprite.sprite;
            } else {
                SaveValues();
            }
        }

        private void OnSpriteSelected(int index, bool select) {
            var item = spriteList.SelectedItem;
            selectedSprite.sprite = item.GetComponent<Image>().sprite;
            selectedSprite.GetComponent<UserData>().SetStringData(item.GetComponent<UserData>().GetStringData());
        }

        public void SaveValues() {
            normalHitSprite.sprite = Game.GetMods(normalHitSpriteNameText.text)[0];
            criticalHitSprite.sprite = Game.GetMods(criticalHitSpriteNameText.text)[0];
            var ud = weaponList.SelectedItem.GetComponent<UserData>();
            var id = ud.GetIntegerData();
            var data = Game.Config.weapons[id];
            data.name = nameBtnText.text;
            data.prefabPath = normalHitSpriteNameText.text;
            data.critPrefabPath = criticalHitSpriteNameText.text;
            if(autoChanging) {
                autoChanging = false;
            } else {
                data.audioId = normalHitAudioSelect.value + 1;
                data.critAudioId = criticalHitAudioSelect.value + 1;
            }
        }

        public void OnSaveExit() {
            SaveValues();
            Present.Manager.AudioManager.StopAllSounds();
            Game.ShowDataEditor();
            Destroy(gameObject);
        }

        public void OnConfirmUpdateModalSprite(Text text) {
            text.text = selectedSprite.GetComponent<UserData>().GetStringData();
            SaveValues();
        }

        public void OnSelectWeaponName() {
            var panel = Instantiate(stringSelector, transform.parent).GetComponent<StringMakerPanel>();
            panel.SelectedKey = nameBtnText.text;
            panel.ApplyCallback = (string value) => {
                nameBtnText.text = value;
                weaponList.SelectedItem.GetComponent<DefaultSelectableElement>().text.text = Game.Config.StringInternational.GetValue(value);
                SaveValues();
            };
        }

        public void OnPlayMusic(Dropdown target) {
            Present.Manager.AudioManager.StopAllSounds();
            Present.Manager.AudioManager.PlaySound(target.value + 1);
        }

        public SelectListView weaponList;
        public SelectListView spriteList;

        public Text idText;
        public Text nameBtnText;
        public Text normalHitSpriteNameText;
        public Text criticalHitSpriteNameText;

        public Image normalHitSprite;
        public Image criticalHitSprite;
        public Dropdown normalHitAudioSelect;
        public Dropdown criticalHitAudioSelect;
        public Image selectedSprite;

        public GameObject stringSelector;

        private bool autoChanging = false;
    }
}
