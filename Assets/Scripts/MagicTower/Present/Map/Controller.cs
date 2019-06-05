using System.Collections.Generic;
using MagicTower.Components.Unit;

namespace MagicTower.Present.Map {

    public class Controller : ArmyAnt.Base.AController<Data, View> {
        public Controller(View mapPanel) {
            InitDataAndView(new Data(this), mapPanel);
            View.Controller = this;
        }

        public void SetStartData(int mapId, Model.MapData[] datas) {
            Data.ClearMapData(mapId, datas);
            ShowMap(mapId);
        }

        public bool ShowMap(int mapid) {
            // 清除地图块
            ClearMap();
            // 载入新的地图数据
            var mapdata = Data.ToMap(mapid);
            // 绘制新地图
            for(int x = 0; x < mapdata.blocks.Length; ++x) {
                for(int y = 0; y < mapdata.blocks[x].Length; ++y) {
                    var thingId = mapdata.blocks[x][y].thing;
                    AddObjectToMap(x, y, thingId);
                }
            }
            // 以渐变的方式改变背景图和背景音乐, 更改地图名字标识   ( TODO : 未实现渐变方式 )
            View.BackgroundImage = Game.Config.modals[mapdata.backThing].prefabPath;
            Manager.AudioManager.PlayMusicLoop(mapdata.music);
            Game.Player.SyncPlayerData();

            return true;
        }

        public void ClickMap(UnityEngine.Vector2 touchedPos) {
            View.OnMapClicked(touchedPos);
        }

        // 清除地图上的一切物块, 仅清除View, 不清除数据, 用于刷新地图
        private void ClearMap() {
            var vs = new List<long>(modals.Keys);
            foreach(var i in vs) {
                Game.ObjPool.RecycleAnElement(modals[i]);
            }
            modals.Clear();
        }

        public Dictionary<int, Model.MapData> GetAllMapData() {
            return Data.GetAllMapData();
        }

        // 用地图数据覆盖某地图层, 一般用于地图编辑器, 但也可用于某些事件技巧, 注意仅仅只改变了数据
        public void OverrideMapData(int mapid, Model.MapData mapdata) {
            Data.ChangeMapData(mapid, mapdata);
        }

        // 更改背景图片
        public void ChangeBack(string prefab) {
            View.BackgroundImage = prefab;
        }

        // 显示黑色幕布, 以便执行一些操作, 例如换楼层
        public void ShowCurtain(Curtain.ContentType type, Model.EmptyBoolCallBack cb, params Model.EmptyBoolCallBack[] hidecbs) {
            var lastStatus = Game.Status;
            Game.Status = Model.EGameStatus.OnMiddleLoading;
            View.curtain.StartShow(type, cb, hidecbs);
        }

        public void HideLoadingCurtain() {
            View.curtain.StartHide(null);
        }

        public void HideLoadingCurtain(Model.EGameStatus status) {
            Game.Status = Model.EGameStatus.OnMiddleLoading;
            View.curtain.StartHide(() => {
                Game.Status = status;
                return false;
            });
        }

        // 更改指定地点的event
        public bool SetEventOn(int eventId, long[] eventData, int posx, int posy, int mapId = 0) {
            return Data.SetEventOn(eventId, eventData, posx, posy, mapId);
        }

        public bool RemoveEventOn(int posx, int posy, int mapId = 0) {
            return Data.RemoveEventOn(posx, posy, mapId);
        }

        // 更改或移动指定处的物品及数据
        public void ChangeThingOnMap(int thingId, int posx, int posy) {
            if(Data.ChangeThingOnMap(thingId, posx, posy)) {
                RemoveThingOnMap(posx, posy, MapId);
                AddObjectToMap(posx, posy, thingId);
            }
        }

        // 在指定处添加物品,仅添加表现, 必须同时配合添加数据的操作, 而且本函数也不检测原先是否已有物品
        private void AddObjectToMap(int posx, int posy, int thingId, int posz = 0) {
            if(thingId > 0) {
                var modal = Game.Config.modals[thingId];
                Modal obj = Game.ObjPool.GetAnElement<Modal, Model.ModalData>(1, ArmyAnt.ViewUtil.ObjectPool.ElementType.Sprite, Game.ModalSprite, modal);
                obj.SetMapPosition(Data.MapId, posx, posy);
                View.AddObjectToMap(obj.gameObject, posx, posy, posz);
                modals.Add(Modal.GetUuid(MapId, posx, posy), obj.gameObject.GetComponent<Modal>());
                obj.name = "MapBlock_" + posx.ToString() + "_" + posy.ToString();
            }
        }

        public void AddPlayerToMap(Components.Unit.Player player, int posx, int posy) {
            View.AddObjectToMap(player.gameObject, posx, posy, -2);
        }

        // 从地图上永久删除mod的信息
        public void RemoveThingOnMap(long uuid) {
            if(modals.ContainsKey(uuid)) {
                RemoveThingOnMap(modals[uuid].PosX, modals[uuid].PosY, modals[uuid].MapId);
            }
        }

        // 从地图上永久删除mod的信息
        private void RemoveThingOnMap(int posx, int posy, int mapId) {
            if(mapId <= 0)
                mapId = Data.MapId;
            var block = Data.GetMapData(mapId).blocks[posx][posy];
            block.thing = 0;
            Data.GetMapData(mapId).blocks[posx][posy] = block;
            long uuid = Modal.GetUuid(Data.MapId, posx, posy);
            if(modals.ContainsKey(uuid)) {
                Game.ObjPool.RecycleAnElement(modals[uuid]);
                modals.Remove(uuid);
            }
        }

        public Model.MonsterData GetMonsterDataByUuid(long uuid) {
            var modId = modals[uuid].ModId;
            return Game.Config.monsters[modId].Clone();
        }

        public Modal GetModalByUuid(long uuid) {
            if(modals.ContainsKey(uuid))
                return modals[uuid];
            return null;
        }

        public int[][] ConvertCurrentMapToFinderArray() {
            var ret = new List<int[]>();
            var mapBlockData = Data.CurrentMap.blocks;
            foreach(var i in mapBlockData) {
                var inserted = new List<int>();
                foreach(var i_elem in i) {
                    if(i_elem.thing == 0) {
                        inserted.Add(0);
                    } else {
                        var thingData = Game.Config.modals[i_elem.thing];
                        switch((ModalType)thingData.typeId) {
                            case ModalType.Walkable:
                                inserted.Add(0);
                                break;
                            case ModalType.MapBlock:
                                inserted.Add(9);    // TODO : 是否需要处理门? 目前这种写法不能自动通过门
                                break;
                            case ModalType.Item:
                                inserted.Add(2);
                                break;
                            case ModalType.Npc:
                                inserted.Add(9);
                                break;
                            case ModalType.Monster:
                                inserted.Add(4);
                                break;
                            case ModalType.Player:
                                inserted.Add(9);
                                break;
                            case ModalType.SendingBlock:
                                inserted.Add(9);
                                break;
                            default:
                                inserted.Add(9);
                                break;
                        }
                    }
                }
                ret.Add(inserted.ToArray());
            }
            return ret.ToArray();
        }

        public int MapId { get { return Data.MapId; } }
        public Model.MapData CurrentMap { get { return Data.CurrentMap; } }
        public int MapsCount { get { return Data.MapsCount; } }

        public UnityEngine.Vector2 HitterLocalScale => View.HitterLocalScale;

        private readonly Dictionary<long, Modal> modals = new Dictionary<long, Modal>();
    }

}
