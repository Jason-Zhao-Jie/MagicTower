using UnityEngine;
using ArmyAnt.ViewUtil;

namespace MagicTower.Components.Unit
{
    public enum ModalType
    {
        Unknown,
        Walkable,
        MapBlock,
        Item,
        Npc,
        Monster,
        Player,
        SendingBlock,
    }

    public class Modal : ObjectPool.AViewUnit
    {
        const double RUN_STATE_DELAY = 0.4;

        public override ObjectPool.ElementType GetPoolTypeId()
        {
            return ObjectPool.ElementType.Sprite;
        }

        public string PrefabPath
        {
            get
            {
                return Game.Config.modals[modId].prefabPath;
            }
        }

        public override string ResourcePath
        {
            get
            {
                return GetResourcePath(modId);
            }
        }

        public Sprite BaseSprite
        {
            get
            {
                return GetResourceBaseSprite(modId);
            }
        }

        public static string GetResourcePath(int modId)
        {
            return GetResourcePath(Game.Config.modals[modId].prefabPath);
        }

        public static string GetResourcePath(string prefabPath)
        {
            return Model.Dirs.PREFAB_DIR + prefabPath;
        }

        public static Sprite GetResourceBaseSprite(int modId)
        {
            var prefab = Resources.Load<GameObject>(GetResourcePath(modId));
            return prefab.GetComponent<SpriteRenderer>().sprite;
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

        public void InitWithMapPos(int mapId, sbyte posx, sbyte posy, Model.ModalData data)
        {
            modId = data.id;
            typeId = data.typeId;
            modName = data.name;
            eventId = data.eventId;
            eventData = null;
            if (data.eventData != null)
            {
                eventData = new long[data.eventData.Length];
                for (var i = 0; i < eventData.Length; ++i)
                {
                    eventData[i] = data.eventData[i];
                }
            }
            this.mapId = mapId;
            this.posx = posx;
            this.posy = posy;
            uuid = mapId * 10000 + posy + posx * 100;
            Game.Map.AddMod(uuid, this);
        }

        public void GoToRunState(Model.EmptyCallBack dCB = null)
        {
            if (Animator.enabled)
                return;
            destroyCallBack = dCB;
            Animator.enabled = true;
            Animator.Play("KeyEvent");
            timeBeforeRemove = 0;
        }

        public void RemoveSelf(bool callManager = true)
        {
            if (callManager)
                Game.Map.RemoveThingOnMap(posx, posy, mapId);
            Game.Map.RemoveMod(uuid);
            Game.ObjPool.RecycleAnElement(this);
        }

        public void RemoveSelf(Model.EmptyCallBack dCB)
        {
            destroyCallBack = dCB;
            RemoveSelf();
        }

        // Use this for initialization
        void Start()
        {
            if (Animator != null && Animator.GetCurrentAnimatorStateInfo(0).IsName("KeyEvent"))
                Animator.enabled = false;

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
            destroyCallBack?.Invoke();
            destroyCallBack = null;
        }

        public override bool RecycleSelf()
        {
            return Game.ObjPoolRecycleSelf(this);
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

        public Animator Animator { get { return GetComponent<Animator>(); } }
        public int ModId { get { return modId; } }
        public int MapId { get { return mapId; } }
        public int PosX { get { return posx; } }
        public int PosY { get { return posy; } }
        public int EventId { get { return eventId; } }
        public long[] EventData { get { return eventData; } }


        private int eventId = 0;
        private long[] eventData = { 0 };
        private sbyte posx = -1;
        private sbyte posy = -1;
        private int mapId = 0;
        private int typeId = 0;
        private string modName = "";
        private int modId = 0;
        private long uuid = 0;
        private double timeBeforeRemove = -10;
        Model.EmptyCallBack destroyCallBack = null;
    }

}