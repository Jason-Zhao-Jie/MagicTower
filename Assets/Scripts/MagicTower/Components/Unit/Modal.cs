using System.Collections;
using System.Collections.Generic;
using ArmyAnt.ViewUtil;
using UnityEngine;

namespace MagicTower.Components.Unit {
    public enum ModalType {
        Unknown,
        Walkable,
        MapBlock,
        Item,
        Npc,
        Monster,
        Player,
        SendingBlock,
    }

    public enum AnimType {
        Static,
        Once,
        Recycle,
        Player,
        Hitter,
    }

    public class Modal : ObjectPool.AViewUnit {
        const float RECYCLE_TIMER_INIT = 0.3f;
        const float ONCE_TIMER_INIT = 0.1f;
        const float HITTER_TIMER_INIT = 0.08f;

        public override ObjectPool.ElementType GetPoolTypeId() {
            var spc = GetComponent<SpriteRenderer>();
            if(spc != null) {
                return ObjectPool.ElementType.Sprite;
            } else {
                return ObjectPool.ElementType.Image;
            }
        }

        public string PrefabPath => Game.Config.modals[ModId].prefabPath;

        public int TypeId { get; protected set; } = 0;

        public long Uuid { get; private set; } = 0;

        public static long GetUuid(int mapId, int posx, int posy) {
            return mapId * 10000 + posy + posx * 100;
        }

        public AnimType AnimType { get; private set; } = AnimType.Static;

        public void SetMapPosition(int mapId, int posx, int posy) {
            MapId = mapId;
            PosX = posx;
            PosY = posy;
            Uuid = GetUuid(mapId, posx, posy);
        }

        public void GoToRunState(Model.EmptyCallBack dCB = null) {
            destroyCallBack = dCB;
            if(AnimType == AnimType.Once) {
                Sprite = sprites[1];
                animPointer = 1;
            } else {
                Game.Map.RemoveThingOnMap(Uuid);
            }
        }

        // Update is called once per frame
        void Update() {
            timer -= Time.deltaTime;
            if(timer <= 0) {
                InitTimer();
                switch(AnimType) {
                    case AnimType.Once:
                        if(animPointer > 0) {
                            ++animPointer;
                            if(animPointer >= sprites.Length) {
                                Game.Map.RemoveThingOnMap(Uuid);
                            } else {
                                Sprite = sprites[animPointer];
                            }
                        }
                        break;
                    case AnimType.Recycle:
                        ++animPointer;
                        if(animPointer >= sprites.Length) {
                            animPointer = 0;
                        }
                        Sprite = sprites[animPointer];
                        break;
                    case AnimType.Hitter:
                        ++animPointer;
                        if(animPointer >= sprites.Length) {
                            Game.ObjPool.RecycleAnElement(this);
                        } else {
                            Sprite = sprites[animPointer];
                        }
                        break;
                }
            }
        }

        private void OnDestroy() {
            destroyCallBack?.Invoke();
            destroyCallBack = null;
        }

        private void InitTimer(bool more = false) {
            switch(AnimType) {
                case AnimType.Recycle:
                    timer = RECYCLE_TIMER_INIT;
                    break;
                case AnimType.Once:
                    timer = ONCE_TIMER_INIT;
                    if (more)
                    {
                        timer += 0.1f;
                    }
                    break;
                case AnimType.Hitter:
                    timer = HITTER_TIMER_INIT;
                    break;
            }
        }

        public override bool OnCreate<T>(ObjectPool.ElementType tid, int elemId, T data, params object[] para) {
            OnInit(tid, elemId, data, para);            
            return true;
        }

        public override void OnInit<T>(ObjectPool.ElementType tid, int elemId, T data, params object[] para) {
            if(data == null) {
                // 传入null表示清空图片
                AnimType = AnimType.Static;
                Sprite = null;
            } else if(data is Model.ModalData modData) {
                //传入正常modal数据
                ModId = modData.id;
                TypeId = modData.typeId;
                EventId = modData.eventId;
                EventData = null;
                if(modData.eventData != null) {
                    EventData = new long[modData.eventData.Length];
                    for(var i = 0; i < EventData.Length; ++i) {
                        EventData[i] = modData.eventData[i];
                    }
                }

                sprites = Game.GetMods(modData.prefabPath);
                Sprite = sprites[0];
                animPointer = 0;
                switch(modData.animator) {
                    case "static":
                        AnimType = AnimType.Static;
                        break;
                    case "once":
                        AnimType = AnimType.Once;
                        break;
                    case "recycle":
                        AnimType = AnimType.Recycle;
                        break;
                    case "player":
                        AnimType = AnimType.Player;
                        break;
                }
            } else if(data is Model.WeaponData weaponData && para != null && para.Length >= 1 && para[0] is bool crit) {
                //传入weapon数据表示此modal为hitter
                Present.Manager.AudioManager.PlaySound(crit ? weaponData.critAudioId : weaponData.audioId);
                sprites = Game.GetMods(crit ? weaponData.critPrefabPath : weaponData.prefabPath);
                Sprite = sprites[0];
                AnimType = AnimType.Hitter;
            } else if(data is Sprite[] spriteData) {
                AnimType = AnimType.Static;
                Sprite = spriteData[0];
                sprites = spriteData;
            }
            InitTimer(true);
        }

        public override bool OnUnuse(ObjectPool.ElementType tid, int elemId) {
            OnDestroy();
            AnimType = AnimType.Static;
            animPointer = 0;
            sprites = null;
            InitTimer();
            return true;
        }

        public Sprite[] GetSprites() {
            return sprites;
        }

        public int ModId { get; private set; } = 0;
        public int MapId { get; private set; } = 0;
        public int PosX { get; private set; } = -1;
        public int PosY { get; private set; } = -1;
        public int EventId { get; private set; } = 0;
        public long[] EventData { get; private set; } = { 0 };

        public Sprite Sprite {
            get {
                var spc = GetComponent<SpriteRenderer>();
                if(spc != null) {
                    return spc.sprite;
                } else {
                    return GetComponent<UnityEngine.UI.Image>().sprite;
                }
            }
            private set {
                var spc = GetComponent<SpriteRenderer>();
                if(spc != null) {
                    spc.sprite = value;
                } else {
                    var img = GetComponent<UnityEngine.UI.Image>();
                    img.sprite = value;
                    if (img.rectTransform.anchorMin == img.rectTransform.anchorMax)
                    {
                        img.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value.rect.width);
                        img.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value.rect.height);
                    }
                    else
                    {
                        Game.DebugLogWarning("Detected an non-standard modal image !");
                        var pos = img.rectTransform.localPosition;
                        var anc = (img.rectTransform.anchorMin + img.rectTransform.anchorMax) / 2;
                        img.rectTransform.anchorMin = anc;
                        img.rectTransform.anchorMax = anc;
                        img.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value.rect.width);
                        img.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value.rect.height);
                        img.rectTransform.localPosition = pos;
                    }
                }
            }
        }

        private float timer;
        private Sprite[] sprites = null;
        private int animPointer;
        private Model.EmptyCallBack destroyCallBack = null;
    }

}
