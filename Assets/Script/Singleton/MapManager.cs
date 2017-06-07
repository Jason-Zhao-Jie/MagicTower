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

    public void SetData(int floorId = 1, DataCenter.MapData[] datas = null){
        if (datas == null)
        {
            datas = DataCenter.instance.NewGameMaps;;
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
		currentFloorId = floorId;
    }

    public bool ShowMap(int floorId = 0){
        if (floorId != 0)
			currentFloorId = floorId;
        int index = -1;
        for (int i = 0; i < maps.Length;++i){
            if (maps[i].mapId == currentFloorId)
                index = i;
        }
        if (index < 0)
            return false;
		ClearMap(index);

        for (int x = 0; x < maps[index].mapBlocks.Length; ++x)
            for (int y = 0; y < maps[index].mapBlocks[x].Length; ++y)
            {
                GameObject obj;
                long uuid = currentFloorId * 10000 + y + x * 100;
                if (ModalManager.Contains(uuid))
                    obj = ModalManager.GetObjectByUuid(uuid);
                else
                {
                    var modal = DataCenter.instance.GetModalById(maps[index].mapBlocks[x][y].thing);
                    obj = Instantiate(Resources.Load(modal.prefabPath) as GameObject);
                    var cmp = obj.GetComponent<Modal>();
                    cmp.InitWithMapPos(currentFloorId, (sbyte)x, (sbyte)y, modal);
                }
                obj.name = "MapBlock_" + x.ToString() + "_" + y.ToString();
                MainScene.instance.AddObjectToMap(obj, x, y);
            }

        return true;
    }

	public void ClearMap(int index)
	{
		var Map = MainScene.instance.transform.Find("MapPanel");
		Map.transform.DetachChildren();
    }

    private int currentFloorId;
    private DataCenter.MapData[] maps;
}
