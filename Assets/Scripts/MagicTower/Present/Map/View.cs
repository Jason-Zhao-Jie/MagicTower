using System.Collections.Generic;
using UnityEngine;

namespace MagicTower.Present.Map {

    public class View : ArmyAnt.Base.AView {
        public const int MAP_BLOCK_LENGTH = 18;
        public const int MAP_BLOCK_BASE_SIZE = 32;

        private const float PIXELS_PER_UNIT = 100f;
        private const float MAP_BLOCK_POS_HALF = 0.5f - MAP_BLOCK_LENGTH / 2;
        private const float BLOCK_SIZE = MAP_BLOCK_BASE_SIZE / PIXELS_PER_UNIT;

        private const float MAPRECT_X_PERCENT = 0.99f - 0.24f;
        private const float MAPRECT_Y_PERCENT = 0.98f - 0.02f;
        private const float CONSTANT_SCREEN_WIDTH_PIXELS = 1280f;
        private const float CONSTANT_MAPRECT_X_PIXELS = CONSTANT_SCREEN_WIDTH_PIXELS * MAPRECT_X_PERCENT;

        private void Awake() {
        }

        public void SetCanvasSize() {
            var localPos = mapRect.localPosition;
            var screen_y_pixels = CONSTANT_SCREEN_WIDTH_PIXELS / Screen.width * Screen.height;
            var mapPartLength = System.Math.Min(CONSTANT_MAPRECT_X_PIXELS, screen_y_pixels * MAPRECT_Y_PERCENT);

            // reset camera position 重置摄像机位置
            mainCamera.transform.position = new Vector3(-mapRect.position.x, -mapRect.position.y, mainCamera.transform.position.z);

            // reset map rect background 重置地图tile区域road背景块的位置
            mapRect.anchorMin = new Vector2(0.5f, 0.5f);
            mapRect.anchorMax = new Vector2(0.5f, 0.5f);
            mapRect.anchoredPosition = localPos;
            mapRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mapPartLength);
            mapRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mapPartLength);

            // 记录地图框 rect
            MapRect = mapRect.rect;
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

        [Tooltip("幕布对象")]
        [Space(4)]
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
