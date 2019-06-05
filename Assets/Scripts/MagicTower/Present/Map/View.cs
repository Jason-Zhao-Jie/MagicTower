using System.Collections.Generic;
using UnityEngine;

namespace MagicTower.Present.Map {

    public class View : ArmyAnt.Base.AView {
        public const int MAP_BLOCK_LENGTH = 18;
        public const int MAP_BLOCK_BASE_SIZE = 32;
        public const float MAP_BLOCK_POS_HALF = 0.5f - MAP_BLOCK_LENGTH / 2;

        public const float SCREEN_X_MIN_PERCENT = 0.205f;
        public const float SCREEN_X_MAX_PERCENT = 0.984f;
        public const float SCREEN_Y_MIN_PERCENT = 0.021f;
        public const float SCREEN_Y_MAX_PERCENT = 0.979f;

        private void Awake() {
            SetAllPositionAndSize();
        }

        private void SetAllPositionAndSize() {
            // calculate parameter 地图tile区域尺寸
            mapPartLength = System.Math.Min(Screen.width * (SCREEN_X_MAX_PERCENT - SCREEN_X_MIN_PERCENT), Screen.height * (SCREEN_Y_MAX_PERCENT - SCREEN_Y_MIN_PERCENT));

            // reset camera position 重置摄像机位置
            var camerapos = mainCamera.transform.position;
            camerapos.x = -Screen.width * (SCREEN_X_MAX_PERCENT + SCREEN_X_MIN_PERCENT - 1) / 2;
            camerapos.y = -Screen.height * (SCREEN_Y_MAX_PERCENT + SCREEN_Y_MIN_PERCENT - 1) / 2;
            mainCamera.transform.position = camerapos;

            // reset map rect background 重置地图tile区域road背景块的尺寸和位置
            mapRect.anchorMin = new Vector2(0.5f, 0.5f);
            mapRect.anchorMax = new Vector2(0.5f, 0.5f);
            mapRect.anchoredPosition = new Vector3(-camerapos.x, -camerapos.y);
            mapRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mapPartLength);
            mapRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mapPartLength);

            // block size 单个sprite的尺寸
            blockSize = mapPartLength / MAP_BLOCK_LENGTH;
            HitterLocalScale = new Vector2(blockSize / MAP_BLOCK_BASE_SIZE * 100, blockSize / MAP_BLOCK_BASE_SIZE * 100);
        }

        public void AddObjectToMap(GameObject obj, int posx, int posy, int posz = 0) {
            obj.transform.SetParent(transform, false);
            obj.transform.position = new Vector3((posx + MAP_BLOCK_POS_HALF) * blockSize , (posy + MAP_BLOCK_POS_HALF) * blockSize, posz);
            obj.transform.localScale = HitterLocalScale;
        }

        public void OnMapClicked(Vector2 pos) {
            pos = mainCamera.ScreenToWorldPoint(pos);
            var _posx = System.Convert.ToInt32(System.Math.Round(pos.x / blockSize - MAP_BLOCK_POS_HALF));
            var _posy = System.Convert.ToInt32(System.Math.Round(pos.y / blockSize - MAP_BLOCK_POS_HALF));
            if(_posx >= 0 && _posx >= 0 && _posx < MAP_BLOCK_LENGTH && _posy < MAP_BLOCK_LENGTH) {
                Game.Player.StartAutoStep(_posx, _posy);
            }
        }

        public Vector2 HitterLocalScale { get; private set; }

        [Tooltip("幕布对象")]
        [Space(4)]
        public Components.Unit.Curtain curtain;

        public string BackgroundImage {
            get { return backgroundImg.sprite.name; }
            set { backgroundImg.sprite = Game.GetMods(value)[0]; }
        }

        [Tooltip("背景所在的对象")]
        [Space(4)]
        public UnityEngine.UI.Image backgroundImg;

        public RectTransform mapRect;

        public Camera mainCamera;

        // 以下是一些界面系数
        private float mapPartLength;
        private float blockSize;
    }

}
