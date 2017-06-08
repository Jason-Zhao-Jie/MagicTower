using UnityEngine;
using System.Collections;

public class MapManager : MonoBehaviour
{
    public static MapManager instance = null;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetData(int floorId = 1, DataCenter.MapData[] datas = null)
    {
        if (datas == null)
        {
            datas = DataCenter.instance.NewGameMaps; ;
            maps = new DataCenter.MapData[datas.Length];
            for (int i = 0; i < datas.Length; ++i)
            {
                maps[i] = datas[i];
                maps[i].mapBlocks = new DataCenter.MapBlock[datas[i].mapBlocks.Length][];
                for (int j = 0; j < maps[i].mapBlocks.Length; ++j)
                {
                    maps[i].mapBlocks[j] = new DataCenter.MapBlock[datas[i].mapBlocks[j].Length];
                    for (int k = 0; k < maps[i].mapBlocks[j].Length; ++k)
                    {
                        maps[i].mapBlocks[j][k] = datas[i].mapBlocks[j][k];
                    }
                }
            }
        }
        else
        {
            maps = datas;
        }
        currentFloorIndex = GetMapIndexByFloorId(floorId);
    }

    public bool ShowMap(int floorId = 0)
    {
        if (floorId != 0)
        {
            var floorIndex = GetMapIndexByFloorId(floorId);
            if (floorIndex < 0)
                return false;
            currentFloorIndex = floorIndex;
        }
        if (currentFloorIndex < 0)
            return false;

        // 清除地图块，并载入新的地图
        ClearMap();
        for (int x = 0; x < maps[currentFloorIndex].mapBlocks.Length; ++x)
            for (int y = 0; y < maps[currentFloorIndex].mapBlocks[x].Length; ++y)
            {
                GameObject obj = null;
                long uuid = maps[currentFloorIndex].mapId * 10000 + y + x * 100;
                if (ModalManager.Contains(uuid))
                    obj = ModalManager.GetObjectByUuid(uuid);
                else
                {
                    var thingId = maps[currentFloorIndex].mapBlocks[x][y].thing;
                    if (thingId > 0)
                    {
                        var modal = DataCenter.instance.GetModalById(thingId);
                        obj = Instantiate(Resources.Load<GameObject>(modal.prefabPath));
                        var cmp = obj.GetComponent<Modal>();
                        cmp.InitWithMapPos(maps[currentFloorIndex].mapId, (sbyte)x, (sbyte)y, modal);
                    }
                }
                if (obj != null)
                {
                    obj.name = "MapBlock_" + x.ToString() + "_" + y.ToString();
                    MainScene.instance.AddObjectToMap(obj, x, y);
                }
            }

        // 以渐变的方式改变背景图和背景音乐, 更改地图名字标识
        MainScene.instance.BackgroundImage = maps[currentFloorIndex].backgroundImage;
        MainScene.instance.MapName = maps[currentFloorIndex].mapName;
        AudioController.instance.PlayMusicLoop(maps[currentFloorIndex].backgroundAudio);

        return true;
    }

    public void ClearMap()
    {
        var Map = MainScene.instance.transform.Find("MapPanel");
        Map.transform.DetachChildren();
    }

    public int GetMapIndexByFloorId(int id)
    {
        for (int i = 0; i < maps.Length; ++i)
        {
            if (maps[i].mapId == id)
                return i;
        }
        return -1;
    }

    public bool SetEventOn(int eventId, int posx, int posy, int floorId = -1)
    {
        var index = GetMapIndexByFloorId(floorId);
        if (floorId < 0)
            index = currentFloorIndex;
        if (index < 0)
            return false;
        maps[index].mapBlocks[posx][posy].eventId = eventId;
        return true;
    }

    public bool RemoveEventOn(int posx, int posy, int floorId = -1){
        return SetEventOn(0, posx, posy, floorId);
    }

    public DataCenter.MapData[] MapData { get { return maps; }}
    public DataCenter.MapData this[int index] { get { return maps[index]; } }
    public DataCenter.MapData CurrentMap { get { return maps[currentFloorIndex]; } }
    public int CurrentFloorId{ get { return maps[currentFloorIndex].mapId; }}

    private int currentFloorIndex;
    private DataCenter.MapData[] maps;
}
