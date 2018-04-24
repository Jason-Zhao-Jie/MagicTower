using UnityEngine;

public static class Initializationer
{
    public static void InitBases(Vector2 screenSize)
    {
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
}
