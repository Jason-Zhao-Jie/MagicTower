using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MagicTower.Components.Control {

    public class InfoDlg : MonoBehaviour {
        // Start is called before the first frame update
        void Awake() {
            if(showed) {
                content.text = contentStr;
                btnText.text = btnStr;
            }
        }

        public void Init(string contentStr, string btnStr = "OK", Model.EmptyBoolCallBack btnCallback = null) {
            // 设定信息
            this.contentStr = contentStr;
            this.btnStr = btnStr;
            this.btnCallback = btnCallback;
            if(isActiveAndEnabled) {
                content.text = contentStr;
                btnText.text = btnStr;
            }
            showed = true;
            lastStatus = Game.Status;
            Game.Status = Model.EGameStatus.InEditorDialog;
        }

        public void OnBtnClick() {
            if(btnCallback == null || btnCallback()) {
                Game.Status = lastStatus;
                Game.HideUI(UIType.AlertDialog);
            }
        }

        public Text content;
        public Button btn;
        public Text btnText;

        private bool showed = false;
        private string contentStr;
        private Model.EmptyBoolCallBack btnCallback;
        private string btnStr;
        private Model.EGameStatus lastStatus;
    }

}
