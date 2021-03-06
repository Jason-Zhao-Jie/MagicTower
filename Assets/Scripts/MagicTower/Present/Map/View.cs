﻿using System.Collections.Generic;
using UnityEngine;

namespace MagicTower.Present.Map {

    public class View : ArmyAnt.Base.AView {
        public const int MAP_BLOCK_LENGTH = 18;
        public const int MAP_BLOCK_BASE_SIZE = 32;

        private const float PIXELS_PER_UNIT = 100f;
        private const float MAP_BLOCK_POS_HALF = 0.5f - MAP_BLOCK_LENGTH / 2;
        private const float BLOCK_SIZE = MAP_BLOCK_BASE_SIZE / PIXELS_PER_UNIT;

        private const float MAPRECT_X_MIN_PERCENT = 0.24f;
        private const float MAPRECT_X_MAX_PERCENT = 0.99f;
        private const float MAPRECT_X_PERCENT = MAPRECT_X_MAX_PERCENT - MAPRECT_X_MIN_PERCENT;
        private const float MAPRECT_Y_MIN_PERCENT = 0.02f;
        private const float MAPRECT_Y_MAX_PERCENT = 0.98f;
        private const float MAPRECT_Y_PERCENT = MAPRECT_Y_MAX_PERCENT - MAPRECT_Y_MIN_PERCENT;
        private const float CONSTANT_SCREEN_WIDTH_PIXELS = 1280f;
        private const float CONSTANT_SCREEN_HEIGHT_PIXELS = 720f;
        private const float CONSTANT_MAPRECT_X_PIXELS = CONSTANT_SCREEN_WIDTH_PIXELS * MAPRECT_X_PERCENT;
        private const float CONSTANT_MAPRECT_Y_PIXELS = CONSTANT_SCREEN_HEIGHT_PIXELS * MAPRECT_Y_PERCENT;

        private void Awake() {
        }

        public void SetCanvasSize() {
            var screen_x_pixels = CONSTANT_SCREEN_HEIGHT_PIXELS / Screen.height * Screen.width;
            var screen_y_pixels = CONSTANT_SCREEN_WIDTH_PIXELS / Screen.width * Screen.height;
            var mapPartLength = System.Math.Min(screen_x_pixels * MAPRECT_X_PERCENT, CONSTANT_MAPRECT_Y_PIXELS);

            // reset map rect background 重置地图tile区域road背景块的位置
            mapRect.anchorMin = new Vector2(0.5f, 0.5f);
            mapRect.anchorMax = new Vector2(0.5f, 0.5f);
            mapRect.anchoredPosition = new Vector2(screen_x_pixels * (MAPRECT_X_MAX_PERCENT + MAPRECT_X_MIN_PERCENT - 1) / 2, CONSTANT_MAPRECT_Y_PIXELS * (MAPRECT_Y_MAX_PERCENT + MAPRECT_Y_MIN_PERCENT - 1) / 2);
            mapRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mapPartLength);
            mapRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mapPartLength);

            // reset camera position 重置摄像机位置

            // 记录地图框 rect
            var halfScreenWidth = mainCamera.orthographicSize * Screen.width / Screen.height;
            mainCamera.transform.position = new Vector3(-halfScreenWidth * (MAPRECT_X_MAX_PERCENT + MAPRECT_X_MIN_PERCENT - 1), -mainCamera.orthographicSize * (MAPRECT_Y_MAX_PERCENT + MAPRECT_Y_MIN_PERCENT - 1) / 2, mainCamera.transform.position.z);

            // 记录地图框 rect
            var mapScreenLen = System.Math.Min(Screen.width * MAPRECT_X_PERCENT, Screen.height * MAPRECT_Y_PERCENT);
            MapRect = new Rect(Screen.width * (MAPRECT_X_MAX_PERCENT + MAPRECT_X_MIN_PERCENT) / 2 - mapScreenLen / 2, Screen.height * (MAPRECT_Y_MAX_PERCENT + MAPRECT_Y_MIN_PERCENT) / 2 - mapScreenLen / 2, mapScreenLen, mapScreenLen);
        }

        public void AddObjectToMap(GameObject obj, int posx, int posy, int posz = 0) {
            obj.transform.SetParent(transform, false);
            obj.transform.position = new Vector3((posx + MAP_BLOCK_POS_HALF) * BLOCK_SIZE, (posy + MAP_BLOCK_POS_HALF) * BLOCK_SIZE, posz);
            obj.transform.localScale = Vector3.one;
        }
            
        public void OnMapClicked(Vector2 pos) {
            pos = mainCamera.ScreenToWorldPoint(pos);
            var _posx = System.Convert.ToInt32(System.Math.Round(pos.x / BLOCK_SIZE - MAP_BLOCK_POS_HALF));
            var _posy = System.Convert.ToInt32(System.Math.Round(pos.y / BLOCK_SIZE - MAP_BLOCK_POS_HALF));
            if(Game.Settings.Settings.autoFindBestRoad && Game.Input.GetArrowState() == Player.Controller.Direction.Default && _posx >= 0 && _posx >= 0 && _posx < MAP_BLOCK_LENGTH && _posy < MAP_BLOCK_LENGTH) {
                Game.Player.StartAutoStep(_posx, _posy);
            }
        }

        [HideInInspector()]
        public Components.Unit.Curtain curtain;

        public string BackgroundImage {
            get { return backgroundImg.sprite.name; }
            set { backgroundImg.sprite = Game.GetMods(value)[0]; }
        }

        public Rect MapRect { get; private set; }

    [Tooltip("背景所在的对象")]
        [Space(4)]
        public UnityEngine.UI.Image backgroundImg;

        public RectTransform mapRect;

        public Camera mainCamera;
    }

}
