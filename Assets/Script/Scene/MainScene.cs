using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScene : MonoBehaviour
{
    public static MainScene instance;
    // Use this for initialization
    void Start()
    {
        instance = this;

        mapNameText = transform.Find("HeroPanel").transform.Find("MapName").GetComponent<UnityEngine.UI.Text>();
        backgroundImg = GetComponent<UnityEngine.UI.Image>();
        mapPartRect = MapManager.GetMapPosition(transform.Find("MapPanel").GetComponent<RectTransform>());
		blockSize = new Vector3(mapPartRect.width * 100 / (Constant.MAP_BLOCK_LENGTH * Constant.MAP_BLOCK_BASE_SIZE), mapPartRect.height * 100 / (Constant.MAP_BLOCK_LENGTH * Constant.MAP_BLOCK_BASE_SIZE));
        UnityEngine.Debug.Log("The current map whole rect is: " + transform.Find("MapPanel").GetComponent<RectTransform>().rect.width + ", " + transform.Find("MapPanel").GetComponent<RectTransform>().rect.height);
		UnityEngine.Debug.Log("The current map part rect is: " + mapPartRect.x + ", " + mapPartRect.y + ", " + mapPartRect.width + ", " + mapPartRect.height);
        UnityEngine.Debug.Log("The current map block size is: " + blockSize.x + ", " + blockSize.y);
        //TODO: 需要在四周添加填充墙，然后再MapManager构造地图时刷新墙

        AudioController.instance.MusicSource = GetComponent<AudioSource>();
        AudioController.instance.SoundSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        MapManager.instance.ShowMap();
		PlayerController.instance.ShowPlayer(true);

        DataCenter.instance.Status = Constant.EGameStatus.InGame;
    }

    void OnDestroy()
    {
        DataCenter.instance.Status = Constant.EGameStatus.Start;
        instance = null;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < InputController.listenedKeys.Length; ++i)
        {
            bool isDown = Input.GetKey(InputController.listenedKeys[i]);
            bool hasDown = InputController.instance.keyStatusMap[InputController.listenedKeys[i]];
            if (isDown && !hasDown)
                InputController.instance.OnKeyDown(InputController.listenedKeys[i]);
            else if (hasDown && !isDown)
                InputController.instance.OnKeyUp(InputController.listenedKeys[i]);
        }

        for (int i = 0; i < Input.touchCount; ++i)
        {
            var tc = Input.GetTouch(i);
            switch (tc.phase)
            {
                case TouchPhase.Began:
                    InputController.instance.OnTouchDown(tc.position);
                    break;
                case TouchPhase.Canceled:
                case TouchPhase.Ended:
                    InputController.instance.OnTouchUp(tc.position, new Vector2(tc.position.x - tc.deltaPosition.x, tc.position.y - tc.deltaPosition.y));
                    break;
            }
        }

        if (Input.GetMouseButtonDown(0) && !InputController.instance.isMouseLeftDown)
            InputController.instance.OnTouchDown(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        if (Input.GetMouseButtonUp(0) && InputController.instance.isMouseLeftDown)
            InputController.instance.OnTouchUp(new Vector2(Input.mousePosition.x, Input.mousePosition.y));

    }

    public void AddObjectToMap(GameObject obj, int posx, int posy, int posz = -2)
    {
        obj.transform.SetParent(transform.Find("MapPanel"));
        obj.transform.position = transform.Find("MapPanel").transform.
            TransformPoint(new Vector3((posx + (float)0.5) * Constant.MAP_BLOCK_BASE_SIZE * blockSize.x / 100 + mapPartRect.x,
                                       (posy + (float)0.5) * Constant.MAP_BLOCK_BASE_SIZE * blockSize.y / 100 + mapPartRect.y,
                                       posz));
        obj.transform.localScale = blockSize;
    }

    public string MapName
    {
        get { return mapNameText.text; }
        set { mapNameText.text = value; }
    }

    public string BackgroundImage
    {
        get { return backgroundImg.sprite.name; }
        set { backgroundImg.sprite = Resources.Load<GameObject>(Constant.PREFAB_DIR + value).GetComponent<SpriteRenderer>().sprite; }
    }

    public Vector3 BlockSize{ get { return blockSize; }}

    private UnityEngine.UI.Text mapNameText;
    private UnityEngine.UI.Image backgroundImg;
    private Rect mapPartRect;
    private Vector3 blockSize;
}
