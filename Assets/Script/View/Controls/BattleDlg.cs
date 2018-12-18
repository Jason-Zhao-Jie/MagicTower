using System;
using UnityEngine;
using UnityEngine.UI;

public class BattleDlg : ObjectPool.AElement
{
    public const string PREFAB_DIR = "BattleDlg";
    public const int PREFAB_ID = 1;
    public delegate bool BattlePauseEventCheck();
    public delegate void BattleOverCallback(int yourId, int yourLife, int goldGain, int expGain, int nextEvent, long nextEventData);

    private const string BATTLE_GOLD_GET_TEXT = "str_battleGoldGet";
    private const string BATTLE_EXP_GET_TEXT = "str_battleExpGet";
    private const string BATTLE_HURTED_TEXT = "str_battleHurted";

    public static BattleDlg StartBattle(Transform parent, BattleOverCallback cb, long enemyUuid, long yourUuid = -1, BattlePauseEventCheck pauseCheck = null, int pauseEvent = 0)
    {
        // 弹出战斗框
        var ret = ObjectPool.instance.GetAnElement<BattleDlg>(PREFAB_ID, ObjectPool.ElementType.Dialog, GetResourcePath());
        // 设定暂停触发器
        ret.battlePauseChecker = pauseCheck;
        ret.battlePauseEvent = pauseEvent;
        ret.overCallback = cb;

        // 设定战斗双方的信息
        ret.transform.SetParent(parent, false);
        ret.SetBattleInfo(enemyUuid, yourUuid);
        // 设定状态, 战斗开始
        DataCenter.instance.Status = Constant.EGameStatus.OnBattle;
        ret.isBattlePaused = false;

        return ret;
    }

    public static void CloseBattle(BattleDlg dlg)
    {
        dlg.StopBattle();
        ObjectPool.instance.RecycleAnElement(dlg);
    }

    // Use this for initialization
    void Awake ()
    {
        // 关联战斗框的控件
        playerSprite = transform.Find("Player").gameObject;
        playerNameText = transform.Find("Name_Player").GetComponent<Text>();
        playerNameText.fontSize = Convert.ToInt32(playerNameText.fontSize * ScreenAdaptator.instance.RealFontSize);
        playerSprite.transform.position = new Vector3(playerNameText.transform.position.x, playerSprite.transform.position.y, baseZOrder);
        playerLifeText = transform.Find("Life_Player").GetComponent<Text>();
        playerLifeText.fontSize = Convert.ToInt32(playerLifeText.fontSize * ScreenAdaptator.instance.RealFontSize);
        playerAttackText = transform.Find("Attack_Player").GetComponent<Text>();
        playerAttackText.fontSize = Convert.ToInt32(playerAttackText.fontSize * ScreenAdaptator.instance.RealFontSize);
        playerDefenseText = transform.Find("Defense_Player").GetComponent<Text>();
        playerDefenseText.fontSize = Convert.ToInt32(playerDefenseText.fontSize * ScreenAdaptator.instance.RealFontSize);
        playerSpeedText = transform.Find("Speed_Player").GetComponent<Text>();
        playerSpeedText.fontSize = Convert.ToInt32(playerSpeedText.fontSize * ScreenAdaptator.instance.RealFontSize);
        enemySprite = transform.Find("Enemy").gameObject;
        enemyNameText = transform.Find("Name_Enemy").GetComponent<Text>();
        enemyNameText.fontSize = Convert.ToInt32(enemyNameText.fontSize * ScreenAdaptator.instance.RealFontSize);
        enemySprite.transform.position = new Vector3(enemyNameText.transform.position.x, enemySprite.transform.position.y, baseZOrder);
        enemyLifeText = transform.Find("Life_Enemy").GetComponent<Text>();
        enemyLifeText.fontSize = Convert.ToInt32(enemyLifeText.fontSize * ScreenAdaptator.instance.RealFontSize);
        enemyAttackText = transform.Find("Attack_Enemy").GetComponent<Text>();
        enemyAttackText.fontSize = Convert.ToInt32(enemyAttackText.fontSize * ScreenAdaptator.instance.RealFontSize);
        enemyDefenseText = transform.Find("Defense_Enemy").GetComponent<Text>();
        enemyDefenseText.fontSize = Convert.ToInt32(enemyDefenseText.fontSize * ScreenAdaptator.instance.RealFontSize);
        enemySpeedText = transform.Find("Speed_Enemy").GetComponent<Text>();
        enemySpeedText.fontSize = Convert.ToInt32(enemySpeedText.fontSize * ScreenAdaptator.instance.RealFontSize);
        battleResultPanel = transform.Find("BattleResultPanel").gameObject;
        battleResultGoldText = battleResultPanel.transform.Find("GoldText").GetComponent<Text>();
        battleResultGoldText.fontSize = Convert.ToInt32(battleResultGoldText.fontSize*ScreenAdaptator.instance.RealFontSize);
        battleResultExpText = battleResultPanel.transform.Find("ExpText").GetComponent<Text>();
        battleResultExpText.fontSize = Convert.ToInt32(battleResultExpText.fontSize*ScreenAdaptator.instance.RealFontSize);
        battleHurtedText = battleResultPanel.transform.Find("HurtedText").GetComponent<Text>();
        battleHurtedText.fontSize = Convert.ToInt32(battleHurtedText.fontSize * ScreenAdaptator.instance.RealFontSize);
        battleResultPanel.SetActive(false);

    }
	
