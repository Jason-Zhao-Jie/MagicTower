
public class GameControllerInitializer
{
    public GameControllerInitializer()
    {
        if (Audio == null)
        {
            Audio = new AudioController();
        }
        if (EventMgr == null)
        {
            EventMgr = new EventManager();
        }
        if (Input == null)
        {
            Input = new InputController();
        }
        if (IOD == null)
        {
            IOD = new IODriver();
        }
    }

    public AudioController Audio
    {
        get; private set;
    }

    public EventManager EventMgr
    {
        get; private set;
    }

    public InputController Input
    {
        get; private set;
    }

    public IODriver IOD
    {
        get; private set;
    }
}
