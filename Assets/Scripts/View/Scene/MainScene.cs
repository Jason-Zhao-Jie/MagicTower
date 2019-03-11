using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainScene : MonoBehaviour {
    public static MainScene instance;
    // Use this for initialization
    void Start()
    {
        instance = this;
        Game.Initial(GetComponent<RectTransform>().rect.size);

        var heroPanel = transform.Find("HeroPanel");
        var itemPanel = transform.Find("ItemPanel");
        var mapPanel = transform.Find("MapPanel");
        dialogCanvas = GameObject.Find("DialogCanvas");
        backgroundImg = GetComponent<Image>();
        Game.View.ScreenAdaptorInst.LoadOnMainScene(mapPanel.GetComponent<RectTransform>().rect);

        curtain = dialogCanvas.transform.Find("Curtain").GetComponent<Curtain>();
        curtain.gameObject.SetActive(false);
        mapNameText = heroPanel.transform.Find("MapName").GetComponent<Text>();
        //TODO: 需要在四周添加填充墙，然后再MapManager构造地图时刷新墙

        // 关联人物数据的text
        roleNameText = heroPanel.transform.Find("Name").GetComponent<Text>();
        portrait = heroPanel.transform.Find("Portrait").GetComponent<Image>();
        levelText = heroPanel.transform.Find("Level").GetComponent<Text>();
        expText = heroPanel.transform.Find("Exp").GetComponent<Text>();
        lifeText = heroPanel.transform.Find("Life").GetComponent<Text>();
        attackText = heroPanel.transform.Find("Attack").GetComponent<Text>();
        defenseText = heroPanel.transform.Find("Defense").GetComponent<Text>();
        speedText = heroPanel.transform.Find("Speed").GetComponent<Text>();
        goldText = itemPanel.transform.Find("Gold").GetComponent<Text>();
        yellowKeyText = itemPanel.transform.Find("YellowKey").GetComponent<Text>();
        blueKeyText = itemPanel.transform.Find("BlueKey").GetComponent<Text>();
        redKeyText = itemPanel.transform.Find("RedKey").GetComponent<Text>();

        // 加载对话框

        topChatPanel = ChatDlg.ShowChat(true);
        topChatPanel.transform.SetParent(dialogCanvas.transform, false);
        topChatPanel.gameObject.SetActive(false);

        bottomChatPanel = ChatDlg.ShowChat(false);
        bottomChatPanel.transform.SetParent(dialogCanvas.transform, false);
        bottomChatPanel.gameObject.SetActive(false);

        tipsPanel = TipBar.ShowTip();
        tipsPanel.transform.SetParent(dialogCanvas.transform, false);
        tipsPanel.gameObject.SetActive(false);

        Game.Controller.Audio.ClearSoundSource();
        Game.Controller.Audio.MusicSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        Game.Controller.Audio.AddSoundSource(GetComponent<AudioSource>());
        Game.Controller.Audio.AddSoundSource(dialogCanvas.GetComponent<AudioSource>());
        Game.Controller.Audio.AddSoundSource(transform.Find("HeroPanel").GetComponent<AudioSource>());
        Game.Controller.Audio.AddSoundSource(transform.Find("ItemPanel").GetComponent<AudioSource>());
        Game.Controller.Audio.AddSoundSource(transform.Find("MapPanel").GetComponent<AudioSource>());

        Game.Controller.MapMgr.ShowMap();

        Game.Controller.Player.ShowPlayer(true);
        Game.Controller.Player.SyncPlayerData();

        Game.Data.Config.Status = Constant.EGameStatus.InGame;

    }

    void OnDestroy() {
        Game.Controller.MapMgr.ClearMap();
        Game.Data.Config.Status = Constant.EGameStatus.Start;
        instance = null;
        Game.View.ObjPool.ClearAll();
    }

    // Update is called once per frame
    void Update() {
        // 监测键盘和手柄按键
        for (int i = 0; i < InputController.listenedKeys.Length; ++i) {
            bool isDown = Input.GetKey(InputController.listenedKeys[i]);
            bool hasDown = Game.Controller.Input.keyStatusMap[InputController.listenedKeys[i]];
            if (isDown && !hasDown)
                Game.Controller.Input.OnKeyDown(InputController.listenedKeys[i]);
            else if (hasDown && !isDown)
                Game.Controller.Input.OnKeyUp(InputController.listenedKeys[i]);
        }

        // 检测手柄摇杆
        Game.Controller.Input.OnJoysticsRockerAxes(InputController.JoysticsAxes.LeftHorizontal, Input.GetAxis("Horizontal_Left"));
        Game.Controller.Input.OnJoysticsRockerAxes(InputController.JoysticsAxes.LeftVertical, Input.GetAxis("Vertical_Left"));
        Game.Controller.Input.OnJoysticsRockerAxes(InputController.JoysticsAxes.RightHorizontal, Input.GetAxis("Horizontal_XBoxRight"));
        Game.Controller.Input.OnJoysticsRockerAxes(InputController.JoysticsAxes.RightVertical, Input.GetAxis("Vertical_XBoxRight"));
        Game.Controller.Input.OnJoysticsRockerAxes(InputController.JoysticsAxes.SpecialHorizontal, Input.GetAxis("Horizontal_XBoxSpecial"));
        Game.Controller.Input.OnJoysticsRockerAxes(InputController.JoysticsAxes.SpecialVertical, Input.GetAxis("Vertical_XBoxSpecial"));

        // 监测鼠标和触屏
        for (int i = 0; i < Input.touchCount; ++i) {
            var tc = Input.GetTouch(i);
            switch (tc.phase) {
                case TouchPhase.Began:
                    Game.Controller.Input.OnTouchDown(tc.position);
                    break;
                case TouchPhase.Canceled:
                case TouchPhase.Ended:
                    Game.Controller.Input.OnTouchUp(tc.position, new Vector2(tc.position.x - tc.deltaPosition.x, tc.position.y - tc.deltaPosition.y));
                    break;
            }
        }

        if (Input.touchCount <= 0) {
            if (Input.GetMouseButtonDown(0) && !Game.Controller.Input.isMouseLeftDown)
                Game.Controller.Input.OnTouchDown(new Vector2(Input.mousePosition.x, Input.mousePosition.y), true);
            if (Input.GetMouseButtonUp(0) && Game.Controller.Input.isMouseLeftDown)
                Game.Controller.Input.OnTouchUp(new Vector2(Input.mousePosition.x, Input.mousePosition.y), false);
        }
    }

    public void BackToStartScene() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("StartScene");
    }

    /********************** Map Utilities **************************************/

    public void AddObjectToMap(GameObject obj, int posx, int posy, int posz = -2) {
        obj.transform.SetParent(transform.Find("MapPanel"), false);
        obj.transform.position = transform.Find("MapPanel").transform.
            TransformPoint(new Vector3((posx + (float)0.5) * Constant.MAP_BLOCK_BASE_SIZE * Game.View.ScreenAdaptorInst.BlockSize.x / 100 + Game.View.ScreenAdaptorInst.MapPartRect.x,
                                       (posy + (float)0.5) * Constant.MAP_BLOCK_BASE_SIZE * Game.View.ScreenAdaptorInst.BlockSize.y / 100 + Game.View.ScreenAdaptorInst.MapPartRect.y,
                                       posz));
        obj.transform.localScale = Game.View.ScreenAdaptorInst.BlockSize;
    }

    public void OnMapClicked(Vector2 pos) {
        var mapPanel = transform.Find("MapPanel").GetComponent<RectTransform>();
        var panelPos = transform.InverseTransformPoint(mapPanel.position);
        pos.x -= panelPos.x + Game.View.ScreenAdaptorInst.MapPartRect.x + GetComponent<RectTransform>().rect.width / 2;
        pos.y -= panelPos.y + Game.View.ScreenAdaptorInst.MapPartRect.y + GetComponent<RectTransform>().rect.height / 2;
        if (pos.x >= 0 && pos.y >= 0) {
            var _posx = (int)(pos.x * Constant.MAP_BLOCK_LENGTH / Game.View.ScreenAdaptorInst.MapPartRect.width);
            var _posy = (int)(pos.y * Constant.MAP_BLOCK_LENGTH / Game.View.ScreenAdaptorInst.MapPartRect.height);
            if (_posx >= Constant.MAP_BLOCK_LENGTH || _posy >= Constant.MAP_BLOCK_LENGTH)
                return;
            Game.Controller.Player.StartAutoStep(_posx, _posy);
        }
    }

    /********************** Chat Part **************************************/

    public void ShowChatOnTop(string content, int speakerId = -1) {
        Game.Data.Config.Status = Constant.EGameStatus.OnTipChat;
        topChatPanel.gameObject.SetActive(true);
        topChatPanel.SetChat(StringInternational.GetValue(content), speakerId);
        topChatPanel.gameObject.SetActive(true);
        bottomChatPanel.gameObject.SetActive(false);
        tipsPanel.gameObject.SetActive(false);
    }

    public void ShowChatOnBottom(string content, int speakerId = -1) {
        Game.Data.Config.Status = Constant.EGameStatus.OnTipChat;
        bottomChatPanel.gameObject.SetActive(true);
        bottomChatPanel.SetChat(StringInternational.GetValue(content), speakerId);
        topChatPanel.gameObject.SetActive(false);
        bottomChatPanel.gameObject.SetActive(true);
        tipsPanel.gameObject.SetActive(false);
    }

    public void ShowTips(string content) {
        Game.Data.Config.Status = Constant.EGameStatus.OnTipChat;
        tipsPanel.gameObject.SetActive(true);
        tipsPanel.SetTipText(StringInternational.GetValue(content));
        topChatPanel.gameObject.SetActive(false);
        bottomChatPanel.gameObject.SetActive(false);
        tipsPanel.gameObject.SetActive(true);
    }

    public void ClearChats() {
        chat = null;
        chatIndex = 0;
        topChatPanel.gameObject.SetActive(false);
        bottomChatPanel.gameObject.SetActive(false);
        tipsPanel.gameObject.SetActive(false);
        Game.Data.Config.Status = (battlePanel != null && battlePanel.isActiveAndEnabled) ? Constant.EGameStatus.OnBattle : Constant.EGameStatus.InGame;
    }

    public void ChatBegan(Constant.ChatData chat, Modal mod) {
        this.chat = chat;
        chatMod = mod;
        ChatStepOn();
    }

    public void ChatStepOn() {
        if (chat == null)    // 没有chat数据, 说明是代码呼出的临时chat
        {
            ClearChats();
        } else if (chatIndex >= chat.data.Length) {
            chatIndex = 0;
            Game.Controller.EventMgr.DispatchEvent(chat.lastEventId, chatMod, chat.lastEventData);
            ClearChats();
        } else {
            var chatData = chat.data[chatIndex];
            if (chatData.speakerId < -100)
                ShowTips(chatData.content);
            else if (chatData.speakerId < 0)
                ShowChatOnTop(chatData.content, chatMod.ModId);
            else if (chatData.speakerId == 0)
                ShowChatOnBottom(chatData.content, Game.Controller.Player.PlayerId);
            else
                ShowChatOnTop(chatData.content, chatData.speakerId);
            ++chatIndex;
        }
    }

    /********************** Battle Part **************************************/

    public void StartBattle(long enemyUuid, long yourUuid = -1, BattleDlg.BattlePauseEventCheck pauseCheck = null, int pauseEvent = 0) {
        battlePanel = BattleDlg.StartBattle(dialogCanvas.transform, OnBattleOver, enemyUuid, yourUuid, pauseCheck, pauseEvent);
        battlePanel.transform.localPosition = new Vector3(0, 0, 12);
        battlePanel.transform.localScale = new Vector3(1, 1, 1);
    }

    public void StopBattle() {
        if (battlePanel != null) {
            BattleDlg.CloseBattle(battlePanel);
            battlePanel = null;
        }
    }

    private void OnBattleOver(int yourId, int yourLife, int goldGain, int expGain, int nextEvent, long nextEventData) {
        // 记录应用战斗结果（金币，经验，血量）
        if (yourId == Game.Controller.Player.PlayerId) {
            Game.Controller.Player.Life = yourLife;
            Game.Controller.Player.Gold += goldGain;
            Game.Controller.Player.Experience += expGain;
        }
        // TODO 添加对后续event的处理
    }

    /********************** Choice Part **************************************/

    public void StartChoice(Constant.ChoiceData choiceData, Modal mod, Constant.EGameStatus nextStatus = Constant.EGameStatus.InGame) {
        choicePanel = ChoiceDlg.StartChoice(dialogCanvas.transform, choiceData, mod, nextStatus);
        if (topChatPanel != null && topChatPanel.isActiveAndEnabled) {
            topChatPanel.gameObject.SetActive(false);
        }
        if (bottomChatPanel != null && bottomChatPanel.isActiveAndEnabled) {
            bottomChatPanel.gameObject.SetActive(false);
        }
        if (tipsPanel != null && tipsPanel.isActiveAndEnabled) {
            tipsPanel.gameObject.SetActive(false);
        }
    }

    /********************** Hero Info Part **************************************/

    public string MapName {
        get { return mapNameText.text; }
        set { mapNameText.text = value; }
    }

    public string BackgroundImage {
        get { return backgroundImg.sprite.name; }
        set { backgroundImg.sprite = Resources.Load<GameObject>(Constant.PREFAB_DIR + value).GetComponent<SpriteRenderer>().sprite; }
    }

    public string RoleName {
        get { return roleNameText.text; }
        set { roleNameText.text = value; }
    }

    public Sprite Portrait {
        get { return portrait.sprite; }
        set { portrait.sprite = value; }
    }

    public string Level {
        get { return levelText.text; }
        set { levelText.text = value; }
    }

    public string Experience {
        get { return expText.text; }
        set { expText.text = value; }
    }

    public string Life {
        get { return lifeText.text; }
        set { lifeText.text = value; }
    }

    public string Attack {
        get { return attackText.text; }
        set { attackText.text = value; }
    }

    public string Defense {
        get { return defenseText.text; }
        set { defenseText.text = value; }
    }

    public string Speed {
        get { return speedText.text; }
        set { speedText.text = value; }
    }

    public string Gold {
        get { return goldText.text; }
        set { goldText.text = value; }
    }

    public string YellowKey {
        get { return yellowKeyText.text; }
        set { yellowKeyText.text = value; }
    }

    public string BlueKey {
        get { return blueKeyText.text; }
        set { blueKeyText.text = value; }
    }

    public string RedKey {
        get { return redKeyText.text; }
        set { redKeyText.text = value; }
    }

    /**************************************************************************************************/

    public Curtain Curtain { get { return curtain; } }

    private Image backgroundImg;
    private Curtain curtain;

    private Text mapNameText;
    private Text roleNameText;
    private Image portrait;
    private Text levelText;
    private Text expText;
    private Text lifeText;
    private Text attackText;
    private Text defenseText;
    private Text speedText;
    private Text goldText;
    private Text yellowKeyText;
    private Text blueKeyText;
    private Text redKeyText;

    private Constant.ChatData chat;
    private Modal chatMod;
    private int chatIndex = 0;

    private GameObject dialogCanvas = null;
    private ChatDlg topChatPanel = null;
    private ChatDlg bottomChatPanel = null;
    private TipBar tipsPanel = null;
    private ChoiceDlg choicePanel = null;
    private BattleDlg battlePanel = null;
}
