
public interface IController {

}

public abstract class AController<TData, TView> : IController where TData : AData where TView : AView
{
    public AController()
    {
    }

    protected void InitDataAndView(TData data, TView view)
    {
        Data = data;
        View = view;
    }

    protected TData Data { get; set; }
    protected TView View { get; set; }
}

