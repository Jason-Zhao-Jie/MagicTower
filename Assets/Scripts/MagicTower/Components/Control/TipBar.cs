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

        private void OnDisable() {
            Game.Status = lastStatus;
        }

        private void OnDestroy() {
            Game.Status = lastStatus;
        }

        public void Init(string content, bool silent = false)
        {
            tipsText.text = content;
            if (!silent)
            {
                Present.Manager.AudioManager.PlaySound(Present.Manager.AudioManager.itemGetSound);
            }
            lastStatus = Game.Status;
            if(Game.Status == Model.EGameStatus.Start) {
                StartAutoRemove(150);
            } else { 
                Game.Status = Model.EGameStatus.OnTipChat;
            }
        }

        public void Init(string content, int autoRemoveTime, bool silent = false) {
            Init(content, silent);
            StartAutoRemove(autoRemoveTime);
        }

        public void StartAutoRemove(int time)
        {
            hideTime = time;
        }
        
        public UnityEngine.UI.Text tipsText;
        private int hideTime = -1;
        private Model.EGameStatus lastStatus;
    }

}