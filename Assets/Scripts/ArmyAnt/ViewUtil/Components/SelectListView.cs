using UnityEngine;
using System.Collections;

namespace ArmyAnt.ViewUtil.Components {

    public class SelectListView : ListView {
        public System.Action<int, bool> selectedFunc = null;

        protected override void Start() {
            base.Start();
            Select(SelectedIndex);
            started = true;
        }

        protected override void OnDisable() {
            base.OnDisable();
            started = false;
        }

        public void Select(int index) {
            if(index < 0 || index >= ItemCount) {
                return;
            }
            if(started && SelectedIndex >= 0 && SelectedIndex < ItemCount) {
                selectedFunc(SelectedIndex, false);
            }
            SelectedIndex = index;
            if(started) {
                selectedFunc(SelectedIndex, true);
            }
        }

        public void Select(RectTransform target) {
            Select(GetItemIndex(target));
        }

        public int SelectedIndex { get; private set; } = 0;
        public RectTransform SelectedItem => this[SelectedIndex];

        private bool started = false;
    }

}
