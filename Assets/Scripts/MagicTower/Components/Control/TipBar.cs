using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArmyAnt.ViewUtil;

namespace MagicTower.Components.Control
{

    public class TipBar : ObjectPool.AViewUnit
    {
        public const string PREFAB_DIR = "TipBar";
        public const int PREFAB_ID = 3;

        public static TipBar ShowTip()
        {
            var ret = Game.ObjPool.GetAnElement<TipBar>(PREFAB_ID, ObjectPool.ElementType.Dialog, Model.Dirs.DIALOG_DIR + PREFAB_DIR);
            return ret;
        }

        // Use this for initialization
        void Awake()
        {
            tipsText.fontSize = System.Convert.ToInt32(tipsText.fontSize * Game.RealFontSize);
        }

        void FixedUpdate()
        {
            if (hideTime > 0)
            {
                hideTime--;
            }
            else if (hideTime == 0)
            {
                hideTime--;
                RecycleSelf();
            }
        }

        public void SetTipText(string content, bool silent = false)
        {
            tipsText.text = content;
            if (!silent)
            {
                Present.Manager.AudioManager.PlaySound(Present.Manager.AudioManager.itemGetSound);
            }
        }

        public void StartAutoRemove(int time)
        {
            hideTime = time;
        }

        public override ObjectPool.ElementType GetPoolTypeId()
        {
            return ObjectPool.ElementType.Dialog;
        }

        public override bool RecycleSelf()
        {
            return Game.ObjPoolRecycleSelf(this);
        }

        public override string ResourcePath { get { return Model.Dirs.DIALOG_DIR + PREFAB_DIR; } }

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

        public UnityEngine.UI.Text tipsText;
        private int hideTime = -1;
    }

}