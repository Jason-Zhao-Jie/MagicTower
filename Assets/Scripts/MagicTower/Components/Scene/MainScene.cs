using System.Collections;
using UnityEngine;
using MagicTower.Components.Control;
using MagicTower.Components.Unit;
using MagicTower.Present.Manager;

namespace MagicTower.Components.Scene {

    public class MainScene : AScene {
        override public SceneType Type { get { return SceneType.MainScene; } }

        // Use this for initialization
        override protected async System.Threading.Tasks.Task Start() {
            await base.Start();

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

            AudioManager.ClearSoundSource();
            AudioManager.MusicSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();
            AudioManager.AddSoundSource(GetComponent<AudioSource>());
            AudioManager.AddSoundSource(dialogCanvas.GetComponent<AudioSource>());
            AudioManager.AddSoundSource(transform.Find("PlayerPanel").GetComponent<AudioSource>());
            AudioManager.AddSoundSource(transform.Find("MapPanel").GetComponent<AudioSource>());

            Game.Player = new Present.Player.Controller(transform.Find("PlayerPanel").GetComponent<Present.Player.View>());
            Game.Map = new Present.Map.Controller(transform.Find("MapPanel").GetComponent<Present.Map.View>());
            await Game.Load(Game.CurrentSaveName);
        }

        // Update is called once per frame
        void Update() {
            Game.SceneUpdate();
        }

        public override void OnMapClicked(int posx, int posy) {
            Game.Player.StartAutoStep(posx, posy);
        }

        public void OnBtnMenuClicked() {
            MainMenuDlg.ShowDialog(this);
        }

        public void PlayAds() { 
            var ret = AdsPluginManager.ShowRewardBasedVideo((string type, double amount) => {
                ShowTips("Google MobAds loaded OK and get reward successful");
            }, () => {
                ShowTips("Google MobAds loaded OK and closed");
            });
            switch (ret) {
                case AdsPluginManager.AdLoadingState.UnablePlatform:
                    ShowTips("Google MobAds cannot be used at current platform " + Application.platform.ToString());
                    break;
                case AdsPluginManager.AdLoadingState.SdkInitializing:
                    ShowTips("Google MobAds sdk is initializing");
                    break;
                case AdsPluginManager.AdLoadingState.NotInitialized:
                    ShowTips("Google MobAds this type of ads does not initialized");
                    break;
                case AdsPluginManager.AdLoadingState.Loading:
                    ShowTips("Google MobAds ads is loading");
                    break;
                case AdsPluginManager.AdLoadingState.LoadFailedAndReloading:
                    ShowTips("Google MobAds loaded failure, message: ", AdsPluginManager.RewardBasedVideoLoadedFailedMessage);
                    break;
            }
        }

        /********************** Chat Part **************************************/

        public void ShowChatOnTop(string content, int speakerId = -1) {
            Game.Status = Model.EGameStatus.OnTipChat;
            topChatPanel.gameObject.SetActive(true);
            bottomChatPanel.gameObject.SetActive(false);
            tipsPanel.gameObject.SetActive(false);
            topChatPanel.SetChat(Game.Config.StringInternational.GetValue(content), speakerId);
        }

        public void ShowChatOnBottom(string content, int speakerId = -1) {
            Game.Status = Model.EGameStatus.OnTipChat;
            topChatPanel.gameObject.SetActive(false);
            bottomChatPanel.gameObject.SetActive(true);
            tipsPanel.gameObject.SetActive(false);
            bottomChatPanel.SetChat(Game.Config.StringInternational.GetValue(content), speakerId);
        }

        public override void ShowTips(params string[] texts) {
            var text = "";
            if (texts != null) {
                var builder = new System.Text.StringBuilder();
                for (var i = 0; i < texts.Length; ++i) {
                    builder.Append(Game.Config.StringInternational.GetValue(texts[i]));
                }
                text = builder.ToString();
            }
            Game.Status = Model.EGameStatus.OnTipChat;
            topChatPanel.gameObject.SetActive(false);
            bottomChatPanel.gameObject.SetActive(false);
            tipsPanel.gameObject.SetActive(true);
            tipsPanel.SetTipText(text);
        }

        private void ClearChats() {
            chat = null;
            chatIndex = 0;
            topChatPanel.gameObject.SetActive(false);
            bottomChatPanel.gameObject.SetActive(false);
            tipsPanel.gameObject.SetActive(false);
            if (battlePanel != null && battlePanel.isActiveAndEnabled) {
                Game.Status = Model.EGameStatus.OnBattle;
            } else if (choicePanel != null && choicePanel.isActiveAndEnabled) {
                Game.Status = Model.EGameStatus.OnChoice;
            } else {
                Game.Status = Model.EGameStatus.InGame;
            }
        }

        public void ChatBegan(Model.ChatData chat, Modal mod) {
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
                EventManager.DispatchEvent(chat.eventId, chatMod, chat.eventData);    // TODO : 这里的事件返回值没有生效
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

        public void StartBattle(Modal enemyModal, bool canFail, long yourUuid = -1, BattleDlg.BattlePauseEventCheck pauseCheck = null, int pauseEvent = 0) {
            battlePanel = BattleDlg.StartBattle(dialogCanvas.transform, OnBattleOver, canFail, enemyModal.Uuid, yourUuid, pauseCheck, pauseEvent);
            battlePanel.transform.localPosition = new Vector3(0, 0, 12);
            battlePanel.transform.localScale = new Vector3(1, 1, 1);
            battleMod = enemyModal;
        }

        public void StopBattle() {
            if (battlePanel != null) {
                BattleDlg.CloseBattle(battlePanel);
                battlePanel = null;
            }
        }

        private void OnBattleOver(bool gameover, int yourId, int yourLife, int goldGain, int expGain, int nextEvent, long[] nextEventData) {
            // 记录应用战斗结果（金币，经验，血量）
            if (yourId == Game.Player.PlayerId) {
                Game.Player.Life = yourLife;
                Game.Player.Gold += goldGain;
                Game.Player.Experience += expGain;
            }
            if (gameover) {
                OnGameOver();
            } else {
                EventManager.DispatchEvent(nextEvent, battleMod, nextEventData);
            }
        }

        private void OnGameOver() {
            StartCoroutine(GameOverResolve());
            Game.Map.ShowCurtain(Curtain.ContentType.GameOver, null);
            BackToStartScene(7);
        }

        private IEnumerator GameOverResolve() {
            yield return new WaitForSeconds(1.6f);
            battlePanel.gameObject.SetActive(false);
        }

        /********************** Choice Part **************************************/

        public void StartChoice(Model.ChoiceData choiceData, Modal mod, Model.EGameStatus nextStatus = Model.EGameStatus.InGame) {
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

        private Model.ChatData chat;
        private Modal chatMod;
        private int chatIndex = 0;
        private Modal battleMod;

        public GameObject dialogCanvas = null;
        private ChatDlg topChatPanel = null;
        private ChatDlg bottomChatPanel = null;
        private TipBar tipsPanel = null;
        private ChoiceDlg choicePanel = null;
        private BattleDlg battlePanel = null;
    }

}
