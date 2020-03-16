using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArmyAnt.Algorithm;
using ArmyAnt.ViewUtil.Components;

namespace MagicTower.Components.DataEditor {
    public class ChatEditorPanel : MonoBehaviour {
        private void Awake() {
            // 设定下拉框内容
            eventTypeSelect.AddOptions(ExtendUtils.GetList(Present.Manager.EventManager.EventName.None, Present.Manager.EventManager.EventName.RemoveSelf));
            speakerModal.gameObject.AddComponent<UserData>();
            // 设定列表回调
            chatList.selectedFunc = OnChatSelected;
            chatDataList.selectedFunc = OnDataSelected;
            eventDataList.selectedFunc = OnEventDataSelected;
            // 设定列表内容
            foreach(var i in Game.Config.chats) {
                var item = chatList.PushbackDefaultItem<DefaultSelectableElement>();
                item.GetComponent<Button>().onClick.AddListener(() => { chatList.Select(item.GetComponent<RectTransform>()); });
                item.text.text = i.Key.ToString();
            }
        }

        private void OnChatSelected(int index, bool select) {
            var item = chatList[index].GetComponent<DefaultSelectableElement>();
            item.Selected = select;
            idInput.text = item.text.text;
            var id = System.Convert.ToInt32(item.text.text);
            if(select) {
                var data = Game.Config.chats[id];
                canonToggle.isOn = data.canOn;
                chatDataList.Clear(false);
                foreach(var i in data.data) {
                    var chatDataItem = chatDataList.PushbackDefaultItem<DefaultSelectableElement>();
                    chatDataItem.GetComponent<Button>().onClick.AddListener(() => { chatDataList.Select(chatDataItem.GetComponent<RectTransform>()); });
                    chatDataItem.text.text = Game.Config.StringInternational.GetValue(i.content);
                }
                eventDataList.Clear(false);
                foreach(var i in data.eventData) {
                    var eventDataItem = eventDataList.PushbackDefaultItem<DefaultSelectableElement>();
                    item.GetComponent<Button>().onClick.AddListener(() => { eventDataList.Select(item.GetComponent<RectTransform>()); });
                    eventDataItem.text.text = i.ToString();
                }
                chatDataList.Select(0);
                eventTypeSelect.value = data.eventId;
            } else {
                SaveValues(id);
            }
        }

        private void OnDataSelected(int index, bool select) {
            var item = chatDataList[index].GetComponent<DefaultSelectableElement>();
            item.Selected = select;
            if(select) {
                var data = Game.Config.chats[System.Convert.ToInt32(chatList.SelectedItem.GetComponent<DefaultSelectableElement>().text.text)].data[index];
                contentStringText.text = data.content;
                switch(data.speakerId) {
                    case -255:
                        speakerTypeSelect.value = data.tipSilent ? 1 : 0;
                        break;
                    case -1:
                        speakerTypeSelect.value = 2;
                        break;
                    case 0:
                        speakerTypeSelect.value = 3;
                        break;
                    default:
                        speakerTypeSelect.value = 4;
                        break;
                }
                OnChangeSpeakerType();
            } else {
                SaveValues(-1, index);
            }
        }

        private void OnEventDataSelected(int index, bool select) {
            var item = eventDataList[index].GetComponent<DefaultSelectableElement>();
            item.Selected = select;
        }

        public void OnChangeSpeakerType() {
            speakerModalPanel.SetActive(speakerTypeSelect.value == 4);
            if(speakerTypeSelect.value == 4) {
                var speakerId = Game.Config.chats[System.Convert.ToInt32(chatList.SelectedItem.GetComponent<DefaultSelectableElement>().text.text)].data[chatDataList.SelectedIndex].speakerId;
                if(speakerId <= 0) {
                    speakerId = speakerModal.GetComponent<UserData>().GetIntegerData();
                    if(speakerId <= 0) {
                        speakerId = 1;
                    }
                }
                speakerModal.image.sprite = Game.GetMods(Game.Config.modals[speakerId].prefabPath)[0];
                speakerModal.text.text = Game.Config.StringInternational.GetValue(Game.Config.modals[speakerId].name);
                speakerModal.GetComponent<UserData>().SetIntegerData(speakerId);
            }
        }

