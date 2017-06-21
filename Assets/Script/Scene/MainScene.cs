﻿using System.Collections;
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
        var dialogCanvas = GameObject.Find("DialogCanvas");
        mapNameText = heroPanel.transform.Find("MapName").GetComponent<Text>();
        backgroundImg = GetComponent<Image>();
        mapPartRect = MapManager.GetMapPosition(transform.Find("MapPanel").GetComponent<RectTransform>());
		blockSize = new Vector3(mapPartRect.width * 100 / (Constant.MAP_BLOCK_LENGTH * Constant.MAP_BLOCK_BASE_SIZE), mapPartRect.height * 100 / (Constant.MAP_BLOCK_LENGTH * Constant.MAP_BLOCK_BASE_SIZE));
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

        // 预设各种对话框，然后隐藏它们
        topChatPanel = dialogCanvas.transform.Find("ChatPanelTop").gameObject;
        topChatSpeaker = topChatPanel.transform.Find("Speaker").gameObject;
        topChatSpeakerText = topChatPanel.transform.Find("SpeakerName").GetComponent<Text>();
        topChatText = topChatPanel.transform.Find("Text").GetComponent<Text>();
        topChatPanel.SetActive(false);
        bottomChatPanel = dialogCanvas.transform.Find("ChatPanelBottom").gameObject;
        bottomChatSpeaker = bottomChatPanel.transform.Find("Speaker").gameObject;
        bottomChatSpeakerText = bottomChatPanel.transform.Find("SpeakerName").GetComponent<Text>();
        bottomChatText = bottomChatPanel.transform.Find("Text").GetComponent<Text>();
        bottomChatPanel.SetActive(false);
        tipsPanel = dialogCanvas.transform.Find("TipPanel").gameObject;
        tipsText = tipsPanel.transform.Find("Text").GetComponent<Text>();
        tipsPanel.SetActive(false);
        battlePanel = dialogCanvas.transform.Find("BattlePanel").gameObject;
		battlePanel.SetActive(false);

		// 关联战斗框的控件
		playerSprite = battlePanel.transform.Find("Player").gameObject;
		playerNameText = battlePanel.transform.Find("Name_Player").GetComponent<Text>();
        playerLifeText = battlePanel.transform.Find("Life_Player").GetComponent<Text>();
        playerAttackText = battlePanel.transform.Find("Attack_Player").GetComponent<Text>();
        playerDefenseText = battlePanel.transform.Find("Defense_Player").GetComponent<Text>();
        playerSpeedText = battlePanel.transform.Find("Speed_Player").GetComponent<Text>();
        enemySprite = battlePanel.transform.Find("Enemy").gameObject;
        enemyNameText = battlePanel.transform.Find("Name_Enemy").GetComponent<Text>();
        enemyLifeText = battlePanel.transform.Find("Life_Enemy").GetComponent<Text>();
        enemyAttackText = battlePanel.transform.Find("Attack_Enemy").GetComponent<Text>();
        enemyDefenseText = battlePanel.transform.Find("Defense_Enemy").GetComponent<Text>();
        enemySpeedText = battlePanel.transform.Find("Speed_Enemy").GetComponent<Text>();


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

    private void FixedUpdate()
    {
        if (battlePanel.activeSelf && !isBattlePaused && DataCenter.instance.Status == Constant.EGameStatus.OnBattle && hitter == null)
        {
			if (playerBattleData.life <= 0 || enemyBattleData.life <= 0)
				OnBattleOver(playerBattleData.life);
            int hurt = 0;
            if (isOurRound)
            {
                hurt = MathHelper.GetHurt(playerBattleData.attack, playerBattleData.critical, enemyBattleData.defense, enemyBattleData.speed);
                enemyBattleData.life -= hurt;
                enemyLifeText.text = enemyBattleData.life.ToString();
            }

            else
            {
                hurt = MathHelper.GetHurt(enemyBattleData.attack, enemyBattleData.critical, playerBattleData.defense, playerBattleData.speed);
                playerBattleData.life -= hurt;
                playerLifeText.text = playerBattleData.life.ToString();
            }
            CreateHitter(isOurRound ? playerBattleData.weaponId : enemyBattleData.weaponId, isOurRound, hurt);
            isOurRound = !isOurRound;
        }
    }

/********************** Map Utilities **************************************/

    public void AddObjectToMap(GameObject obj, int posx, int posy, int posz = -2)
    {
        obj.transform.SetParent(transform.Find("MapPanel"));
        obj.transform.position = transform.Find("MapPanel").transform.
            TransformPoint(new Vector3((posx + (float)0.5) * Constant.MAP_BLOCK_BASE_SIZE * blockSize.x / 100 + mapPartRect.x,
                                       (posy + (float)0.5) * Constant.MAP_BLOCK_BASE_SIZE * blockSize.y / 100 + mapPartRect.y,
                                       posz));
        obj.transform.localScale = blockSize;
    }

