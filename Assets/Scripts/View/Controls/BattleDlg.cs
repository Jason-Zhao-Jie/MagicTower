﻿using System;
using UnityEngine;
using UnityEngine.UI;

public class BattleDlg : ObjectPool.AViewUnit
{
    public const string PREFAB_DIR = "BattleDlg";
    public const int PREFAB_ID = 1;
    public delegate bool BattlePauseEventCheck();
    public delegate void BattleOverCallback(int yourId, int yourLife, int goldGain, int expGain, int nextEvent, long nextEventData);

    private const int BATTLE_SLEEP_TIME = 2; // 修改此值以控制两次攻击之间的间隔, 使得攻击表现更为流畅, 已扣除攻击动画时间. 单位:帧
    private const string BATTLE_GOLD_GET_TEXT = "str_battleGoldGet";
    private const string BATTLE_EXP_GET_TEXT = "str_battleExpGet";
    private const string BATTLE_HURTED_TEXT = "str_battleHurted";

    public static BattleDlg StartBattle(Transform parent, BattleOverCallback cb, long enemyUuid, long yourUuid = -1, BattlePauseEventCheck pauseCheck = null, int pauseEvent = 0)
    {
        // 弹出战斗框
        var ret = Game.ObjPool.GetAnElement<BattleDlg>(PREFAB_ID, ObjectPool.ElementType.Dialog, GetResourcePath());
        // 设定暂停触发器
        ret.battlePauseChecker = pauseCheck;
        ret.battlePauseEvent = pauseEvent;
        ret.overCallback = cb;

        // 设定战斗双方的信息
        ret.transform.SetParent(parent, false);
        ret.SetBattleInfo(enemyUuid, yourUuid);
        // 设定状态, 战斗开始
        Game.Status = Constant.EGameStatus.OnBattle;
        ret.isBattlePaused = false;
        ret.battleResultPanel.SetActive(false);

        return ret;
    }

    public static void CloseBattle(BattleDlg dlg)
    {
        dlg.StopBattle();
        Game.ObjPool.RecycleAnElement(dlg);
    }

