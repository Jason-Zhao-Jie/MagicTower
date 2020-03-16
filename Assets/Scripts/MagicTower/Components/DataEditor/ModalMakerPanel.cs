using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArmyAnt.Algorithm;
using ArmyAnt.ViewUtil.Components;

namespace MagicTower.Components.DataEditor {
    public class ModalMakerPanel : MonoBehaviour {
        void Awake() {
            // 设定下拉框内容
            modalTypeDD.AddOptions(ExtendUtils.GetList(Unit.ModalType.Walkable, Unit.ModalType.SendingBlock));
            modalAnimateTypeDD.AddOptions(ExtendUtils.GetList(Unit.AnimType.Static, Unit.AnimType.Player));
            eventTypeDD.AddOptions(ExtendUtils.GetList(Present.Manager.EventManager.EventName.None, Present.Manager.EventManager.EventName.RemoveSelf));
            // 设定列表回调
            modalList.selectedFunc = OnModalSelected;
            spriteList.selectedFunc = OnSpriteSelected;
            eventDataList.selectedFunc = OnEventDataSelected;
            // 设定列表内容
            foreach(var i in Game.Config.modals) {
                var item = modalList.PushbackDefaultItem<DefaultSelectableElement>();
                item.text.text = i.Value.id + " " + Game.Config.StringInternational.GetValue(i.Value.name);
                item.AddOnclickEvent(() => { modalList.Select(item.GetComponent<RectTransform>()); });
                var data = item.gameObject.AddComponent<UserData>();
                data.SetIntegerData(i.Value.id);
            }
            foreach(var i in Game.GetMods()) {
                var item = spriteList.PushbackDefaultItem<Unit.Modal>(2f);
                item.OnCreate(ArmyAnt.ViewUtil.ObjectPool.ElementType.Image, 0, i.Value);
                var btn = item.gameObject.AddComponent<Button>();
                btn.onClick.AddListener(() => { spriteList.Select(item.GetComponent<RectTransform>()); });
                var data = item.gameObject.AddComponent<UserData>();
                data.SetStringData(i.Key);
            }
        }

        void OnModalSelected(int index, bool select) {
            var item = modalList[index].GetComponent<DefaultSelectableElement>();
            item.Selected = select;
            if(select) {
                var ud = item.GetComponent<UserData>();
                var data = Game.Config.modals[ud.GetIntegerData()];
                oldSpriteImage.GetComponent<Unit.Modal>().OnCreate(ArmyAnt.ViewUtil.ObjectPool.ElementType.Image, data.id, data);
                oldSpriteName.text = data.prefabPath;
                newSpriteImage.GetComponent<Unit.Modal>().OnCreate(ArmyAnt.ViewUtil.ObjectPool.ElementType.Image, data.id, data);
                newSpriteName.text = data.prefabPath;
                modalIDText.text = data.id.ToString();
                modalNameBtnText.text = data.name;
                modalTypeDD.value = data.typeId - 1;
                switch(data.animator) {
                    case "static":
                        modalAnimateTypeDD.value = (int)Unit.AnimType.Static;
                        break;
                    case "once":
                        modalAnimateTypeDD.value = (int)Unit.AnimType.Once;
                        break;
                    case "recycle":
                        modalAnimateTypeDD.value = (int)Unit.AnimType.Recycle;
                        break;
                    case "player":
                        modalAnimateTypeDD.value = (int)Unit.AnimType.Player;
                        break;
                }
                eventTypeDD.value = data.eventId;
                eventDataList.Clear();
                foreach(var i in data.eventData) {
                    var eventDataItem = eventDataList.PushbackDefaultItem<DefaultSelectableElement>();
                    eventDataItem.text.text = i.ToString();
                }
            } else {
                SaveValues();
            }
        }

        void OnSpriteSelected(int index, bool select) {
            if(select) {
                var item = spriteList[index];
                newSpriteImage.GetComponent<Unit.Modal>().OnCreate(ArmyAnt.ViewUtil.ObjectPool.ElementType.Image, 0, item.GetComponent<Unit.Modal>().GetSprites());
                newSpriteName.text = item.GetComponent<UserData>().GetStringData();
            }
        }

