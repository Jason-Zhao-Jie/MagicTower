using ArmyAnt.ViewUtil.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MagicTower.Components.DataEditor.EventEditorPanels
{
    public class NormalChatEventEditor : MonoBehaviour
    {
        // param0: chatDataIndex

        void Awake()
        {
            chatList.selectedFunc = OnSelect;
            foreach (var i in Game.Config.chats)
            {
                var item = chatList.PushbackDefaultItem<DefaultSelectableElement>();
                item.text.text = i.Value.id.ToString();
                var data = item.gameObject.AddComponent<UserData>();
                data.SetIntegerData(i.Value.id);
            }
        }

        public void OnSelect(int index, bool select)
        {
            var id = chatList.SelectedItem.GetComponent<UserData>().GetIntegerData();
            GetComponent<Common_EventEditor>().parent.SelectedKey[0] = id;
            foreach (var i in Game.Config.chats[id].data)
            {
                contentList.PushbackDefaultItem<DefaultSelectableElement>().text.text = Game.Config.StringInternational.GetValue(i.content);
            }
        }

        public SelectListView chatList;
        public SelectListView contentList;
    }

}
