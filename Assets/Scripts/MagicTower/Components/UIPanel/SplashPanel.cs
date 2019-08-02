using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MagicTower.Components.UIPanel {
    public class SplashPanel : MonoBehaviour {
        // Start is called before the first frame update
        private void Start() {
            StartCoroutine(Splashing());
        }

        private void Update() {
            if(fadeInTime > float.Epsilon && fadeTotalTime > float.Epsilon) {
                fadeInTime -= Time.deltaTime;
                logoImage.color = new Color(logoImage.color.r, logoImage.color.g, logoImage.color.b, (fadeTotalTime - fadeInTime) / fadeTotalTime);
            }else if(fadeOutTime > float.Epsilon && fadeTotalTime > float.Epsilon) {
                fadeOutTime -= Time.deltaTime;
                logoImage.color = new Color(logoImage.color.r, logoImage.color.g, logoImage.color.b, fadeOutTime / fadeTotalTime);
            }
        }

        private IEnumerator Splashing() {
            fadeTotalTime = 1f;
            logoImage.sprite = logos[0];
            logoImage.color = new Color(logoImage.color.r, logoImage.color.g, logoImage.color.b, 0);
            fadeInTime = 1f;
            yield return new WaitUntil(() => logoImage.color.a >= 1-float.Epsilon);
            var startTime = System.DateTime.Now;
            var curtain = Instantiate(curtainPrefab);
            curtain.transform.SetParent(uiCanvas.transform, false);
            Game.Initial(scene, Instantiate(Resources.Load<GameObject>("LoadingObject"), mapTileRoot.transform).GetComponent<GlobalLoading>(), curtain.GetComponent<Unit.Curtain>());
            transform.SetAsLastSibling();
            var waitingTime = System.Convert.ToSingle(3f + (startTime - System.DateTime.Now).TotalSeconds);
            if(waitingTime > float.Epsilon) {
                yield return new WaitForSeconds(waitingTime);
            }
            for(var i = 1; i < logos.Length; ++i) {
                fadeOutTime = 1f;
                yield return new WaitUntil(() => logoImage.color.a <= float.Epsilon);
                logoImage.sprite = logos[i];
                fadeInTime = 1f;
                yield return new WaitUntil(() => logoImage.color.a >= 1 - float.Epsilon);
                yield return new WaitForSeconds(2f);
            }
            // 打开开始界面
            fadeOutTime = 1f;
            yield return new WaitUntil(() => logoImage.color.a <= float.Epsilon);
            Instantiate(virtualRockerPrefab).transform.SetParent(uiCanvas.transform, false);
            Game.StopAndBackToStart();
            Destroy(gameObject);
        }

        public Sprite[] logos;
        public Image logoImage;

        public Present.Map.View mapTileRoot;
        public GameObject curtainPrefab;
        public GameObject virtualRockerPrefab;

        public Scene.SceneView scene;
        public GameObject uiCanvas;

        private float fadeInTime = 0f;
        private float fadeOutTime = 0f;
        private float fadeTotalTime = 1f;
    }
}
