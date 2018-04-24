using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainScene : MonoBehaviour
{
    public static MainScene instance;
    // Use this for initialization
    void Start()
    {
        instance = this;


        var heroPanel = transform.Find("HeroPanel");
        var itemPanel = transform.Find("ItemPanel");
        var mapPanel = transform.Find("MapPanel");
        dialogCanvas = GameObject.Find("DialogCanvas");
        backgroundImg = GetComponent<Image>();
        ScreenAdaptator.instance.LoadOnMainScene(mapPanel.GetComponent<RectTransform>().rect);

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

        if (AudioController.instance != null)
        {
            AudioController.instance.ClearSoundSource();
            AudioController.instance.MusicSource = GetComponent<AudioSource>();
            AudioController.instance.AddSoundSource(GameObject.Find("Main Camera").GetComponent<AudioSource>());
            AudioController.instance.AddSoundSource(dialogCanvas.GetComponent<AudioSource>());
            AudioController.instance.AddSoundSource(transform.Find("HeroPanel").GetComponent<AudioSource>());
            AudioController.instance.AddSoundSource(transform.Find("ItemPanel").GetComponent<AudioSource>());
            AudioController.instance.AddSoundSource(transform.Find("MapPanel").GetComponent<AudioSource>());
        }
        if (MapManager.instance != null)
            MapManager.instance.ShowMap();
        if (PlayerController.instance != null)
        {
            PlayerController.instance.ShowPlayer(true);
            PlayerController.instance.SyncPlayerData();
        }
        if (DataCenter.instance != null)
            DataCenter.instance.Status = Constant.EGameStatus.InGame;
        
    }

    void OnDestroy()
    {
        MapManager.instance.ClearMap();
        DataCenter.instance.Status = Constant.EGameStatus.Start;
        instance = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (InputController.instance == null)
            return;
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

    /********************** Map Utilities **************************************/

    public void AddObjectToMap(GameObject obj, int posx, int posy, int posz = -2)
    {
        obj.transform.SetParent(transform.Find("MapPanel"), false);
        obj.transform.position = transform.Find("MapPanel").transform.
            TransformPoint(new Vector3((posx + (float)0.5) * Constant.MAP_BLOCK_BASE_SIZE * ScreenAdaptator.instance.BlockSize.x / 100 + ScreenAdaptator.instance.MapPartRect.x,
                                       (posy + (float)0.5) * Constant.MAP_BLOCK_BASE_SIZE * ScreenAdaptator.instance.BlockSize.y / 100 + ScreenAdaptator.instance.MapPartRect.y,
                                       posz));
        obj.transform.localScale = ScreenAdaptator.instance.BlockSize;
    }

/********************** Chat Part **************************************/

	public void ShowChatOnTop(string content, int speakerId = -1)
    {
        DataCenter.instance.Status = Constant.EGameStatus.OnTipChat;
        topChatPanel.gameObject.SetActive(true);
        topChatPanel.SetChat(content, speakerId);
        topChatPanel.gameObject.SetActive(true);
        bottomChatPanel.gameObject.SetActive(false);
        tipsPanel.gameObject.SetActive(false);
    }

    public void ShowChatOnBottom(string content, int speakerId = -1)
    {
        DataCenter.instance.Status = Constant.EGameStatus.OnTipChat;
        bottomChatPanel.gameObject.SetActive(true);
        bottomChatPanel.SetChat(content, speakerId);
        topChatPanel.gameObject.SetActive(false);
        bottomChatPanel.gameObject.SetActive(true);
        tipsPanel.gameObject.SetActive(false);
    }

    public void ShowTips(string content)
	{
		DataCenter.instance.Status = Constant.EGameStatus.OnTipChat;
        tipsPanel.gameObject.SetActive(true);
        tipsPanel.SetTipText(content);
        topChatPanel.gameObject.SetActive(false);
        bottomChatPanel.gameObject.SetActive(false);
        tipsPanel.gameObject.SetActive(true);
    }

    public void ClearChats()
    {
        topChatPanel.gameObject.SetActive(false);
        bottomChatPanel.gameObject.SetActive(false);
        tipsPanel.gameObject.SetActive(false);
        DataCenter.instance.Status = (battlePanel!=null && battlePanel.isActiveAndEnabled) ? Constant.EGameStatus.OnBattle : Constant.EGameStatus.InGame;
    }

    public void ChatBegan(Constant.ChatData chat, Modal mod)
    {
        this.chat = chat;
        chatMod = mod;
        ChatStepOn();
    }

    public void ChatStepOn(){
        if (chatIndex >= chat.data.Length)
        {
            chatIndex = 0;
            ClearChats();
            EventManager.instance.DispatchEvent(chat.lastEventId, chatMod, chat.lastEventData);
        }
        else{
            var chatData = chat.data[chatIndex];
            if (chatData.speakerId < -100)
                ShowTips(chatData.content);
            else if (chatData.speakerId < 0)
                ShowChatOnTop(chatData.content, chatMod.ModId);
            else if (chatData.speakerId == 0)
                ShowChatOnBottom(chatData.content, PlayerController.instance.PlayerId);
			else
				ShowChatOnTop(chatData.content, chatData.speakerId);
            ++chatIndex;
        }
    }

    /********************** Battle Part **************************************/

    public void StartBattle(long enemyUuid, long yourUuid = -1, BattleDlg.BattlePauseEventCheck pauseCheck = null, int pauseEvent = 0)
    {
        battlePanel = BattleDlg.StartBattle(dialogCanvas.transform, OnBattleOver, enemyUuid, yourUuid, pauseCheck, pauseEvent);
        battlePanel.transform.localPosition = new Vector3(0, 0, 12);
        battlePanel.transform.localScale = new Vector3(1, 1, 1);
    }

    public void StopBattle()
    {
        if (battlePanel != null)
        {
            BattleDlg.CloseBattle(battlePanel);
            battlePanel = null;
        }
    }

    private void OnBattleOver(int yourId, int yourLife, int goldGain, int expGain, int nextEvent, long nextEventData)
    {
        // 记录应用战斗结果（金币，经验，血量）
        if (yourId == PlayerController.instance.PlayerId)
        {
            PlayerController.instance.Life = yourLife;
            PlayerController.instance.Gold += goldGain;
            PlayerController.instance.Experience += expGain;
        }
        // TODO 添加对后续event的处理
    }

    /********************** Choice Part **************************************/

    public void StartChoice(Constant.ChoiceData choiceData, Modal mod, Constant.EGameStatus nextStatus = Constant.EGameStatus.InGame)
    {
        choicePanel = ChoiceDlg.StartChoice(dialogCanvas.transform, choiceData, mod, nextStatus);
        if (topChatPanel != null && topChatPanel.isActiveAndEnabled)
        {
            topChatPanel.gameObject.SetActive(false);
        }
        if (bottomChatPanel != null && bottomChatPanel.isActiveAndEnabled)
        {
            bottomChatPanel.gameObject.SetActive(false);
        }
        if (tipsPanel != null && tipsPanel.isActiveAndEnabled)
        {
            tipsPanel.gameObject.SetActive(false);
        }
    }

    /********************** Hero Info Part **************************************/

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

    public string RoleName
    {
        get { return roleNameText.text; }
        set { roleNameText.text = value; }
    }

    public Sprite Portrait
    {
        get { return portrait.sprite; }
        set { portrait.sprite = value; }
    }

	public string Level
	{
        get { return levelText.text; }
		set { levelText.text = value; }
	}

	public string Experience
	{
        get { return expText.text; }
		set { expText.text = value; }
	}

    public string Life
	{
        get { return lifeText.text; }
		set { lifeText.text = value; }
	}

	public string Attack
	{
        get { return attackText.text; }
		set { attackText.text = value; }
	}

    public string Defense
	{
        get { return defenseText.text; }
		set { defenseText.text = value; }
	}

	public string Speed
	{
		get { return speedText.text; }
        set { speedText.text = value; }
	}

	public string Gold
	{
        get { return goldText.text; }
		set { goldText.text = value; }
	}

	public string YellowKey
	{
        get { return yellowKeyText.text; }
		set { yellowKeyText.text = value; }
	}

	public string BlueKey
	{
        get { return blueKeyText.text; }
		set { blueKeyText.text = value; }
	}

	public string RedKey
	{
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
