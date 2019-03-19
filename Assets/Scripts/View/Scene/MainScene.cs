using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainScene : AScene {
    override public SceneType Type { get { return SceneType.MainScene; } }

    // Use this for initialization
    void Start()
    {
        Game.CurrentScene = this;
        Game.Initial();

        dialogCanvas = GameObject.Find("DialogCanvas");

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

        Game.Managers.Audio.ClearSoundSource();
        Game.Managers.Audio.MusicSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        Game.Managers.Audio.AddSoundSource(GetComponent<AudioSource>());
        Game.Managers.Audio.AddSoundSource(dialogCanvas.GetComponent<AudioSource>());
        Game.Managers.Audio.AddSoundSource(transform.Find("PlayerPanel").GetComponent<AudioSource>());
        Game.Managers.Audio.AddSoundSource(transform.Find("MapPanel").GetComponent<AudioSource>());

        Game.Player = new PlayerController(transform.Find("PlayerPanel").GetComponent<PlayerView>());
        Game.Map = new MapController(transform.Find("MapPanel").GetComponent<MapView>(), dialogCanvas.transform.Find("Curtain").GetComponent<Curtain>(), GetComponent<Image>());
        Game.Map.ShowMap();
        Game.Player.ShowPlayer(true);

        Game.Status = Constant.EGameStatus.InGame;
    }

    void OnDestroy()
    {
        Game.ObjPool.ClearAll();
        Game.Map.ClearMap();
        Game.Map = null;
        Game.Player = null;
    }

    // Update is called once per frame
    void Update() {
        // 监测键盘和手柄按键
        for (int i = 0; i < InputManager.listenedKeys.Length; ++i) {
            bool isDown = Input.GetKey(InputManager.listenedKeys[i]);
            bool hasDown = Game.Managers.Input.keyStatusMap[InputManager.listenedKeys[i]];
            if (isDown && !hasDown)
                Game.Managers.Input.OnKeyDown(InputManager.listenedKeys[i]);
            else if (hasDown && !isDown)
                Game.Managers.Input.OnKeyUp(InputManager.listenedKeys[i]);
        }

        // 检测手柄摇杆
        Game.Managers.Input.OnJoysticsRockerAxes(InputManager.JoysticsAxes.LeftHorizontal, Input.GetAxis("Horizontal_Left"));
        Game.Managers.Input.OnJoysticsRockerAxes(InputManager.JoysticsAxes.LeftVertical, Input.GetAxis("Vertical_Left"));
        Game.Managers.Input.OnJoysticsRockerAxes(InputManager.JoysticsAxes.RightHorizontal, Input.GetAxis("Horizontal_XBoxRight"));
        Game.Managers.Input.OnJoysticsRockerAxes(InputManager.JoysticsAxes.RightVertical, Input.GetAxis("Vertical_XBoxRight"));
        Game.Managers.Input.OnJoysticsRockerAxes(InputManager.JoysticsAxes.SpecialHorizontal, Input.GetAxis("Horizontal_XBoxSpecial"));
        Game.Managers.Input.OnJoysticsRockerAxes(InputManager.JoysticsAxes.SpecialVertical, Input.GetAxis("Vertical_XBoxSpecial"));

        // 监测鼠标和触屏
        for (int i = 0; i < Input.touchCount; ++i) {
            var tc = Input.GetTouch(i);
            switch (tc.phase) {
                case TouchPhase.Began:
                    Game.Managers.Input.OnTouchDown(tc.position);
                    break;
                case TouchPhase.Canceled:
                case TouchPhase.Ended:
                    Game.Managers.Input.OnTouchUp(tc.position, new Vector2(tc.position.x - tc.deltaPosition.x, tc.position.y - tc.deltaPosition.y));
                    break;
            }
        }

        if (Input.touchCount <= 0) {
            if (Input.GetMouseButtonDown(0) && !Game.Managers.Input.isMouseLeftDown)
                Game.Managers.Input.OnTouchDown(new Vector2(Input.mousePosition.x, Input.mousePosition.y), true);
            if (Input.GetMouseButtonUp(0) && Game.Managers.Input.isMouseLeftDown)
                Game.Managers.Input.OnTouchUp(new Vector2(Input.mousePosition.x, Input.mousePosition.y), false);
        }
    }

    public override void OnMapClicked(int posx, int posy)
    {
        Game.Player.StartAutoStep(posx, posy);
    }

    /********************** Chat Part **************************************/

    public void ShowChatOnTop(string content, int speakerId = -1) {
        Game.Status = Constant.EGameStatus.OnTipChat;
        topChatPanel.gameObject.SetActive(true);
        bottomChatPanel.gameObject.SetActive(false);
        tipsPanel.gameObject.SetActive(false);
        topChatPanel.SetChat(Game.Config.StringInternational.GetValue(content), speakerId);
    }

    public void ShowChatOnBottom(string content, int speakerId = -1) {
        Game.Status = Constant.EGameStatus.OnTipChat;
        topChatPanel.gameObject.SetActive(false);
        bottomChatPanel.gameObject.SetActive(true);
        tipsPanel.gameObject.SetActive(false);
        bottomChatPanel.SetChat(Game.Config.StringInternational.GetValue(content), speakerId);
    }

    public void ShowTips(string content) {
        Game.Status = Constant.EGameStatus.OnTipChat;
        topChatPanel.gameObject.SetActive(false);
        bottomChatPanel.gameObject.SetActive(false);
        tipsPanel.gameObject.SetActive(true);
        tipsPanel.SetTipText(Game.Config.StringInternational.GetValue(content));
    }

    public void ClearChats() {
        chat = null;
        chatIndex = 0;
        topChatPanel.gameObject.SetActive(false);
        bottomChatPanel.gameObject.SetActive(false);
        tipsPanel.gameObject.SetActive(false);
        Game.Status = (battlePanel != null && battlePanel.isActiveAndEnabled) ? Constant.EGameStatus.OnBattle : Constant.EGameStatus.InGame;
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
            Game.Managers.EventMgr.DispatchEvent(chat.lastEventId, chatMod, chat.lastEventData);
            ClearChats();
        } else {
            var chatData = chat.data[chatIndex];
            if (chatData.speakerId < -100)
                ShowTips(chatData.content);
            else if (chatData.speakerId < 0)
                ShowChatOnTop(chatData.content, chatMod.ModId);
            else if (chatData.speakerId == 0)
                ShowChatOnBottom(chatData.content, Game.Player.PlayerId);
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
        if (yourId == Game.Player.PlayerId) {
            Game.Player.Life = yourLife;
            Game.Player.Gold += goldGain;
            Game.Player.Experience += expGain;
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

    /**************************************************************************************************/

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
