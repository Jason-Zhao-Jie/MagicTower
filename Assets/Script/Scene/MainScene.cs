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
        var dialogCanvas = GameObject.Find("DialogCanvas");
        backgroundImg = GetComponent<Image>();
        ScreenAdaptator.instance.LoadOnMainScene(mapPanel.GetComponent<RectTransform>().rect);

        curtain = dialogCanvas.transform.Find("Curtain").GetComponent<Curtain>();
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

        // 预设各种对话框，然后隐藏它们
        //     顶部对话框
        topChatPanel = dialogCanvas.transform.Find("ChatPanelTop").gameObject;
        topChatSpeaker = topChatPanel.transform.Find("Speaker").gameObject;
        topChatSpeakerText = topChatPanel.transform.Find("SpeakerName").GetComponent<Text>();
        topChatSpeaker.transform.position = new Vector3(topChatSpeakerText.transform.position.x, topChatSpeaker.transform.position.y, topChatSpeaker.transform.position.z);
        topChatText = topChatPanel.transform.Find("Text").GetComponent<Text>();
        topChatPanel.transform.position = new Vector3(mapPanel.position.x + mapPanel.GetComponent<RectTransform>().rect.width, topChatPanel.transform.position.y, topChatPanel.transform.position.z);
        topChatPanel.SetActive(false);
        //     底部对话框
        bottomChatPanel = dialogCanvas.transform.Find("ChatPanelBottom").gameObject;
        bottomChatSpeaker = bottomChatPanel.transform.Find("Speaker").gameObject;
        bottomChatSpeakerText = bottomChatPanel.transform.Find("SpeakerName").GetComponent<Text>();
        bottomChatSpeaker.transform.position = new Vector3(bottomChatSpeakerText.transform.position.x, bottomChatSpeaker.transform.position.y, bottomChatSpeaker.transform.position.z);
        bottomChatText = bottomChatPanel.transform.Find("Text").GetComponent<Text>();
        bottomChatPanel.transform.position = new Vector3(mapPanel.position.x + mapPanel.GetComponent<RectTransform>().rect.width, bottomChatPanel.transform.position.y, bottomChatPanel.transform.position.z);
        bottomChatPanel.SetActive(false);
        //     提示框
        tipsPanel = dialogCanvas.transform.Find("TipPanel").gameObject;
        tipsText = tipsPanel.transform.Find("Text").GetComponent<Text>();
        tipsPanel.transform.position = new Vector3(mapPanel.position.x + mapPanel.GetComponent<RectTransform>().rect.width, tipsPanel.transform.position.y, tipsPanel.transform.position.z);
        tipsPanel.SetActive(false);
        //     选择对话框
        choicePanel = dialogCanvas.transform.Find("ChoicePanel").gameObject;
        choiceSpeaker = choicePanel.transform.Find("Speaker").gameObject;
        choiceSpeakerText = choicePanel.transform.Find("SpeakerName").GetComponent<Text>();
        choiceSpeaker.transform.position = new Vector3(choiceSpeakerText.transform.position.x, choiceSpeaker.transform.position.y, choiceSpeaker.transform.position.z);
        choiceTitleText = choicePanel.transform.Find("TitleText").GetComponent<Text>();
        choiceItemPanel = choicePanel.transform.Find("ItemPanel").gameObject;
        choicePanel.transform.position = new Vector3(mapPanel.position.x + mapPanel.GetComponent<RectTransform>().rect.width, choicePanel.transform.position.y, choicePanel.transform.position.z);
        firstChoiceItem = choiceItemPanel.transform.Find("FirstItem").gameObject;
        firstChoiceItem.GetComponent<Text>().text = "";
        choiceItems = new List<GameObject>();
        choicePanel.SetActive(false);

        // 关联战斗框的控件
        battlePanel = dialogCanvas.transform.Find("BattlePanel").gameObject;
        battlePanel.transform.position = new Vector3(mapPanel.position.x + mapPanel.GetComponent<RectTransform>().rect.width, battlePanel.transform.position.y, battlePanel.transform.position.z);
        playerSprite = battlePanel.transform.Find("Player").gameObject;
		playerNameText = battlePanel.transform.Find("Name_Player").GetComponent<Text>();
        playerSprite.transform.position = new Vector3(playerNameText.transform.position.x, playerSprite.transform.position.y, playerSprite.transform.position.z);
        playerLifeText = battlePanel.transform.Find("Life_Player").GetComponent<Text>();
        playerAttackText = battlePanel.transform.Find("Attack_Player").GetComponent<Text>();
        playerDefenseText = battlePanel.transform.Find("Defense_Player").GetComponent<Text>();
        playerSpeedText = battlePanel.transform.Find("Speed_Player").GetComponent<Text>();
        enemySprite = battlePanel.transform.Find("Enemy").gameObject;
        enemyNameText = battlePanel.transform.Find("Name_Enemy").GetComponent<Text>();
        enemySprite.transform.position = new Vector3(enemyNameText.transform.position.x, enemySprite.transform.position.y, enemySprite.transform.position.z);
        enemyLifeText = battlePanel.transform.Find("Life_Enemy").GetComponent<Text>();
        enemyAttackText = battlePanel.transform.Find("Attack_Enemy").GetComponent<Text>();
        enemyDefenseText = battlePanel.transform.Find("Defense_Enemy").GetComponent<Text>();
        enemySpeedText = battlePanel.transform.Find("Speed_Enemy").GetComponent<Text>();
        battleResultPanel = battlePanel.transform.Find("BattleResultPanel").gameObject;
        battleResultGoldText = battleResultPanel.transform.Find("GoldText").GetComponent<Text>();
        battleResultExpText = battleResultPanel.transform.Find("ExpText").GetComponent<Text>();
        battleHurtedText = battleResultPanel.transform.Find("HurtedText").GetComponent<Text>();
        battleResultPanel.SetActive(false);
        battlePanel.SetActive(false);


        AudioController.instance.ClearSoundSource();
        AudioController.instance.MusicSource = GetComponent<AudioSource>();
        AudioController.instance.AddSoundSource(GameObject.Find("Main Camera").GetComponent<AudioSource>());
        AudioController.instance.AddSoundSource(dialogCanvas.GetComponent<AudioSource>());
        AudioController.instance.AddSoundSource(transform.Find("HeroPanel").GetComponent<AudioSource>());
        AudioController.instance.AddSoundSource(transform.Find("ItemPanel").GetComponent<AudioSource>());
        AudioController.instance.AddSoundSource(transform.Find("MapPanel").GetComponent<AudioSource>());
        MapManager.instance.ShowMap();
        PlayerController.instance.ShowPlayer(true);
        PlayerController.instance.SyncPlayerData();
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
            {
                OnBattleOver(playerBattleData.life);
            }
            else
            {
                int hurt = 0;
                if (isOurRound)
                {
                    hurt = MathHelper.GetHurt(playerBattleData.attack, playerBattleData.critical, enemyBattleData.defense, enemyBattleData.speed);
                    if (hurt == -1)
                    {
                        CreateHitter(Constant.MISS_HITTER, true, 0, false);
                        Debug.Log("Enemy has missed a hurt");
                    }
                    else if (hurt == 0)
                    {
                        CreateHitter(Constant.NOHURT_HITTER, true, 0, false);
                        Debug.Log("Enemy was hurted failed");
                    }
                    else if (hurt < 0)
                    {
                        CreateHitter(playerBattleData.weaponId, true, -hurt, true);
                        enemyBattleData.life += hurt;
                        Debug.Log("Enemy was hurted critical : " + -hurt);
                    }
                    else
                    {
                        CreateHitter(playerBattleData.weaponId, true, hurt, false);
                        enemyBattleData.life -= hurt;
                        Debug.Log("Enemy was hurted normally : " + hurt);
                    }
                    if (enemyBattleData.life < 0)
                        enemyBattleData.life = 0;
                    enemyLifeText.text = enemyBattleData.life.ToString();
                    ++rounds;
                }

                else
                {
                    hurt = MathHelper.GetHurt(enemyBattleData.attack, enemyBattleData.critical, playerBattleData.defense, playerBattleData.speed);
                    if (hurt == -1)
                    {
                        CreateHitter(Constant.MISS_HITTER, false, 0, false);
                        Debug.Log("Player has missed a hurt");
                    }
                    else if (hurt == 0)
                    {
                        CreateHitter(Constant.NOHURT_HITTER, false, 0, false);
                        Debug.Log("Player was hurted failed");
                    }
                    else if (hurt < 0)
                    {
                        CreateHitter(enemyBattleData.weaponId, false, -hurt, true);
                        playerBattleData.life += hurt;
                        Debug.Log("Player was hurted critical : " + -hurt);
                        hurted -= hurt;
                    }
                    else
                    {
                        CreateHitter(enemyBattleData.weaponId, false, hurt, false);
                        playerBattleData.life -= hurt;
                        Debug.Log("Player was hurted normally : " + hurt);
                        hurted += hurt;
                    }
                    if (playerBattleData.life < 0)
                        playerBattleData.life = 0;
                    playerLifeText.text = playerBattleData.life.ToString();
                }
                isOurRound = !isOurRound;
            }
        }
    }

    /********************** Map Utilities **************************************/

    public void AddObjectToMap(GameObject obj, int posx, int posy, int posz = -2)
    {
        obj.transform.SetParent(transform.Find("MapPanel"));
        obj.transform.position = transform.Find("MapPanel").transform.
            TransformPoint(new Vector3((posx + (float)0.5) * Constant.MAP_BLOCK_BASE_SIZE * ScreenAdaptator.instance.BlockSize.x / 100 + ScreenAdaptator.instance.MapPartRect.x,
                                       (posy + (float)0.5) * Constant.MAP_BLOCK_BASE_SIZE * ScreenAdaptator.instance.BlockSize.y / 100 + ScreenAdaptator.instance.MapPartRect.y,
                                       posz));
        obj.transform.localScale = ScreenAdaptator.instance.BlockSize;
    }

