using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ArmyAnt.ViewUtil.Components {
    public class DefaultSelectableElement : MonoBehaviour {
        public Text text;
        public bool Selected {
            get => GetComponent<Button>().interactable;
            set {
                GetComponent<Button>().interactable = !value;
                if(value) {
                    text.color = new Color(0.1f, 0.2f, 0.3f);
                } else {
                    text.color = new Color(0.7f, 0.8f, 0.9f);
                }
            }
        }

        public void AddOnclickEvent(UnityEngine.Events.UnityAction cb) {
            GetComponent<Button>().onClick.AddListener(cb);
        }

        private void Awake() {
            Selected = false;
        }
    }
}
