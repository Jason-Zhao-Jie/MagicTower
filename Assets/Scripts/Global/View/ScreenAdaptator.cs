using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScreenAdaptator {
    public ScreenAdaptator(Vector2 screen) {
        SetScreenSize(screen);
    }

    public void SetScreenSize(Vector2 screen) {
        ScreenSize = screen;
        pixes = screen.y;
    }

    public void LoadOnMainScene(Rect map) {
        mapPanel = map;

        // 计算游戏场景的有效地图区域
        var totalWidth = map.width;
        var totalHeight = map.height;
        bool isHorizenFull = totalWidth >= totalHeight;
        var finalX = isHorizenFull ? ((totalWidth - totalHeight) / 2) : 0;
        var finalY = isHorizenFull ? 0 : ((totalHeight - totalWidth) / 2);
        MapPartRect = new Rect(finalX, finalY, totalWidth - 2 * finalX, totalHeight - 2 * finalY);

        // 计算单个地图块的真实大小
        BlockSize = new Vector2(MapPartRect.width * 100 / (Constant.MAP_BLOCK_LENGTH * Constant.MAP_BLOCK_BASE_SIZE), MapPartRect.height * 100 / (Constant.MAP_BLOCK_LENGTH * Constant.MAP_BLOCK_BASE_SIZE));
    }

    public Vector2 ScreenSize { get; private set; }

    public Rect MapPartRect { get; private set; }

    public Vector2 BlockSize { get; private set; }

    public float RealFontSize {
        get {
            return pixes / STANDARD_PIXES;
        }
    }

    private const int STANDARD_PIXES = 650;
    private float pixes;
    private Rect mapPanel;
}
