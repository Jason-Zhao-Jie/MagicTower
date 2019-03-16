

using System.Collections.Generic;

public class MapData : AData
{
    public MapData(MapController controller) : base(controller)
    {

    }

    public Constant.MapData GetMapData(int id)
    {
        if (!mapdata.ContainsKey(id))
            mapdata.Add(id, Game.Data.Config.GetCopiedMap(id - 1));
        return mapdata[id];
    }

    public bool ChangeMapData(int id, Constant.MapData data)
    {
        if (data == null)
        {
            return false;
        }
        if (mapdata.ContainsKey(id))
            mapdata.Remove(id);
        mapdata.Add(id, data);
        return true;
    }

    public void ClearMapData(int mapId = 0, Constant.MapData[] newData = null)
    {
        mapdata = new Dictionary<int, Constant.MapData>();
        if (newData != null)
        {
            foreach (var i in newData)
            {
                mapdata.Add(i.mapId, i);
            }
        }
        MapId = mapId;
    }

    public Constant.MapData ToMap(int mapId)
    {
        MapId = mapId;
        return CurrentMap;
    }

    // 更改或移动指定处的数据
    public bool ChangeThingOnMap(int thingId, int posx, int posy, int oldPosx = -1, int oldPosy = -1)
    {
        if (CurrentMap.blocks[posx][posy].thing == thingId)
            return false;
        var block = CurrentMap.blocks[posx][posy];
        block.thing = thingId;
        CurrentMap.blocks[posx][posy] = block;
        return true;
    }

    // 更改指定地点的event
    public bool SetEventOn(int eventId, long eventData, int posx, int posy, int mapId = 0)
    {
        if (mapId <= 0)
            mapId = MapId;
        var block = GetMapData(mapId).blocks[posx][posy];
        block.eventId = eventId;
        block.eventData = eventData;
        GetMapData(mapId).blocks[posx][posy] = block;
        return true;
    }

    public bool RemoveEventOn(int posx, int posy, int mapId = 0)
    {
        return SetEventOn(0, 0, posx, posy, mapId);
    }

    public Dictionary<int, Constant.MapData> GetAllMapData()
    {
        return mapdata;
    }

    public int MapId { get; private set; }
    public Constant.MapData CurrentMap { get { return GetMapData(MapId); } }
    public int MapsCount { get { return Game.Data.Config.MapsCount; } }

    private Dictionary<int, Constant.MapData> mapdata = new Dictionary<int, Constant.MapData>();
}

