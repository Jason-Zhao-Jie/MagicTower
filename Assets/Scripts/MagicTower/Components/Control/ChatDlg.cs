using System;
using UnityEngine;
using ArmyAnt.ViewUtil;
using MagicTower.Components.Unit;

namespace MagicTower.Components.Control
{
    public class ChatDlg : MonoBehaviour
    {
        // Use this for initialization
        void Awake()
        {
            speakerText.fontSize = Convert.ToInt32(speakerText.fontSize * Game.RealFontSize);
            content.fontSize = Convert.ToInt32(content.fontSize * Game.RealFontSize);
        }

        public void Init(string content, int speakerId = -1)
        {
            // 查找对话者数据, 贴上人物头像
            if (speakerId < 0)
                speakerId = Game.Player.PlayerId;
            var modal = Game.Config.modals[speakerId];
            speaker.OnInit(ObjectPool.ElementType.Image, modal.id, modal);
            // 对话者名字
            speakerText.text = Game.Config.StringInternational.GetValue(modal.name);
            // 对话内容
            this.content.text = content;
        }
        
        public Modal speaker;
        public UnityEngine.UI.Text speakerText;
        public UnityEngine.UI.Text content;
    }

}