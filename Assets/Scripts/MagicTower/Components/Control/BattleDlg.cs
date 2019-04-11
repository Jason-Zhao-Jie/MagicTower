using System;
using UnityEngine;
using UnityEngine.UI;
using ArmyAnt.ViewUtil;
using MagicTower.Components.Unit;

namespace MagicTower.Components.Control
{
    public class BattleDlg : ObjectPool.AViewUnit
    {
        private const string PREFAB_DIR = "BattleDlg";
        private const int PREFAB_ID = 1;

        private const int MISS_HITTER = 9;       // TODO: To set the right "MISS" icon
        private const int NOHURT_HITTER = 9;

        public delegate bool BattlePauseEventCheck();
        public delegate void BattleOverCallback(bool gameover, int yourId, int yourLife, int goldGain, int expGain, int nextEvent, long[] nextEventData);

        private const int BATTLE_SLEEP_TIME = 2; // 修改此值以控制两次攻击之间的间隔, 使得攻击表现更为流畅, 已扣除攻击动画时间. 单位:帧
        private const string BATTLE_GOLD_GET_TEXT = "str_battleGoldGet";
        private const string BATTLE_EXP_GET_TEXT = "str_battleExpGet";
        private const string BATTLE_HURTED_TEXT = "str_battleHurted";

