using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MagicTower.Components.Unit {

    public class VirtualRocker : MonoBehaviour {
        // Start is called before the first frame update
        protected virtual void Awake() {
            spriteList = new Dictionary<KeyCode, Sprite>() {
                { default, NormalImage},
                { Present.Manager.InputManager.VirtualKeyCode.Up, UpImage},
                { Present.Manager.InputManager.VirtualKeyCode.Down, DownImage},
                { Present.Manager.InputManager.VirtualKeyCode.Left, LeftImage},
                { Present.Manager.InputManager.VirtualKeyCode.Right, RightImage},
            };
        }

        protected virtual void Update() {
            if(!Game.Settings.Settings.virtualJoysticksOn || Game.Status == Model.EGameStatus.Start || Game.Status == Model.EGameStatus.InEditor) {
                btnShowHide.gameObject.SetActive(false);
                rocker.gameObject.SetActive(false);
            } else {
                btnShowHide.gameObject.SetActive(true);
                rocker.gameObject.SetActive(true);
                transform.SetAsLastSibling();
                if(cancelForceVirtualingTime > 0) {
                    --cancelForceVirtualingTime;
                } else if(cancelForceVirtualingTime == 0) {
                    --cancelForceVirtualingTime;
                    Game.Input.forceVirtualing = false;
                }
                rocker.enabled = shown && Game.Status == Model.EGameStatus.InGame;
                btnShowHide.GetComponent<Image>().sprite = rocker.enabled ? btnHideImage : btnShowImage;
                if(rocker.enabled && rocker.sprite != spriteList[lastDowned]) {
                    rocker.sprite = spriteList[lastDowned];
                }
            }
        }

        public void OnTouchDown(UnityEngine.EventSystems.BaseEventData data) {
            var p = data as UnityEngine.EventSystems.PointerEventData;
            var selfPos = Camera.main.WorldToScreenPoint(rocker.transform.position);
            var leftright = p.position.x - selfPos.x;
            var downup = p.position.y - selfPos.y;
            if(Mathf.Abs(leftright) <= Mathf.Abs(downup)) {
                leftright = 0;
            } else {
                downup = 0;
            }
            KeyCode downed;
            if(downup > 0) {
                downed = Present.Manager.InputManager.VirtualKeyCode.Up;
            } else if(downup < 0) {
                downed = Present.Manager.InputManager.VirtualKeyCode.Down;
            } else if(leftright > 0) {
                downed = Present.Manager.InputManager.VirtualKeyCode.Right;
            } else if(leftright < 0) {
                downed = Present.Manager.InputManager.VirtualKeyCode.Left;
            } else {
                downed = default;
            }
            if(downed != lastDowned) {
                if(lastDowned != default) {
                    Game.Input.CallKeyUp(lastDowned, true);
                }
                if(downed != default) {
                    Game.Input.CallKeyDown(downed, true);
                }
                lastDowned = downed;
            }
            cancelForceVirtualingTime = -1;
            Game.Input.forceVirtualing = true;
        }

        public void OnTouchUp(UnityEngine.EventSystems.BaseEventData data) {
            if(lastDowned != default) {
                Game.Input.CallKeyUp(lastDowned, true);
                lastDowned = default;
            }
            cancelForceVirtualingTime = 10;
        }

        public void OnShowHide() {
            shown = !shown;
        }

        public Sprite NormalImage;
        public Sprite UpImage;
        public Sprite DownImage;
        public Sprite LeftImage;
        public Sprite RightImage;

        public Image rocker;
        public Button btnShowHide;

        public Sprite btnHideImage;
        public Sprite btnShowImage;

        public bool shown;

        private KeyCode lastDowned;
        private Dictionary<KeyCode, Sprite> spriteList;
        private int cancelForceVirtualingTime;
    }

}