        public void OnChangeEventType() {
            Game.Config.chats[System.Convert.ToInt32(chatList.SelectedItem.GetComponent<DefaultSelectableElement>().text.text)].eventId = eventTypeSelect.value;
        }

        public void SaveValues(int chatId = -1) {
            SaveValues(chatId, -1);
        }

        private void SaveValues(int chatId, int dataIndex) {
            if(chatId < 0) {
                chatId = System.Convert.ToInt32(chatList.SelectedItem.GetComponent<DefaultSelectableElement>().text.text);
            }
            if(dataIndex < 0) {
                dataIndex = chatDataList.SelectedIndex;
            }
            Game.Config.chats[chatId].canOn = canonToggle.isOn;
            Game.Config.chats[chatId].eventId = eventTypeSelect.value;
            Game.Config.chats[chatId].eventData = new long[eventDataList.ItemCount];
            for(var i = 0; i < eventDataList.ItemCount; ++i) {
                Game.Config.chats[chatId].eventData[i] = System.Convert.ToInt64(eventDataList[i].GetComponent<DefaultSelectableElement>().text.text);
            }
            Game.Config.chats[chatId].data[dataIndex].content = contentStringText.text;
            Game.Config.chats[chatId].data[dataIndex].tipSilent = speakerTypeSelect.value == 1;
            switch(speakerTypeSelect.value) {
                case 0:
                case 1:
                    Game.Config.chats[chatId].data[dataIndex].speakerId = -255;
                    break;
                case 2:
                    Game.Config.chats[chatId].data[dataIndex].speakerId = -1;
                    break;
                case 3:
                    Game.Config.chats[chatId].data[dataIndex].speakerId = 0;
                    break;
                case 4:
                    Game.Config.chats[chatId].data[dataIndex].speakerId = speakerModal.GetComponent<UserData>().GetIntegerData();
                    break;
            }
        }

        public void OnEditEventData() {
            var panel = Instantiate(eventEditor, transform.parent).GetComponent<EventEditorPanel>();
            var chatId = System.Convert.ToInt32(chatList.SelectedItem.GetComponent<DefaultSelectableElement>().text.text);
            panel.EventId = Game.Config.chats[chatId].eventId;
            panel.SelectedKey = Game.Config.chats[chatId].eventData;
            panel.ApplyCallback = (long[] value) => {
                Game.Config.chats[chatId].eventData = value;
                chatList.Select(chatList.SelectedIndex);
                SaveValues();
            };
        }

        public void OnEditContentString() {
            var panel = Instantiate(stringSelector, transform.parent).GetComponent<StringMakerPanel>();
            panel.SelectedKey = contentStringText.text;
            panel.ApplyCallback = (string value) => {
                contentStringText.text = value;
                chatDataList.SelectedItem.GetComponent<DefaultSelectableElement>().text.text = Game.Config.StringInternational.GetValue(value);
                SaveValues();
            };
        }

        public void OnSaveExit() {
            SaveValues();
            Game.ShowDataEditor();
            Destroy(gameObject);
        }

        public void OnSelectModal()
        {
            var panel = Instantiate(modalSelector, transform.parent).GetComponent<ModalSelectorDlg>();
            panel.SelectedKey = speakerModal.GetComponent<UserData>().GetIntegerData();
            panel.ApplyCallback = (int value) => {
                speakerModal.GetComponent<UserData>().SetIntegerData(value);
                speakerModal.image.sprite = Game.GetMods(Game.Config.modals[value].prefabPath)[0];
                speakerModal.text.text = Game.Config.StringInternational.GetValue(Game.Config.modals[value].name);
                SaveValues();
            };
        }

