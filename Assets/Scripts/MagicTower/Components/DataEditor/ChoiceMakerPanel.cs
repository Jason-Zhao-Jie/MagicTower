using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArmyAnt.Algorithm;
using ArmyAnt.ViewUtil.Components;

namespace MagicTower.Components.DataEditor
{
    public class ChoiceMakerPanel : MonoBehaviour
    {
        private void Awake()
        {
            // 设定下拉框内容
            contentTypeSelect.AddOptions(ExtendUtils.GetList(Game.VariablePriceType.NoChange, Game.VariablePriceType.KeyStoreDoubling));
            eventTypeSelect.AddOptions(ExtendUtils.GetList(Present.Manager.EventManager.EventName.None, Present.Manager.EventManager.EventName.RemoveSelf));
            // 设定列表回调
            choiceList.selectedFunc = OnChoiceSelected;
            choiceDataList.selectedFunc = OnDataSelected;
            eventDataList.selectedFunc = OnEventDataSelected;
            // 设定列表内容
            foreach (var i in Game.Config.choices)
            {
                var item = choiceList.PushbackDefaultItem<DefaultSelectableElement>();
                item.GetComponent<Button>().onClick.AddListener(() => { choiceList.Select(item.GetComponent<RectTransform>()); });
                item.text.text = i.Key.ToString();
            }
        }

        private void OnChoiceSelected(int index, bool select)
        {
            var item = choiceList[index].GetComponent<DefaultSelectableElement>();
            item.Selected = select;
            idInput.text = item.text.text;
            var id = System.Convert.ToInt32(item.text.text);
            if (select)
            {
                var data = Game.Config.choices[id];
                switch (data.speakerId)
                {
                    case -255:
                        speakerTypeSelect.value = 0;
                        break;
                    case -1:
                        speakerTypeSelect.value = 1;
                        break;
                    case 0:
                        speakerTypeSelect.value = 2;
                        break;
                    default:
                        speakerTypeSelect.value = 3;
                        break;
                }
                canonToggle.isOn = data.canOn;
                titleStringText.text = data.title;
                tailStringText.text = data.tail;
                OnChangeSpeakerType();
                choiceDataList.Clear(false);
                foreach (var i in data.data)
                {
                    var choiceDataItem = choiceDataList.PushbackDefaultItem<DefaultSelectableElement>();
                    choiceDataItem.GetComponent<Button>().onClick.AddListener(() => { choiceDataList.Select(choiceDataItem.GetComponent<RectTransform>()); });
                    choiceDataItem.text.text = Game.Config.StringInternational.GetValue(i.content);
                }
                choiceDataList.Select(0);
            }
            else
            {
                SaveValues(id);
            }
        }

        private void OnDataSelected(int index, bool select)
        {
            var item = choiceDataList[index].GetComponent<DefaultSelectableElement>();
            item.Selected = select;
            if (select)
            {
                var data = Game.Config.choices[System.Convert.ToInt32(choiceList.SelectedItem.GetComponent<DefaultSelectableElement>().text.text)].data[index];
                contentStringText.text = data.content;
                contentTypeSelect.value = data.contentType;
                willClose.isOn = data.close;
                eventTypeSelect.value = data.eventId;
                eventDataList.Clear(false);
                foreach (var i in data.eventData)
                {
                    var eventDataItem = eventDataList.PushbackDefaultItem<DefaultSelectableElement>();
                    item.GetComponent<Button>().onClick.AddListener(() => { eventDataList.Select(item.GetComponent<RectTransform>()); });
                    eventDataItem.text.text = i.ToString();
                }
            }
            else
            {
                SaveValues(-1, index);
            }
        }

        private void OnEventDataSelected(int index, bool select)
        {
            var item = eventDataList[index].GetComponent<DefaultSelectableElement>();
            item.Selected = select;
        }

        public void OnChangeSpeakerType()
        {
            speakerModalPanel.SetActive(speakerTypeSelect.value == 3);
            if (speakerTypeSelect.value == 3)
            {
                var speakerId = Game.Config.choices[System.Convert.ToInt32(choiceList.SelectedItem.GetComponent<DefaultSelectableElement>().text.text)].speakerId;
                if (speakerId <= 0)
                {
                    speakerId = speakerModal.GetComponent<UserData>().GetIntegerData();
                    if (speakerId <= 0)
                    {
                        speakerId = 1;
                    }
                }
                speakerModal.image.sprite = Game.GetMods(Game.Config.modals[speakerId].prefabPath)[0];
                speakerModal.text.text = Game.Config.StringInternational.GetValue(Game.Config.modals[speakerId].name);
                speakerModal.GetComponent<UserData>().SetIntegerData(speakerId);
            }
        }