	// Update is called once per frame
	void FixedUpdate () {

        if (gameObject.activeSelf && !isBattlePaused && DataCenter.instance.Status == Constant.EGameStatus.OnBattle && (hitter == null || !hitter.isActiveAndEnabled))
        {
            if (playerBattleData.life <= 0 || enemyBattleData.life <= 0)
            {
                OnBattleOver();
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

    void SetBattleInfo(long enemy, long you = -1)
    {
        // 设定战斗双方的属性数据
        enemyUuid = enemy;        
        enemyBattleData = MapManager.instance.GetMonsterDataByUuid(enemyUuid);
        if (you < 0)
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
            playerBattleData = MapManager.instance.GetMonsterDataByUuid(you);

        rounds = 1;
        hurted = 0;
        isOurRound = true;

        // 设定我方的头像
        var playerModal = DataCenter.instance.modals[playerBattleData.id];
        ObjectPool.AElement obj = ObjectPool.instance.GetAnElement<Modal>(playerModal.id, ObjectPool.ElementType.Sprite, Constant.PREFAB_DIR + playerModal.prefabPath);
        if(obj == null)
            obj = ObjectPool.instance.GetAnElement<Player>(playerModal.id, ObjectPool.ElementType.Sprite, Constant.PREFAB_DIR + playerModal.prefabPath);
        obj.transform.SetParent(transform, false);
        obj.transform.position = playerSprite.transform.position;
        obj.transform.localScale = ScreenAdaptator.instance.BlockSize;
        obj.GetComponent<SpriteRenderer>().sortingOrder = playerSprite.GetComponent<SpriteRenderer>().sortingOrder;
        var mod = playerSprite.GetComponent<Modal>();
        if (mod != null)
            mod.RemoveSelf(false);
        else
            playerSprite.GetComponent<Player>().RemoveSelf();
        playerSprite = obj.gameObject;

        // 设定敌方的头像
        var enemyModal = DataCenter.instance.modals[enemyBattleData.id];
        obj = ObjectPool.instance.GetAnElement<Modal>(enemyModal.id, ObjectPool.ElementType.Sprite, Constant.PREFAB_DIR + enemyModal.prefabPath);
        obj.transform.SetParent(transform, false);
        obj.transform.position = enemySprite.transform.position;
        obj.transform.localScale = ScreenAdaptator.instance.BlockSize;
        obj.GetComponent<SpriteRenderer>().sortingOrder = enemySprite.GetComponent<SpriteRenderer>().sortingOrder;
        mod = enemySprite.GetComponent<Modal>();
        if (mod != null)
            mod.RemoveSelf(false);
        else
            enemySprite.GetComponent<Player>().RemoveSelf();
        enemySprite = obj.gameObject;

        // 将双方数据显示到界面
        playerNameText.text = StringInternational.GetValue(playerModal.name);
        playerLifeText.text = playerBattleData.life.ToString();
        playerAttackText.text = playerBattleData.attack.ToString();
        playerDefenseText.text = playerBattleData.defense.ToString();
        playerSpeedText.text = playerBattleData.speed.ToString();
        enemyNameText.text = StringInternational.GetValue(enemyModal.name);
        enemyLifeText.text = enemyBattleData.life.ToString();
        enemyAttackText.text = enemyBattleData.attack.ToString();
        enemyDefenseText.text = enemyBattleData.defense.ToString();
        enemySpeedText.text = enemyBattleData.speed.ToString();
    }

    public void PauseBattle()
    {
        isBattlePaused = true;
    }

    public void ResumeBattle()
    {
        isBattlePaused = false;
    }

    public void StopBattle()
    {
        MapManager.instance.RemoveThingOnMapWithModal(enemyUuid);
        DataCenter.instance.Status = Constant.EGameStatus.InGame;
        isBattlePaused = false;
    }

    private void CreateHitter(int weaponId, bool isOnEnemy, int damage, bool isCritical)
    {
        var data = DataCenter.instance.weapons[weaponId];
        hitter = ObjectPool.instance.GetAnElement<Zzhit>(weaponId * 2 + (isCritical?0:1),ObjectPool.ElementType.Hitter, Constant.PREFAB_DIR + (isCritical ? data.critPrefabPath : data.prefabPath));
        hitter.SetParam(data, isCritical);
        hitter.transform.SetParent((isOnEnemy ? enemySprite : playerSprite).transform, false);
        hitter.transform.position = hitter.transform.parent.position;
        hitter.transform.localScale = ScreenAdaptator.instance.BlockSize / 200;
        hitter.GetComponent<SpriteRenderer>().sortingOrder = hitter.transform.parent.GetComponent<SpriteRenderer>().sortingOrder + 1;
    }

    private void OnBattleOver()
    {
        isBattlePaused = true;
        battleResultGoldText.text = StringInternational.GetValue(BATTLE_GOLD_GET_TEXT, enemyBattleData.gold.ToString());
        battleResultExpText.text = StringInternational.GetValue(BATTLE_EXP_GET_TEXT, enemyBattleData.exp.ToString());
        battleHurtedText.text = StringInternational.GetValue(BATTLE_HURTED_TEXT, hurted.ToString());
        battleResultPanel.SetActive(true);
        overCallback(playerBattleData.id, playerBattleData.life, enemyBattleData.gold, enemyBattleData.exp, 0, 0);
        DataCenter.instance.Status = Constant.EGameStatus.OnBattleResult;
    }

    public override ObjectPool.ElementType GetPoolTypeId()
    {
        return ObjectPool.ElementType.Dialog;
    }

    public override bool OnCreate(ObjectPool.ElementType tid, int elemId, string resourcePath)
    {
        return true;
    }

    public override void OnReuse(ObjectPool.ElementType tid, int elemId)
    {
    }

    public override bool OnUnuse(ObjectPool.ElementType tid, int elemId)
    {
        return true;
    }

    public override bool RecycleSelf()
    {
        return RecycleSelf<BattleDlg>();
    }

    public override string ResourcePath { get { return Constant.DIALOG_DIR + PREFAB_DIR; } }
    public static string GetResourcePath() { return Constant.DIALOG_DIR + PREFAB_DIR; }

    public float BaseZOrder {
        get { return baseZOrder;}
        set {
            baseZOrder = value;
            transform.position = new Vector3(transform.position.x, transform.position.y, value);
            playerSprite.transform.position = new Vector3(playerNameText.transform.position.x, playerSprite.transform.position.y, value);
            enemySprite.transform.position = new Vector3(playerNameText.transform.position.x, playerSprite.transform.position.y, value);
        }
    }

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
    private BattleOverCallback overCallback;
    private BattlePauseEventCheck battlePauseChecker;
    private int battlePauseEvent;
    private bool isBattlePaused = true;
    private int rounds;
    private int hurted;
    private bool isOurRound;
    internal Zzhit hitter;

    private float baseZOrder;
}
