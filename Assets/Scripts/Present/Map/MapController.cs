using System.Collections.Generic;
public class MapController : AController<MapData, MapView>
{
    public MapController(MapView mapPanel)
    {
        InitDataAndView(new MapData(this), mapPanel);
        View.Controller = this;
    }

    public void SetStartData(int mapId = 1, Constant.MapData[] datas = null)
    {
        Data.ClearMapData(mapId, datas);
    }

    public bool ShowMap()
    {
        if (Data.MapId == 0)
        {
            return ShowMap(1);
        }
        else
        {
            return ShowMap(Data.MapId);
        }
    }

    public bool ShowMap(int mapid)
    {
        var mapdata = Data.ToMap(mapid);
        // 清除地图块，并载入新的地图
        ClearMap();
        for (int x = 0; x < mapdata.blocks.Length; ++x)
            for (int y = 0; y < mapdata.blocks[x].Length; ++y)
            {
                var thingId = mapdata.blocks[x][y].thing;
                AddObjectToMap(x, y, thingId);
            }

        // 以渐变的方式改变背景图和背景音乐, 更改地图名字标识   ( TODO : 未实现渐变方式 )
        View.BackgroundImage = Game.Config.modals[mapdata.backThing].prefabPath;
        if (Game.CurrentScene.Type == AScene.SceneType.MainScene)
        {
            Game.Managers.Audio.PlayMusicLoop(mapdata.music);
            Game.Player.SyncPlayerData();
        }

        return true;
    }

    public void ClickMap(UnityEngine.Vector2 touchedPos)
    {
        View.OnMapClicked(touchedPos);
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

    public Dictionary<int, Constant.MapData> GetAllMapData()
    {
        return Data.GetAllMapData();
    }

    // 用地图数据覆盖某地图层, 一般用于地图编辑器, 但也可用于某些事件技巧, 注意仅仅只改变了数据
    public void OverrideMapData(int mapid, Constant.MapData mapdata)
    {
        Data.ChangeMapData(mapid, mapdata);
    }

    // 更改背景图片
    public void ChangeBack(string prefab)
    {
        View.BackgroundImage = prefab;
    }

    // 显示黑色幕布, 以便执行一些操作, 例如换楼层
    public void ShowLoadingCurtain(Constant.EmptyBoolCallBack cb, params Constant.EmptyBoolCallBack[] hidecbs)
    {
        var lastStatus = Game.Status;
        Game.Status = Constant.EGameStatus.OnMiddleLoading;
        Curtain.StartShow(cb, hidecbs);
    }

    public void HideLoadingCurtain(Constant.EGameStatus status)
    {
        Game.Status = Constant.EGameStatus.OnMiddleLoading;
        Curtain.StartHide(() =>
        {
            Game.Status = status;
            return false;
        });
    }

    // 更改指定地点的event
    public bool SetEventOn(int eventId, long[] eventData, int posx, int posy, int mapId = 0)
    {
        return Data.SetEventOn(eventId, eventData, posx, posy, mapId);
    }

    public bool RemoveEventOn(int posx, int posy, int mapId = 0)
    {
        return Data.RemoveEventOn(posx, posy, mapId);
    }

    // 更改或移动指定处的物品及数据
    public void ChangeThingOnMap(int thingId, int posx, int posy, int oldPosx = -1, int oldPosy = -1)
    {
        if (oldPosx >= 0 && oldPosy >= 0)
            UnityEngine.GameObject.Find("MapPanel").transform.Find("MapBlock_" + oldPosx + "_" + oldPosy).GetComponent<UnityEngine.SpriteRenderer>().sprite = UnityEngine.Resources.Load<UnityEngine.GameObject>(Constant.PREFAB_DIR + Game.Config.modals[Data.CurrentMap.blocks[oldPosx][oldPosy].thing].prefabPath).GetComponent<UnityEngine.SpriteRenderer>().sprite;
        if (!Data.ChangeThingOnMap(thingId, posx, posy, oldPosx, oldPosy))
            return;
        long uuid = Data.MapId * 10000 + posy + posx * 100;
        if (modals.ContainsKey(uuid))
        {
            RemoveThingOnMapWithModal(uuid);
        }
        AddObjectToMap(posx, posy, thingId);
    }

    // 在指定处添加物品,仅添加表现, 必须同时配合添加数据的操作, 而且本函数也不检测原先是否已有物品
    public void AddObjectToMap(int posx, int posy, int thingId, int posz = -15)
    {
        RemoveThingOnMapWithModal(posx, posy);
        if (thingId > 0)
        {
            var modal = Game.Config.modals[thingId];
            Modal obj = Game.ObjPool.GetAnElement<Modal>(modal.id, ObjectPool.ElementType.Sprite, Constant.PREFAB_DIR + modal.prefabPath);
            obj.InitWithMapPos(Data.MapId, (sbyte)posx, (sbyte)posy, modal);
            obj.name = "MapBlock_" + posx.ToString() + "_" + posy.ToString();
            AddObjectToMap(obj.gameObject, posx, posy, posz);
        }
    }

    public void AddObjectToMap(UnityEngine.GameObject gameObject, int posx, int posy, int posz = -15)
    {
        View.AddObjectToMap(gameObject, posx, posy, posz);
    }

    // 从地图上永久删除mod的信息, 仅数据
    public void RemoveThingOnMap(int posx, int posy, int mapId = 0)
    {
        if (mapId <= 0)
            mapId = Data.MapId;
        var block = Data.GetMapData(mapId).blocks[posx][posy];
        block.thing = 0;
        Data.GetMapData(mapId).blocks[posx][posy] = block;
    }

    // 从地图上永久删除mod,包括表现和数据
    public void RemoveThingOnMapWithModal(long uuid)
    {
        if (modals.ContainsKey(uuid))
        {
            modals[uuid].RemoveSelf();
        }
    }
    public void RemoveThingOnMapWithModal(int posx, int posy, int mapId = 0)
    {
        if (mapId <= 0)
            mapId = Data.MapId;
        long uuid = mapId * 10000 + posx * 100 + posy;
        if (modals.ContainsKey(uuid))
        {
            modals[uuid].RemoveSelf();
        }
    }

    public Constant.MonsterData GetMonsterDataByUuid(long uuid)
    {
        var modId = modals[uuid].ModId;
        return Game.Config.monsters[modId].Clone();
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

    public int[][] ConvertCurrentMapToFinderArray()
    {
        var ret = new List<int[]>();
        var mapBlockData = Data.CurrentMap.blocks;
        foreach (var i in mapBlockData)
        {
            var inserted = new List<int>();
            foreach (var i_elem in i)
            {
                if (i_elem.thing == 0)
                {
                    inserted.Add(0);
                }
                else
                {
                    var thingData = Game.Config.modals[i_elem.thing];
                    switch ((Modal.ModalType)thingData.typeId)
                    {
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
                        case Modal.ModalType.SendingBlock:
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

    public Curtain Curtain
    {
        get
        {
            return View.curtain;
        }
    }

    public int MapId { get { return Data.MapId; } }
    public Constant.MapData CurrentMap { get { return Data.CurrentMap; } }
    public int MapsCount { get { return Data.MapsCount; } }

    public UnityEngine.Vector2 ModalLocalScale { get { return View.BlockSize; } }

    private Dictionary<long, Modal> modals = new Dictionary<long, Modal>();
}
