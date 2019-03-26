namespace MagicTower
{

    public static class Game
    {
        public const bool DEBUG = true;

        public static void DebugLog(params string[] content)
        {
            if (DEBUG)
            {
                UnityEngine.Debug.Log(content);
            }
        }

        public static void Initial()
        {
            if (!InitOK)
            {
#if UNITY_EDITOR
                // 编辑器退出时销毁
                UnityEditor.EditorApplication.quitting += OnApplicationExit;
#endif
                // 程序退出时按顺序回收, 或做其他必要操作
                UnityEngine.Application.quitting += OnApplicationExit;
                InitOK = true;
            }

            if (Config == null)
            {
                Config = new Model.ConfigCenter();
            }

            if (ObjPool == null)
            {
                ObjPool = new ArmyAnt.ViewUtil.ObjectPool();
            }

            if (CurrentSaveName == null)
            {
                CurrentSaveName = "";
            }

            Present.Manager.AdsPluginManager.Initialize(false, true);

            status = Model.EGameStatus.Start;
        }

        public static void ExitGame()
        {
            UnityEngine.Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        public static bool ObjPoolRecycleSelf<T>(T self) where T : ArmyAnt.ViewUtil.ObjectPool.AViewUnit
        {
            return ObjPool.RecycleAnElement(self);
        }

        public static Components.AScene CurrentScene
        {
            get; set;
        }

        public static Model.ConfigCenter Config
        {
            get; private set;
        }

        public static Present.Map.Controller Map
        {
            get; set;
        }

        public static Present.Player.Controller Player
        {
            get; set;
        }

        public static ArmyAnt.ViewUtil.ObjectPool ObjPool
        {
            get; private set;
        }

        public static bool InitOK { get; private set; }

        ////////////// Runtime Data Save / Load ///////////////

        [System.Serializable]
        private struct RuntimePositionData
        {
            public int mapId;
            public int x;
            public int y;
        }

        [System.Serializable]
        private struct RuntimeNumberData
        {
            public int id;
            public long value;
        }

        [System.Serializable]
        private class RuntimeGameData
        {
            public Model.PlayerData player;
            public RuntimePositionData pos;
            public RuntimeNumberData[] numbers;
        }

        public static async System.Threading.Tasks.Task<bool> Save(string saveName)
        {
            if (!ArmyAnt.Manager.IOManager.MkdirIfNotExist("save", saveName) || !ArmyAnt.Manager.IOManager.MkdirIfNotExist("save", saveName, "MapData"))
            {
                return false;
            }
            var maps = new Model.MapData[Map.MapsCount];
            Map.GetAllMapData().Values.CopyTo(maps, 0);
            var numbers = new RuntimeNumberData[numberData.Count];
            int index_numbers = 0;
            foreach (var i in numberData)
            {
                numbers[index_numbers++] = new RuntimeNumberData
                {
                    id = i.Key,
                    value = i.Value,
                };
            }
            var json = UnityEngine.JsonUtility.ToJson(new RuntimeGameData
            {
                player = Player.PlayerData,
                pos = new RuntimePositionData
                {
                    mapId = Map.MapId,
                    x = Player.PlayerPosX,
                    y = Player.PlayerPosY,
                },
                numbers = numbers,
            }, false);
            try
            {
                await ArmyAnt.Manager.IOManager.SaveToFile(System.Text.Encoding.UTF8.GetBytes(json), "save", saveName, "RuntimeData.json");
                foreach (var i in maps)
                {
                    await ArmyAnt.Manager.IOManager.SaveToFile(System.Text.Encoding.UTF8.GetBytes(json), "save", saveName, "MapData", i.mapId.ToString() + ".json");
                }
            }
            catch (System.IO.IOException)
            {
                return false;
            }
            return true;
        }

        public static async System.Threading.Tasks.Task<bool> Load(string saveName = null)
        {
            RuntimeGameData data = null;
            Model.MapData[] maps = null;
            if (saveName == null)
            {
                saveName = CurrentSaveName;
            }
            try
            {
                if (saveName == "")
                {
                    var json = UnityEngine.Resources.Load<UnityEngine.TextAsset>("RuntimeData").text;
                    data = UnityEngine.JsonUtility.FromJson<RuntimeGameData>(json);
                    data.player = Config.players[62];   // TODO : 这里是新游戏载入的player，由于需要进入MainScene之前作选择，因此这里的逻辑需要进一步拓展，目前先写死
                }
                else
                {
                    var bin = await ArmyAnt.Manager.IOManager.LoadFromFile("save", saveName, "RuntimeData.json");
                    var json = System.Text.Encoding.UTF8.GetString(bin);
                    data = UnityEngine.JsonUtility.FromJson<RuntimeGameData>(json);
                    var mapFiles = ArmyAnt.Manager.IOManager.ListAllFiles("*.json", saveName, "MapData");
                    maps = new Model.MapData[mapFiles.Length];
                    int index = 0;
                    foreach (var i in mapFiles)
                    {
                        var mapBin = await ArmyAnt.Manager.IOManager.LoadFromFileWholePath(i);
                        var mapStr = System.Text.Encoding.UTF8.GetString(mapBin);
                        var mapData = UnityEngine.JsonUtility.FromJson<Model.MapData>(mapStr);
                        maps[index++] = mapData;
                    }
                }
            }
            catch (System.IO.IOException)
            {
                return false;
            }
            numberData = new System.Collections.Generic.Dictionary<int, long>();
            foreach (var i in data.numbers)
            {
                numberData.Add(i.id, i.value);
            }
            Map.SetStartData(data.pos.mapId, maps);
            Player.ShowPlayer(data.pos.x, data.pos.y, data.player, true);
            Status = Model.EGameStatus.InGame;
            return true;
        }

        public static string CurrentSaveName { get; set; }

        ////////////// Runtime Data Part ///////////////

        /// <summary>
        /// 定义变价资源的价格缓存位，每个缓存位有其独特的变价算法
        /// </summary>
        public enum VariablePriceType : int
        {
            NoChange = 0, // 默认值，不变价
            GoldenIncreasing = 1, // 每次+1价格，用于普通金币商店
            KeyStoreDoubling = 2, // 每次价格乘以2，用于后期钥匙商店
        }

        public static float RealFontSize { get { return CurrentScene.GetComponent<UnityEngine.RectTransform>().rect.height / 650; } }

        public static Model.EGameStatus Status
        {
            get { return status; }
            set
            {
                status = value;
                Present.Manager.InputManager.OnChangeWalkState();
            }
        }

        private static Model.EGameStatus status;

        /// <summary>
        /// Gets the number data.
        /// </summary>
        /// <returns>The number data.</returns>
        /// <param name="id"> id </param>
        /// <param name="useType"> 该值如何变化， 不输入此参数则不变化 </param>
        public static long GetNumberData(int id, VariablePriceType useType = VariablePriceType.NoChange)
        {
            var data = numberData[id];
            switch (useType)
            {
                case VariablePriceType.GoldenIncreasing:
                    numberData[id] += 1;
                    break;
                case VariablePriceType.KeyStoreDoubling:
                    numberData[id] *= 2;
                    break;
            }
            return data;
        }

        private static System.Collections.Generic.Dictionary<int, long> numberData = null;

        private static void OnApplicationExit()
        {
            ObjPool.ClearAll();
        }
    }

}