        public void OnChangeChatId() {
            var oldId = System.Convert.ToInt32(chatList.SelectedItem.GetComponent<DefaultSelectableElement>().text.text);
            var newId = System.Convert.ToInt32(idInput.text);
            if(newId <= 0) {
                Game.ShowAlert("您修改了ID, 但修改后的ID: " + newId + " 必须是正整数, 修改失败!", TextAnchor.MiddleCenter);
                idInput.text = oldId.ToString();
            }
            if(oldId != newId && Game.Config.chats.ContainsKey(newId)) {
                Game.ShowAlert("您修改了ID, 但修改后的ID: " + newId + " 已被其他对象使用, 修改失败!", TextAnchor.MiddleCenter);
                idInput.text = oldId.ToString();
            }
            if(oldId != newId) {
                Game.Config.chats.Add(newId, Game.Config.chats[oldId]);
                Game.Config.chats.Remove(oldId);
                chatList.SelectedItem.GetComponent<DefaultSelectableElement>().text.text = idInput.text;
            }
        }

        public void OnAddChat() {
            int id = 0;
            while(Game.Config.chats.ContainsKey(++id)) { }
            Game.Config.chats.Add(id, new Model.ChatData { id = id, canOn = true, eventId = 0, eventData = new long[] { 0 }, data = new Model.OneChatData[] { new Model.OneChatData { speakerId = -255, tipSilent = false, content = "str_ui_ok" } } });
            var item = chatList.PushbackDefaultItem<DefaultSelectableElement>();
            item.GetComponent<Button>().onClick.AddListener(() => { chatList.Select(item.GetComponent<RectTransform>()); });
            item.text.text = id.ToString();
            chatList.Select(item.GetComponent<RectTransform>());
            chatList.ScrollToItem(item.GetComponent<RectTransform>());
        }

        public void OnRemoveChat() {
            var chatId = System.Convert.ToInt32(chatList.SelectedItem.GetComponent<DefaultSelectableElement>().text.text);
            Game.Config.chats.Remove(chatId);
            chatList.DeleteItem(chatList.SelectedIndex);
        }

        public void OnAddData() {
            var chatId = System.Convert.ToInt32(chatList.SelectedItem.GetComponent<DefaultSelectableElement>().text.text);
            var chatData = Game.Config.chats[chatId];
            var data = chatData.data;
            chatData.data = new Model.OneChatData[data.Length - 1];
            for(var i = 0; i < chatDataList.ItemCount; ++i) {
                chatData.data[i] = data[i];
            }
            chatData.data[chatData.data.Length - 1] = new Model.OneChatData { speakerId = -255, tipSilent = false, content = "str_ui_ok" };
            var chatDataItem = chatDataList.PushbackDefaultItem<DefaultSelectableElement>();
            chatDataItem.GetComponent<Button>().onClick.AddListener(() => { chatDataList.Select(chatDataItem.GetComponent<RectTransform>()); });
            chatDataItem.text.text = Game.Config.StringInternational.GetValue("str_ui_ok");
            chatDataList.Select(chatData.data.Length - 1);
            chatDataList.ScrollToItem(chatData.data.Length - 1);
        }

        public void OnRemoveData() {
            if(chatDataList.ItemCount == 1) {
                Game.ShowAlert("只剩下一条对话了, 无法再删除, 你可以直接删除此条 chat 数据", TextAnchor.MiddleCenter);
                return;
            }
            var chatId = System.Convert.ToInt32(chatList.SelectedItem.GetComponent<DefaultSelectableElement>().text.text);
            var chatData = Game.Config.chats[chatId];
            var data = chatData.data;
            chatData.data = new Model.OneChatData[data.Length - 1];
            for(var i = 0; i < chatDataList.SelectedIndex; ++i) {
                chatData.data[i] = data[i];
            }
            for(var i = chatDataList.SelectedIndex; i < chatDataList.ItemCount - 1; ++i) {
                chatData.data[i] = data[i + 1];
            }
            chatDataList.DeleteItem(chatDataList.SelectedIndex);
        }

        public SelectListView chatList;
        public SelectListView chatDataList;
        public SelectListView eventDataList;

        public InputField idInput;
        public Toggle canonToggle;
        public Text contentStringText;
        public Dropdown speakerTypeSelect;
        public MonstersSelectItem speakerModal;
        public GameObject speakerModalPanel;
        public Dropdown eventTypeSelect;

        public Text btnSaveApplyText;
        public GameObject stringSelector;
        public GameObject eventEditor;
        public GameObject modalSelector;
    }
}