    // Use this for initialization
    void Awake ()
    {
        // 关联战斗框的控件
        playerSprite = transform.Find("Player").gameObject;
        playerNameText = transform.Find("Name_Player").GetComponent<Text>();
        playerNameText.fontSize = Convert.ToInt32(playerNameText.fontSize * Game.ScreenAdaptorInst.RealFontSize);
        playerLifeText = transform.Find("Life_Player").GetComponent<Text>();
        playerLifeText.fontSize = Convert.ToInt32(playerLifeText.fontSize * Game.ScreenAdaptorInst.RealFontSize);
        playerAttackText = transform.Find("Attack_Player").GetComponent<Text>();
        playerAttackText.fontSize = Convert.ToInt32(playerAttackText.fontSize * Game.ScreenAdaptorInst.RealFontSize);
        playerDefenseText = transform.Find("Defense_Player").GetComponent<Text>();
        playerDefenseText.fontSize = Convert.ToInt32(playerDefenseText.fontSize * Game.ScreenAdaptorInst.RealFontSize);
        playerSpeedText = transform.Find("Speed_Player").GetComponent<Text>();
        playerSpeedText.fontSize = Convert.ToInt32(playerSpeedText.fontSize * Game.ScreenAdaptorInst.RealFontSize);
        enemySprite = transform.Find("Enemy").gameObject;
        enemyNameText = transform.Find("Name_Enemy").GetComponent<Text>();
        enemyNameText.fontSize = Convert.ToInt32(enemyNameText.fontSize * Game.ScreenAdaptorInst.RealFontSize);
        enemyLifeText = transform.Find("Life_Enemy").GetComponent<Text>();
        enemyLifeText.fontSize = Convert.ToInt32(enemyLifeText.fontSize * Game.ScreenAdaptorInst.RealFontSize);
        enemyAttackText = transform.Find("Attack_Enemy").GetComponent<Text>();
        enemyAttackText.fontSize = Convert.ToInt32(enemyAttackText.fontSize * Game.ScreenAdaptorInst.RealFontSize);
        enemyDefenseText = transform.Find("Defense_Enemy").GetComponent<Text>();
        enemyDefenseText.fontSize = Convert.ToInt32(enemyDefenseText.fontSize * Game.ScreenAdaptorInst.RealFontSize);
        enemySpeedText = transform.Find("Speed_Enemy").GetComponent<Text>();
        enemySpeedText.fontSize = Convert.ToInt32(enemySpeedText.fontSize * Game.ScreenAdaptorInst.RealFontSize);
        battleResultPanel = transform.Find("BattleResultPanel").gameObject;
        battleResultGoldText = battleResultPanel.transform.Find("GoldText").GetComponent<Text>();
        battleResultGoldText.fontSize = Convert.ToInt32(battleResultGoldText.fontSize*Game.ScreenAdaptorInst.RealFontSize);
        battleResultExpText = battleResultPanel.transform.Find("ExpText").GetComponent<Text>();
        battleResultExpText.fontSize = Convert.ToInt32(battleResultExpText.fontSize*Game.ScreenAdaptorInst.RealFontSize);
        battleHurtedText = battleResultPanel.transform.Find("HurtedText").GetComponent<Text>();
        battleHurtedText.fontSize = Convert.ToInt32(battleHurtedText.fontSize * Game.ScreenAdaptorInst.RealFontSize);
        battleResultPanel.SetActive(false);
        battleHitSleep = 0;
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (gameObject.activeSelf && !isBattlePaused && Game.Status == Constant.EGameStatus.OnBattle && (hitter == null || !hitter.isActiveAndEnabled)) {
            ++battleHitSleep;
            if (battleHitSleep > BATTLE_SLEEP_TIME) {
                battleHitSleep -= BATTLE_SLEEP_TIME;
                if (playerBattleData.life <= 0 || enemyBattleData.life <= 0) {
                    OnBattleOver();
                } else {
                    int hurt = 0;
                    if (isOurRound) {
                        hurt = MathHelper.GetHurt(playerBattleData.attack, playerBattleData.critical, enemyBattleData.defense, enemyBattleData.speed);
                        if (hurt == -1) {
                            CreateHitter(Constant.MISS_HITTER, true, 0, false);
                            Debug.Log("Enemy has missed a hurt");
                        } else if (hurt == 0) {
                            CreateHitter(Constant.NOHURT_HITTER, true, 0, false);
                            Debug.Log("Enemy was hurted failed");
                        } else if (hurt < 0) {
                            CreateHitter(playerBattleData.weaponId, true, -hurt, true);
                            enemyBattleData.life += hurt;
                            Debug.Log("Enemy was hurted critical : " + -hurt);
                        } else {
                            CreateHitter(playerBattleData.weaponId, true, hurt, false);
                            enemyBattleData.life -= hurt;
                            Debug.Log("Enemy was hurted normally : " + hurt);
                        }
                        if (enemyBattleData.life < 0)
                            enemyBattleData.life = 0;
                        enemyLifeText.text = enemyBattleData.life.ToString();
                        ++rounds;
                    } else {
                        hurt = MathHelper.GetHurt(enemyBattleData.attack, enemyBattleData.critical, playerBattleData.defense, playerBattleData.speed);
                        if (hurt == -1) {
                            CreateHitter(Constant.MISS_HITTER, false, 0, false);
                            Debug.Log("Player has missed a hurt");
                        } else if (hurt == 0) {
                            CreateHitter(Constant.NOHURT_HITTER, false, 0, false);
                            Debug.Log("Player was hurted failed");
                        } else if (hurt < 0) {
                            CreateHitter(enemyBattleData.weaponId, false, -hurt, true);
                            playerBattleData.life += hurt;
                            Debug.Log("Player was hurted critical : " + -hurt);
                            hurted -= hurt;
                        } else {
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
    }

    void SetBattleInfo(long enemy, long you = -1)
    {
        // 设定战斗双方的属性数据
        enemyUuid = enemy;        
        enemyBattleData = Game.Map.GetMonsterDataByUuid(enemyUuid);
        if (you < 0)
            playerBattleData = new Constant.MonsterData()
            {
                id = Game.Player.PlayerId,
                level = Game.Player.Level,
                exp = 0,
                life = Game.Player.Life,
                attack = Game.Player.Attack,
                defense = Game.Player.Defense,
                speed = Game.Player.Speed,
                critical = Game.Player.Critical,
                gold = 0,
                weaponId = Game.Player.Weapon
            };
        else
            playerBattleData = Game.Map.GetMonsterDataByUuid(you);

        rounds = 1;
        hurted = 0;
        isOurRound = true;

        // 设定我方的头像
        var playerModal = Game.Config.modals[playerBattleData.id];
        ObjectPool.AViewUnit obj = Game.ObjPool.GetAnElement<Modal>(playerModal.id, ObjectPool.ElementType.Sprite, Constant.PREFAB_DIR + playerModal.prefabPath, Constant.SPRITE_IN_DIALOG_SORTING_ORDER);
        if(obj == null)
            obj = Game.ObjPool.GetAnElement<Player>(playerModal.id, ObjectPool.ElementType.Sprite, Constant.PREFAB_DIR + playerModal.prefabPath, Constant.SPRITE_IN_DIALOG_SORTING_ORDER);
        obj.transform.SetParent(playerSprite.transform.parent, false);
        obj.transform.position = playerSprite.transform.position;
        obj.transform.localScale = Game.ScreenAdaptorInst.BlockSize;
        var mod = playerSprite.GetComponent<Modal>();
        if (mod != null)
            mod.RemoveSelf(false);
        else if (playerSprite.GetComponent<Player>() != null)
            playerSprite.GetComponent<Player>().RemoveSelf();
        else
            Destroy(playerSprite);
        playerSprite = obj.gameObject;

        // 设定敌方的头像
        var enemyModal = Game.Config.modals[enemyBattleData.id];
        obj = Game.ObjPool.GetAnElement<Modal>(enemyModal.id, ObjectPool.ElementType.Sprite, Constant.PREFAB_DIR + enemyModal.prefabPath, Constant.SPRITE_IN_DIALOG_SORTING_ORDER);
        if (obj == null)
            obj = Game.ObjPool.GetAnElement<Player>(enemyModal.id, ObjectPool.ElementType.Sprite, Constant.PREFAB_DIR + enemyModal.prefabPath, Constant.SPRITE_IN_DIALOG_SORTING_ORDER);
        obj.transform.SetParent(enemySprite.transform.parent, false);
        obj.transform.position = enemySprite.transform.position;
        obj.transform.localScale = Game.ScreenAdaptorInst.BlockSize;
        mod = enemySprite.GetComponent<Modal>();
        if (mod != null)
            mod.RemoveSelf(false);
        else if (enemySprite.GetComponent<Player>() != null)
            enemySprite.GetComponent<Player>().RemoveSelf();
        else
            Destroy(enemySprite);
        enemySprite = obj.gameObject;

        // 将双方数据显示到界面
        playerNameText.text = Game.Config.StringInternational.GetValue(playerModal.name);
        playerLifeText.text = playerBattleData.life.ToString();
        playerAttackText.text = playerBattleData.attack.ToString();
        playerDefenseText.text = playerBattleData.defense.ToString();
        playerSpeedText.text = playerBattleData.speed.ToString();
        enemyNameText.text = Game.Config.StringInternational.GetValue(enemyModal.name);
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
        Game.Map.RemoveThingOnMapWithModal(enemyUuid);
        Game.Status = Constant.EGameStatus.InGame;
        isBattlePaused = false;
    }

    private void CreateHitter(int weaponId, bool isOnEnemy, int damage, bool isCritical)
    {
        var data = Game.Config.weapons[weaponId];
        hitter = Game.ObjPool.GetAnElement<Zzhit>(weaponId * 2 + (isCritical?0:1),ObjectPool.ElementType.Hitter, Constant.PREFAB_DIR + (isCritical ? data.critPrefabPath : data.prefabPath), Constant.HITTER_IN_DIALOG_SORTING_ORDER);
        hitter.SetParam(data, isCritical);
        hitter.PlaySound();
        hitter.transform.SetParent((isOnEnemy ? enemySprite : playerSprite).transform, false);
        hitter.transform.position = hitter.transform.parent.position;
        hitter.transform.localScale = Game.ScreenAdaptorInst.BlockSize / 200;
    }

    private void OnBattleOver()
    {
        isBattlePaused = true;
        battleResultGoldText.text = Game.Config.StringInternational.GetValue(BATTLE_GOLD_GET_TEXT, enemyBattleData.gold.ToString());
        battleResultExpText.text = Game.Config.StringInternational.GetValue(BATTLE_EXP_GET_TEXT, enemyBattleData.exp.ToString());
        battleHurtedText.text = Game.Config.StringInternational.GetValue(BATTLE_HURTED_TEXT, hurted.ToString());
        battleResultPanel.SetActive(true);
        overCallback(playerBattleData.id, playerBattleData.life, enemyBattleData.gold, enemyBattleData.exp, 0, 0);
        Game.Status = Constant.EGameStatus.OnBattleResult;
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
    
    private int battleHitSleep;
}
