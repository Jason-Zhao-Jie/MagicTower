public static class Game {
    public static void Initial(UnityEngine.Vector2 screenSize) {
#if UNITY_EDITOR
        // 编辑器退出时销毁
        UnityEditor.EditorApplication.quitting += OnApplicationExit;
#endif
        // 程序退出时按顺序回收, 或做其他必要操作
        UnityEngine.Application.quitting += OnApplicationExit;

        if (Data == null)
        {
            Data = new GameDataInitializer();
        }

        if (Controller == null)
        {
            Controller = new GameControllerInitializer();
        }

        if (View == null)
        {
            View = new GameViewInitializer(screenSize);
        }
    }

    public static GameDataInitializer Data
    {
        get; private set;
    }

    public static GameControllerInitializer Controller
    {
        get; private set;
    }

    public static GameViewInitializer View
    {
        get; private set;
    }

    private static void OnApplicationExit()
    {
        View.OnViewExit();
    }
}
