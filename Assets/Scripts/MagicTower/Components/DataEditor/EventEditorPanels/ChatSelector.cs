using ArmyAnt.ViewUtil.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MagicTower.Components.DataEditor.EventEditorPanels
{
    public class ChatSelector : MonoBehaviour
    {
        // param0: chatDataIndex

        void Awake()
        {
            chatList.selectedFunc = OnSelect;
            foreach (var i in Game.Config.chats)
            {
                var item = chatList.PushbackDefaultItem<DefaultSelectableElement>();
                item.text.text = i.Value.id.ToString();
            }
        }

        public void OnSelect(int index, bool select)
        {

        }

        public SelectListView chatList;
        public SelectListView contentList;
    }

}
