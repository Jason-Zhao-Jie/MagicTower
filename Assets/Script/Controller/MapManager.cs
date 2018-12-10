using System.Collections.Generic;
public class MapManager {
    public static MapManager instance = null;

    public void SetStartData(int floorId = 0, Constant.MapData[] datas = null) {
        if (datas != null) {
            MapData = datas;
        } else {
            MapData = new Constant.MapData[DataCenter.instance.mapLength];
        }
        currentFloor = floorId;
    }

    public bool ShowMap() {
        return ShowMap(currentFloor + 1);
    }

    public bool ShowMap(int mapid) {
        currentFloor = mapid - 1;
        if (currentFloor < 0)
            return false;
        if (MapData == null)
            MapData = new Constant.MapData[DataCenter.instance.mapLength];

        // 清除地图块，并载入新的地图
        ClearMap();
        if (MapData[currentFloor] == null)
            MapData[currentFloor] = DataCenter.instance.GetCopiedMap(currentFloor);
        for (int x = 0; x < MapData[currentFloor].mapBlocks.Length; ++x)
            for (int y = 0; y < MapData[currentFloor].mapBlocks[x].Length; ++y) {
                var thingId = MapData[currentFloor].mapBlocks[x][y].thing;
                AddObjectToMap(x, y, thingId);
            }

        // 以渐变的方式改变背景图和背景音乐, 更改地图名字标识   ( TODO : 未实现渐变方式 )
        if (MainScene.instance != null) {
            MainScene.instance.BackgroundImage = DataCenter.instance.modals[MapData[currentFloor].backThing].prefabPath;
            MainScene.instance.MapName = StringInternational.GetValue(MapData[currentFloor].mapName);
            AudioController.instance.PlayMusicLoop(MapData[currentFloor].music);
        } else {
            DataEditorScene.instance.BackgroundImage = DataCenter.instance.modals[MapData[currentFloor].backThing].prefabPath;
        }

        return true;
    }

    // 清除地图上的一切物块, 不能单独调用, 必须紧跟其他重刷map的操作
    public void ClearMap() {
        var vs = new List<long>(modals.Keys);
        for (int i = 0; i < vs.Count; ++i) {
            modals[vs[i]].RemoveSelf(false);
        }
        modals.Clear();
    }

    // 用地图数据覆盖某地图层, 一般用于地图编辑器, 但也可用于某些事件技巧, 注意仅仅只改变了数据
    public void OverrideMapData(int mapid, Constant.MapData mapdata) {
        MapData[mapid] = mapdata;
    }

    // 更改背景图片
    public void ChangeBack(string prefab) {
        if (MainScene.instance != null)
            MainScene.instance.BackgroundImage = prefab;
        else
            DataEditorScene.instance.BackgroundImage = prefab;
    }

    // 显示黑色幕布, 以便执行一些操作, 例如换楼层
    public void ShowLoadingCurtain(Constant.EmptyCallBack cb = null) {
        Curtain.StartShow(cb);
    }

    // 更改或移动指定处的物品及数据
    public void ChangeThingOnMap(int thingId, int posx, int posy, int oldPosx = -1, int oldPosy = -1) {
        if (oldPosx >= 0 && oldPosy >= 0)
            UnityEngine.GameObject.Find("MapPanel").transform.Find("MapBlock_" + oldPosx + "_" + oldPosy).GetComponent<UnityEngine.SpriteRenderer>().sprite = UnityEngine.Resources.Load<UnityEngine.GameObject>(Constant.PREFAB_DIR + DataCenter.instance.modals[MapData[currentFloor].mapBlocks[oldPosx][oldPosy].thing].prefabPath).GetComponent<UnityEngine.SpriteRenderer>().sprite;
        if (MapData[currentFloor].mapBlocks[posx][posy].thing == thingId)
            return;
        MapData[currentFloor].mapBlocks[posx][posy].thing = thingId;
        long uuid = MapData[currentFloor].mapId * 10000 + posy + posx * 100;
        if (modals.ContainsKey(uuid)) {
            RemoveThingOnMapWithModal(uuid);
        }
        AddObjectToMap(posx, posy, thingId);
    }

