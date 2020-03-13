using UnityEngine;
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

            splashPanel.SetActive(true);
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

        public GameObject playerPanel;

        public GameObject uiCanvas;
        public GameObject splashPanel;

    }

}
