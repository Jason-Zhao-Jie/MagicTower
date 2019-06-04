using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArmyAnt.ViewUtil;

namespace MagicTower.Components.Control
{

    public class TipBar : MonoBehaviour
    {
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
                Game.HideUI(UIType.TipBar);
            }
        }

        public void Init(string content, bool silent = false)
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
        
        public UnityEngine.UI.Text tipsText;
        private int hideTime = -1;
    }

}