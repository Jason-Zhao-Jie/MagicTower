using System.Collections;
using UnityEngine;
using MagicTower.Components.Control;
using MagicTower.Components.Unit;
using MagicTower.Present.Manager;

namespace MagicTower.Components.Scene
{

    public class MainScene : AScene
    {
        override public SceneType Type { get { return SceneType.MainScene; } }

        // Use this for initialization
        override protected async System.Threading.Tasks.Task Start()
        {
            var ret = base.Start();
            if (ret != null)
            {
                await ret;
            }

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

        void OnDestroy()
        {
            Game.ObjPool.ClearAll();
            Game.Map.ClearMap();
            Game.Map = null;
            Game.Player = null;
        }

        // Update is called once per frame
        void Update()
        {
            // 监测键盘和手柄按键
            for (int i = 0; i < InputManager.listenedKeys.Length; ++i)
            {
                bool isDown = Input.GetKey(InputManager.listenedKeys[i]);
                bool hasDown = InputManager.keyStatusMap[InputManager.listenedKeys[i]];
                if (isDown && !hasDown)
                    InputManager.OnKeyDown(InputManager.listenedKeys[i]);
                else if (hasDown && !isDown)
                    InputManager.OnKeyUp(InputManager.listenedKeys[i]);
            }

            // 检测手柄摇杆
            InputManager.OnJoysticsRockerAxes(InputManager.JoysticsAxes.LeftHorizontal, Input.GetAxis("Horizontal_Left"));
            InputManager.OnJoysticsRockerAxes(InputManager.JoysticsAxes.LeftVertical, Input.GetAxis("Vertical_Left"));
            InputManager.OnJoysticsRockerAxes(InputManager.JoysticsAxes.RightHorizontal, Input.GetAxis("Horizontal_XBoxRight"));
            InputManager.OnJoysticsRockerAxes(InputManager.JoysticsAxes.RightVertical, Input.GetAxis("Vertical_XBoxRight"));
            InputManager.OnJoysticsRockerAxes(InputManager.JoysticsAxes.SpecialHorizontal, Input.GetAxis("Horizontal_XBoxSpecial"));
            InputManager.OnJoysticsRockerAxes(InputManager.JoysticsAxes.SpecialVertical, Input.GetAxis("Vertical_XBoxSpecial"));

            // 监测鼠标和触屏
            for (int i = 0; i < Input.touchCount; ++i)
            {
                var tc = Input.GetTouch(i);
                switch (tc.phase)
                {
                    case TouchPhase.Began:
                        InputManager.OnTouchDown(tc.position);
                        break;
                    case TouchPhase.Canceled:
                    case TouchPhase.Ended:
                        InputManager.OnTouchUp(tc.position, new Vector2(tc.position.x - tc.deltaPosition.x, tc.position.y - tc.deltaPosition.y));
                        break;
                }
            }

            if (Input.touchCount <= 0)
            {
                if (Input.GetMouseButtonDown(0) && !InputManager.isMouseLeftDown)
                    InputManager.OnTouchDown(new Vector2(Input.mousePosition.x, Input.mousePosition.y), true);
                if (Input.GetMouseButtonUp(0) && InputManager.isMouseLeftDown)
                    InputManager.OnTouchUp(new Vector2(Input.mousePosition.x, Input.mousePosition.y), false);
            }
        }

        public override void OnMapClicked(int posx, int posy)
        {
            Game.Player.StartAutoStep(posx, posy);
        }

        /********************** Chat Part **************************************/

        public void ShowChatOnTop(string content, int speakerId = -1)
        {
            Game.Status = Model.EGameStatus.OnTipChat;
            topChatPanel.gameObject.SetActive(true);
            bottomChatPanel.gameObject.SetActive(false);
            tipsPanel.gameObject.SetActive(false);
            topChatPanel.SetChat(Game.Config.StringInternational.GetValue(content), speakerId);
        }

        public void ShowChatOnBottom(string content, int speakerId = -1)
        {
            Game.Status = Model.EGameStatus.OnTipChat;
            topChatPanel.gameObject.SetActive(false);
            bottomChatPanel.gameObject.SetActive(true);
            tipsPanel.gameObject.SetActive(false);
            bottomChatPanel.SetChat(Game.Config.StringInternational.GetValue(content), speakerId);
        }

        public void ShowTips(string content)
        {
            Game.Status = Model.EGameStatus.OnTipChat;
            topChatPanel.gameObject.SetActive(false);
            bottomChatPanel.gameObject.SetActive(false);
            tipsPanel.gameObject.SetActive(true);
            tipsPanel.SetTipText(Game.Config.StringInternational.GetValue(content));
        }

        private void ClearChats()
        {
            chat = null;
            chatIndex = 0;
            topChatPanel.gameObject.SetActive(false);
            bottomChatPanel.gameObject.SetActive(false);
            tipsPanel.gameObject.SetActive(false);
            if (battlePanel != null && battlePanel.isActiveAndEnabled)
            {
                Game.Status = Model.EGameStatus.OnBattle;
            }
            else if (choicePanel != null && choicePanel.isActiveAndEnabled)
            {
                Game.Status = Model.EGameStatus.OnChoice;
            }
            else
            {
                Game.Status = Model.EGameStatus.InGame;
            }
        }

        public void ChatBegan(Model.ChatData chat, Modal mod)
        {
            this.chat = chat;
            chatMod = mod;
            ChatStepOn();
        }

        public void ChatStepOn()
        {
            if (chat == null)    // 没有chat数据, 说明是代码呼出的临时chat
            {
                ClearChats();
            }
            else if (chatIndex >= chat.data.Length)
            {
                chatIndex = 0;
                EventManager.DispatchEvent(chat.eventId, chatMod, chat.eventData);    // TODO : 这里的事件返回值没有生效
                ClearChats();
            }
            else
            {
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

        public void StartBattle(Modal enemyModal, bool canFail, long yourUuid = -1, BattleDlg.BattlePauseEventCheck pauseCheck = null, int pauseEvent = 0)
        {
            battlePanel = BattleDlg.StartBattle(dialogCanvas.transform, OnBattleOver, canFail, enemyModal.Uuid, yourUuid, pauseCheck, pauseEvent);
            battlePanel.transform.localPosition = new Vector3(0, 0, 12);
            battlePanel.transform.localScale = new Vector3(1, 1, 1);
            battleMod = enemyModal;
        }

        public void StopBattle()
        {
            if (battlePanel != null)
            {
                BattleDlg.CloseBattle(battlePanel);
                battlePanel = null;
            }
        }

        private void OnBattleOver(bool gameover, int yourId, int yourLife, int goldGain, int expGain, int nextEvent, long[] nextEventData)
        {
            // 记录应用战斗结果（金币，经验，血量）
            if (yourId == Game.Player.PlayerId)
            {
                Game.Player.Life = yourLife;
                Game.Player.Gold += goldGain;
                Game.Player.Experience += expGain;
            }
            if (gameover)
            {
                OnGameOver();
            }
            else
            {
                EventManager.DispatchEvent(nextEvent, battleMod, nextEventData);
            }
        }

        private void OnGameOver()
        {
            StartCoroutine(GameOverResolve());
            Game.Map.ShowCurtain(Curtain.ContentType.GameOver, null);
            BackToStartScene(7);
        }

        private IEnumerator GameOverResolve()
        {
            yield return new WaitForSeconds(1.6f);
            battlePanel.gameObject.SetActive(false);
        }

        /********************** Choice Part **************************************/

        public void StartChoice(Model.ChoiceData choiceData, Modal mod, Model.EGameStatus nextStatus = Model.EGameStatus.InGame)
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

        /**************************************************************************************************/

        private Model.ChatData chat;
        private Modal chatMod;
        private int chatIndex = 0;
        private Modal battleMod;

        private GameObject dialogCanvas = null;
        private ChatDlg topChatPanel = null;
        private ChatDlg bottomChatPanel = null;
        private TipBar tipsPanel = null;
        private ChoiceDlg choicePanel = null;
        private BattleDlg battlePanel = null;
    }

}