using UnityEngine;
using System.Collections;

public class DataCenter : MonoBehaviour
{
    public static DataCenter instance = null;

    public enum EGameStatus
    {
        Start,
        InGame,
        OnMiddleLoading,
        OnBattle,
        OnTipChat,
        OnDialog,
        OnCG,
        OnSmallGame,
    }

    [System.Serializable]
    public struct MapBlock
    {
        public int thing;
        public int eventId;
    }

    [System.Serializable]
    public struct MapData
    {
        public int mapId;
        public string mapName;
        public string backgroundImage;
        public int backgroundAudio;
        public MapBlock[][] mapBlocks;
    }

    [System.Serializable]
    public class ModalData
    {
        public int id;
        public int typeId;
        public string name;
        public string prefabPath;
        public int eventId;
    }

    [System.Serializable]
    public class Audio
    {
        public int id;
        public string path;
    }

    [System.Serializable]
    public class MonsterData
    {
        public int id;
        public int level;
        public int exp;
        public int life;
        public int attack;
        public int defense;
        public int speed;
        public double critical;
        public int gold;
        public int[] special;
    }

    [System.Serializable]
    public struct PlayerData
    {
        public int id;
        public int level;
        public int exp;
        public int life;
        public int attack;
        public int defense;
        public int speed;
        public double critical;
        public int gold;
        public int yellowKey;
        public int blueKey;
        public int redKey;
        public int greenKey;
    }

    [System.Serializable]
    public class Event
    {
        public int id;
        public int typeId;
        public long dataId;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public MapData[] NewGameMaps { get { return data.newGameMaps; } }
    public ModalData[] Modals { get { return data.modals; } }
    public ModalData GetModalById(int id)
    {
        for (int i = 0; i < data.modals.Length; ++i)
            if (data.modals[i].id == id)
                return data.modals[i];
        return null;
    }
    public Audio[] Audios { get { return data.audios; } }
    public string GetAudioById(int id)
    {
        for (int i = 0; i < data.audios.Length; ++i)
            if (data.audios[i].id == id)
                return data.audios[i].path;
        return "";
    }
    public MonsterData[] Monsters { get { return data.monsters; } }
    public MonsterData GetMonsterDataById(int id)
    {
        for (int i = 0; i < data.monsters.Length; ++i)
            if (data.monsters[i].id == id)
                return data.monsters[i];
        return null;
    }
    public PlayerData[] Players { get { return data.players; } }
    public PlayerData GetPlayerDataById(int id)
    {
        for (int i = 0; i < data.players.Length; ++i)
            if (data.players[i].id == id)
                return data.players[i];
        return new PlayerData();
    }
    public Event[] Events { get { return data.events; } }
    public Event GetEventDataById(int id)
    {
        for (int i = 0; i < data.events.Length; ++i)
            if (data.audios[i].id == id)
                return data.events[i];
        return null;
    }

    /// <summary>
    /// When the game loading begins, the data should be load and the instance should be initialized
    /// This function is used to replace the "Start" function
    /// </summary>
    public void LoadData()
    {
        status = EGameStatus.Start;

        // Reading json config files:
        var txt = Resources.Load<TextAsset>("GameData");
        //data = JsonUtility.FromJson<GameData>(txt.text);
        data = GetGameDataFromJsonString(txt.text);

    }

    public EGameStatus Status
    {
        get { return status; }
        set { status = value; }
    }

    public static string GetRuntimeDataJson(){
        var saver = new JObject();
        saver["player"] = GetJsonOfPlayerData(PlayerController.instance.data);
		var pos = new JObject();
        pos["x"] = new JNumber(PlayerController.instance.posx);
        pos["y"] = new JNumber(PlayerController.instance.posy);
        pos["mapId"] = new JNumber(MapManager.instance.CurrentFloorId);
        saver["pos"] = pos;
        saver["maps"] = GetJsonOfMapData(MapManager.instance.MapData);
        return saver.String;
    }

    public static bool LoadGameDataFromJson(string json){
        var jo = JObject.isThis(json) as JObject;
        if (jo == null || !jo.ContainsKey("player") || !jo.ContainsKey("pos") || !jo.ContainsKey("maps"))
            return false;
        var pos = jo["pos"] as JObject;
        if (pos == null)
            return false;
        PlayerController.instance.PlayerData = GetPlayerDataOfJson(jo["player"] as JObject);
        MapManager.instance.SetData(pos["mapId"].ToInt(), GetMapDataOfJson(jo["maps"] as JArray));
		PlayerController.instance.posx = pos["x"].ToInt();
		PlayerController.instance.posy = pos["y"].ToInt();
        return true;
    }

    [System.Serializable]
    private class GameData
    {
        internal MapData[] newGameMaps;
        internal ModalData[] modals;
        internal Audio[] audios;
        internal MonsterData[] monsters;
        internal PlayerData[] players;
        internal Event[] events;
    }

    private EGameStatus status;
    private GameData data;

