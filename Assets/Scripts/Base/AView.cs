public abstract class AView
{
    public AView(IController controller)
    {
        Controller = controller;
    }

    protected IController Controller { get; set; }
}

