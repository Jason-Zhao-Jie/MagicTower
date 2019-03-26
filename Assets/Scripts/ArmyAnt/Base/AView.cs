namespace ArmyAnt.Base
{

    public abstract class AView : UnityEngine.MonoBehaviour
    {
        public AView(IController controller)
        {
            Controller = controller;
        }

        public IController Controller { get; set; }
    }

}
