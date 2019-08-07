using UnityEngine;

namespace ArmyAnt.ViewUtil.Components {

    public class SelectListView : ListView {
        public System.Action<int, bool> selectedFunc = null;

        protected override void Start() {
            base.Start();
            if(ItemCount > SelectedIndex && SelectedIndex >= 0) {
                Started = true;
                CallSelectedFunc(SelectedIndex, true);
            }else if(ItemCount > 0) {
                SelectedIndex = 0;
                selectedFunc(0, true);
            } else {
                SelectedIndex = -1;
            }
            Started = true;
        }

        protected override void OnDisable() {
            base.OnDisable();
            Started = false;
        }

        public override RectTransform InsertDefaultItem(int index, float localScale = 1) {
            var ret = base.InsertDefaultItem(index, localScale);
            if(index <= SelectedIndex) {
                SelectedIndex += 1;
            }
            return ret;
        }

        public override RectTransform DeleteItem(int index) {
            if(ItemCount == 1 && index == 0 && SelectedIndex == 0) {
                CallSelectedFunc(SelectedIndex, false);
            }
            var ret = base.DeleteItem(index);
            if(SelectedIndex == index) {
                if(index >= ItemCount) {
                    CallSelectedFunc(SelectedIndex - 1, true);
                } else {
                    CallSelectedFunc(SelectedIndex, true);
                }
            } else if(SelectedIndex > index) {
                SelectedIndex -= 1;
            }
            return ret;
        }

        public override void Clear() {
            CallSelectedFunc(SelectedIndex, false);
            base.Clear();
            CallSelectedFunc(-1, false);
        }

        public override RectTransform ReplaceItemToDefault(int index) {
            if(index == SelectedIndex) {
                SelectedIndex = -1;
                CallSelectedFunc(index, false);
            }
            return base.ReplaceItemToDefault(index);
        }

        public void Select(int index) {
            if(index < 0 || index >= ItemCount) {
                return;
            }
            CallSelectedFunc(SelectedIndex, false);
            CallSelectedFunc(index, true);
        }

        public void Select(RectTransform target) {
            Select(GetItemIndex(target));
        }

        private void CallSelectedFunc(int index, bool select) {
            if(select) {
                SelectedIndex = index;
            }
            if(Started && index >= 0 && index < ItemCount) {
                selectedFunc(index, select);
            }
        }

        public int SelectedIndex { get; private set; } = -1;
        public RectTransform SelectedItem => this[SelectedIndex];

        public bool Started { get; private set; } = false;
    }

}