        public void OnChangeEventType()
        {
            Game.Config.choices[System.Convert.ToInt32(choiceList.SelectedItem.GetComponent<DefaultSelectableElement>().text.text)].data[choiceDataList.SelectedIndex].eventId = eventTypeSelect.value;
        }

        public void SaveValues(int choiceId = -1)
        {
            SaveValues(choiceId, -1);
        }

        private void SaveValues(int choiceId, int dataIndex)
        {
            if (choiceId < 0)
            {
                choiceId = System.Convert.ToInt32(choiceList.SelectedItem.GetComponent<DefaultSelectableElement>().text.text);
            }
            if (dataIndex < 0)
            {
                dataIndex = choiceDataList.SelectedIndex;
            }
            switch (speakerTypeSelect.value)
            {
                case 0:
                    Game.Config.choices[choiceId].speakerId = -255;
                    break;
                case 1:
                    Game.Config.choices[choiceId].speakerId = -1;
                    break;
                case 2:
                    Game.Config.choices[choiceId].speakerId = 0;
                    break;
                case 3:
                    Game.Config.choices[choiceId].speakerId = speakerModal.GetComponent<UserData>().GetIntegerData();
                    break;
            }
            Game.Config.choices[choiceId].canOn = canonToggle.isOn;
            Game.Config.choices[choiceId].title = titleStringText.text;
            Game.Config.choices[choiceId].tail = tailStringText.text;

            Game.Config.choices[choiceId].data[dataIndex].contentType = contentTypeSelect.value;
            Game.Config.choices[choiceId].data[dataIndex].content = contentStringText.text;
            Game.Config.choices[choiceId].data[dataIndex].eventId = eventTypeSelect.value;
            Game.Config.choices[choiceId].data[dataIndex].eventData = new long[eventDataList.ItemCount];
            for (var i = 0; i < eventDataList.ItemCount; ++i)
            {
                Game.Config.choices[choiceId].data[dataIndex].eventData[i] = System.Convert.ToInt64(eventDataList[i].GetComponent<DefaultSelectableElement>().text.text);
            }
            Game.Config.choices[choiceId].data[dataIndex].close = willClose.isOn;
        }

        public void OnEditEventData()
        {
            var panel = Instantiate(eventEditor, transform.parent).GetComponent<EventEditorPanel>();
            var choiceId = System.Convert.ToInt32(choiceList.SelectedItem.GetComponent<DefaultSelectableElement>().text.text);
            panel.EventId = Game.Config.choices[choiceId].data[choiceDataList.SelectedIndex].eventId;
            panel.SelectedKey = Game.Config.choices[choiceId].data[choiceDataList.SelectedIndex].eventData;
            panel.ApplyCallback = (long[] value) => {
                Game.Config.choices[choiceId].data[choiceDataList.SelectedIndex].eventData = value;
                choiceDataList.Select(choiceDataList.SelectedIndex);
                SaveValues();
            };
        }

        public void OnEditTitleString()
        {
            var panel = Instantiate(stringSelector, transform.parent).GetComponent<StringMakerPanel>();
            panel.SelectedKey = titleStringText.text;
            panel.ApplyCallback = (string value) =>
            {
                titleStringText.text = value;
                SaveValues();
            };
        }

        public void OnEditTailString()
        {
            var panel = Instantiate(stringSelector, transform.parent).GetComponent<StringMakerPanel>();
            panel.SelectedKey = tailStringText.text;
            panel.ApplyCallback = (string value) =>
            {
                tailStringText.text = value;
                SaveValues();
            };
        }

        public void OnEditContentString()
        {
            var panel = Instantiate(stringSelector, transform.parent).GetComponent<StringMakerPanel>();
            panel.SelectedKey = contentStringText.text;
            panel.ApplyCallback = (string value) =>
            {
                contentStringText.text = value;
                choiceDataList.SelectedItem.GetComponent<DefaultSelectableElement>().text.text = Game.Config.StringInternational.GetValue(value);
                SaveValues();
            };
        }

        public void OnEditContentData()
        {
            // todo
        }

