
public class GameDataInitializer
{
    public GameDataInitializer()
    {
        if (Config == null)
        {
            Config = new ConfigCenter();
        }
        if (RuntimeData == null)
        {
            RuntimeData = new RuntimeDataCenter();
        }
    }

    public ConfigCenter Config
    {
        get; private set;
    }

    public RuntimeDataCenter RuntimeData
    {
        get; private set;
    }
}