        public static BattleDlg StartBattle(Transform parent, BattleOverCallback cb, bool canFail, long enemyUuid, long yourUuid = -1, BattlePauseEventCheck pauseCheck = null, int pauseEvent = 0)
        {
            // 弹出战斗框
            var ret = Game.ObjPool.GetAnElement<BattleDlg>(PREFAB_ID, ObjectPool.ElementType.Dialog, GetResourcePath());
            // 设定暂停触发器
            ret.battlePauseChecker = pauseCheck;
            ret.battlePauseEvent = pauseEvent;
            ret.overCallback = cb;

            // 设定战斗双方的信息
            ret.transform.SetParent(parent, false);
            ret.transform.SetSiblingIndex(1);
            ret.canFail = canFail;
            ret.SetBattleInfo(enemyUuid, yourUuid);
            // 设定状态, 战斗开始
            Game.Status = Model.EGameStatus.OnBattle;
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
        void Awake()
        {
            // 关联战斗框的控件
            playerNameText.fontSize = Convert.ToInt32(playerNameText.fontSize * Game.RealFontSize);
            playerLifeText.fontSize = Convert.ToInt32(playerLifeText.fontSize * Game.RealFontSize);
            playerAttackText.fontSize = Convert.ToInt32(playerAttackText.fontSize * Game.RealFontSize);
            playerDefenseText.fontSize = Convert.ToInt32(playerDefenseText.fontSize * Game.RealFontSize);
            playerSpeedText.fontSize = Convert.ToInt32(playerSpeedText.fontSize * Game.RealFontSize);
            enemyNameText.fontSize = Convert.ToInt32(enemyNameText.fontSize * Game.RealFontSize);
            enemyLifeText.fontSize = Convert.ToInt32(enemyLifeText.fontSize * Game.RealFontSize);
            enemyAttackText.fontSize = Convert.ToInt32(enemyAttackText.fontSize * Game.RealFontSize);
            enemyDefenseText.fontSize = Convert.ToInt32(enemyDefenseText.fontSize * Game.RealFontSize);
            enemySpeedText.fontSize = Convert.ToInt32(enemySpeedText.fontSize * Game.RealFontSize);
            battleResultGoldText.fontSize = Convert.ToInt32(battleResultGoldText.fontSize * Game.RealFontSize);
            battleResultExpText.fontSize = Convert.ToInt32(battleResultExpText.fontSize * Game.RealFontSize);
            battleHurtedText.fontSize = Convert.ToInt32(battleHurtedText.fontSize * Game.RealFontSize);

            playerLifeTitleText.text = Game.Config.StringInternational.GetValue(Present.Player.View.str_life_title);
            playerAttackTitleText.text = Game.Config.StringInternational.GetValue(Present.Player.View.str_attack_title);
            playerDefenseTitleText.text = Game.Config.StringInternational.GetValue(Present.Player.View.str_defense_title);
            playerSpeedTitleText.text = Game.Config.StringInternational.GetValue(Present.Player.View.str_speed_title);
            enemyLifeTitleText.text = Game.Config.StringInternational.GetValue(Present.Player.View.str_life_title);
            enemyAttackTitleText.text = Game.Config.StringInternational.GetValue(Present.Player.View.str_attack_title);
            enemyDefenseTitleText.text = Game.Config.StringInternational.GetValue(Present.Player.View.str_defense_title);
            enemySpeedTitleText.text = Game.Config.StringInternational.GetValue(Present.Player.View.str_speed_title);
        }

        private void Start()
        {
            battleResultPanel.SetActive(false);
            battleHitSleep = 0;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (gameObject.activeSelf && !isBattlePaused && Game.Status == Model.EGameStatus.OnBattle && (hitter == null || !hitter.isActiveAndEnabled))
            {
                ++battleHitSleep;
                if (battleHitSleep > BATTLE_SLEEP_TIME)
                {
                    battleHitSleep -= BATTLE_SLEEP_TIME;
                    if (!canFail && playerBattleData.life <= 0)
                    {
                        OnBattleFailure();
                    }
                    else if (playerBattleData.life <= 0 || enemyBattleData.life <= 0)
                    {
                        OnBattleOver();
                    }
                    else
                    {
                        int hurt = 0;
                        if (isOurRound)
                        {
                            hurt = Model.MathHelper.GetHurt(playerBattleData.attack, playerBattleData.critical, enemyBattleData.defense, enemyBattleData.speed);
                            if (hurt == -1)
                            {
                                CreateHitter(MISS_HITTER, true, 0, false);
                                Game.DebugLog("Enemy has missed a hurt");
                            }
                            else if (hurt == 0)
                            {
                                CreateHitter(NOHURT_HITTER, true, 0, false);
                                Game.DebugLog("Enemy was hurted failed");
                            }
                            else if (hurt < 0)
                            {
                                CreateHitter(playerBattleData.weaponId, true, -hurt, true);
                                enemyBattleData.life += hurt;
                                Game.DebugLog("Enemy was hurted critical : " + -hurt);
                            }
                            else
                            {
                                CreateHitter(playerBattleData.weaponId, true, hurt, false);
                                enemyBattleData.life -= hurt;
                                Game.DebugLog("Enemy was hurted normally : " + hurt);
                            }
                            if (enemyBattleData.life < 0)
                                enemyBattleData.life = 0;
                            enemyLifeText.text = enemyBattleData.life.ToString();
                            ++rounds;
                        }
                        else
                        {
                            hurt = Model.MathHelper.GetHurt(enemyBattleData.attack, enemyBattleData.critical, playerBattleData.defense, playerBattleData.speed);
                            if (hurt == -1)
                            {
                                CreateHitter(MISS_HITTER, false, 0, false);
                                Game.DebugLog("Player has missed a hurt");
                            }
                            else if (hurt == 0)
                            {
                                CreateHitter(NOHURT_HITTER, false, 0, false);
                                Game.DebugLog("Player was hurted failed");
                            }
                            else if (hurt < 0)
                            {
                                CreateHitter(enemyBattleData.weaponId, false, -hurt, true);
                                playerBattleData.life += hurt;
                                Game.DebugLog("Player was hurted critical : " + -hurt);
                                hurted -= hurt;
                            }
                            else
                            {
                                CreateHitter(enemyBattleData.weaponId, false, hurt, false);
                                playerBattleData.life -= hurt;
                                Game.DebugLog("Player was hurted normally : " + hurt);
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
                playerBattleData = new Model.MonsterData()
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
            ObjectPool.AViewUnit obj = Game.ObjPool.GetAnElement<Modal>(playerModal.id, ObjectPool.ElementType.Sprite, Model.Dirs.PREFAB_DIR + playerModal.prefabPath, ObjectPool.SPRITE_IN_DIALOG_SORTING_ORDER);
            if (obj == null)
                obj = Game.ObjPool.GetAnElement<Player>(playerModal.id, ObjectPool.ElementType.Sprite, Model.Dirs.PREFAB_DIR + playerModal.prefabPath, ObjectPool.SPRITE_IN_DIALOG_SORTING_ORDER);
            obj.transform.SetParent(playerSprite.transform.parent, false);
            obj.transform.position = playerSprite.transform.position;
            obj.transform.localScale = Game.Map.ModalLocalScale;
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
            obj = Game.ObjPool.GetAnElement<Modal>(enemyModal.id, ObjectPool.ElementType.Sprite, Model.Dirs.PREFAB_DIR + enemyModal.prefabPath, ObjectPool.SPRITE_IN_DIALOG_SORTING_ORDER);
            if (obj == null)
                obj = Game.ObjPool.GetAnElement<Player>(enemyModal.id, ObjectPool.ElementType.Sprite, Model.Dirs.PREFAB_DIR + enemyModal.prefabPath, ObjectPool.SPRITE_IN_DIALOG_SORTING_ORDER);
            obj.transform.SetParent(enemySprite.transform.parent, false);
            obj.transform.position = enemySprite.transform.position;
            obj.transform.localScale = Game.Map.ModalLocalScale;
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
            Game.Status = Model.EGameStatus.InGame;
            isBattlePaused = false;
        }

        private void CreateHitter(int weaponId, bool isOnEnemy, int damage, bool isCritical)
        {
            var data = Game.Config.weapons[weaponId];
            hitter = Game.ObjPool.GetAnElement<Zzhit>(weaponId * 2 + (isCritical ? 0 : 1), ObjectPool.ElementType.Hitter, Model.Dirs.PREFAB_DIR + (isCritical ? data.critPrefabPath : data.prefabPath), ObjectPool.HITTER_IN_DIALOG_SORTING_ORDER);
            hitter.SetParam(data, isCritical);
            hitter.PlaySound();
            hitter.transform.SetParent((isOnEnemy ? enemySprite : playerSprite).transform, false);
            hitter.transform.position = hitter.transform.parent.position;
            hitter.transform.localScale = Game.Map.ModalLocalScale / 200;
        }

        private void OnBattleFailure()
        {
            isBattlePaused = true;
            Present.Manager.AudioManager.StopMusic();
            Present.Manager.AudioManager.PlaySound(Present.Manager.AudioManager.gameoverSound);
            Game.Status = Model.EGameStatus.OnBattleResult;
            overCallback(true, playerBattleData.id, playerBattleData.life, 0, 0, 0, null);
        }

        private void OnBattleOver()
        {
            isBattlePaused = true;
            battleResultGoldText.text = Game.Config.StringInternational.GetValue(BATTLE_GOLD_GET_TEXT, enemyBattleData.gold.ToString());
            battleResultExpText.text = Game.Config.StringInternational.GetValue(BATTLE_EXP_GET_TEXT, enemyBattleData.exp.ToString());
            battleHurtedText.text = Game.Config.StringInternational.GetValue(BATTLE_HURTED_TEXT, hurted.ToString());
            battleResultPanel.SetActive(true);
            Game.Status = Model.EGameStatus.OnBattleResult;
            overCallback(false, playerBattleData.id, playerBattleData.life, enemyBattleData.gold, enemyBattleData.exp, 0, null);
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
            return Game.ObjPoolRecycleSelf(this);
        }


        public override string ResourcePath => Model.Dirs.DIALOG_DIR + PREFAB_DIR;
        public static string GetResourcePath() => Model.Dirs.DIALOG_DIR + PREFAB_DIR;

        [Tooltip("我方角色头像")]
        [Space(4)]
        public GameObject playerSprite;
        [Tooltip("我方角色名")]
        [Space(4)]
        public Text playerNameText;
        [Tooltip("我方生命值标题")]
        [Space(4)]
        public Text playerLifeTitleText;
        [Tooltip("我方生命值")]
        [Space(4)]
        public Text playerLifeText;
        [Tooltip("我方攻击值标题")]
        [Space(4)]
        public Text playerAttackTitleText;
        [Tooltip("我方攻击值")]
        [Space(4)]
        public Text playerAttackText;
        [Tooltip("我方防御值标题")]
        [Space(4)]
        public Text playerDefenseTitleText;
        [Tooltip("我方防御值")]
        [Space(4)]
        public Text playerDefenseText;
        [Tooltip("我方敏捷值标题")]
        [Space(4)]
        public Text playerSpeedTitleText;
        [Tooltip("我方敏捷值")]
        [Space(4)]
        public Text playerSpeedText;
        [Tooltip("敌方角色头像")]
        [Space(4)]
        public GameObject enemySprite;
        [Tooltip("敌方角色名")]
        [Space(4)]
        public Text enemyNameText;
        [Tooltip("敌方生命值标题")]
        [Space(4)]
        public Text enemyLifeTitleText;
        [Tooltip("敌方生命值")]
        [Space(4)]
        public Text enemyLifeText;
        [Tooltip("敌方攻击值标题")]
        [Space(4)]
        public Text enemyAttackTitleText;
        [Tooltip("敌方攻击值")]
        [Space(4)]
        public Text enemyAttackText;
        [Tooltip("敌方防御值标题")]
        [Space(4)]
        public Text enemyDefenseTitleText;
        [Tooltip("敌方防御值")]
        [Space(4)]
        public Text enemyDefenseText;
        [Tooltip("敌方敏捷值标题")]
        [Space(4)]
        public Text enemySpeedTitleText;
        [Tooltip("敌方敏捷值")]
        [Space(4)]
        public Text enemySpeedText;

        [Tooltip("战斗结果结算条")]
        [Space(4)]
        public GameObject battleResultPanel;
        [Tooltip("战斗获得金币值")]
        [Space(4)]
        public Text battleResultGoldText;
        [Tooltip("战斗获得经验值")]
        [Space(4)]
        public Text battleResultExpText;
        [Tooltip("战斗所消耗的生命值")]
        [Space(4)]
        public Text battleHurtedText;

        private bool canFail;
        private long enemyUuid;
        private Model.MonsterData playerBattleData;
        private Model.MonsterData enemyBattleData;
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

}