/********************** Chat Part **************************************/

	public void ShowChatOnTop(string content, int speakerId = -1)
    {
        DataCenter.instance.Status = Constant.EGameStatus.OnTipChat;
        topChatPanel.SetActive(true);
        bottomChatPanel.SetActive(false);
        tipsPanel.SetActive(false);
        if (speakerId < 0)
            speakerId = PlayerController.instance.PlayerId;
        var modal = DataCenter.instance.GetModalById(speakerId);
        var obj = Instantiate(Resources.Load<GameObject>(Constant.PREFAB_DIR + modal.prefabPath));
        obj.transform.SetParent(topChatPanel.transform);
        obj.transform.position = topChatSpeaker.transform.position;
        obj.transform.localScale = blockSize;
        obj.GetComponent<SpriteRenderer>().sortingOrder = topChatSpeaker.GetComponent<SpriteRenderer>().sortingOrder;
        var mod = topChatSpeaker.GetComponent<Modal>();
        if (mod != null)
            mod.DestroySelf();
        else
            bottomChatSpeaker.GetComponent<Player>().DestroySelf();
        topChatSpeaker = obj;
        topChatSpeakerText.text = modal.name;
        topChatText.text = content;
    }

    public void ShowChatOnBottom(string content, int speakerId = -1)
	{
		DataCenter.instance.Status = Constant.EGameStatus.OnTipChat;
        topChatPanel.SetActive(false);
        bottomChatPanel.SetActive(true);
        tipsPanel.SetActive(false);
        if (speakerId < 0)
            speakerId = PlayerController.instance.PlayerId;
        var modal = DataCenter.instance.GetModalById(speakerId);
        var obj = Instantiate(Resources.Load<GameObject>(Constant.PREFAB_DIR + modal.prefabPath));
        obj.transform.SetParent(bottomChatPanel.transform);
        obj.transform.position = bottomChatSpeaker.transform.position;
        obj.transform.localScale = blockSize;
        obj.GetComponent<SpriteRenderer>().sortingOrder = bottomChatSpeaker.GetComponent<SpriteRenderer>().sortingOrder;
        var mod = bottomChatSpeaker.GetComponent<Modal>();
        if (mod != null)
            mod.DestroySelf();
        else
            bottomChatSpeaker.GetComponent<Player>().DestroySelf();
        bottomChatSpeaker = obj;
        bottomChatSpeakerText.text = modal.name;
        bottomChatText.text = content;
    }

    public void ShowTips(string content)
	{
		DataCenter.instance.Status = Constant.EGameStatus.OnTipChat;
		topChatPanel.SetActive(false);
		bottomChatPanel.SetActive(false);
        tipsPanel.SetActive(true);
        tipsText.text = content;
        AudioController.instance.PlaySound(20);
    }

    public void ClearChats()
    {
        topChatPanel.SetActive(false);
        bottomChatPanel.SetActive(false);
        tipsPanel.SetActive(false);
        DataCenter.instance.Status = battlePanel.activeSelf ? Constant.EGameStatus.OnBattle : Constant.EGameStatus.InGame;
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
            EventManager.instance.DispatchEvent(chat.lastEventId, chatMod);
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

	public void StartBattle(long enemyUuid, long yourUuid = -1, Constant.BattlePauseEventCheck pauseCheck = null, int pauseEvent = 0)
    {
        DataCenter.instance.Status = Constant.EGameStatus.OnBattle;
        battlePanel.SetActive(true);
        battlePauseChecker = pauseCheck;
        battlePauseEvent = pauseEvent;
        enemyBattleData = DataCenter.instance.GetMonsterDataById(ModalManager.GetModalByUuid(enemyUuid).ModId);
        if (yourUuid < 0)
            playerBattleData = new Constant.MonsterData()
            {
                id = PlayerController.instance.PlayerId,
                level = PlayerController.instance.Level,
                exp = 0,
                life = PlayerController.instance.Life,
                attack = PlayerController.instance.Attack,
                defense = PlayerController.instance.Defense,
                speed = PlayerController.instance.Speed,
                critical = PlayerController.instance.Critical,
                gold = 0,
                weaponId = PlayerController.instance.Weapon
            };
        else
            playerBattleData = DataCenter.instance.GetMonsterDataById(ModalManager.GetModalByUuid(yourUuid).ModId);

        rounds = 0;
        isOurRound = true;

        var playerModal = DataCenter.instance.GetModalById(playerBattleData.id);
        var obj = Instantiate(Resources.Load<GameObject>(Constant.PREFAB_DIR + playerModal.prefabPath));
        obj.transform.SetParent(bottomChatPanel.transform);
        obj.transform.position = playerSprite.transform.position;
        obj.transform.localScale = blockSize;
        obj.GetComponent<SpriteRenderer>().sortingOrder = playerSprite.GetComponent<SpriteRenderer>().sortingOrder;
        var mod = playerSprite.GetComponent<Modal>();
        if (mod != null)
            mod.DestroySelf();
        else
            playerSprite.GetComponent<Player>().DestroySelf();
        playerSprite = obj;

        var enemyModal = DataCenter.instance.GetModalById(enemyBattleData.id);
        obj = Instantiate(Resources.Load<GameObject>(Constant.PREFAB_DIR + enemyModal.prefabPath));
        obj.transform.SetParent(bottomChatPanel.transform);
        obj.transform.position = enemySprite.transform.position;
        obj.transform.localScale = blockSize;
        obj.GetComponent<SpriteRenderer>().sortingOrder = enemySprite.GetComponent<SpriteRenderer>().sortingOrder;
        mod = enemySprite.GetComponent<Modal>();
        if (mod != null)
            mod.DestroySelf();
        else
            enemySprite.GetComponent<Player>().DestroySelf();
        enemySprite = obj;

		playerNameText.text = playerModal.name;
		playerLifeText.text = playerBattleData.life.ToString();
		playerAttackText.text = playerBattleData.attack.ToString();
		playerDefenseText.text = playerBattleData.defense.ToString();
		playerSpeedText.text = playerBattleData.speed.ToString();
        enemyNameText.text = enemyModal.name;
        enemyLifeText.text = enemyBattleData.life.ToString();
		enemyAttackText.text = enemyBattleData.attack.ToString();
		enemyDefenseText.text = enemyBattleData.defense.ToString();
		enemySpeedText.text = enemyBattleData.speed.ToString();

	}

    public void PauseBattle(){
        isBattlePaused = true;
    }

    public void ResumeBattle(){
        isBattlePaused = false;
    }

    public void StopBattle(){
        battlePanel.SetActive(false);
        DataCenter.instance.Status = Constant.EGameStatus.InGame;
        isBattlePaused = false;
    }

    public void CreateHitter(int weaponId, bool isOnEnemy, int damage){
        var data = DataCenter.instance.GetWeaponDataById(weaponId);
		var obj = Instantiate(Resources.Load<GameObject>(Constant.PREFAB_DIR + data.prefabPath));
		hitter = obj.GetComponent<Zzhit>();
        hitter.SetParam(data);
        obj.transform.SetParent((isOnEnemy ? enemySprite : playerSprite).transform);
        obj.transform.position = obj.transform.parent.position;
		obj.transform.localScale = blockSize;
		obj.GetComponent<SpriteRenderer>().sortingOrder = obj.transform.parent.GetComponent<SpriteRenderer>().sortingOrder + 1;
    }

    private void OnBattleOver(int playerLife){
        isBattlePaused = true;
        // TODO: Create the result UI
    }

/********************** Choice Part **************************************/

    public void ShowChoice(){
        // TODO
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
    public Vector3 BlockSize{ get { return blockSize; }}

    private Text mapNameText;
    private Image backgroundImg;
    private Rect mapPartRect;
    private Vector3 blockSize;

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

    private GameObject topChatPanel;
    private GameObject topChatSpeaker;
    private Text topChatSpeakerText;
    private Text topChatText;
    private GameObject bottomChatPanel;
    private GameObject bottomChatSpeaker;
    private Text bottomChatSpeakerText;
    private Text bottomChatText;
    private GameObject tipsPanel;
    private Text tipsText;
    private Constant.ChatData chat;
    private Modal chatMod;
    private int chatIndex = 0;

	private GameObject battlePanel;
	private GameObject playerSprite;
	private Text playerNameText;
	private Text playerLifeText;
	private Text playerAttackText;
	private Text playerDefenseText;
	private Text playerSpeedText;
	private GameObject enemySprite;
	private Text enemyNameText;
	private Text enemyLifeText;
	private Text enemyAttackText;
	private Text enemyDefenseText;
	private Text enemySpeedText;
	private Constant.MonsterData playerBattleData;
	private Constant.MonsterData enemyBattleData;
    private Constant.BattlePauseEventCheck battlePauseChecker;
    private int battlePauseEvent;
    private bool isBattlePaused;
    private int rounds;
    private bool isOurRound;
    internal Zzhit hitter;
}