    // 在指定处添加物品,仅添加表现, 必须同时配合添加数据的操作, 而且本函数也不检测原先是否已有物品
    public void AddObjectToMap(int posx, int posy, int thingId) {
        Modal obj = null;
        if (thingId > 0) {
            var modal = DataCenter.instance.modals[thingId];
            obj = ObjectPool.instance.GetAnElement<Modal>(modal.id, ObjectPool.ElementType.Sprite, Constant.PREFAB_DIR + modal.prefabPath);
            obj.InitWithMapPos(MapData[currentFloor].mapId, (sbyte)posx, (sbyte)posy, modal);
        }
        if (obj != null) {
            obj.name = "MapBlock_" + posx.ToString() + "_" + posy.ToString();
            if (MainScene.instance != null)
                MainScene.instance.AddObjectToMap(obj.gameObject, posx, posy, -2);
            else if (DataEditorScene.instance != null)
                DataEditorScene.instance.AddObjectToMap(obj.gameObject, posx, posy, -2);
        }
    }

    // 从地图上永久删除mod的信息, 仅数据
    public void RemoveThingOnMap(int posx, int posy, int mapId = -1) {
        if (mapId < 0)
            mapId = currentFloor;
        else
            mapId--;
        if (MapData[mapId] == null)
            MapData[mapId] = DataCenter.instance.GetCopiedMap(mapId);
        MapData[mapId].mapBlocks[posx][posy].thing = 0;
    }

    // 从地图上永久删除mod,包括表现和数据
    public void RemoveThingOnMapWithModal(long uuid) {
        modals[uuid].RemoveSelf();
    }
    public void RemoveThingOnMapWithModal(int posx, int posy, int mapId = -1) {
        if (mapId < 0)
            mapId = currentFloor;
        else
            mapId--;
        long uuid = mapId * 10000 + posx * 100 + posy;
        modals[uuid].RemoveSelf();
    }

    // 更改指定地点的event
    public bool SetEventOn(int eventId, long eventData, int posx, int posy, int mapId = 0) {
        if (mapId <= 0)
            mapId = currentFloor;
        else
            mapId--;
        if (MapData[mapId] == null)
            MapData[mapId] = DataCenter.instance.GetCopiedMap(mapId);
        MapData[mapId].mapBlocks[posx][posy].eventId = eventId;
        MapData[mapId].mapBlocks[posx][posy].eventData = eventData;
        return true;
    }

    public bool RemoveEventOn(int posx, int posy, int mapId = 0) {
        return SetEventOn(0, 0, posx, posy, mapId);
    }

    public Constant.MonsterData GetMonsterDataByUuid(long uuid) {
        var modId = modals[uuid].ModId;
        return DataCenter.instance.monsters[modId].Clone();
    }

    public Modal GetModalByUuid(long uuid) {
        if (modals.ContainsKey(uuid))
            return modals[uuid];
        return null;
    }

    public bool AddMod(long uuid, Modal mod) {
        if (modals.ContainsKey(uuid))
            return false;
        modals.Add(uuid, mod);
        return true;
    }

    public void RemoveMod(long uuid) {
        modals.Remove(uuid);
    }

    public int[][] ConvertCurrentMapToFinderArray() {
        var ret = new List<int[]>();
        var mapBlockData = MapData[currentFloor].mapBlocks;
        foreach (var i in mapBlockData) {
            var inserted = new List<int>();
            foreach (var i_elem in i) {
                if (i_elem.thing == 0) {
                    inserted.Add(0);
                } else {
                    var thingData = DataCenter.instance.modals[i_elem.thing];
                    switch ((Modal.ModalType)thingData.typeId) {
                        case Modal.ModalType.Walkable:
                            inserted.Add(0);
                            break;
                        case Modal.ModalType.MapBlock:
                            inserted.Add(9);    // TODO : 是否需要处理门? 目前这种写法不能自动通过门
                            break;
                        case Modal.ModalType.Item:
                            inserted.Add(2);
                            break;
                        case Modal.ModalType.Npc:
                            inserted.Add(9);
                            break;
                        case Modal.ModalType.Monster:
                            inserted.Add(4);
                            break;
                        case Modal.ModalType.Player:
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

    public Constant.MapData[] MapData { get; private set; }
    public Constant.MapData this[int index] { get { return MapData[index]; } }
    public Constant.MapData CurrentMap { get { return MapData[currentFloor]; } }
    public int CurrentFloorId { get { return MapData[currentFloor].mapId; } }

    private Curtain Curtain {
        get {
            if (MainScene.instance != null)
                return MainScene.instance.Curtain;
            else if (DataEditorScene.instance != null)
                return DataEditorScene.instance.Curtain;
            return null;
        }
    }

    private int currentFloor = 0;
    private Dictionary<long, Modal> modals = new Dictionary<long, Modal>();
}
