﻿using System.Collections;
using System.Collections.Generic;

namespace MagicTower {

    public static class Game {
        public const bool DEBUG = true;

        public static void DebugLog(params string[] content) {
            if (DEBUG && content != null) {
                string text = "";
                for (var i = 0; i < content.Length; ++i) {
                    text += content[i];
                }
                UnityEngine.Debug.Log(text);
            }
        }

        static Game() {
            // private manager
            if (Input == null) {
                Input = new Present.Manager.InputManager();
            }

            CurrentSaveName = "";
        }

        public static async System.Threading.Tasks.Task Initial(Components.AScene scene) {
            if (!InitOK) {
                Present.Manager.AdsPluginManager.Initialize(false, true);
                InitOK = true;
            }

            if (Config == null) {
                Config = new Model.ConfigCenter();
            }

            if(Settings == null) {
                Settings = new Model.GlobalSettings();
                await Settings.Load();
                Present.Manager.AudioManager.MusicVolume = Settings.Settings.musicVolume;
                Present.Manager.AudioManager.SoundVolume = Settings.Settings.soundVolume;
            }

            if (ObjPool == null) {
                ObjPool = new ArmyAnt.ViewUtil.ObjectPool();
            }

            // game state
            CurrentScene = scene;
            status = Model.EGameStatus.Start;
        }

        public static void ExitGame() {
            UnityEngine.Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        public static Components.AScene CurrentScene {
            get; private set;
        }

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

        public static bool InitOK { get; private set; }

        public static bool ObjPoolRecycleSelf<T>(T self) where T : ArmyAnt.ViewUtil.ObjectPool.AViewUnit {
            return ObjPool.RecycleAnElement(self);
        }

        public static void InvokeInMainThread(int delaySeconds, Model.EmptyCallBack cb) {
            InvokeInMainThread(() => { CurrentScene.StartCoroutine(CoroutineFunc(delaySeconds, cb)); });
        }

        private static IEnumerator CoroutineFunc(int seconds, Model.EmptyCallBack cb) {
            yield return new UnityEngine.WaitForSeconds(seconds);
            cb();
        }

        public static void InvokeInMainThread(Model.EmptyCallBack func) {
            if (func == null) {
                return;
            }
            if (System.Threading.Thread.CurrentThread.ManagedThreadId == mainThreadId) {
                func();
            } else {
                lock (mainThreadTaskQueue) {
                    mainThreadTaskQueue.Enqueue(func);
                }
            }
        }

        private static Present.Manager.InputManager Input;

        public static void SceneUpdate() {
            Input.UpdateScene();

            Model.EmptyCallBack resolvingTask = null;
            lock (mainThreadTaskQueue) {
                if (mainThreadTaskQueue.Count > 0) {
                    resolvingTask = mainThreadTaskQueue.Dequeue();
                }
            }
            resolvingTask?.Invoke();
        }

        private static readonly int mainThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
        private static Queue<Model.EmptyCallBack> mainThreadTaskQueue = new Queue<Model.EmptyCallBack>();

        ////////////// Runtime Data Save / Load ///////////////

        public static async System.Threading.Tasks.Task<bool> Save(string saveName) {
            var maps = new Model.MapData[Map.MapsCount];
            Map.GetAllMapData().Values.CopyTo(maps, 0);
            return await Present.Manager.SaveManager.Write(saveName, maps, numberData, Map.MapId, Player.PlayerPosX, Player.PlayerPosY, Player.PlayerData);
        }

        public static async System.Threading.Tasks.Task<bool> Load(string saveName = null) {
            if (saveName == null) {
                saveName = CurrentSaveName;
            }
            try {
                var (maps, numberData, mapId, playerPosX, playerPosY, PlayerData) = await Present.Manager.SaveManager.Read(saveName);
                Map.SetStartData(mapId, maps);
                Player.ShowPlayer(playerPosX, playerPosY, PlayerData, true);
            } catch (System.IO.IOException) {
                return false;
            }
            Status = Model.EGameStatus.InGame;
            return true;
        }

        public static string CurrentSaveName { get; set; }

        ////////////// Runtime Data Part ///////////////

        /// <summary>
        /// 定义变价资源的价格缓存位，每个缓存位有其独特的变价算法
        /// </summary>
        public enum VariablePriceType : int {
            NoChange = 0, // 默认值，不变价
            GoldenIncreasing = 1, // 每次+1价格，用于普通金币商店
            KeyStoreDoubling = 2, // 每次价格乘以2，用于后期钥匙商店
        }

        public static float RealFontSize { get { return CurrentScene.GetComponent<UnityEngine.RectTransform>().rect.height / 650; } }

        public static Model.EGameStatus Status {
            get { return status; }
            set {
                status = value;
                if (Player != null) {
                    Input.OnChangeWalkState();
                }
            }
        }

        private static Model.EGameStatus status;

        /// <summary>
        /// Gets the number data.
        /// </summary>
        /// <returns>The number data.</returns>
        /// <param name="id"> id </param>
        /// <param name="useType"> 该值如何变化， 不输入此参数则不变化 </param>
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

}
