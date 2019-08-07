using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArmyAnt.Algorithm;
using ArmyAnt.ViewUtil.Components;

namespace MagicTower.Components.DataEditor {
    public class StringMakerPanel : MonoBehaviour {
        private void Awake() {
            for(var i = 1; i <= Game.Config.languages.Count; ++i) {
                languageNameTexts[i - 1].text = Game.Config.languages[i].key;
            }
            allData = Game.Config.StringInternational.GetAllValues(true);
            stringList.selectedFunc = OnStringSelected;
            foreach(var i in allData) {
                var item = stringList.PushbackDefaultItem<DefaultSelectableElement>();
                item.text.text = i.Key;
                item.AddOnclickEvent(() => { stringList.Select(item.GetComponent<RectTransform>()); });
            }
        }

        private void OnStringSelected(int index, bool select) {
            var item = stringList[index].GetComponent<DefaultSelectableElement>();
            item.Selected = select;
            if(select) {
                var key = stringList[index].GetComponent<DefaultSelectableElement>().text.text;
                stringKeyInput.text = key;
                for(var i = 0; i < stringValueInputs.Length; ++i) {
                    if(!allData[key].ContainsKey(i + 1)) {
                        allData[key].Add(i + 1, "");
                    }
                    stringValueInputs[i].text = allData[key][i + 1];
                }
            } else {
                SaveValues(stringList[index].GetComponent<DefaultSelectableElement>().text.text);
            }
        }

        public void SaveValues(string key = null) {
            if(string.IsNullOrEmpty(key)) {
                key = stringList.SelectedItem.GetComponent<DefaultSelectableElement>().text.text;
            }
            allData[key].Clear();
            for(var i = 0; i < stringValueInputs.Length; ++i) {
                allData[key][i + 1] = stringValueInputs[i].text;
            }
        }

        public void OnEndKeyEdit() {
            var oldKey = stringList.SelectedItem.GetComponent<DefaultSelectableElement>().text.text;
            var newKey = stringKeyInput.text;
            if(string.IsNullOrEmpty(newKey)) {
                Game.ShowAlert("您修改了该字符串的key, 但key不能为空, 修改失败!", TextAnchor.MiddleCenter);
                stringKeyInput.text = oldKey;
            }
            if(!string.Equals(oldKey, newKey) && allData.ContainsKey(newKey)) {
                Game.ShowAlert("您修改了该字符串的key, 但修改后的key: " + newKey + " 已被其他字符串使用, 修改失败!", TextAnchor.MiddleCenter);
                stringKeyInput.text = oldKey;
            }
            allData.Remove(oldKey);
            allData.Add(newKey, new Dictionary<int, string>());
            stringList.SelectedItem.GetComponent<DefaultSelectableElement>().text.text = newKey;
            SaveValues(newKey);
        }

        public void OnAddString() {
            var defaultKey = "str_new";
            while(allData.ContainsKey(defaultKey)) {
                defaultKey = defaultKey + '+';
            }
            allData.Add(defaultKey, new Dictionary<int, string>());
            var item = stringList.PushbackDefaultItem<DefaultSelectableElement>();
            item.text.text = defaultKey;
            item.AddOnclickEvent(() => { stringList.Select(item.GetComponent<RectTransform>()); });
            stringList.Select(item.GetComponent<RectTransform>());
            stringKeyInput.ActivateInputField();
        }

        public void OnRemoveString() {
            Game.ShowAlert("删除字符串可能导致部分游戏字符串内容缺失, 确定删除此字符串?", TextAnchor.MiddleCenter, () => {
                var key = stringList.SelectedItem.GetComponent<DefaultSelectableElement>().text.text;
                allData.Remove(key);
                stringList.DeleteItem(stringList.SelectedIndex);
                return true;
            });
        }

        public void OnSaveExitApply() {
            // 保存数据
            SaveValues();
            // 保存所有数据至文件
            var fileData = Game.Config.SaveStrings(allData);
            foreach(var i in Game.Config.languages) {
                ArmyAnt.Manager.IOManager.MkdirIfNotExist("Strings");
                ArmyAnt.Manager.IOManager.SaveToFile(System.Text.Encoding.UTF8.GetBytes(fileData[i.Key]), "Strings", i.Value.path + ".json");
            }
            Game.Config.StringInternational.UsingSaveData = true;
            // 返回选择值
            if(ApplyCallback == null) {
                Game.ShowDataEditor();
            } else {
                ApplyCallback(SelectedKey);
            }
            // 退出窗口
            Destroy(gameObject);
        }

        public string SelectedKey {
            get {
                return stringList.SelectedItem.GetComponent<DefaultSelectableElement>().text.text;
            }
            set {
                if(!string.IsNullOrEmpty(value)) {
                    btnSaveApplyText.text = "Save & Apply";
                    foreach(var i in stringList) {
                        if(string.Equals(i.GetComponent<DefaultSelectableElement>().text.text, value)) {
                            stringList.Select(i);
                            stringList.ScrollToItem(i);
                        }
                    }
                }
            }
        }

        public System.Action<string> ApplyCallback { get; set; }

        public SelectListView stringList;
        public Text btnSaveApplyText;
        public Text[] languageNameTexts;

        public InputField stringKeyInput;
        public InputField[] stringValueInputs;

        private Dictionary<string, Dictionary<int, string>> allData;
    }
}
