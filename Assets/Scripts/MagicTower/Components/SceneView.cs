using System.Collections;
using UnityEngine;
using MagicTower.Components.Control;
using MagicTower.Components.Unit;
using MagicTower.Present.Manager;

namespace MagicTower.Components.Scene {

    public class SceneView : MonoBehaviour {

        // Use this for initialization
        void Awake() {
            Input.multiTouchEnabled = false;    // NOTE : 多点触摸会导致寻路出现bug, 先禁用

            AudioManager.ClearSoundSource();
            AudioManager.MusicSource = musicSource;
            foreach(var i in audioSources) {
                AudioManager.AddSoundSource(i);
            }

            Loading();
        }

        // Update is called once per frame
        void Update() {
            Game.SceneUpdate();
        }

        private void OnPostRender() {
            Game.SceneOnPostRender();
        }

        void OnDestroy() {
            Resources.UnloadUnusedAssets();
        }

        public void Loading() {
            Game.Initial(this, Instantiate(loadingObj, mapTileRoot.transform).GetComponent<UIPanel.GlobalLoading>());

            // 打开开始界面
            Game.StopAndBackToStart();
        }

        public Present.Player.View RefreshPlayerPanel() {
            return Instantiate(playerPanel, uiCanvas.transform).GetComponent<Present.Player.View>();
        }

        public void OnBtnMenuClicked() {
            Game.ShowMainMenu();
        }

        public Present.Map.View mapTileRoot;
        public AudioSource musicSource;
        public AudioSource[] audioSources;

        public GameObject loadingObj;
        public GameObject playerPanel;

        public GameObject uiCanvas = null;

    }

}
