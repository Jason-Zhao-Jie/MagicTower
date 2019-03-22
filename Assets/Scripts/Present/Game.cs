public static class Game {
    public static void Initial() {
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
            Config = new ConfigCenter();
        }

        if (Managers == null)
        {
            Managers = new ManagerSet();
        }

        if (ObjPool == null)
        {
            ObjPool = new ObjectPool();
        }

        if(CurrentSaveName == null)
        {
            CurrentSaveName = "";
        }

        status = Constant.EGameStatus.Start;
    }

    public static AScene CurrentScene
    {
        get; set;
    }

    public static ConfigCenter Config
    {
        get; private set;
    }

    public static ManagerSet Managers
    {
        get; private set;
    }

    public static MapController Map
    {
        get; set;
    }

    public static PlayerController Player
    {
        get; set;
    }

    public static ObjectPool ObjPool
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
        public Constant.PlayerData player;
        public RuntimePositionData pos;
        //public Constant.MapData[] maps;
        public RuntimeNumberData[] numbers;
    }

    public static async System.Threading.Tasks.Task<bool> Save(string saveName)
    {
        if (!Managers.IOD.MkdirIfNotExist("save", saveName) || !Managers.IOD.MkdirIfNotExist("save", saveName, "MapData"))
        {
            return false;
        }
        var maps = new Constant.MapData[Map.MapsCount];
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
            await Managers.IOD.SaveToFile(System.Text.Encoding.UTF8.GetBytes(json), "save", saveName, "RuntimeData.json");
            foreach (var i in maps)
            {
                await Managers.IOD.SaveToFile(System.Text.Encoding.UTF8.GetBytes(json), "save", saveName, "MapData", i.mapId.ToString() + ".json");
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
        string json = "";
        Constant.MapData[] maps = null;
        if(saveName == null)
        {
            saveName = CurrentSaveName;
        }
        try
        {
            if (saveName == "")
            {
                json = UnityEngine.Resources.Load<UnityEngine.TextAsset>("RuntimeData").text;
            }
            else
            {
                var bin = await Managers.IOD.LoadFromFile("save", saveName, "RuntimeData.json");
                json = System.Text.Encoding.UTF8.GetString(bin);
                var mapFiles = Managers.IOD.ListAllFiles("*.json", saveName, "MapData");
                maps = new Constant.MapData[mapFiles.Length];
                int index = 0;
                foreach (var i in mapFiles)
                {
                    var mapBin = await Managers.IOD.LoadFromFileWholePath(i);
                    var mapStr = System.Text.Encoding.UTF8.GetString(mapBin);
                    var mapData = UnityEngine.JsonUtility.FromJson<Constant.MapData>(mapStr);
                    maps[index++] = mapData;
                }
            }
        }
        catch (System.IO.IOException)
        {
            return false;
        }
        var data = UnityEngine.JsonUtility.FromJson<RuntimeGameData>(json);
        numberData = new System.Collections.Generic.Dictionary<int, long>();
        foreach(var i in data.numbers)
        {
            numberData.Add(i.id, i.value);
        }
        Map.SetStartData(data.pos.mapId, maps);
        Player.ShowPlayer(data.pos.x, data.pos.y, data.player, true);
        Status = Constant.EGameStatus.InGame;
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

    public static Constant.EGameStatus Status
    {
        get { return status; }
        set
        {
            status = value;
            Managers.Input.OnChangeWalkState();
        }
    }

    private static Constant.EGameStatus status;

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
