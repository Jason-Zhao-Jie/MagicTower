using UnityEngine;

public class Modal : ObjectPool.AElement
{
    const double RUN_STATE_DELAY = 0.4;
    public enum ModalType{
        Unknown,
        Walkable,
        MapBlock,
        Item,
        Npc,
        Monster,
        Player
    }

    public override ObjectPool.ElementType GetPoolTypeId()
    {
        return ObjectPool.ElementType.Sprite;
    }

    public override string ResourcePath {
        get {
            return Constant.PREFAB_DIR + DataCenter.instance.modals[modId].prefabPath;
        }
    }

    public int TypeId
    {
        get
        {
            return typeId;
        }
        protected set
        {
            typeId = value;
        }
    }
    public long Uuid
    {
        get
        {
            return uuid;
        }
    }

    public void InitWithMapPos(int mapId, sbyte posx, sbyte posy, Constant.ModalData data)
    {
        modId = data.id;
        typeId = data.typeId;
        modName = data.name;
        eventId = data.eventId;
        eventData = data.eventData;
        this.mapId = mapId;
        this.posx = posx;
        this.posy = posy;
        uuid = mapId * 10000 + posy + posx * 100;
        MapManager.instance.AddMod(uuid, this);
    }

    public void GoToRunState(Constant.EmptyCallBack dCB = null)
    {
        if (animator.enabled)
            return;
        destroyCallBack = dCB;
        animator.enabled = true;
        animator.Play("KeyEvent");
        timeBeforeRemove = 0;
    }

    public void RemoveSelf(bool callManager = true)
    {
        if (callManager)
            MapManager.instance.RemoveThingOnMap(posx, posy, mapId);
        MapManager.instance.RemoveMod(uuid);
        ObjectPool.instance.RecycleAnElement(this);
    }

    public void RemoveSelf(Constant.EmptyCallBack dCB) {
        destroyCallBack = dCB;
        RemoveSelf();
    }

    // Use this for initialization
    void Start(){
        if (animator != null && animator.GetCurrentAnimatorStateInfo(0).IsName("KeyEvent"))
            animator.enabled = false;
        
    }
    // Update is called once per frame
    void Update()
    {
        if (timeBeforeRemove > -4)
            timeBeforeRemove += Time.deltaTime;
        if (timeBeforeRemove - RUN_STATE_DELAY > double.Epsilon)
            RemoveSelf();
    }

    private void OnDestroy()
    {
        if (destroyCallBack != null)
            destroyCallBack();
        destroyCallBack = null;
    }

    public override bool RecycleSelf()
    {
        return RecycleSelf<Modal>();
    }

    public override bool OnCreate(ObjectPool.ElementType tid, int elemId, string resourcePath)
    {
        return true;
    }

    public override void OnReuse(ObjectPool.ElementType tid, int elemId)
    {
    }

    public override bool OnUnuse(ObjectPool.ElementType tid, int elemId)
    {
        OnDestroy();
        return true;
    }

    public Animator animator { get { return GetComponent<Animator>(); } }
    public int ModId { get { return modId; } }
    public int MapId { get { return mapId; } }
    public int PosX { get { return posx; } }
    public int PosY { get { return posy; } }
    public int EventId { get { return eventId; } }
    public long EventData { get { return eventData; } }
    

    private int eventId = 0;
    private long eventData = 0;
    private sbyte posx = -1;
    private sbyte posy = -1;
    private int mapId = 0;
    private int typeId = 0;
    private string modName = "";
    private int modId = 0;
    private long uuid = 0;
    private double timeBeforeRemove = -10;
    Constant.EmptyCallBack destroyCallBack = null;
}
