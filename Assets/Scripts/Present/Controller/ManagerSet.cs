
public class ManagerSet
{
    public ManagerSet()
    {
        if (Audio == null)
        {
            Audio = new AudioManager();
        }
        if (EventMgr == null)
        {
            EventMgr = new EventManager();
        }
        if (Input == null)
        {
            Input = new InputManager();
        }
        if (IOD == null)
        {
            IOD = new IOManager();
        }
    }

    public AudioManager Audio
    {
        get; private set;
    }

    public EventManager EventMgr
    {
        get; private set;
    }

    public InputManager Input
    {
        get; private set;
    }

    public IOManager IOD
    {
        get; private set;
    }
}
