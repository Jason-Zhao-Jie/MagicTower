using System.Collections.Generic;

public class ObjectPool {
    public enum ElementType : short
    {
        Unknown,
        Sprite,
        Hitter,
        Dialog,
        Other,
    }
    public abstract class AElement : UnityEngine.MonoBehaviour
    {
        public AElement()
        {}
        

        public int Id {
            get {
                return id;
            }
            protected set {
                id = value;
            }
        }

        private void OnDestroy()
        {
            
        }

        public abstract ElementType GetPoolTypeId();
        public abstract string ResourcePath { get; }
        public abstract bool OnCreate(ElementType tid, int elemId, string resourcePath);
        public abstract void OnReuse(ElementType tid, int elemId);
        public abstract bool OnUnuse(ElementType tid, int elemId);

        public abstract bool RecycleSelf();
        public bool RecycleSelf<T>() where T:AElement
        {
            return instance.RecycleAnElement((T)this);
        }

        public long GetPoolUuid()
        {
            return id + (long)GetPoolTypeId() * 0x0100000000;
        }

        internal bool usingTag = true;
        private int id = 0;
    }

    public static ObjectPool instance = new ObjectPool();
    ObjectPool() { }

    public void ClearAll()
    {
        unusePool.Clear();
    }

    public T GetAnElement<T>(int id, ElementType typeid, string resourcePath) where T : AElement
    {
        var uuid = id + (long)(typeid) * 0x0100000000;
        // 找到目标对象队列, 如不存在, 新建
        Queue<AElement> tar = null;
        if (unusePool.ContainsKey(uuid))
        {
            tar = unusePool[uuid];
        }
        else
        {
            tar = new Queue<AElement>();
            unusePool.Add(uuid, tar);
        }
        // 从队列中取出可用的对象, 如无可用, 新创建
        T ret = null;
        if(tar.Count <= 0)
        {
            var ret_obj = UnityEngine.Object.Instantiate(UnityEngine.Resources.Load<UnityEngine.GameObject>(resourcePath));
            if (ret_obj == null)
                return null;
            ret = ret_obj.GetComponent<T>();
            if (ret == null)
                return null;
            if (!ret.OnCreate(typeid, id, resourcePath))
                return null;
        }
        else
        {
            var deq = tar.Dequeue();
            if(deq.GetComponent<T>() == null)
            {
                tar.Enqueue(deq);
                return null;
            }
            ret = deq.GetComponent<T>();
            ret.OnReuse(typeid, id);
        }
        // 返回对象
        ret.usingTag = true;
        return ret;
    }

    public bool RecycleAnElement<T>(T tarElem) where T : AElement
    {
        if (!tarElem.usingTag)
            return false;
        var uuid = tarElem.GetPoolUuid();
        // 找到目标对象队列, 如不存在, 新建
        Queue<AElement> tar = null;
        if (unusePool.ContainsKey(uuid))
        {
            tar = unusePool[uuid];
        }
        else
        {
            tar = new Queue<AElement>();
            unusePool.Add(uuid, tar);
        }
        if (!tarElem.OnUnuse(tarElem.GetPoolTypeId(), tarElem.Id))
            return false;
        tarElem.gameObject.SetActive(false);
        tarElem.transform.SetParent(null, false);
        tarElem.usingTag = false;
        tar.Enqueue(tarElem);
        return true;
    }
    
    private Dictionary<long, Queue<AElement>> unusePool = new Dictionary<long, Queue<AElement>>();
}
