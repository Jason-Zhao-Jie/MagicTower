using System;
using UnityEngine;
using ArmyAnt.ViewUtil;
using MagicTower.Components.Unit;

namespace MagicTower.Components.Control
{

    public class ChatDlg : ObjectPool.AViewUnit
    {
        public const string TOP_PREFAB_DIR = "TopChat";
        public const int TOP_PREFAB_ID = 4;
        public const string BOTTOM_PREFAB_DIR = "BottomChat";
        public const int BOTTOM_PREFAB_ID = 5;

        public static ChatDlg ShowChat(bool isTop)
        {
            string dir;
            int id;
            if (isTop)
            {
                dir = TOP_PREFAB_DIR;
                id = TOP_PREFAB_ID;
            }
            else
            {
                dir = BOTTOM_PREFAB_DIR;
                id = BOTTOM_PREFAB_ID;
            }
            var ret = Game.ObjPool.GetAnElement<ChatDlg>(id, ObjectPool.ElementType.Dialog, Model.Dirs.DIALOG_DIR + dir);
            ret.isTop = isTop;
            return ret;
        }

        // Use this for initialization
        void Awake()
        {
            speaker = transform.Find("Speaker").gameObject;
            speakerText = transform.Find("SpeakerName").GetComponent<UnityEngine.UI.Text>();
            speakerText.fontSize = Convert.ToInt32(speakerText.fontSize * Game.RealFontSize);
            text = transform.Find("Text").GetComponent<UnityEngine.UI.Text>();
            text.fontSize = Convert.ToInt32(text.fontSize * Game.RealFontSize);
        }

        public void SetChat(string content, int speakerId = -1)
        {
            // 查找对话者数据, 贴上人物头像
            if (speakerId < 0)
                speakerId = Game.Player.PlayerId;
            var modal = Game.Config.modals[speakerId];
            ObjectPool.AViewUnit obj = null;
            if ((ModalType)modal.typeId == ModalType.Player)
            {
                obj = Game.ObjPool.GetAnElement<Player>(modal.id, ObjectPool.ElementType.Sprite, Model.Dirs.PREFAB_DIR + modal.prefabPath, ObjectPool.SPRITE_IN_DIALOG_SORTING_ORDER);
            }
            else
            {
                obj = Game.ObjPool.GetAnElement<Modal>(modal.id, ObjectPool.ElementType.Sprite, Model.Dirs.PREFAB_DIR + modal.prefabPath, ObjectPool.SPRITE_IN_DIALOG_SORTING_ORDER);
            }
            obj.transform.SetParent(speaker.transform.parent, false);
            obj.transform.position = speaker.transform.position;
            obj.transform.localScale = Game.Map.ModalLocalScale;
            var mod = speaker.GetComponent<Modal>();
            if (mod != null)
                mod.RemoveSelf(false);
            else if (speaker.GetComponent<Player>() != null)
                speaker.GetComponent<Player>().RemoveSelf();
            else
                Destroy(speaker);
            speaker = obj.gameObject;
            // 对话者名字
            speakerText.text = Game.Config.StringInternational.GetValue(modal.name);
            // 对话内容
            text.text = content;
        }

        public override ObjectPool.ElementType GetPoolTypeId()
        {
            return ObjectPool.ElementType.Dialog;
        }

        public override bool RecycleSelf()
        {
            return Game.ObjPoolRecycleSelf(this);
        }

        public override string ResourcePath
        {
            get
            {
                return Model.Dirs.DIALOG_DIR + (isTop ? TOP_PREFAB_DIR : BOTTOM_PREFAB_DIR);
            }
        }

        public override bool OnCreate(ObjectPool.ElementType tid, int elemId, string resourcePath)
        {
            return true;
        }

        public override void OnReuse(ObjectPool.ElementType tid, int elemId)
        {
        }

        public override bool OnUnuse(ObjectPool.ElementType tid, int elemId)
        {
            return true;
        }

        private GameObject speaker;
        private UnityEngine.UI.Text speakerText;
        private UnityEngine.UI.Text text;
        private bool isTop = false;
    }

}