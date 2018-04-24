using System.Collections.Generic;
public class MapManager
{
    public static MapManager instance = null;

    public void SetData(int floorId = 0, Constant.MapData[] datas = null)
    {
        maps = new Constant.MapData[DataCenter.instance.mapLength];
        if (datas != null)
        {
            maps = datas;
        }
        currentFloor = floorId;
    }

    public bool ShowMap()
    {
        return ShowMap(currentFloor + 1);
    }

    public bool ShowMap(int mapid)
    {
        currentFloor = mapid - 1;
        if (currentFloor < 0)
            return false;

        // 清除地图块，并载入新的地图
        ClearMap();
        if (maps[currentFloor] == null)
            maps[currentFloor] = DataCenter.instance.GetCopiedMap(currentFloor);
        for (int x = 0; x < maps[currentFloor].mapBlocks.Length; ++x)
            for (int y = 0; y < maps[currentFloor].mapBlocks[x].Length; ++y)
            {
                var thingId = maps[currentFloor].mapBlocks[x][y].thing;
                AddObjectToMap(x, y, thingId);
            }

        // 以渐变的方式改变背景图和背景音乐, 更改地图名字标识
        if (MainScene.instance != null)
        {
            MainScene.instance.BackgroundImage = DataCenter.instance.modals[maps[currentFloor].backThing].prefabPath;
            MainScene.instance.MapName = maps[currentFloor].mapName;
            AudioController.instance.PlayMusicLoop(maps[currentFloor].music);
        }
        else
        {
            DataEditorScene.instance.BackgroundImage = DataCenter.instance.modals[maps[currentFloor].backThing].prefabPath;
        }

        return true;
    }

    // 清除地图上的一切物块, 不能单独调用, 必须紧跟其他重刷map的操作
    public void ClearMap()
    {
        var vs = new List<long>(modals.Keys);
        for (int i = 0; i < vs.Count; ++i)
        {
            modals[vs[i]].RemoveSelf(false);
        }
        modals.Clear();
    }

    // 更改背景图片
    public void ChangeBack(string prefab)
    {
        if (MainScene.instance != null)
            MainScene.instance.BackgroundImage = prefab;
        else
            DataEditorScene.instance.BackgroundImage = prefab;
    }

    // 显示黑色幕布, 以便执行一些操作, 例如换楼层
    public void ShowLoadingCurtain(Constant.EmptyCallBack cb = null)
    {
        Curtain.StartShow(cb);
    }

    // 更改或移动指定处的物品及数据
    public void ChangeThingOnMap(int thingId, int posx, int posy, int oldPosx = -1, int oldPosy = -1)
    {
        if (oldPosx >= 0 && oldPosy >= 0)
            UnityEngine.GameObject.Find("MapPanel").transform.Find("MapBlock_" + oldPosx + "_" + oldPosy).GetComponent<UnityEngine.SpriteRenderer>().sprite = UnityEngine.Resources.Load<UnityEngine.GameObject>(Constant.PREFAB_DIR + DataCenter.instance.modals[maps[currentFloor].mapBlocks[oldPosx][oldPosy].thing].prefabPath).GetComponent<UnityEngine.SpriteRenderer>().sprite;
        if (maps[currentFloor].mapBlocks[posx][posy].thing == thingId)
            return;
        maps[currentFloor].mapBlocks[posx][posy].thing = thingId;
        long uuid = maps[currentFloor].mapId * 10000 + posy + posx * 100;
        if (modals.ContainsKey(uuid))
        {
            RemoveThingOnMapWithModal(uuid);
        }
        AddObjectToMap(posx, posy, thingId);
    }

    // 在指定处添加物品,仅添加表现, 必须同时配合添加数据的操作, 而且本函数也不检测原先是否已有物品
    public void AddObjectToMap(int posx, int posy, int thingId)
    {
        Modal obj = null;
        if (thingId > 0)
        {
            var modal = DataCenter.instance.modals[thingId];
            obj = ObjectPool.instance.GetAnElement<Modal>(modal.id, ObjectPool.ElementType.Sprite, Constant.PREFAB_DIR + modal.prefabPath);
            obj.InitWithMapPos(maps[currentFloor].mapId, (sbyte)posx, (sbyte)posy, modal);
        }
        if (obj != null)
        {
            obj.name = "MapBlock_" + posx.ToString() + "_" + posy.ToString();
            if (MainScene.instance != null)
                MainScene.instance.AddObjectToMap(obj.gameObject, posx, posy, -2);
            else if (DataEditorScene.instance != null)
                DataEditorScene.instance.AddObjectToMap(obj.gameObject, posx, posy, -2);
        }
    }

    // 从地图上永久删除mod的信息, 仅数据
    public void RemoveThingOnMap(int posx, int posy, int mapId = -1)
    {
        if (mapId < 0)
            mapId = currentFloor;
        else
            mapId--;
        if (maps[mapId] == null)
            maps[mapId] = DataCenter.instance.GetCopiedMap(mapId);
        maps[mapId].mapBlocks[posx][posy].thing = 0;
    }

    // 从地图上永久删除mod,包括表现和数据
    public void RemoveThingOnMapWithModal(long uuid)
    {
        modals[uuid].RemoveSelf();
    }
    public void RemoveThingOnMapWithModal(int posx, int posy, int mapId = -1)
    {
        if (mapId < 0)
            mapId = currentFloor;
        else
            mapId--;
        long uuid = mapId * 10000 + posx * 100 + posy;
        modals[uuid].RemoveSelf();
    }

    // 更改指定地点的event
    public bool SetEventOn(int eventId, long eventData, int posx, int posy, int mapId = 0)
    {
        if (mapId <= 0)
            mapId = currentFloor;
        else
            mapId--;
        if (maps[mapId] == null)
            maps[mapId] = DataCenter.instance.GetCopiedMap(mapId);
        maps[mapId].mapBlocks[posx][posy].eventId = eventId;
        maps[mapId].mapBlocks[posx][posy].eventData = eventData;
        return true;
    }

    public bool RemoveEventOn(int posx, int posy, int mapId = 0)
    {
        return SetEventOn(0, 0, posx, posy, mapId);
    }

    public Constant.MonsterData GetMonsterDataByUuid(long uuid)
    {
        var modId = modals[uuid].ModId;
        return DataCenter.instance.monsters[modId].Clone();
    }

    public Modal GetModalByUuid(long uuid)
    {
        if (modals.ContainsKey(uuid))
            return modals[uuid];
        return null;
    }

    public bool AddMod(long uuid, Modal mod)
    {
        if (modals.ContainsKey(uuid))
            return false;
        modals.Add(uuid, mod);
        return true;
    }

    public void RemoveMod(long uuid)
    {
        modals.Remove(uuid);
    }

    public Constant.MapData[] MapData { get { return maps; } }
    public Constant.MapData this[int index] { get { return maps[index]; } }
    public Constant.MapData CurrentMap { get { return maps[currentFloor]; } }
    public int CurrentFloorId { get { return maps[currentFloor].mapId; } }

    private Curtain Curtain
    {
        get
        {
            if (MainScene.instance != null)
                return MainScene.instance.Curtain;
            else if (DataEditorScene.instance != null)
                return DataEditorScene.instance.Curtain;
            return null;
        }
    }

    private int currentFloor;
    private Constant.MapData[] maps;
    private Dictionary<long, Modal> modals = new Dictionary<long, Modal>();
}