    private static GameData GetGameDataFromJsonString(string json)
    {
        var _root = ArmyAntJson.Create(json) as JObject;
        var data = new GameData();

        // parse map data
        data.newGameMaps = GetMapDataOfJson(_root["newGameMaps"] as JArray);

        // parse modal data
        var _modals = _root["modals"] as JArray;
        data.modals = new ModalData[_modals.Length];
        for (var i = 0; i < _modals.Length; ++i)
        {
            var __oneModal = _modals[i] as JObject;
            data.modals[i] = new ModalData();
            var __modalData = data.modals[i];
            __modalData.id = __oneModal["id"].ToInt();
            __modalData.typeId = __oneModal["typeId"].ToInt();
            __modalData.name = __oneModal["name"].ToString();
            __modalData.prefabPath = __oneModal["prefabPath"].ToString();
            __modalData.eventId = __oneModal["eventId"].ToInt();
        }

        // parse audios
        var _audios = _root["audios"] as JArray;
        data.audios = new Audio[_audios.Length];
        for (var i = 0; i < _audios.Length; ++i)
        {
            var __oneAudio = _audios[i] as JObject;
            data.audios[i] = new Audio();
            var __audioData = data.audios[i];
            __audioData.id = __oneAudio["id"].ToInt();
            __audioData.path = __oneAudio["path"].ToString();
        }

        // parse monster data
        var _monsters = _root["monsters"] as JArray;
        data.monsters = new MonsterData[_monsters.Length];
        for (var i = 0; i < _monsters.Length; ++i)
        {
            var __oneMonster = _monsters[i] as JObject;
            data.monsters[i] = new MonsterData();
            var __monsterData = data.monsters[i];
            __monsterData.id = __oneMonster["id"].ToInt();
            __monsterData.level = __oneMonster["level"].ToInt();
            __monsterData.exp = __oneMonster["exp"].ToInt();
            __monsterData.life = __oneMonster["life"].ToInt();
            __monsterData.attack = __oneMonster["attack"].ToInt();
            __monsterData.defense = __oneMonster["defense"].ToInt();
            __monsterData.speed = __oneMonster["speed"].ToInt();
            __monsterData.critical = __oneMonster["critical"].ToFloat();
            __monsterData.gold = __oneMonster["gold"].ToInt();

            var __specials = __oneMonster["special"] as JArray;
            __monsterData.special = new int[__specials.Length];
            for (var n = 0; n < __specials.Length; ++n)
            {
                __monsterData.special[n] = __specials[i].ToInt();
            }
        }

        // parse player data
        var _players = _root["players"] as JArray;
		data.players = new PlayerData[_players.Length];
		for (var i = 0; i < _players.Length; ++i)
		{
            data.players[i] = GetPlayerDataOfJson(_players[i] as JObject);
		}

        // parse event data
        var _events = _root["events"] as JArray;
        data.events = new Event[_events.Length];
        for (var i = 0; i < _events.Length; ++i)
        {
            var __oneEvent = _events[i] as JObject;
            data.events[i] = new Event();
            var __eventData = data.events[i];
            __eventData.id = __oneEvent["id"].ToInt();
            __eventData.typeId = __oneEvent["typeId"].ToInt();
            __eventData.dataId = System.Convert.ToInt64(__oneEvent["dataId"].ToFloat());
        }

        //


        return data;
    }

    private static string GetJsonStringFromGameData(GameData data){
        var json = new JObject();

        // Set map data
        json["newGameMaps"] = GetJsonOfMapData(data.newGameMaps);

        // Set modal data
        var modals = new JArray();
        for (int i = 0; i < data.modals.Length;++i){
			var _oneModal = new JObject();
			_oneModal["id"] = new JNumber(data.modals[i].id);
            _oneModal["typeId"] = new JNumber(data.modals[i].typeId);
            _oneModal["prefabPath"] = new JString(data.modals[i].prefabPath);
			_oneModal["eventId"] = new JNumber(data.modals[i].eventId);
            _oneModal["name"] = new JString(data.modals[i].name);
            modals.Add(_oneModal);
        }
        json["modals"] = modals;

        // Set audios
        var audios = new JArray();
        for (int i = 0; i < data.audios.Length;++i){
            var _oneAudio = new JObject();
            _oneAudio["id"] = new JNumber(data.audios[i].id);
            _oneAudio["path"] = new JString(data.audios[i].path);
            audios.Add(_oneAudio);
        }
        json["audios"] = audios;

        // Set monster data
        var monsters = new JArray();
        for (int i = 0; i < data.monsters.Length;++i){
			var _oneMonster = new JObject();
			_oneMonster["id"] = new JNumber(data.monsters[i].id);
			_oneMonster["level"] = new JNumber(data.monsters[i].level);
			_oneMonster["exp"] = new JNumber(data.monsters[i].exp);
			_oneMonster["life"] = new JNumber(data.monsters[i].life);
			_oneMonster["attack"] = new JNumber(data.monsters[i].attack);
            _oneMonster["defense"] = new JNumber(data.monsters[i].defense);
			_oneMonster["speed"] = new JNumber(data.monsters[i].speed);
            _oneMonster["critical"] = new JNumber(data.monsters[i].critical);
            _oneMonster["gold"] = new JNumber(data.monsters[i].gold);
            var _special = new JArray();
            for (int n = 0; n < data.monsters[i].special.Length;++n){
                _special.Add(new JNumber(data.monsters[i].special[n]));
            }
            _oneMonster["special"] = _special;
            monsters.Add(_oneMonster);
        }
        json["monsters"] = monsters;

        // Set player data
        var players = new JArray();
        for (int i = 0; i < data.players.Length;++i){
            players.Add(GetJsonOfPlayerData(data.players[i]));
        }
        json["players"] = players;

        // Set event data
        var events = new JArray();
        for (int i = 0; i < data.events.Length;++i){
			var _oneEvent = new JObject();
			_oneEvent["id"] = new JNumber(data.events[i].id);
			_oneEvent["typeId"] = new JNumber(data.events[i].typeId);
            _oneEvent["dataId"] = new JNumber(data.events[i].dataId);
            events.Add(_oneEvent);
        }
        json["events"] = events;

        return json.String;
    }

