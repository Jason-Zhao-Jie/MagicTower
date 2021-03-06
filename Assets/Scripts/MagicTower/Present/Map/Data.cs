﻿using System.Collections.Generic;

namespace MagicTower.Present.Map
{

    public class Data : ArmyAnt.Base.AData
    {
        public Data(Controller controller) : base(controller)
        {

        }

        public Model.MapData GetMapData(int id)
        {
            if (!mapdata.ContainsKey(id))
                mapdata.Add(id, Game.Config.GetCopiedMap(id - 1));
            return mapdata[id];
        }

        public bool ChangeMapData(int id, Model.MapData data)
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

        public void ClearMapData(int mapId, Model.MapData[] newData = null)
        {
            mapdata = new Dictionary<int, Model.MapData>();
            if (newData != null)
            {
                foreach (var i in newData)
                {
                    mapdata.Add(i.mapId, i);
                }
            }
            MapId = mapId;
        }

        public Model.MapData ToMap(int mapId)
        {
            MapId = mapId;
            return CurrentMap;
        }

        // 更改指定处的数据
        public bool ChangeThingOnMap(int thingId, int posx, int posy)
        {
            if (CurrentMap.blocks[posx][posy].thing == thingId)
                return false;
            var block = CurrentMap.blocks[posx][posy];
            block.thing = thingId;
            CurrentMap.blocks[posx][posy] = block;
            return true;
        }

        // 更改指定地点的event
        public bool SetEventOn(int eventId, long[] eventData, int posx, int posy, int mapId = 0)
        {
            if (mapId <= 0)
                mapId = MapId;
            var block = GetMapData(mapId).blocks[posx][posy];
            block.eventId = eventId;
            block.eventData = null;
            if (eventData != null)
            {
                block.eventData = new long[eventData.Length];
                for (var i = 0; i < eventData.Length; ++i)
                {
                    block.eventData[i] = eventData[i];
                }
            }
            GetMapData(mapId).blocks[posx][posy] = block;
            return true;
        }

        public bool RemoveEventOn(int posx, int posy, int mapId = 0)
        {
            return SetEventOn(0, new long[1] { 0 }, posx, posy, mapId);
        }

        public Dictionary<int, Model.MapData> GetAllMapData()
        {
            return mapdata;
        }

        public int MapId { get; private set; }
        public Model.MapData CurrentMap { get { return GetMapData(MapId); } }
        public int MapsCount { get { return mapdata.Count; } }

        private Dictionary<int, Model.MapData> mapdata = new Dictionary<int, Model.MapData>();
    }

}
