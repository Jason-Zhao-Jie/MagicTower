public class MapManager
{
    public static MapManager instance = null;

    public void SetData(int floorId = 1, Constant.MapData[] datas = null)
    {
        if (datas == null)
        {
            datas = DataCenter.instance.NewGameMaps;
            maps = new Constant.MapData[datas.Length];
            for (int i = 0; i < datas.Length; ++i)
            {
                maps[i] = datas[i];
                maps[i].mapBlocks = new Constant.MapBlock[datas[i].mapBlocks.Length][];
                for (int j = 0; j < maps[i].mapBlocks.Length; ++j)
                {
                    maps[i].mapBlocks[j] = new Constant.MapBlock[datas[i].mapBlocks[j].Length];
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
                UnityEngine.GameObject obj = null;
                long uuid = maps[currentFloorIndex].mapId * 10000 + y + x * 100;
                if (ModalManager.Contains(uuid))
                    obj = ModalManager.GetObjectByUuid(uuid);
                else
                {
                    var thingId = maps[currentFloorIndex].mapBlocks[x][y].thing;
                    if (thingId > 0)
                    {
                        var modal = DataCenter.instance.GetModalById(thingId);
                    obj = UnityEngine.Object.Instantiate(UnityEngine.Resources.Load<UnityEngine.GameObject>(Constant.PREFAB_DIR +  modal.prefabPath));
                        var cmp = obj.GetComponent<Modal>();
                        cmp.InitWithMapPos(maps[currentFloorIndex].mapId, (sbyte)x, (sbyte)y, modal);
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
            MainScene.instance.BackgroundImage = DataCenter.instance.GetModalById(maps[currentFloorIndex].backThing).prefabPath;
			MainScene.instance.MapName = maps[currentFloorIndex].mapName;
			AudioController.instance.PlayMusicLoop(maps[currentFloorIndex].music);
		}
		else
		{
			DataEditorScene.instance.BackgroundImage = DataCenter.instance.GetModalById(maps[currentFloorIndex].backThing).prefabPath;
        }

        return true;
    }

    public void ClearMap()
    {
        if (MainScene.instance != null)
            MainScene.instance.transform.Find("MapPanel").transform.DetachChildren();
        if (DataEditorScene.instance != null)
            UnityEngine.GameObject.Find("MapPanel").transform.DetachChildren();
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
            UnityEngine.GameObject.Find("MapPanel").transform.Find("MapBlock_" + oldPosx + "_" + oldPosy).GetComponent<UnityEngine.SpriteRenderer>().sprite = UnityEngine.Resources.Load<UnityEngine.GameObject>(Constant.PREFAB_DIR + DataCenter.instance.GetModalById(maps[currentFloorIndex].mapBlocks[oldPosx][oldPosy].thing).prefabPath).GetComponent<UnityEngine.SpriteRenderer>().sprite;
        if (UnityEngine.GameObject.Find("MapPanel").transform.Find("MapBlock_" + posx + "_" + posy) == null)
        {
            UnityEngine.GameObject obj = null;
            long uuid = maps[currentFloorIndex].mapId * 10000 + posy + posx * 100;
            if (ModalManager.Contains(uuid))
                obj = ModalManager.GetObjectByUuid(uuid);
            else
            {
                var thingId = maps[currentFloorIndex].mapBlocks[posx][posy].thing;
                if (thingId > 0)
                {
                    var modal = DataCenter.instance.GetModalById(thingId);
                    obj = UnityEngine.Object.Instantiate(UnityEngine.Resources.Load<UnityEngine.GameObject>(Constant.PREFAB_DIR + modal.prefabPath));
                    var cmp = obj.GetComponent<Modal>();
                    cmp.InitWithMapPos(maps[currentFloorIndex].mapId, (sbyte)posx, (sbyte)posy, modal);
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

    public static UnityEngine.Rect GetMapPosition(UnityEngine.RectTransform mapPanel)
    {
        var totalWidth = mapPanel.rect.width;
        var totalHeight = mapPanel.rect.height;
        bool isHorizenFull = totalWidth >= totalHeight;
        var finalX = isHorizenFull ? ((totalWidth - totalHeight) / 2) : 0;
        var finalY = isHorizenFull ? 0 : ((totalHeight - totalWidth) / 2);

        return new UnityEngine.Rect(finalX, finalY, totalWidth - 2 * finalX, totalHeight - 2 * finalY);
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

    public Constant.MapData[] MapData { get { return maps; }}
    public Constant.MapData this[int index] { get { return maps[index]; } }
    public Constant.MapData CurrentMap { get { return maps[currentFloorIndex]; } }
    public int CurrentFloorId{ get { return maps[currentFloorIndex].mapId; }}

    private int currentFloorIndex;
private Constant.MapData[] maps;
}