/********************** Chat Part **************************************/

	public void ShowChatOnTop(string content, int speakerId = -1)
    {
        // 设定游戏状态
        DataCenter.instance.Status = Constant.EGameStatus.OnTipChat;
        // 弹出对话框
        topChatPanel.SetActive(true);
        bottomChatPanel.SetActive(false);
        tipsPanel.SetActive(false);
        // 查找对话者数据, 贴上人物头像
        if (speakerId < 0)
            speakerId = PlayerController.instance.PlayerId;
        var modal = DataCenter.instance.modals[speakerId];
        var obj = Instantiate(Resources.Load<GameObject>(Constant.PREFAB_DIR + modal.prefabPath));
        obj.transform.SetParent(topChatPanel.transform);
        obj.transform.position = topChatSpeaker.transform.position;
        obj.transform.localScale = ScreenAdaptator.instance.BlockSize;
        obj.GetComponent<SpriteRenderer>().sortingOrder = topChatSpeaker.GetComponent<SpriteRenderer>().sortingOrder;
        var mod = topChatSpeaker.GetComponent<Modal>();
        if (mod != null)
            mod.RemoveSelf(false);
        else
            bottomChatSpeaker.GetComponent<Player>().RemoveSelf();
        topChatSpeaker = obj;
        // 对话者名字
        topChatSpeakerText.text = modal.name;
        // 对话内容
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
        var modal = DataCenter.instance.modals[speakerId];
        var obj = Instantiate(Resources.Load<GameObject>(Constant.PREFAB_DIR + modal.prefabPath));
        obj.transform.SetParent(bottomChatPanel.transform);
        obj.transform.position = bottomChatSpeaker.transform.position;
        obj.transform.localScale = ScreenAdaptator.instance.BlockSize;
        obj.GetComponent<SpriteRenderer>().sortingOrder = bottomChatSpeaker.GetComponent<SpriteRenderer>().sortingOrder;
        var mod = bottomChatSpeaker.GetComponent<Modal>();
        if (mod != null)
            mod.RemoveSelf(false);
        else
            bottomChatSpeaker.GetComponent<Player>().RemoveSelf();
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

    public void StartBattle(long enemyUuid, long yourUuid = -1, Constant.BattlePauseEventCheck pauseCheck = null, int pauseEvent = 0)
    {
        // 设定状态
        DataCenter.instance.Status = Constant.EGameStatus.OnBattle;
        // 弹出战斗框
        battlePanel.SetActive(true);
        // 设定暂停触发器
        battlePauseChecker = pauseCheck;
        battlePauseEvent = pauseEvent;
        // 设定战斗双方的属性数据
        enemyBattleData = MapManager.instance.GetMonsterDataByUuid(enemyUuid);
        this.enemyUuid = enemyUuid;
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
            playerBattleData = MapManager.instance.GetMonsterDataByUuid(yourUuid);

        rounds = 1;
        hurted = 0;
        isOurRound = true;

        // 设定我方的头像
        var playerModal = DataCenter.instance.modals[playerBattleData.id];
        var obj = Instantiate(Resources.Load<GameObject>(Constant.PREFAB_DIR + playerModal.prefabPath));
        obj.transform.SetParent(battlePanel.transform);
        obj.transform.position = playerSprite.transform.position;
        obj.transform.localScale = ScreenAdaptator.instance.BlockSize;
        obj.GetComponent<SpriteRenderer>().sortingOrder = playerSprite.GetComponent<SpriteRenderer>().sortingOrder;
        var mod = playerSprite.GetComponent<Modal>();
        if (mod != null)
            mod.RemoveSelf(false);
        else
            playerSprite.GetComponent<Player>().RemoveSelf();
        playerSprite = obj;

        // 设定敌方的头像
        var enemyModal = DataCenter.instance.modals[enemyBattleData.id];
        obj = Instantiate(Resources.Load<GameObject>(Constant.PREFAB_DIR + enemyModal.prefabPath));
        obj.transform.SetParent(battlePanel.transform);
        obj.transform.position = enemySprite.transform.position;
        obj.transform.localScale = ScreenAdaptator.instance.BlockSize;
        obj.GetComponent<SpriteRenderer>().sortingOrder = enemySprite.GetComponent<SpriteRenderer>().sortingOrder;
        mod = enemySprite.GetComponent<Modal>();
        if (mod != null)
            mod.RemoveSelf(false);
        else
            enemySprite.GetComponent<Player>().RemoveSelf();
        enemySprite = obj;

        // 将双方数据显示到界面
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
        battleResultPanel.SetActive(false);
        battlePanel.SetActive(false);
        MapManager.instance.RemoveThingOnMapWithModal(enemyUuid);
        DataCenter.instance.Status = Constant.EGameStatus.InGame;
        isBattlePaused = false;
    }

    private void CreateHitter(int weaponId, bool isOnEnemy, int damage, bool isCritical){
        var data = DataCenter.instance.weapons[weaponId];
		var obj = Instantiate(Resources.Load<GameObject>(Constant.PREFAB_DIR + (isCritical?data.critPrefabPath:data.prefabPath)));
		hitter = obj.GetComponent<Zzhit>();
        hitter.SetParam(data, isCritical);
        obj.transform.SetParent((isOnEnemy ? enemySprite : playerSprite).transform);
        obj.transform.position = obj.transform.parent.position;
		obj.transform.localScale = ScreenAdaptator.instance.BlockSize / 200;
		obj.GetComponent<SpriteRenderer>().sortingOrder = obj.transform.parent.GetComponent<SpriteRenderer>().sortingOrder + 1;
    }

    private void OnBattleOver(int playerLife){
        isBattlePaused = true;
        battleResultGoldText.text = enemyBattleData.gold.ToString();
        battleResultExpText.text = enemyBattleData.exp.ToString();
        battleHurtedText.text = hurted.ToString();
        battleResultPanel.SetActive(true);
        // 记录应用战斗结果（金币，经验，血量）
        if(playerBattleData.id == PlayerController.instance.PlayerId)
        {
            PlayerController.instance.Life = playerBattleData.life;
            PlayerController.instance.Gold += enemyBattleData.gold;
            PlayerController.instance.Experience += enemyBattleData.exp;
        }

        DataCenter.instance.Status = Constant.EGameStatus.OnBattleResult;
    }

    /********************** Choice Part **************************************/

    private void ShowChoice()
    {
        // 设定游戏状态
        DataCenter.instance.Status = Constant.EGameStatus.OnChoice;
        // 显示选择对话框
        choicePanel.SetActive(true);
        // 显示对话者头像
        if (choice.speakerId < 0)
            choice.speakerId = PlayerController.instance.PlayerId;
        var modal = DataCenter.instance.modals[choice.speakerId];
        var obj = Instantiate(Resources.Load<GameObject>(Constant.PREFAB_DIR + modal.prefabPath));
        obj.transform.SetParent(choicePanel.transform);
        obj.transform.position = choiceSpeaker.transform.position;
        obj.transform.localScale = ScreenAdaptator.instance.BlockSize;
        obj.GetComponent<SpriteRenderer>().sortingOrder = choiceSpeaker.GetComponent<SpriteRenderer>().sortingOrder;
        var mod = choiceSpeaker.GetComponent<Modal>();
        if (mod != null)
            mod.RemoveSelf(false);
        else
            bottomChatSpeaker.GetComponent<Player>().RemoveSelf();
        choiceSpeaker = obj;
        choiceSpeakerText.text = modal.name;
        // 设定选择的标题介绍对话的内容
        choiceTitleText.text = choice.title;
        // 添加选项
        foreach(var i in choice.data)
        {
            CreateChoiceItem(i.content);
        }
        // 矫正选择框的大小
        RedrawItems();
    }

    public void ClearChoice(Constant.EGameStatus nextStatue = Constant.EGameStatus.InGame)
    {
        choicePanel.SetActive(true);
        // 清除选项
        foreach (var i in choiceItems)
        {
            Destroy(i);
        }
        choiceItems.Clear();
        firstChoiceItem.GetComponent<Text>().text = "";
        // 隐藏选择对话框
        choicePanel.SetActive(false);
        DataCenter.instance.Status = nextStatue;
    }

    public void ChoiceBegan(Constant.ChoiceData choiceData, Modal mod)
    {
        choice = choiceData;
        choiceMod = mod;
        ShowChoice();
    }

    private GameObject CreateChoiceItem(string content)
    {
        if (firstChoiceItem.GetComponent<Text>().text.Equals(""))
        {
            firstChoiceItem.GetComponent<Text>().text = content;
            return firstChoiceItem;
        }
        else
        {
            var clonedItem = Instantiate(firstChoiceItem);
            clonedItem.GetComponent<Text>().text = content;
            choiceItems.Add(clonedItem);
            clonedItem.transform.SetParent(choiceItemPanel.transform);
            clonedItem.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            clonedItem.GetComponent<Button>().onClick.AddListener(delegate () { OnItemClicked(choiceItems.Count); });
            return clonedItem;
        }
    }

    public void ChooseToItem(uint index)
    {
        if (index > choiceItems.Count)
            return;
        if (index == 0)
            firstChoiceItem.GetComponent<Text>().fontStyle = FontStyle.Bold;
        else
            choiceItems[System.Convert.ToInt32(index) + 1].GetComponent<Text>().fontStyle = FontStyle.Bold;
        if (chosenIndex == 0)
            firstChoiceItem.GetComponent<Text>().fontStyle = FontStyle.Bold;
        else
            choiceItems[System.Convert.ToInt32(chosenIndex) + 1].GetComponent<Text>().fontStyle = FontStyle.Normal;
        chosenIndex = index;
    }

    private void RedrawItems()
    {
        // TODO
    }

    public void OnItemClicked(int index)
    {
        choicePanel.SetActive(false);
        EventManager.instance.DispatchEvent(choice.data[chosenIndex].eventId, choiceMod, choice.data[chosenIndex].eventData);
        ClearChoice();
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
    
    private GameObject choicePanel;
    private GameObject choiceSpeaker;
    private Text choiceTitleText;
    private Text choiceSpeakerText;
    private GameObject choiceItemPanel;
    private Constant.ChoiceData choice;
    private Modal choiceMod;
    private GameObject firstChoiceItem;
    private List<GameObject> choiceItems;
    private uint chosenIndex;

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
    private GameObject battleResultPanel;
    private Text battleResultGoldText;
    private Text battleResultExpText;
    private Text battleHurtedText;
    private long enemyUuid;
    private Constant.MonsterData playerBattleData;
	private Constant.MonsterData enemyBattleData;
    private Constant.BattlePauseEventCheck battlePauseChecker;
    private int battlePauseEvent;
    private bool isBattlePaused;
    private int rounds;
    private int hurted;
    private bool isOurRound;
    internal Zzhit hitter;
}
