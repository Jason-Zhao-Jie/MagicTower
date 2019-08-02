using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArmyAnt.ViewUtil.Components;

namespace MagicTower.Components.DataEditor {
    public class ModalMakerPanel : MonoBehaviour {
        void Awake() {
        }

        public SelectListView ModalList;
        public SelectListView SpriteList;
        public SelectListView eventDataList;

        public InputField oldSpriteName;
        public Image oldSpriteImage;
        public InputField newSpriteName;
        public Image newSpriteImage;
        public InputField modalIDText;
        public Text modalNameBtnText;

        public Dropdown modalTypeDD;
        public Dropdown modalAnimateTypeDD;
        public Dropdown eventTypeDD;
    }

}
