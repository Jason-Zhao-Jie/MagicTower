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

    ////////////// Runtime Data ///////////////

    [System.Serializable]
    private struct RuntimePositionData
    {
        public int x;
        public int y;
        public int mapId;
    }

    [System.Serializable]
    private class RuntimeGameData
    {
        public Constant.PlayerData player;
        public RuntimePositionData pos;
        public Constant.MapData[] maps;
    }

    public static string GetJsonOfRuntimeInfoData()
    {
        var maps = new Constant.MapData[Map.MapsCount];
        Map.GetAllMapData().Values.CopyTo(maps, 0);
        return UnityEngine.JsonUtility.ToJson(new RuntimeGameData
        {
            player = Player.PlayerData,
            pos = new RuntimePositionData
            {
                x = Player.PlayerPosX,
                y = Player.PlayerPosY,
                mapId = Map.MapId,
            },
            maps = maps
        }, false);
    }

    public static bool LoadRuntimeInfoDataFromJson(string json)
    {
        var data = UnityEngine.JsonUtility.FromJson<RuntimeGameData>(json);
        Player.PlayerData = data.player;
        Map.SetStartData(data.pos.mapId, data.maps);    // Warning : 因Map结构修改，此处map有可能为null，请在做存档读档功能时修改此处
        Player.PlayerPosX = data.pos.x;
        Player.PlayerPosY = data.pos.y;
        return true;
    }

    public static Constant.EGameStatus Status
    {
        get { return status; }
        set
        {
            status = value;
            Managers.Input.OnChangeWalkState();
        }
    }

    public static float RealFontSize { get { return CurrentScene.GetComponent<UnityEngine.RectTransform>().rect.height / 650; } }

    private static Constant.EGameStatus status;

    private static void OnApplicationExit()
    {
        ObjPool.ClearAll();
    }
}
