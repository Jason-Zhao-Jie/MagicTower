using System.Collections;
using System.Collections.Generic;

using MagicTower.Components;
using MagicTower.Components.Control;
using MagicTower.Present.Manager;

using UnityEngine;

namespace MagicTower {

    public static class Game {
        static Game() {
            // private manager
            if(Input == null) {
                Input = new InputManager();
            }

            if(Settings == null) {
                Settings = new Model.GlobalSettings();
                Settings.Load();
                AudioManager.MusicVolume = Settings.Settings.musicVolume;
                AudioManager.SoundVolume = Settings.Settings.soundVolume;
            }

            if(ObjPool == null) {
                ObjPool = new ArmyAnt.ViewUtil.ObjectPool();
            }

            GamePaused = true;
            Status = Model.EGameStatus.Start;
        }

        public static void Initial(Components.Scene.SceneView scene, Components.UIPanel.GlobalLoading resource, Components.Unit.Curtain curtain) {
            AdsPluginManager.Initialize(false, true);

            if (Config == null) {
                Config = new Model.ConfigCenter();
            }

            // game state
            View = scene;
            Resource = resource;
            Resource.DialogCanvas = View.uiCanvas.transform;
            Player = new Present.Player.Controller(View.RefreshPlayerPanel());
            Map = new Present.Map.Controller(View.mapTileRoot, curtain);
        }