        void OnEventDataSelected(int index, bool select) {
            var item = eventDataList[index].GetComponent<DefaultSelectableElement>();
            item.Selected = select;
        }

        public void SaveValues() {
            var oldId = modalList.SelectedItem.GetComponent<UserData>().GetIntegerData();
            var cfg = Game.Config.modals[oldId];
            cfg.id = System.Convert.ToInt32(modalIDText.text);
            cfg.name = modalNameBtnText.text;
            cfg.prefabPath = oldSpriteName.text;
            cfg.typeId = modalTypeDD.value + 1;
            cfg.animator = modalAnimateTypeDD.captionText.text.ToLower();
            cfg.eventId = eventTypeDD.value;
            cfg.eventData = new long[eventDataList.ItemCount];
            for(var i = 0; i < eventDataList.ItemCount; ++i) {
                cfg.eventData[i] = System.Convert.ToInt64(eventDataList[i].GetComponent<DefaultSelectableElement>().text.text);
            }
            if(oldId != cfg.id) {
                Game.Config.modals.Remove(oldId);
                Game.Config.modals.Add(cfg.id, cfg);
                modalList.SelectedItem.GetComponent<UserData>().SetIntegerData(cfg.id);
            }
            modalList.SelectedItem.GetComponent<DefaultSelectableElement>().text.text = cfg.id + " " + Game.Config.StringInternational.GetValue(cfg.name);
        }

        public void OnSaveExit()
        {
            // 保存数据
            SaveValues();
            // 退出窗口
            Game.ShowDataEditor();
            Destroy(gameObject);
        }

        public void OnConfirmSetModalSprite() {
            oldSpriteName.text = newSpriteName.text;
            SaveValues();
            var id = System.Convert.ToInt32(modalIDText.text);
            oldSpriteImage.OnCreate(ArmyAnt.ViewUtil.ObjectPool.ElementType.Image, id, Game.Config.modals[id]);
        }

        public void OnConfirmChangeID() {
            var oldId = modalList.SelectedItem.GetComponent<UserData>().GetIntegerData();
            var newId = System.Convert.ToInt32(modalIDText.text);
            if(newId <= 0) {
                Game.ShowAlert("您修改了ID, 但修改后的ID: " + newId + " 必须是正整数, 修改失败!", TextAnchor.MiddleCenter);
                modalIDText.text = oldId.ToString();
            }
            if(oldId != newId && Game.Config.modals.ContainsKey(newId)) {
                Game.ShowAlert("您修改了ID, 但修改后的ID: " + newId + " 已被其他对象使用, 修改失败!", TextAnchor.MiddleCenter);
                modalIDText.text = oldId.ToString();
            }
            SaveValues();
        }

        public void OnSelectModalName() {
            var panel = Instantiate(stringSelector, transform.parent).GetComponent<StringMakerPanel>();
            panel.SelectedKey = modalNameBtnText.text;
            panel.ApplyCallback = (string value) => {
                modalNameBtnText.text = value;
                modalList.SelectedItem.GetComponent<DefaultSelectableElement>().text.text = Game.Config.StringInternational.GetValue(value);
                SaveValues();
            };
        }

        public void OnEditEventData()
        {
            var panel = Instantiate(eventEditor, transform.parent).GetComponent<EventEditorPanel>();
            var modalId = modalList.SelectedItem.GetComponent<UserData>().GetIntegerData();
            panel.EventId = Game.Config.modals[modalId].eventId;
            panel.SelectedKey = Game.Config.modals[modalId].eventData;
            panel.ApplyCallback = (long[] value) => {
                Game.Config.chats[modalId].eventData = value;
                modalList.Select(modalList.SelectedIndex);
                SaveValues();
            };
        }

        public SelectListView modalList;
        public SelectListView spriteList;
        public SelectListView eventDataList;

        public Text oldSpriteName;
        public Unit.Modal oldSpriteImage;
        public Text newSpriteName;
        public Unit.Modal newSpriteImage;
        public InputField modalIDText;
        public Text modalNameBtnText;

        public Button btnEditEventData;
        public Text btnSaveApplyText;

        public Dropdown modalTypeDD;
        public Dropdown modalAnimateTypeDD;
        public Dropdown eventTypeDD;

        public GameObject stringSelector;
        public GameObject eventEditor;

    }

}
