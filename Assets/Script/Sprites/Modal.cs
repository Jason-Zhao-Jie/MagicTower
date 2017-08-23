using UnityEngine;
using System.Collections;

public class Modal : MonoBehaviour
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

    public void GoToRunState()
    {
        if (animator.enabled)
            return;
        animator.enabled = true;
        animator.Play("KeyEvent");
        timeBeforeRemove = 0;
    }

    public void RemoveSelf(bool callManager = true)
    {
        if (callManager)
            MapManager.instance.RemoveThingOnMap(posx, posy, mapId);
        MapManager.instance.RemoveMod(uuid);
        if (gameObject != null)
            Destroy(gameObject);
        else
            Destroy(this);
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
    }

    public Animator animator { get { return GetComponent<Animator>(); } }
    public int ModId { get { return modId; } }
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
}
