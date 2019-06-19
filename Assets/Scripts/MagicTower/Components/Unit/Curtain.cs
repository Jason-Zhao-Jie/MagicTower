using UnityEngine;
using UnityEngine.UI;

namespace MagicTower.Components.Unit {

    public class Curtain : MonoBehaviour {
        private Model.EmptyBoolCallBack firstcallback = null;
        private Model.EmptyBoolCallBack[] callbacks = null;

        public enum ContentType {
            Default,
            Nothing,
            Loading,
            GameOver,
            WholeScreenText,
            FloorSelector,
            Video,
            Game,
        }

        public void StartShow(ContentType type, Model.EmptyBoolCallBack hideCb, params Model.EmptyBoolCallBack[] showCb) {
            //gameObject.SetActive(true);
            switch(type) {
                case ContentType.Nothing:
                    txtLoading.gameObject.SetActive(false);
                    imgGameOver.gameObject.SetActive(false);
                    panelWholeScreen.gameObject.SetActive(false);
                    break;
                case ContentType.Loading:
                    txtLoading.gameObject.SetActive(true);
                    imgGameOver.gameObject.SetActive(false);
                    panelWholeScreen.gameObject.SetActive(false);
                    break;
                case ContentType.GameOver:
                    txtLoading.gameObject.SetActive(false);
                    imgGameOver.gameObject.SetActive(true);
                    panelWholeScreen.gameObject.SetActive(false);
                    Animator.speed /= 3;
                    break;
                case ContentType.WholeScreenText:
                    txtLoading.gameObject.SetActive(false);
                    imgGameOver.gameObject.SetActive(false);
                    panelWholeScreen.gameObject.SetActive(true);
                    break;
            }
            transform.SetAsLastSibling();
            firstcallback = hideCb;
            callbacks = showCb;
            Animator.enabled = true;

            Animator.Play("Curtain_show");
        }

        public void StartHide(Model.EmptyBoolCallBack showCb, params Model.EmptyBoolCallBack[] hideCb) {
            //gameObject.SetActive(true);
            firstcallback = showCb;
            callbacks = hideCb;
            Animator.enabled = true;

            Animator.Play("Curtain_hide");
        }

        // Use this for initialization
        void Awake() {
        }

        private void Start() {
            Animator.enabled = false;
            //gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update() {
        }

        private void OnShow() {
            Animator.enabled = false;
            if(firstcallback != null && firstcallback()) {
                Model.EmptyBoolCallBack cb = null;
                Model.EmptyBoolCallBack[] cbs = null;
                if(callbacks != null && callbacks.Length > 0) {
                    cb = callbacks[0];
                }
                if(callbacks != null && callbacks.Length > 1) {
                    cbs = new Model.EmptyBoolCallBack[callbacks.Length - 1];
                    for(var i = 0; i < cbs.Length; ++i) {
                        cbs[i] = callbacks[i + 1];
                    }
                }
                StartHide(cb, cbs);
            }
        }

        private void OnHide() {
            Animator.enabled = false;
            if(firstcallback != null && firstcallback()) {
                Model.EmptyBoolCallBack cb = null;
                Model.EmptyBoolCallBack[] cbs = null;
                if(callbacks != null && callbacks.Length > 0) {
                    cb = callbacks[0];
                }
                if(callbacks != null && callbacks.Length > 1) {
                    cbs = new Model.EmptyBoolCallBack[callbacks.Length - 1];
                    for(var i = 0; i < cbs.Length; ++i) {
                        cbs[i] = callbacks[i + 1];
                    }
                }
                StartShow(ContentType.Default, cb, cbs);
            } else {
                transform.SetAsFirstSibling();
            }
        }

        private void FixedUpdate() {

        }

        private Animator Animator { get { return GetComponent<Animator>(); } }

        public Text txtLoading;
        public Image imgGameOver;
        public GameObject panelWholeScreen;
        public Text txtWholeScreen;
        public Text txtWholeScreenBottom;
    }

}