        public static void ExitGame() {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        #region Properties

        private static Components.Scene.SceneView View;
        private static Components.UIPanel.GlobalLoading Resource;

        public static bool IsDebug => Config.debug;

        public static GameObject ModalImage => Resource.modalImage;

        public static GameObject ModalSprite => Resource.modalSprite;

        public static GameObject JumpWord => Resource.jumpWordPrefab;

        public static Model.ConfigCenter Config {
            get; private set;
        }

        public static Model.GlobalSettings Settings {
            get; private set;
        }

        public static Present.Map.Controller Map {
            get; set;
        }

        public static Present.Player.Controller Player {
            get; set;
        }

        public static ArmyAnt.ViewUtil.ObjectPool ObjPool {
            get; private set;
        }

        #endregion

        #region Logger

        private enum LogType : int {
            Verbose,
            Note,
            Warning,
            Error,
            Assertion,
        }

        private delegate void Log(object msg);

        private static readonly Log[] loggerEvents = { null, Debug.Log, Debug.LogWarning, Debug.LogError, DebugLogAssertion };

        private static void DebugLogAssertion(object msg) {
            Debug.LogAssertion(msg);
        }

        private static void DebugLog(LogType type, params object[] content) {
            if(IsDebug && content != null) {
                string text = "";
                for(var i = 0; i < content.Length; ++i) {
                    text += content[i];
                }
                loggerEvents[(int)type](text);
            }
        }

        public static void DebugLogNote(params object[] content) => DebugLog(LogType.Note, content);

        public static void DebugLogWarning(params object[] content) => DebugLog(LogType.Warning, content);

        public static void DebugLogError(params object[] content) => DebugLog(LogType.Error, content);

        public static void DebugLogAssertion(params object[] content) => DebugLog(LogType.Assertion, content);

        #endregion

        #region Resource API 

        public static Sprite[] GetMods(string modName) {
            return Resource.modSprites[modName];
        }

        public static IReadOnlyDictionary<string, Sprite[]> GetMods() {
            return Resource.modSprites;
        }

        public static AudioClip GetAudio(string audioName) {
            return Resource.audioClips[audioName];
        }

        public static IReadOnlyDictionary<string, AudioClip> GetAudio() {
            return Resource.audioClips;
        }

        public static GameObject GetPlayer(string player) {
            return Resource.players[player];
        }

        public static void PlayAds() {
            var ret = AdsPluginManager.ShowRewardBasedVideo((string type, double amount) => {
                ShowTip("Google MobAds loaded OK and get reward successful");
            }, () => {
                ShowTip("Google MobAds loaded OK and closed");
            });
            switch(ret) {
                case AdsPluginManager.AdLoadingState.UnablePlatform:
                    ShowTip("Google MobAds cannot be used at current platform " + Application.platform.ToString());
                    break;
                case AdsPluginManager.AdLoadingState.SdkInitializing:
                    ShowTip("Google MobAds sdk is initializing");
                    break;
                case AdsPluginManager.AdLoadingState.NotInitialized:
                    ShowTip("Google MobAds this type of ads does not initialized");
                    break;
                case AdsPluginManager.AdLoadingState.Loading:
                    ShowTip("Google MobAds ads is loading");
                    break;
                case AdsPluginManager.AdLoadingState.LoadFailedAndReloading:
                    ShowTip("Google MobAds loaded failure, message: ", AdsPluginManager.RewardBasedVideoLoadedFailedMessage);
                    break;
            }
        }

        private static T ShowUI<T>(UIType type) where T : MonoBehaviour => Resource.ShowUI(type).GetComponent<T>();

        private static T GetUI<T>(UIType type) where T : MonoBehaviour => Resource.GetUI(type).GetComponent<T>();

        public static bool HideUI(UIType type) => Resource.HideUI(type);

        public static void HideAllDialog() {
            HideUI(UIType.AlertDialog);
            HideUI(UIType.BattleDialog);
            HideUI(UIType.BottomChat);
            HideUI(UIType.ChoiceDialog);
            HideUI(UIType.MainMenu);
            HideUI(UIType.SaveLoadDialog);
            HideUI(UIType.SettingDialog);
            HideUI(UIType.TipBar);
            HideUI(UIType.TopChat);
        }

        #endregion

        #region Invoke in Main Thread

        public static void InvokeInMainThread(int delaySeconds, Model.EmptyCallBack cb) {
            InvokeInMainThread(() => { View.StartCoroutine(CoroutineFunc(delaySeconds, cb)); });
        }

        private static IEnumerator CoroutineFunc(int seconds, Model.EmptyCallBack cb) {
            if(seconds > 0) {
                yield return new WaitForSeconds(seconds);
            }
            cb();
        }

        public static void InvokeInMainThread(Model.EmptyCallBack func) {
            if(func != null) {
                if(System.Threading.Thread.CurrentThread.ManagedThreadId == mainThreadId) {
                    func();
                } else {
                    lock(mainThreadTaskQueue) {
                        mainThreadTaskQueue.Enqueue(func);
                    }
                }
            }
        }

        public static readonly InputManager Input;

        public static void SceneUpdate() {
            // 输入输出检测
            Input.UpdateScene();

            // loading标识
            if (Resource == null || !LoadingTipShowed)
            {
                loadingShowedTotalTime = 0f;
            }
            else
            {
                loadingShowedTotalTime += Time.deltaTime;
            }
            for(var i = 0; i < loadingShowedConditions.Count; )
            {
                LoadingTipShowed = false;
                if (!loadingShowedConditions[i](Time.deltaTime, loadingShowedTotalTime))
                {
                    loadingShowedConditions.RemoveAt(i);
                }
                else
                {
                    LoadingTipShowed = true;
                    break;
                }
            }

            // InvokeInMainThread
            Model.EmptyCallBack resolvingTask = null;
            lock (mainThreadTaskQueue) {
                if (mainThreadTaskQueue.Count > 0) {
                    resolvingTask = mainThreadTaskQueue.Dequeue();
                }
            }
            resolvingTask?.Invoke();

            // 游戏总时间记录
            if(!GamePaused) {
                GameTime += Time.deltaTime;
            }
        }

        public static void SceneOnPostRender() {
            if(CapturedTexture == null && Status != Model.EGameStatus.Start) {
                CapturedTexture = ArmyAnt.ViewUtil.ScreenUtil.CaptureRect(Map.MapRect);
                Resource.GetUI(UIType.AlertDialog)?.gameObject?.SetActive(true);
                Resource.GetUI(UIType.SaveLoadDialog)?.gameObject?.SetActive(true);
                Resource.GetUI(UIType.MainMenu)?.gameObject?.SetActive(true);
            }
        }

        private static readonly int mainThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
        private static readonly Queue<Model.EmptyCallBack> mainThreadTaskQueue = new Queue<Model.EmptyCallBack>();

        #endregion

        #region Runtime Data Save / Load

        public static void RecaptureMap() {
            CapturedTexture = null;
            if(Status != Model.EGameStatus.Start) {
                Resource.GetUI(UIType.AlertDialog)?.gameObject?.SetActive(false);
                Resource.GetUI(UIType.SaveLoadDialog)?.gameObject?.SetActive(false);
                Resource.GetUI(UIType.MainMenu)?.gameObject?.SetActive(false);
            }
        }

        public static Texture2D CapturedTexture { get; private set; }

        public static bool LoadGame(string saveName = "") {
            try {
                var (maps, data) = SaveManager.Read(saveName);
                Map.SetStartData(data.pos.mapId, maps);
                numberData = new Dictionary<int, long>();
                foreach(var i in data.numbers) {
                    numberData.Add(i.id, i.value);
                }
                Player.ShowPlayer(data.pos.x, data.pos.y, data.player, true);
                StartRecordTime(data.totalTime);
            } catch(System.IO.IOException) {
                return false;
            }
            HideUI(UIType.StartPanel);
            HideUI(UIType.AlertDialog);
            HideUI(UIType.SaveLoadDialog);
            HideUI(UIType.MainMenu);
            Status = Model.EGameStatus.InGame;
            return true;
        }

        public static bool SaveGame(string saveName, Texture2D captured) {
            // get maps data
            var maps = new Model.MapData[Map.MapsCount];
            Map.GetAllMapData().Values.CopyTo(maps, 0);

            // collect numbers data
            var numbers = new Model.RuntimeNumberData[numberData.Count];
            int index_numbers = 0;
            foreach(var i in numberData) {
                numbers[index_numbers++] = new Model.RuntimeNumberData {
                    id = i.Key,
                    value = i.Value,
                };
            }

            // get all save data
            var saveData = new Model.RuntimeGameData {
                player = Player.PlayerData,
                pos = new Model.RuntimePositionData {
                    mapId = Map.MapId,
                    x = Player.PlayerPosX,
                    y = Player.PlayerPosY,
                },
                numbers = numbers,
                lastTime = System.DateTime.Now.ToFileTime(),
                totalTime = System.Convert.ToInt64(GameTime),
            };

            return SaveManager.Write(saveName, maps, saveData, captured);
        }

        public static void StopAndBackToStart() {
            AudioManager.StopMusic();
            Resource.ShowUI(UIType.StartPanel);
        }

        private static void StopAndBackToStart(int seconds) {
            View.StartCoroutine(WaitAndBackToStart(seconds));
        }

        private static IEnumerator WaitAndBackToStart(int seconds) {
            yield return new WaitForSeconds(seconds);
            StopAndBackToStart();
        }

        #endregion

        #region Runtime Data

        private static void StartRecordTime(long init) {
            GameTime = init;
            GamePaused = false;
        }

        public static bool GamePaused { get; set; }

        public static double GameTime { get; private set; }

        #endregion

        #region Start Panel

        public static void ShowStartPanel() {
            Resource.ShowUI(UIType.StartPanel);
        }

        #endregion

        #region Data Editor

        public static void ShowDataEditor() {
            Resource.ShowUI(UIType.DataEditor);
        }

        #endregion

        #region Main Menu

        public static void ShowMainMenu() {
            Resource.ShowUI(UIType.MainMenu);
        }

        #endregion

        #region Chat Part

        public static TipBar ShowTip(params string[] textKeys) {
            return ShowTip(false, textKeys);
        }

        public static TipBar ShowTip(int autoRemoveTime, params string[] textKeys) {
            var ui = ShowTip(textKeys);
            ui.StartAutoRemove(autoRemoveTime);
            return ui;
        }

        public static TipBar ShowTip(bool silent, params string[] textKeys) {
            var text = "";
            if(textKeys != null) {
                var builder = new System.Text.StringBuilder();
                for(var i = 0; i < textKeys.Length; ++i) {
                    builder.Append(Config.StringInternational.GetValue(textKeys[i]));
                }
                text = builder.ToString();
            }
            HideUI(UIType.TopChat);
            HideUI(UIType.BottomChat);
            var ui = ShowUI<TipBar>(UIType.TipBar);
            ui.Init(text, silent);
            return ui;
        }

        private static void ShowChatOnTop(string content, int speakerId = -1) {
            Status = Model.EGameStatus.OnTipChat;
            HideUI(UIType.BottomChat);
            HideUI(UIType.TipBar);
            ShowUI<ChatDlg>(UIType.TopChat).Init(Config.StringInternational.GetValue(content), speakerId);
        }

        private static void ShowChatOnBottom(string content, int speakerId = -1) {
            Status = Model.EGameStatus.OnTipChat;
            HideUI(UIType.TopChat);
            HideUI(UIType.TipBar);
            ShowUI<ChatDlg>(UIType.BottomChat).Init(Config.StringInternational.GetValue(content), speakerId);
        }

        private static void ClearChats() {
            chat = null;
            chatIndex = 0;
            HideUI(UIType.TopChat);
            HideUI(UIType.BottomChat);
            HideUI(UIType.TipBar);
            if(Resource.GetUI(UIType.BattleDialog) != null) {
                Status = Model.EGameStatus.OnBattle;
            } else if(Resource.GetUI(UIType.ChoiceDialog) != null) {
                Status = Model.EGameStatus.OnChoice;
            } else {
                Status = Model.EGameStatus.InGame;
            }
        }

        public static void ChatBegan(Model.ChatData chatData, Components.Unit.Modal mod) {
            chat = chatData;
            chatMod = mod;
            ChatStepOn();
        }

        public static void ChatStepOn() {
            if(chat == null) {
                // 没有chat数据, 说明是代码呼出的临时tips
                ClearChats();
            } else if(chatIndex >= chat.data.Length) {
                chatIndex = 0;
                EventManager.DispatchEvent(chat.eventId, chatMod, chat.eventData);    // TODO : 这里的事件返回值没有生效
                ClearChats();
            } else {
                var chatData = chat.data[chatIndex];
                if(chatData.speakerId < -100)
                    ShowTip(chatData.tipSilent, chatData.content);
                else if(chatData.speakerId < 0)
                    ShowChatOnTop(chatData.content, chatMod.ModId);
                else if(chatData.speakerId == 0)
                    ShowChatOnBottom(chatData.content, Player.PlayerId);
                else
                    ShowChatOnTop(chatData.content, chatData.speakerId);
                ++chatIndex;
            }
        }

        private static Model.ChatData chat;
        private static Components.Unit.Modal chatMod;
        private static int chatIndex = 0;

        #endregion

        #region Loading Tip

        public static bool LoadingTipShowed {
            get => Resource.LoadingUIShowed;
            set => Resource.LoadingUIShowed = value;
        }

        public static void ShowLoadingUntil(System.Func<float, float, bool> condition)
        {
            if (condition != null)
            {
                loadingShowedConditions.Add(condition);
            }
        }

        private static readonly List<System.Func<float, float, bool>> loadingShowedConditions = new List<System.Func<float, float, bool>>();
        private static float loadingShowedTotalTime = 0f;

        #endregion

        #region Alert Dialog

        public static void ShowAlert(string contentStrId, TextAnchor contentAlignment, Model.EmptyBoolCallBack leftCallback = null, params string[] contentStrValues) {
            ShowUI<AlertDlg>(UIType.AlertDialog).Init(contentStrId, contentAlignment, leftCallback, AlertDlg.STR_UI_OK, null, AlertDlg.STR_UI_CANCEL, contentStrValues);
        }

        public static void ShowAlert(string contentStrId, TextAnchor contentAlignment, Model.EmptyBoolCallBack leftCallback, string leftStrId, Model.EmptyBoolCallBack rightCallback, string rightStrId, params string[] contentStrValues) {
            ShowUI<AlertDlg>(UIType.AlertDialog).Init(contentStrId, contentAlignment, leftCallback, leftStrId, rightCallback, rightStrId, contentStrValues);
        }

        /// <summary>
        /// (coroutine) 显示 Alert Dialog, 并阻塞调用直到对话框关闭
        /// </summary>
        /// <param name="contentStr"> 对话框内容文字 </param>
        /// <param name="contentAlignment"> 对话框内容文字对齐方式 </param>
        /// <param name="leftCallback"> 点击左边按钮的回调, 无回调或返回true表示回调结束后关闭 Alert Dialog </param>
        /// <param name="leftStr"> 左边按钮文字 Id </param>
        /// <param name="rightCallback"> 点击右边按钮的回调, 无回调或返回true表示回调结束后关闭 Alert Dialog </param>
        /// <param name="rightStr"> 右边按钮文字 Id </param>
        public static IEnumerator ShowAlertModal(string contentStr, TextAnchor contentAlignment, Model.EmptyBoolCallBack leftCallback, string leftStrId, Model.EmptyBoolCallBack rightCallback, string rightStrId, params string[] contentStrValues) {
            ShowAlert(contentStr, contentAlignment, leftCallback, leftStrId, rightCallback, rightStrId, contentStrValues);
            yield return new WaitUntil(() => Resource.GetUI(UIType.AlertDialog) == null);
        }

        #endregion

        #region Alert Dialog

        public static void ShowInfo() {
            Resource.ShowUI(UIType.InfoDialog);
        }

        #endregion

        #region Choice Part

        public static void ShowChoice(Model.ChoiceData choiceData, Components.Unit.Modal mod, Model.EGameStatus nextStatus = Model.EGameStatus.InGame) {
            HideUI(UIType.TopChat);
            HideUI(UIType.BottomChat);
            HideUI(UIType.TipBar);
            ShowUI<ChoiceDlg>(UIType.ChoiceDialog).Init(choiceData, mod, nextStatus);
        }

        #endregion

        #region Battle Part

        public static void StartBattle(Components.Unit.Modal enemyModal, bool canFail, long yourUuid = -1, BattleDlg.BattlePauseEventCheck pauseCheck = null, int pauseEvent = 0) {
            ShowUI<BattleDlg>(UIType.BattleDialog).Init(OnBattleOver, canFail, enemyModal.Uuid, yourUuid, pauseCheck, pauseEvent);
            battleMod = enemyModal;
        }

        public static void StopBattle() {
            GetUI<BattleDlg>(UIType.BattleDialog)?.StopBattle();
            HideUI(UIType.BattleDialog);
        }

        private static void OnBattleOver(bool gameover, int yourId, int yourLife, int goldGain, int expGain, int nextEvent, long[] nextEventData) {
            // 记录应用战斗结果（金币，经验，血量）
            if(yourId == Player.PlayerId) {
                Player.Life = yourLife;
                Player.Gold += goldGain;
                Player.Experience += expGain;
            }
            if(gameover) {
                OnGameOver();
            } else {
                EventManager.DispatchEvent(nextEvent, battleMod, nextEventData);
            }
        }

        private static void OnGameOver() {
            Map.ShowCurtain(Components.Unit.Curtain.ContentType.GameOver, null);
            StopAndBackToStart(Config.gameoverBackTime);
        }

        private static Components.Unit.Modal battleMod;

        #endregion

        #region Save Load Dialog

        public static void ShowSaveLoadDialog(bool save) {
            ShowUI<SaveLoadDlg>(UIType.SaveLoadDialog).Init(save);
        }

        #endregion

        #region Setting Dialog

        public static void ShowSettings() {
            Resource.ShowUI(UIType.SettingDialog);
        }

        #endregion

        #region Runtime Data Part

        /// <summary>
        /// 定义变价资源的价格缓存位，每个缓存位有其独特的变价算法
        /// </summary>
        public enum VariablePriceType : int {
            NoChange = 0, // 默认值，不变价
            GoldenIncreasing = 1, // 每次+1价格，用于普通金币商店
            KeyStoreDoubling = 2, // 每次价格乘以2，用于后期钥匙商店
        }

        public static float RealFontSize { get { return View.uiCanvas.GetComponent<RectTransform>().rect.height / 650; } }

        public static Model.EGameStatus Status;

        /// <summary>
        /// Gets the number data.
        /// </summary>
        /// <returns> The number data. </returns>
        /// <param name="id"> id </param>
        /// <param name="useType"> 该值如何变化, 不输入此参数则不变化 </param>
        public static long GetNumberData(int id, VariablePriceType useType = VariablePriceType.NoChange) {
            var data = numberData[id];
            switch (useType) {
                case VariablePriceType.GoldenIncreasing:
                    numberData[id] += 1;
                    break;
                case VariablePriceType.KeyStoreDoubling:
                    numberData[id] *= 2;
                    break;
            }
            return data;
        }

        private static Dictionary<int, long> numberData = null;
    }

    #endregion

}
