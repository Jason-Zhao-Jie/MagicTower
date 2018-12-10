using System.Collections.Generic;

public class DataCenter {
    public static DataCenter instance = null;

    public readonly int mapLength;
    private Dictionary<int, Constant.MapData> newGameMaps = new Dictionary<int, Constant.MapData>();

    public readonly Dictionary<int, Constant.ModalData> modals = new Dictionary<int, Constant.ModalData>();
    public readonly Dictionary<int, Constant.Audio> audios = new Dictionary<int, Constant.Audio>();
    public readonly Dictionary<int, Constant.MonsterData> monsters = new Dictionary<int, Constant.MonsterData>();
    public readonly Dictionary<int, Constant.PlayerData> players = new Dictionary<int, Constant.PlayerData>();
    public readonly Dictionary<int, Constant.WeaponData> weapons = new Dictionary<int, Constant.WeaponData>();
    public readonly Dictionary<int, Constant.ChatData> chats = new Dictionary<int, Constant.ChatData>();
    public readonly Dictionary<int, Constant.ChoiceData> choices = new Dictionary<int, Constant.ChoiceData>();
    public readonly Dictionary<int, Constant.LanguageData> languages = new Dictionary<int, Constant.LanguageData>();
    public readonly Dictionary<string, Constant.InternationalString> strings = new Dictionary<string, Constant.InternationalString>();

    public DataCenter() {
        mapLength = UnityEngine.Resources.LoadAll<UnityEngine.TextAsset>(Constant.MAP_DATA_DIR).Length;
    }

    public Constant.MapData GetGameMap(int index) {
        if (!newGameMaps.ContainsKey(index)) {
            string path = Constant.MAP_DATA_DIR;
            if (index < 9)
                path += "00" + (index + 1);
            else if (index < 99)
                path += "0" + (index + 1);
            else
                path += index + 1;
            var asset = UnityEngine.Resources.Load<UnityEngine.TextAsset>(path).text;

            //UnityEngine.Debug.Log("Loading map:" + i + ", Time:" + UnityEngine.Time.realtimeSinceStartup);
            var __oneMap = ArmyAntJson.Create(asset) as JObject;
            newGameMaps[index] = new Constant.MapData {
                Json = __oneMap
            };
        }
        return newGameMaps[index];
    }

    public static Constant.MapData[] GetMapDataOfJson(JArray json) {
        var loadedGameMaps = new List<Constant.MapData>();
        foreach (int k in json) {
            loadedGameMaps[k] = new Constant.MapData {
                Json = json[k] as JObject
            };
        }
        return loadedGameMaps.ToArray();
    }

    public void SaveMapTo(int index) {
        if (!newGameMaps.ContainsKey(index)) {
            UnityEngine.Debug.LogError("Saving map " + index + " failed, cannot find map old data.");
        } else {
            MapManager.instance.OverrideMapData(index, newGameMaps[index]);
        }
    }

    public Constant.MapData GetCopiedMap(int index) {
        var dt = GetGameMap(index);
        var ret = new Constant.MapData {
            backThing = dt.backThing,
            mapId = dt.mapId,
            mapName = dt.mapName,
            music = dt.music,
            mapBlocks = new Constant.MapBlock[dt.mapBlocks.Length][]
        };
        for (var x = 0; x < ret.mapBlocks.Length; ++x) {
            ret.mapBlocks[x] = new Constant.MapBlock[dt.mapBlocks[x].Length];
            for (var y = 0; y < ret.mapBlocks[x].Length; ++y) {
                ret.mapBlocks[x][y].thing = dt.mapBlocks[x][y].thing;
                ret.mapBlocks[x][y].eventId = dt.mapBlocks[x][y].eventId;
                ret.mapBlocks[x][y].eventData = dt.mapBlocks[x][y].eventData;
            }
        }
        return ret;
    }

