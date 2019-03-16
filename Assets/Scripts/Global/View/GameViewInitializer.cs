
public class GameViewInitializer
{
    public GameViewInitializer(UnityEngine.Vector2 screenSize)
    {
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
    }

    public void OnViewExit()
    {
        ObjPool.ClearAll();
    }

    public ScreenAdaptator ScreenAdaptorInst
    {
        get; private set;
    }

    public ObjectPool ObjPool
    {
        get; private set;
    }
}
