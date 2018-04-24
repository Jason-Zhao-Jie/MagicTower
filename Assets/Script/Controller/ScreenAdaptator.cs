using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScreenAdaptator
{
    public static ScreenAdaptator instance = new ScreenAdaptator();
    ScreenAdaptator() { }

    public void LoadOnStartScene(Vector2 screen)
    {
        screenSize = screen;
    }

    public void LoadOnMainScene(Rect map)
    {
        mapPanel = map;

        // 计算游戏场景的有效地图区域
        var totalWidth = map.width;
        var totalHeight = map.height;
        bool isHorizenFull = totalWidth >= totalHeight;
        var finalX = isHorizenFull ? ((totalWidth - totalHeight) / 2) : 0;
        var finalY = isHorizenFull ? 0 : ((totalHeight - totalWidth) / 2);
        mapPartRect = new Rect(finalX, finalY, totalWidth - 2 * finalX, totalHeight - 2 * finalY);

        // 计算单个地图块的真实大小
        blockSize = new Vector2(mapPartRect.width * 100 / (Constant.MAP_BLOCK_LENGTH * Constant.MAP_BLOCK_BASE_SIZE), mapPartRect.height * 100 / (Constant.MAP_BLOCK_LENGTH * Constant.MAP_BLOCK_BASE_SIZE));
    }

    public Vector2 ScreenSize {
        get {
            return screenSize;
        }
    }

    public Rect MapPartRect {
        get {
            return mapPartRect;
        }
    }

    public Vector2 BlockSize {
        get {
            return blockSize;
        }
    }

    private Vector2 screenSize;
    private Rect mapPanel;

    private Rect mapPartRect;
    private Vector2 blockSize;
}
