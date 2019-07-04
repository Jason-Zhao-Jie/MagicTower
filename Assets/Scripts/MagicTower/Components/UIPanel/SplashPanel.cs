using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MagicTower.Components.UIPanel {
    public class SplashPanel : MonoBehaviour {
        // Start is called before the first frame update
        void Start() {
            StartCoroutine(Splashing());
        }

        private IEnumerator Splashing() {
            logoImage.sprite = logos[0];
            yield return new WaitForSeconds(1f);
            OnStart();
            yield return new WaitForSeconds(1f);
            for(var i = 1; i < logos.Length; ++i) {
                logoImage.sprite = logos[i];
                yield return new WaitForSeconds(2.0f);
            }
            OnEnd();
        }

        private void OnStart() {
            var curtain = Instantiate(curtainPrefab);
            curtain.transform.SetParent(uiCanvas.transform);
            Game.Initial(scene, Instantiate(loadingObj, mapTileRoot.transform).GetComponent<GlobalLoading>(), curtain.GetComponent<Unit.Curtain>());
            transform.SetAsLastSibling();
        } 

        private void OnEnd() {
            // 打开开始界面
            Instantiate(virtualRockerPrefab).transform.SetParent(uiCanvas.transform);
            Game.StopAndBackToStart();
            Destroy(gameObject);
        }

        public Sprite[] logos;
        public Image logoImage;

        public Present.Map.View mapTileRoot;
        public GameObject loadingObj;
        public GameObject curtainPrefab;
        public GameObject virtualRockerPrefab;

        public Scene.SceneView scene;
        public GameObject uiCanvas;
    }
}
