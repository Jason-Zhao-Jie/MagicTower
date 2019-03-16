public static class Game {
    public static void Initial(UnityEngine.Vector2 screenSize) {
#if UNITY_EDITOR
        // 编辑器退出时销毁
        UnityEditor.EditorApplication.quitting += OnApplicationExit;
#endif
        // 程序退出时按顺序回收, 或做其他必要操作
        UnityEngine.Application.quitting += OnApplicationExit;

        if (Config == null)
        {
            Config = new ConfigCenter();
        }

        if (Controller == null)
        {
            Controller = new GameControllerInitializer();
        }

        if (Map == null)
        {
            Map = new MapController();
        }

        if (Player == null)
        {
            Player = new PlayerController();
        }

        if (ScreenAdaptorInst == null)
        {
            ScreenAdaptorInst = new ScreenAdaptator(screenSize);
        }
        else
        {
            ScreenAdaptorInst.SetScreenSize(screenSize);
        }

        if (ObjPool == null)
        {
            ObjPool = new ObjectPool();
        }

        status = Constant.EGameStatus.Start;
    }

    public static ConfigCenter Config
    {
        get; private set;
    }

    public static GameControllerInitializer Controller
    {
        get; private set;
    }

    public static MapController Map
    {
        get; private set;
    }

    public static PlayerController Player
    {
        get; private set;
    }

    public static ScreenAdaptator ScreenAdaptorInst
    {
        get; private set;
    }

    public static ObjectPool ObjPool
    {
        get; private set;
    }

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
        var maps = new Constant.MapData[Game.Map.MapsCount];
        int index = 0;
        foreach (var item in Game.Map.GetAllMapData())
        {
            maps[index++] = item.Value;
        }
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
        Map.SetStartData(data.pos.mapId, data.maps);
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
            Controller.Input.OnChangeWalkState();
        }
    }

    private static Constant.EGameStatus status;

    private static void OnApplicationExit()
    {
        ObjPool.ClearAll();
    }
}
