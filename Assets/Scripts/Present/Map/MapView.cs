﻿using UnityEngine;
using System.Collections;

public class MapView : AView
{
    public MapView() : base(null)
    {
    }

    private void OnDestroy()
    {
    }

    public void Initial(Curtain curtain, UnityEngine.UI.Image backgroundImage)
    {
        Curtain = curtain;
        backgroundImg = backgroundImage;
    }

    void Awake()
    {
        Game.Initial();
        //TODO: 需要在四周添加填充墙，然后再MapManager构造地图时刷新墙
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddObjectToMap(GameObject obj, int posx, int posy, int posz = -2)
    {
        obj.transform.SetParent(transform, false);
        obj.transform.position = transform.
            TransformPoint(new Vector3((posx + (float)0.5) * Constant.MAP_BLOCK_BASE_SIZE * BlockSize.x / 100 + MapPartRect.x,
                                       (posy + (float)0.5) * Constant.MAP_BLOCK_BASE_SIZE * BlockSize.y / 100 + MapPartRect.y,
                                       posz));
        obj.transform.localScale = BlockSize;
    }

    public void OnMapClicked(Vector2 pos)
    {
        var panelPos = Game.CurrentScene.GetComponent<RectTransform>().transform.InverseTransformPoint(GetComponent<RectTransform>().position);
        pos.x -= panelPos.x + MapPartRect.x + Game.CurrentScene.GetComponent<RectTransform>().rect.width / 2;
        pos.y -= panelPos.y + MapPartRect.y + Game.CurrentScene.GetComponent<RectTransform>().rect.height / 2;
        if (pos.x >= 0 && pos.y >= 0)
        {
            var _posx = (int)(pos.x * Constant.MAP_BLOCK_LENGTH / MapPartRect.width);
            var _posy = (int)(pos.y * Constant.MAP_BLOCK_LENGTH / MapPartRect.height);
            if (_posx >= Constant.MAP_BLOCK_LENGTH || _posy >= Constant.MAP_BLOCK_LENGTH)
                return;
            Game.CurrentScene.OnMapClicked(_posx, _posy);
        }
    }

    public Curtain Curtain;

    public string BackgroundImage
    {
        get { return backgroundImg.sprite.name; }
        set { backgroundImg.sprite = Resources.Load<GameObject>(Constant.PREFAB_DIR + value).GetComponent<SpriteRenderer>().sprite; }
    }
    private UnityEngine.UI.Image backgroundImg;


    public Rect MapPartRect
    {
        get
        {
            var rect = GetComponent<RectTransform>().rect;
            bool isHorizenFull = rect.width >= rect.height;
            mapPartRect.x = isHorizenFull ? ((rect.width - rect.height) / 2) : 0;
            mapPartRect.y = isHorizenFull ? 0 : ((rect.height - rect.width) / 2);
            mapPartRect.width = rect.width - 2 * mapPartRect.x;
            mapPartRect.height = rect.height - 2 * mapPartRect.y;
            return mapPartRect;
        }
    }
    private Rect mapPartRect;

    public Vector2 BlockSize
    {
        get
        {
            mapPartRect = MapPartRect;
            blockSize.x = mapPartRect.width * 100 / (Constant.MAP_BLOCK_LENGTH * Constant.MAP_BLOCK_BASE_SIZE);
            blockSize.y = mapPartRect.height * 100 / (Constant.MAP_BLOCK_LENGTH * Constant.MAP_BLOCK_BASE_SIZE);
            return blockSize;
        }
    }
    private Vector2 blockSize;

}
