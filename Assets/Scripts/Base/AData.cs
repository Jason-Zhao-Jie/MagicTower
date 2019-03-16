
public abstract class AData
{
    public AData(IController controller)
    {
        Controller = controller;
    }

    protected IController Controller { get; set; }
}