    private static JArray GetJsonOfMapData(MapData[] data)
    {
        var ret = new JArray();
        for (var i = 0; i < data.Length; ++i)
        {
            var ob = new JObject();
            ob["mapId"] = new JNumber(data[i].mapId);
            ob["mapName"] = new JString(data[i].mapName);
            ob["backgroundImage"] = new JString(data[i].backgroundImage);
            ob["backgroundAudio"] = new JNumber(data[i].backgroundAudio);
            var blocks = new JArray();
            for (int x = 0; x < data[i].mapBlocks.Length; ++x)
            {
                var blockX = new JArray();
                for (int y = 0; y < data[i].mapBlocks[x].Length; ++y)
                {
                    var blockY = new JObject();
                    blockY["eventId"] = new JNumber(data[i].mapBlocks[x][y].eventId);
                    blockY["thing"] = new JNumber(data[i].mapBlocks[x][y].thing);
                    blockX.Add(blockY);
                }
                blocks.Add(blockX);
            }
            ob["mapBlocks"] = blocks;
            ret.Add(ob);
        }
        return ret;
    }

	private static MapData[] GetMapDataOfJson(JArray json)
	{
		var ret = new MapData[json.Length];
		for (var i = 0; i < json.Length; ++i)
		{
			var __oneMap = json[i] as JObject;
			ret[i].mapId = __oneMap["mapId"].ToInt();
			ret[i].mapName = __oneMap["mapName"].ToString();
			ret[i].backgroundImage = __oneMap["backgroundImage"].ToString();
			ret[i].backgroundAudio = __oneMap["backgroundAudio"].ToInt();
			var __mapBlocks = __oneMap["mapBlocks"] as JArray;
			ret[i].mapBlocks = new MapBlock[__mapBlocks.Length][];
			for (var x = 0; x < __mapBlocks.Length; ++x)
			{
				var ___mapBlocksX = __mapBlocks[x] as JArray;
				ret[i].mapBlocks[x] = new MapBlock[___mapBlocksX.Length];
				for (var y = 0; y < ___mapBlocksX.Length; ++y)
				{
					var ____mapBlockY = ___mapBlocksX[y] as JObject;
					ret[i].mapBlocks[x][y].eventId = ____mapBlockY["eventId"].ToInt();
					ret[i].mapBlocks[x][y].thing = ____mapBlockY["thing"].ToInt();
				}
			}
		}
        return ret;
    }

    private static JObject GetJsonOfPlayerData(PlayerData data)
	{
		var player = new JObject();
		player["id"] = new JNumber(data.id);
		player["level"] = new JNumber(data.level);
		player["exp"] = new JNumber(data.exp);
		player["life"] = new JNumber(data.life);
		player["attack"] = new JNumber(data.attack);
		player["defense"] = new JNumber(data.defense);
		player["speed"] = new JNumber(data.speed);
		player["critical"] = new JNumber(data.critical);
		player["gold"] = new JNumber(data.gold);
		player["yellowKey"] = new JNumber(data.yellowKey);
		player["blueKey"] = new JNumber(data.blueKey);
		player["redKey"] = new JNumber(data.redKey);
		player["greenKey"] = new JNumber(data.greenKey);
        return player;
    }

    private static PlayerData GetPlayerDataOfJson(JObject json)
    {
        var player = new PlayerData();
        player.id = json["id"].ToInt();
        player.level = json["level"].ToInt();
        player.exp = json["exp"].ToInt();
        player.life = json["life"].ToInt();
        player.attack = json["attack"].ToInt();
        player.defense = json["defense"].ToInt();
        player.speed = json["speed"].ToInt();
        player.critical = json["critical"].ToFloat();
        player.gold = json["gold"].ToInt();
        player.yellowKey = json["yellowKey"].ToInt();
        player.blueKey = json["blueKey"].ToInt();
        player.redKey = json["redKey"].ToInt();
        player.greenKey = json["greenKey"].ToInt();
        return player;
    }
}
