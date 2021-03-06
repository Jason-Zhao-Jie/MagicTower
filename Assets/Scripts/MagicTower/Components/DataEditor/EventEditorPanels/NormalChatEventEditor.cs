﻿using ArmyAnt.ViewUtil.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MagicTower.Components.DataEditor.EventEditorPanels
{
    public class NormalChatEventEditor : MonoBehaviour
    {
        // param0: chatDataIndex

        void Start() {
            chatList.selectedFunc = OnSelect;
            var c = GetComponent<Common_EventEditor>();
            long selectedId = 0;
            if(c.parent.SelectedKey.Length > 0) {
                selectedId = c.parent.SelectedKey[0];
            }
            foreach(var i in Game.Config.chats) {
                var item = chatList.PushbackDefaultItem<DefaultSelectableElement>();
                item.text.text = i.Value.id.ToString();
                item.AddOnclickEvent(() => { chatList.Select(item.GetComponent<RectTransform>()); });
                var data = item.gameObject.AddComponent<UserData>();
                data.SetIntegerData(i.Value.id);
                if(i.Value.id == selectedId) {
                    chatList.Select(item.GetComponent<RectTransform>());
                }
            }
        }

        public void OnSelect(int index, bool select)
        {
            if(select) {
                Game.DebugLogNote("NormalChatEventEditor select");
                var id = chatList.SelectedItem.GetComponent<UserData>().GetIntegerData();
                GetComponent<Common_EventEditor>().parent.SelectedKey = new long[]{id};
                contentList.Clear();
                foreach(var i in Game.Config.chats[id].data) {
                    contentList.PushbackDefaultItem<DefaultSelectableElement>().text.text = Game.Config.StringInternational.GetValue(i.content);
                }
            }
        }

        public SelectListView chatList;
        public SelectListView contentList;
    }

}
