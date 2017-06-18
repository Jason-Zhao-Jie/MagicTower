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

    public bool ShowMap(int floorId = 0)
    {
        if (floorId != 0)
        {
            currentFloor = floorId;
        }
        if (currentFloor < 0)
            return false;

        // 清除地图块，并载入新的地图
        ClearMap();
        if (maps[currentFloor] == null)
            maps[currentFloor] = DataCenter.instance.data.GetCopiedMap(currentFloor);
        for (int x = 0; x < maps[currentFloor].mapBlocks.Length; ++x)
            for (int y = 0; y < maps[currentFloor ].mapBlocks[x].Length; ++y)
            {
                UnityEngine.GameObject obj = null;
                long uuid = maps[currentFloor ].mapId * 10000 + y + x * 100;
                if (ModalManager.Contains(uuid))
                    obj = ModalManager.GetObjectByUuid(uuid);
                else
                {
                    var thingId = maps[currentFloor].mapBlocks[x][y].thing;
                    if (thingId > 0)
                    {
                        var modal = DataCenter.instance.GetModalById(thingId);
                    obj = UnityEngine.Object.Instantiate(UnityEngine.Resources.Load<UnityEngine.GameObject>(Constant.PREFAB_DIR +  modal.prefabPath));
                        var cmp = obj.GetComponent<Modal>();
                        cmp.InitWithMapPos(maps[currentFloor].mapId, (sbyte)x, (sbyte)y, modal);
                    }
                }
                if (obj != null)
                {
                    obj.name = "MapBlock_" + x.ToString() + "_" + y.ToString();
                    if (MainScene.instance != null)
                        MainScene.instance.AddObjectToMap(obj, x, y, -2);
                    else if (DataEditorScene.instance != null)
                        DataEditorScene.instance.AddObjectToMap(obj, x, y, -2);
                }
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
        if (MainScene.instance != null)
            MainScene.instance.transform.Find("MapPanel").transform.DetachChildren();
        if (DataEditorScene.instance != null)
            UnityEngine.GameObject.Find("MapPanel").transform.BroadcastMessage("RemoveSelf");
    }

	public void ChangeBack(string prefab)
	{
		if (MainScene.instance != null)
            MainScene.instance.BackgroundImage =  prefab;
        else
            DataEditorScene.instance.BackgroundImage = prefab;
	}

    public void ChangeOneBlock(string prefab, int posx, int posy, int oldPosx = -1, int oldPosy = -1)
    {
        if (oldPosx >= 0 && oldPosy >= 0)
            UnityEngine.GameObject.Find("MapPanel").transform.Find("MapBlock_" + oldPosx + "_" + oldPosy).GetComponent<UnityEngine.SpriteRenderer>().sprite = UnityEngine.Resources.Load<UnityEngine.GameObject>(Constant.PREFAB_DIR + DataCenter.instance.GetModalById(maps[currentFloor].mapBlocks[oldPosx][oldPosy].thing).prefabPath).GetComponent<UnityEngine.SpriteRenderer>().sprite;
        if (UnityEngine.GameObject.Find("MapPanel").transform.Find("MapBlock_" + posx + "_" + posy) == null)
        {
            UnityEngine.GameObject obj = null;
            long uuid = maps[currentFloor].mapId * 10000 + posy + posx * 100;
            if (ModalManager.Contains(uuid))
                obj = ModalManager.GetObjectByUuid(uuid);
            else
            {
                var thingId = maps[currentFloor].mapBlocks[posx][posy].thing;
                if (thingId > 0)
                {
                    var modal = DataCenter.instance.GetModalById(thingId);
                    obj = UnityEngine.Object.Instantiate(UnityEngine.Resources.Load<UnityEngine.GameObject>(Constant.PREFAB_DIR + modal.prefabPath));
                    var cmp = obj.GetComponent<Modal>();
                    cmp.InitWithMapPos(maps[currentFloor].mapId, (sbyte)posx, (sbyte)posy, modal);
                }
            }
            if (obj != null)
            {
                obj.name = "MapBlock_" + posx + "_" + posy;
                if (MainScene.instance != null)
                    MainScene.instance.AddObjectToMap(obj, posx, posy, -2);
                else if (DataEditorScene.instance != null)
                    DataEditorScene.instance.AddObjectToMap(obj, posx, posy, -2);
            }
        }
        else
            UnityEngine.GameObject.Find("MapPanel").transform.Find("MapBlock_" + posx + "_" + posy).GetComponent<UnityEngine.SpriteRenderer>().sprite = UnityEngine.Resources.Load<UnityEngine.GameObject>(Constant.PREFAB_DIR + prefab).GetComponent<UnityEngine.SpriteRenderer>().sprite;
    }

    public void ChangeThingOnMap(int posx, int posy, int thingId = 0, int mapId = -1)
    {
        if (mapId < 0)
            mapId = currentFloor;
        else
            mapId--;
        if (maps[mapId] == null)
            maps[mapId] = DataCenter.instance.data.GetCopiedMap(mapId);
        maps[mapId].mapBlocks[posx][posy].thing = thingId;
    }

    public void ChangeEventOnMap(int posx, int posy, int eventId = 0, int mapId = -1)
    {
        if (mapId < 0)
            mapId = currentFloor;
        else
            mapId--;
        if (maps[mapId] == null)
            maps[mapId] = DataCenter.instance.data.GetCopiedMap(mapId);
        maps[mapId].mapBlocks[posx][posy].eventId = eventId;
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

    public bool RemoveEventOn(int posx, int posy, int floorId = -1){
        return SetEventOn(0, posx, posy, floorId);
    }

    public Constant.MapData[] MapData { get { return maps; }}
    public Constant.MapData this[int index] { get { return maps[index]; } }
    public Constant.MapData CurrentMap { get { return maps[currentFloor]; } }
    public int CurrentFloorId{ get { return maps[currentFloor].mapId; }}

    private int currentFloor;
private Constant.MapData[] maps;
}
