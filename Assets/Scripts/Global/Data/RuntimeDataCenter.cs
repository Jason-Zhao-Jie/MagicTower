using System.Collections.Generic;

public class RuntimeDataCenter
{

    public RuntimeDataCenter()
    {
        status = Constant.EGameStatus.Start;
    }

    ////////////// Map Data ///////////////

    [System.Serializable]
    private struct RuntimePositionData
    {
        public int x;
        public int y;
        public int mapId;
    }

    [System.Serializable]
    private class RuntimeGameData
    {
        public Constant.PlayerData player;
        public RuntimePositionData pos;
        public Constant.MapData[] maps;
    }

    public string GetJsonOfRuntimeInfoData()
    {
        var maps = new Constant.MapData[Game.Map.MapsCount];
        int index = 0;
        foreach (var item in Game.Map.GetAllMapData())
        {
            maps[index++] = item.Value;
        }
        return UnityEngine.JsonUtility.ToJson(new RuntimeGameData
        {
            player = Game.Player.PlayerData,
            pos = new RuntimePositionData
            {
                x = Game.Player.PlayerPosX,
                y = Game.Player.PlayerPosY,
                mapId = Game.Map.MapId,
            },
            maps = maps
        }, false);
    }

    public bool LoadRuntimeInfoDataFromJson(string json)
    {
        var data = UnityEngine.JsonUtility.FromJson<RuntimeGameData>(json);
        Game.Player.PlayerData = data.player;
        Game.Map.SetStartData(data.pos.mapId, data.maps);
        Game.Player.PlayerPosX = data.pos.x;
        Game.Player.PlayerPosY = data.pos.y;
        return true;
    }

    public Constant.EGameStatus Status
    {
        get { return status; }
        set
        {
            status = value;
            Game.Controller.Input.OnChangeWalkState();
        }
    }

    private Constant.EGameStatus status;

}

