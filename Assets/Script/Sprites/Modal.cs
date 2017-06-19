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
        this.modId = data.id;
        this.typeId = data.typeId;
        this.modName = data.name;
        this.eventId = data.eventId;
        this.mapId = mapId;
        this.posx = posx;
        this.posy = posy;
        uuid = mapId * 10000 + posy + posx * 100;
		ModalManager.AddMod(uuid, this);

    }

    public void GoToRunState()
    {
        if (animator.enabled)
            return;
        animator.enabled = true;
        animator.Play("KeyEvent");
        timeBeforeRemove = 0;
    }

    public void RemoveSelf(){
        MapManager.instance.ChangeThingOnMap(posx, posy, 0, mapId);
        Destroy(gameObject);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
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

    public Animator animator { get { return GetComponent<Animator>(); } }
    public int ModId { get { return modId; } }

    private int eventId = 0;
    private sbyte posx = -1;
    private sbyte posy = -1;
    private int mapId = 0;
    private int typeId = 0;
    private string modName = "";
    private int modId = 0;
    private long uuid = 0;
    private double timeBeforeRemove = -10;
}