        public void OnSaveExit()
        {
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

        public void OnChangeChoiceId()
        {
            var oldId = System.Convert.ToInt32(choiceList.SelectedItem.GetComponent<DefaultSelectableElement>().text.text);
            var newId = System.Convert.ToInt32(idInput.text);
            if (newId <= 0)
            {
                Game.ShowAlert("您修改了ID, 但修改后的ID: " + newId + " 必须是正整数, 修改失败!", TextAnchor.MiddleCenter);
                idInput.text = oldId.ToString();
            }
            if (oldId != newId && Game.Config.choices.ContainsKey(newId))
            {
                Game.ShowAlert("您修改了ID, 但修改后的ID: " + newId + " 已被其他对象使用, 修改失败!", TextAnchor.MiddleCenter);
                idInput.text = oldId.ToString();
            }
            if (oldId != newId)
            {
                Game.Config.choices.Add(newId, Game.Config.choices[oldId]);
                Game.Config.choices.Remove(oldId);
                choiceList.SelectedItem.GetComponent<DefaultSelectableElement>().text.text = idInput.text;
            }
        }

        public void OnAddChoice()
        {
            int id = 0;
            while (Game.Config.choices.ContainsKey(++id)) { }
            Game.Config.choices.Add(id, new Model.ChoiceData { id = id, speakerId = -255, title = "", tail = "", canOn = true,  data = new Model.OneChoiceData[] { new Model.OneChoiceData {  contentType = 0, content = "str_ui_ok", contentData = new string[] { }, eventId = 0, eventData = new long[] { 0 }, close = true} } });
            var item = choiceList.PushbackDefaultItem<DefaultSelectableElement>();
            item.GetComponent<Button>().onClick.AddListener(() => { choiceList.Select(item.GetComponent<RectTransform>()); });
            item.text.text = id.ToString();
            choiceList.Select(item.GetComponent<RectTransform>());
            choiceList.ScrollToItem(item.GetComponent<RectTransform>());
        }

        public void OnRemoveChoice()
        {
            var id = System.Convert.ToInt32(choiceList.SelectedItem.GetComponent<DefaultSelectableElement>().text.text);
            Game.Config.choices.Remove(id);
            choiceList.DeleteItem(choiceList.SelectedIndex);
        }

        public void OnAddData()
        {
            var id = System.Convert.ToInt32(choiceList.SelectedItem.GetComponent<DefaultSelectableElement>().text.text);
            var choiceData = Game.Config.choices[id];
            var data = choiceData.data;
            choiceData.data = new Model.OneChoiceData[data.Length - 1];
            for (var i = 0; i < choiceDataList.ItemCount; ++i)
            {
                choiceData.data[i] = data[i];
            }
            choiceData.data[choiceData.data.Length - 1] = new Model.OneChoiceData { contentType = 0, content = "str_ui_ok", contentData = new string[] { }, eventId = 0, eventData = new long[] { 0 }, close = true };
            var choiceDataItem = choiceDataList.PushbackDefaultItem<DefaultSelectableElement>();
            choiceDataItem.GetComponent<Button>().onClick.AddListener(() => { choiceDataList.Select(choiceDataItem.GetComponent<RectTransform>()); });
            choiceDataItem.text.text = Game.Config.StringInternational.GetValue("str_ui_ok");
            choiceDataList.Select(choiceData.data.Length - 1);
            choiceDataList.ScrollToItem(choiceData.data.Length - 1);
        }

        public void OnRemoveData()
        {
            if (choiceDataList.ItemCount == 1)
            {
                Game.ShowAlert("只剩下一条选项了, 无法再删除, 你可以直接删除此条 choice 数据", TextAnchor.MiddleCenter);
                return;
            }
            var id = System.Convert.ToInt32(choiceList.SelectedItem.GetComponent<DefaultSelectableElement>().text.text);
            var choiceData = Game.Config.choices[id];
            var data = choiceData.data;
            choiceData.data = new Model.OneChoiceData[data.Length - 1];
            for (var i = 0; i < choiceDataList.SelectedIndex; ++i)
            {
                choiceData.data[i] = data[i];
            }
            for (var i = choiceDataList.SelectedIndex; i < choiceDataList.ItemCount - 1; ++i)
            {
                choiceData.data[i] = data[i + 1];
            }
            choiceDataList.DeleteItem(choiceDataList.SelectedIndex);
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
        public GameObject eventEditor;
        public GameObject modalSelector;
    }

}
