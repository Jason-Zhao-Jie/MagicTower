using System.Collections.Generic;

namespace ArmyAnt.ViewUtil {

    public class ObjectPool {
        public const int SPRITE_DEFAULT_SORTING_ORDER = 0;
        public const int SPRITE_IN_DIALOG_SORTING_ORDER = 2;
        public const int HITTER_IN_DIALOG_SORTING_ORDER = 3;

        public enum ElementType : short {
            Unknown,
            Sprite,
            Image,
            Player,
        }
        public abstract class AViewUnit : UnityEngine.MonoBehaviour {
            public int Id {
                get {
                    return id;
                }
                protected set {
                    id = value;
                }
            }

            public bool Destroyed {
                get {
                    return destroyed || (null == this);
                }
            }

            private void OnDestroy() {
                destroyed = true;
            }

            public abstract ElementType GetPoolTypeId();
            public abstract bool OnCreate<T>(ElementType tid, int elemId, T data, params object[] para);
            public abstract void OnInit<T>(ElementType tid, int elemId, T data, params object[] para);
            public abstract bool OnUnuse(ElementType tid, int elemId);

            internal bool OnCreateId<T>(ElementType tid, int elemId, T data, params object[] para) {
                Id = elemId;
                return OnCreate(tid, elemId, data, para);
            }

            public long GetPoolUuid() {
                return id + (long)GetPoolTypeId() * 0x0100000000;
            }

            internal bool usingTag = true;
            private int id = 0;
            private bool destroyed = false;
        }

        public void ClearAll() {
            foreach(var k in unusePool) {
                if(k.Value != null) {
                    foreach(var v in k.Value) {
                        if(v != null && v.gameObject != null) {
                            UnityEngine.Object.Destroy(v.gameObject);
                        }
                    }
                }
                k.Value.Clear();
            }
            unusePool.Clear();
        }

        public T GetAnElement<T, T_Data>(int id, ElementType typeid, UnityEngine.GameObject prefab, T_Data data, params object[] para) where T : AViewUnit {
            var uuid = id + (long)(typeid) * 0x0100000000;
            // 找到目标对象队列, 如不存在, 新建
            Queue<AViewUnit> tar;
            if(unusePool.ContainsKey(uuid)) {
                tar = unusePool[uuid];
            } else {
                tar = new Queue<AViewUnit>();
                unusePool.Add(uuid, tar);
            }
            // 从队列中取出可用的对象, 如无可用, 新创建
            T ret;
            if(tar.Count <= 0) {
                var ret_obj = UnityEngine.Object.Instantiate(prefab);
                if(ret_obj == null)
                    return null;
                ret = ret_obj.GetComponent<T>();
                if(ret == null)
                    return null;
                if(!ret.OnCreateId(typeid, id, data, para))
                    return null;
            } else {
                var deq = tar.Dequeue();
                if(deq.GetComponent<T>() == null) {
                    tar.Enqueue(deq);
                    return null;
                }
                deq.gameObject.SetActive(true);
                ret = deq.GetComponent<T>();
                ret.OnInit(typeid, id, data, para);
            }
            // 返回对象
            ret.usingTag = true;
            return ret;
        }

        public bool RecycleAnElement<T>(T tarElem) where T : AViewUnit {
            if(!tarElem.usingTag)
                return false;
            var uuid = tarElem.GetPoolUuid();
            // 找到目标对象队列, 如不存在, 新建
            Queue<AViewUnit> tar;
            if(unusePool.ContainsKey(uuid)) {
                tar = unusePool[uuid];
            } else {
                tar = new Queue<AViewUnit>();
                unusePool.Add(uuid, tar);
            }
            if(!tarElem.OnUnuse(tarElem.GetPoolTypeId(), tarElem.Id))
                return false;
            tarElem.usingTag = false;
            if(!tarElem.Destroyed) {
                tarElem.transform.SetParent(null, false);
                tar.Enqueue(tarElem);
                tarElem.gameObject.SetActive(false);
            }
            return true;
        }

        private readonly Dictionary<long, Queue<AViewUnit>> unusePool = new Dictionary<long, Queue<AViewUnit>>();
    }

}