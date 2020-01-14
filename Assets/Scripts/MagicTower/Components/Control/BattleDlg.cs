using System;
using UnityEngine;
using UnityEngine.UI;
using ArmyAnt.ViewUtil;
using MagicTower.Components.Unit;

namespace MagicTower.Components.Control
{
    public class BattleDlg : MonoBehaviour
    {
        private const int MISS_HITTER = 9;       // TODO: To set the right "MISS" icon
        private const int NOHURT_HITTER = 9;

        public delegate bool BattlePauseEventCheck();
        public delegate void BattleOverCallback(bool gameover, int yourId, int yourLife, int goldGain, int expGain, int nextEvent, long[] nextEventData);

        private const string BATTLE_GOLD_GET_TEXT = "str_battleGoldGet";
        private const string BATTLE_EXP_GET_TEXT = "str_battleExpGet";
        private const string BATTLE_HURTED_TEXT = "str_battleHurted";
        
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
        }

        // Update is called once per frame
        void FixedUpdate() {
            if(gameObject.activeSelf && !isBattlePaused && Game.Status == Model.EGameStatus.OnBattle && (hitter == null || !hitter.isActiveAndEnabled)) {
                if(!canFail && playerBattleData.life <= 0) {
                    OnBattleFailure();
                } else if(playerBattleData.life <= 0 || enemyBattleData.life <= 0) {
                    OnBattleOver();
                } else {
                    int hurt;
                    if(isOurRound) {
                        hurt = Model.MathHelper.GetHurt(playerBattleData.attack, playerBattleData.critical, enemyBattleData.defense, enemyBattleData.speed);
                        if(hurt == -1) {
                            CreateHitter(MISS_HITTER, true, 0, false, true);
                            Game.DebugLogNote("Enemy has missed a hurt");
                        } else if(hurt == 0) {
                            CreateHitter(NOHURT_HITTER, true, 0, false);
                            Game.DebugLogNote("Enemy was hurted failed");
                        } else if(hurt < 0) {
                            CreateHitter(playerBattleData.weaponId, true, -hurt, true);
                            enemyBattleData.life += hurt;
                            Game.DebugLogNote("Enemy was hurted critical : ", -hurt);
                        } else {
                            CreateHitter(playerBattleData.weaponId, true, hurt, false);
                            enemyBattleData.life -= hurt;
                            Game.DebugLogNote("Enemy was hurted normally : ", hurt);
                        }
                        if(enemyBattleData.life < 0)
                            enemyBattleData.life = 0;
                        enemyLifeText.text = enemyBattleData.life.ToString();
                        ++rounds;
                    } else {
                        hurt = Model.MathHelper.GetHurt(enemyBattleData.attack, enemyBattleData.critical, playerBattleData.defense, playerBattleData.speed);
                        if(hurt == -1) {
                            CreateHitter(MISS_HITTER, false, 0, false, true);
                            Game.DebugLogNote("Player has missed a hurt");
                        } else if(hurt == 0) {
                            CreateHitter(NOHURT_HITTER, false, 0, false);
                            Game.DebugLogNote("Player was hurted failed");
                        } else if(hurt < 0) {
                            CreateHitter(enemyBattleData.weaponId, false, -hurt, true);
                            playerBattleData.life += hurt;
                            Game.DebugLogNote("Player was hurted critical : ", -hurt);
                            hurted -= hurt;
                        } else {
                            CreateHitter(enemyBattleData.weaponId, false, hurt, false);
                            playerBattleData.life -= hurt;
                            Game.DebugLogNote("Player was hurted normally : ", hurt);
                            hurted += hurt;
                        }
                        if(playerBattleData.life < 0)
                            playerBattleData.life = 0;
                        playerLifeText.text = playerBattleData.life.ToString();
                    }
                    isOurRound = !isOurRound;
                }
            }
        }

        public void Init(BattleOverCallback cb, bool canFail, long enemyUuid, long yourUuid = -1, BattlePauseEventCheck pauseCheck = null, int pauseEvent = 0) {
            // 设定暂停触发器
            battlePauseChecker = pauseCheck;
            battlePauseEvent = pauseEvent;
            overCallback = cb;

            // 设定战斗双方的信息
            this.canFail = canFail;

            // 设定战斗双方的属性数据
            this.enemyUuid = enemyUuid;
            enemyBattleData = Game.Map.GetMonsterDataByUuid(enemyUuid);
            if(yourUuid < 0)
                playerBattleData = new Model.MonsterData() {
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
                playerBattleData = Game.Map.GetMonsterDataByUuid(yourUuid);

            rounds = 1;
            hurted = 0;
            isOurRound = true;

            // 设定我方的头像
            var playerModal = Game.Config.modals[playerBattleData.id];
            playerSprite.OnInit(ObjectPool.ElementType.Image, playerBattleData.id, playerModal);

            // 设定敌方的头像
            var enemyModal = Game.Config.modals[enemyBattleData.id];
            enemySprite.OnInit(ObjectPool.ElementType.Image, enemyBattleData.id, enemyModal);

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

            // 设定状态, 战斗开始
            Game.Status = Model.EGameStatus.OnBattle;
            isBattlePaused = false;
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
            Game.Map.RemoveThingOnMap(enemyUuid);
            Game.Status = Model.EGameStatus.InGame;
            isBattlePaused = false;
        }

        private void CreateHitter(int weaponId, bool isOnEnemy, int damage, bool isCritical, bool isMiss = false)
        {
            var data = Game.Config.weapons[weaponId];
            var parent = (isOnEnemy ? enemySprite : playerSprite).transform;
            hitter = Game.ObjPool.GetAnElement<Modal, Model.WeaponData>(0, ObjectPool.ElementType.Image, Game.ModalImage, data, isCritical);
            hitter.transform.SetParent(parent, false);
            hitter.transform.localPosition = Vector3.zero;
            hitter.transform.localScale = Vector3.one * (isCritical ? data.critPrefabLocalScale : data.prefabLocalScale);
            if (damage > 0)
            {
                var jumpWord = Instantiate(Game.JumpWord);
                jumpWord.GetComponent<JumpWord>().Word = damage.ToString();
                jumpWord.transform.SetParent(parent, false);
            }
            else if (isMiss)
            {
                var jumpWord = Instantiate(Game.JumpWord);
                jumpWord.GetComponent<JumpWord>().Word = "MISS";
                jumpWord.transform.SetParent(parent, false);
            }
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

        [Tooltip("我方角色头像")]
        [Space(4)]
        public Modal playerSprite;
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
        public Modal enemySprite;
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
        internal Modal hitter;
    }

}