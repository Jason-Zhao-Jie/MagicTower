using System.Collections.Generic;
public class MapManager
{
    public static MapManager instance = null;

    public void SetData(int floorId = 0, Constant.MapData[] datas = null)
    {
        maps = new Constant.MapData[DataCenter.instance.data.MapLength];
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
            maps[currentFloor] = DataCenter.instance.data.GetCopiedMap(currentFloor);
        for (int x = 0; x < maps[currentFloor].mapBlocks.Length; ++x)
            for (int y = 0; y < maps[currentFloor].mapBlocks[x].Length; ++y)
            {
                var thingId = maps[currentFloor].mapBlocks[x][y].thing;
                AddObjectToMap(x, y, thingId);
            }

        // 以渐变的方式改变背景图和背景音乐, 更改地图名字标识
        if (MainScene.instance != null)
        {
            MainScene.instance.BackgroundImage = DataCenter.instance.GetModalById(maps[currentFloor].backThing).prefabPath;
            MainScene.instance.MapName = maps[currentFloor].mapName;
            AudioController.instance.PlayMusicLoop(maps[currentFloor].music);
        }
        else
        {
            DataEditorScene.instance.BackgroundImage = DataCenter.instance.GetModalById(maps[currentFloor].backThing).prefabPath;
        }

        return true;
    }

    public void ClearMap()
    {
        var vs = new List<long>(modals.Keys);
        for (int i = 0; i < vs.Count; ++i)
        {
            modals[vs[i]].RemoveSelf(false);
        }
        modals.Clear();
    }

    public void ChangeBack(string prefab)
    {
        if (MainScene.instance != null)
            MainScene.instance.BackgroundImage = prefab;
        else
            DataEditorScene.instance.BackgroundImage = prefab;
    }

    public void ChangeThingOnMap(int thingId, int posx, int posy, int oldPosx = -1, int oldPosy = -1)
    {
        if (oldPosx >= 0 && oldPosy >= 0)
            UnityEngine.GameObject.Find("MapPanel").transform.Find("MapBlock_" + oldPosx + "_" + oldPosy).GetComponent<UnityEngine.SpriteRenderer>().sprite = UnityEngine.Resources.Load<UnityEngine.GameObject>(Constant.PREFAB_DIR + DataCenter.instance.GetModalById(maps[currentFloor].mapBlocks[oldPosx][oldPosy].thing).prefabPath).GetComponent<UnityEngine.SpriteRenderer>().sprite;
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

    public void AddObjectToMap(int posx, int posy, int thingId)
    {
        UnityEngine.GameObject obj = null;
        if (thingId > 0)
        {
            var modal = DataCenter.instance.GetModalById(thingId);
            obj = UnityEngine.Object.Instantiate(UnityEngine.Resources.Load<UnityEngine.GameObject>(Constant.PREFAB_DIR + modal.prefabPath));
            var cmp = obj.GetComponent<Modal>();
            cmp.InitWithMapPos(maps[currentFloor].mapId, (sbyte)posx, (sbyte)posy, modal);
        }
        if (obj != null)
        {
            obj.name = "MapBlock_" + posx.ToString() + "_" + posy.ToString();
            if (MainScene.instance != null)
                MainScene.instance.AddObjectToMap(obj, posx, posy, -2);
            else if (DataEditorScene.instance != null)
                DataEditorScene.instance.AddObjectToMap(obj, posx, posy, -2);
        }
    }

    // 从地图上永久删除mod的信息. 
    public void RemoveThingOnMap(int posx, int posy, int mapId = -1)
    {
        if (mapId < 0)
            mapId = currentFloor;
        else
            mapId--;
        if (maps[mapId] == null)
            maps[mapId] = DataCenter.instance.data.GetCopiedMap(mapId);
        maps[mapId].mapBlocks[posx][posy].thing = 0;
    }

    // 从地图上永久删除mod,用uuid
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

    public void ChangeEventOnMap(int posx, int posy, int eventId = 0, int eventBlockData = 0, int mapId = -1)
    {
        if (mapId < 0)
            mapId = currentFloor;
        else
            mapId--;
        if (maps[mapId] == null)
            maps[mapId] = DataCenter.instance.data.GetCopiedMap(mapId);
        maps[mapId].mapBlocks[posx][posy].eventId = eventId;
        maps[mapId].mapBlocks[posx][posy].eventData = eventBlockData;
    }

    public static UnityEngine.Rect GetMapPosition(UnityEngine.RectTransform mapPanel)
    {
        var totalWidth = mapPanel.rect.width;
        var totalHeight = mapPanel.rect.height;
        bool isHorizenFull = totalWidth >= totalHeight;
        var finalX = isHorizenFull ? ((totalWidth - totalHeight) / 2) : 0;
        var finalY = isHorizenFull ? 0 : ((totalHeight - totalWidth) / 2);

        return new UnityEngine.Rect(finalX, finalY, totalWidth - 2 * finalX, totalHeight - 2 * finalY);
    }

    public bool SetEventOn(int eventId, int posx, int posy, int floorId = -1)
    {
        if (floorId < 0)
            floorId = currentFloor;
        maps[floorId].mapBlocks[posx][posy].eventId = eventId;
        return true;
    }

    public bool RemoveEventOn(int posx, int posy, int floorId = -1)
    {
        return SetEventOn(0, posx, posy, floorId);
    }

    public Constant.MonsterData GetMonsterDataByUuid(long uuid)
    {
        var modId = modals[uuid].ModId;
        return DataCenter.instance.GetMonsterDataById(modId).Clone();
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

    private int currentFloor;
    private Constant.MapData[] maps;
    private Dictionary<long, Modal> modals = new Dictionary<long, Modal>();
}
