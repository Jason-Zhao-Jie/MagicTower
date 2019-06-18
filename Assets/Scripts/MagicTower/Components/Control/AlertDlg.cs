using ArmyAnt.ViewUtil;
using UnityEngine;
using UnityEngine.UI;

namespace MagicTower.Components.Control {

    public class AlertDlg : MonoBehaviour {
        // Start is called before the first frame update
        void Awake() {
            if (showed) {
                content.text = contentStr;
                leftText.text = leftStr;
                rightText.text = rightStr;
            }
        }

        public void Init(string contentStrId, TextAnchor contentAlignment, Model.EmptyBoolCallBack leftCallback, string leftStr = "OK", Model.EmptyBoolCallBack rightCallback = null, string rightStr = "Cancel", params string[] contentStrValues) {
            Init(contentStrId, leftCallback, leftStr, rightCallback, rightStr, contentStrValues);
            content.alignment = contentAlignment;
        }

        public void Init(string contentStrId, Model.EmptyBoolCallBack leftCallback, string leftStr = "OK", Model.EmptyBoolCallBack rightCallback = null, string rightStr = "Cancel", params string[] contentStrValues) {
            // 设定信息
            this.contentStr = Game.Config.StringInternational.GetValue(contentStrId, contentStrValues);
            this.leftCallback = leftCallback;
            this.leftStr = leftStr;
            this.rightCallback = rightCallback;
            this.rightStr = rightStr;
            if(isActiveAndEnabled) {
                content.text = contentStr;
                leftText.text = leftStr;
                rightText.text = rightStr;
            }
            showed = true;
            lastStatus = Game.Status;
            if(Game.Status != Model.EGameStatus.Start) {
                Game.Status = Model.EGameStatus.InEditorDialog;
            }
        }

        public void OnLeftClick() {
            if (leftCallback == null || leftCallback()) {
                Game.Status = lastStatus;
                Game.HideUI(UIType.AlertDialog);
            }
        }

        public void OnRightClick() {
            if (rightCallback == null || rightCallback()) {
                Game.Status = lastStatus;
                Game.HideUI(UIType.AlertDialog);
            }
        }

        public Text content;
        public Button left;
        public Text leftText;
        public Button right;
        public Text rightText;

        private bool showed = false;
        private string contentStr;
        private Model.EmptyBoolCallBack leftCallback;
        private string leftStr;
        private Model.EmptyBoolCallBack rightCallback;
        private string rightStr;
        private Model.EGameStatus lastStatus;
    }

}
