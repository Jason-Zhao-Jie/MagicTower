public static class Initializationer
{
    public static void InitBases(UnityEngine.Vector2 screenSize)
    {
#if UNITY_EDITOR
        // 编辑器退出时销毁
        UnityEditor.EditorApplication.quitting += OnApplicationExit;
#endif
        // 程序退出时按顺序回收, 或做其他必要操作
        UnityEngine.Application.quitting += OnApplicationExit;

        

        if (DataCenter.instance == null)
        {
            DataCenter.instance = new DataCenter();
            DataCenter.instance.LoadData();
        }
        if (InputController.instance == null)
        {
            InputController.instance = new InputController();
            InputController.instance.Init();
        }
        if (EventManager.instance == null)
        {
            EventManager.instance = new EventManager();
        }
        if (AudioController.instance == null)
        {
            AudioController.instance = new AudioController();
        }
        if (MapManager.instance == null)
        {
            MapManager.instance = new MapManager();
        }
        if (PlayerController.instance == null)
        {
            PlayerController.instance = new PlayerController();
        }
        ScreenAdaptator.instance.SetScreenSize(screenSize);
    }

    private static void OnApplicationExit()
    {
        ObjectPool.instance.ClearAll();
    }
}