    /// <summary>
    /// When the game loading begins, the data should be load and the instance should be initialized
    /// This function is used to replace the "Start" function
    /// </summary>
    public void LoadData() {
        status = Constant.EGameStatus.Start;

        // Reading json config files:
        var json = UnityEngine.Resources.Load<UnityEngine.TextAsset>("GameData").text;
        var _root = ArmyAntJson.Create(json) as JObject;

        // modals
        var json_modals = _root["modals"] as JArray;
        modals.Clear();
        for (var i = 0; i < json_modals.Length; ++i) {
            var elem = new Constant.ModalData() { Json = json_modals[i] as JObject };
            modals.Add(elem.id, elem);
        }

        // audios
        var json_audios = _root["audios"] as JArray;
        audios.Clear();
        for (var i = 0; i < json_audios.Length; ++i) {
            var elem = new Constant.Audio() { Json = json_audios[i] as JObject };
            audios.Add(elem.id, elem);
        }

        // monsters
        var json_monsters = _root["monsters"] as JArray;
        monsters.Clear();
        for (var i = 0; i < json_monsters.Length; ++i) {
            var elem = new Constant.MonsterData() { Json = json_monsters[i] as JObject };
            monsters.Add(elem.id, elem);
        }

        // players
        var json_players = _root["players"] as JArray;
        players.Clear();
        for (var i = 0; i < json_players.Length; ++i) {
            var elem = new Constant.PlayerData() { Json = json_players[i] as JObject };
            players.Add(elem.id, elem);
        }

        // weapons
        var json_weapons = _root["weapons"] as JArray;
        weapons.Clear();
        for (var i = 0; i < json_weapons.Length; ++i) {
            var elem = new Constant.WeaponData() { Json = json_weapons[i] as JObject };
            weapons.Add(elem.id, elem);
        }

        // chats
        var json_chats = _root["chats"] as JArray;
        chats.Clear();
        for (var i = 0; i < json_chats.Length; ++i) {
            var elem = new Constant.ChatData() { Json = json_chats[i] as JObject };
            chats.Add(elem.id, elem);
        }

        // choices
        var json_choices = _root["choices"] as JArray;
        choices.Clear();
        for (var i = 0; i < json_choices.Length; ++i) {
            var elem = new Constant.ChoiceData() { Json = json_choices[i] as JObject };
            choices.Add(elem.id, elem);
        }

        // languages
        var json_languages = _root["languages"] as JArray;
        languages.Clear();
        for (var i = 0; i < json_languages.Length; ++i) {
            var elem = new Constant.LanguageData() { Json = json_languages[i] as JObject };
            languages.Add(elem.id, elem);
        }

        // strings
        var json_strings = _root["strings"] as JArray;
        strings.Clear();
        for (var i = 0; i < json_strings.Length; ++i) {
            var elem = new Constant.InternationalString() { Json = json_strings[i] as JObject };
            strings.Add(elem.key, elem);
        }
    }

    public string SaveData() {
        // modals
        var json_modals = new JArray();
        foreach (var i in modals) {
            json_modals.Add(i.Value.Json);
        }

        // audios
        var json_audios = new JArray();
        foreach (var i in audios) {
            json_audios.Add(i.Value.Json);
        }

        // monsters
        var json_monsters = new JArray();
        foreach (var i in monsters) {
            json_monsters.Add(i.Value.Json);
        }

        // players
        var json_players = new JArray();
        foreach (var i in players) {
            json_players.Add(i.Value.Json);
        }

        // weapons
        var json_weapons = new JArray();
        foreach (var i in weapons) {
            json_weapons.Add(i.Value.Json);
        }

        // chats
        var json_chats = new JArray();
        foreach (var i in chats) {
            json_chats.Add(i.Value.Json);
        }

        // choices
        var json_choices = new JArray();
        foreach (var i in choices) {
            json_choices.Add(i.Value.Json);
        }

        // languages
        var json_languages = new JArray();
        foreach (var i in languages) {
            json_languages.Add(i.Value.Json);
        }

        // strings
        var json_strings = new JArray();
        foreach (var i in strings) {
            json_strings.Add(i.Value.Json);
        }

        return new JObject(new Dictionary<string, IUnit>
        {
            {"modals",json_modals },
            {"audios",json_audios },
            {"monsters",json_monsters },
            {"players",json_players },
            {"weapons",json_weapons },
            {"chats",json_chats },
            {"choices",json_choices },
            {"languages",json_languages },
            {"strings",json_strings }
        }).String;
    }

    public Constant.EGameStatus Status {
        get { return status; }
        set {
            status = value;
            InputController.instance.OnChangeWalkState();
        }
    }

    public JObject GetJsonOfRuntimeInfoData() {
        var saver = new JObject {
            ["player"] = PlayerController.instance.PlayerData.Json,
            ["pos"] = new JObject {
                ["x"] = new JNumber(PlayerController.instance.posx),
                ["y"] = new JNumber(PlayerController.instance.posy),
                ["mapId"] = new JNumber(MapManager.instance.CurrentFloorId)
            }
        };
        return saver;
    }
    public Dictionary<int, JObject> GetJsonOfRuntimeMapData() {
        var ret = new Dictionary<int, JObject>();
        for (var i = 0; i < MapManager.instance.MapData.Length; ++i) {
            ret[MapManager.instance.MapData[i].mapId] = MapManager.instance.MapData[i].Json;
        }
        return ret;
    }

    public static bool LoadRuntimeInfoDataFromJson(string json) {
        var jo = JObject.Create(json) as JObject;
        if (jo == null || !jo.ContainsKey("player") || !jo.ContainsKey("pos") || !jo.ContainsKey("maps"))
            return false;
        var pos = jo["pos"] as JObject;
        if (pos == null)
            return false;
        PlayerController.instance.PlayerData = new Constant.PlayerData() { Json = jo["player"] as JObject };
        MapManager.instance.SetStartData(pos["mapId"].ToInt(), GetMapDataOfJson(jo["maps"] as JArray));
        PlayerController.instance.posx = pos["x"].ToInt();
        PlayerController.instance.posy = pos["y"].ToInt();
        return true;
    }

    private Constant.EGameStatus status;

}
