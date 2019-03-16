using System.Collections.Generic;

/// <summary>
/// 游戏核心数据处理类
/// </summary>
public class ConfigCenter {
    public readonly int mapLength;

    public readonly Dictionary<int, Constant.ModalData> modals = new Dictionary<int, Constant.ModalData>();
    public readonly Dictionary<int, Constant.Audio> audios = new Dictionary<int, Constant.Audio>();
    public readonly Dictionary<int, Constant.MonsterData> monsters = new Dictionary<int, Constant.MonsterData>();
    public readonly Dictionary<int, Constant.PlayerData> players = new Dictionary<int, Constant.PlayerData>();
    public readonly Dictionary<int, Constant.WeaponData> weapons = new Dictionary<int, Constant.WeaponData>();
    public readonly Dictionary<int, Constant.ChatData> chats = new Dictionary<int, Constant.ChatData>();
    public readonly Dictionary<int, Constant.ChoiceData> choices = new Dictionary<int, Constant.ChoiceData>();
    public readonly Dictionary<int, Constant.LanguageData> languages = new Dictionary<int, Constant.LanguageData>();
    public readonly Dictionary<string, Constant.InternationalString> strings = new Dictionary<string, Constant.InternationalString>();

    public ConfigCenter() {
        mapLength = UnityEngine.Resources.LoadAll<UnityEngine.TextAsset>(Constant.MAP_DATA_DIR).Length;

        // Reading json config files:
        var json = UnityEngine.Resources.Load<UnityEngine.TextAsset>("GameData").text;
        gamedata = UnityEngine.JsonUtility.FromJson<GameData>(json);

        // Reset references
        modals.Clear();
        for (var i = 0; i < gamedata.modals.Length; ++i) {
            modals.Add(gamedata.modals[i].id, gamedata.modals[i]);
        }

        audios.Clear();
        for (var i = 0; i < gamedata.audios.Length; ++i) {
            audios.Add(gamedata.audios[i].id, gamedata.audios[i]);
        }

        monsters.Clear();
        for (var i = 0; i < gamedata.monsters.Length; ++i) {
            monsters.Add(gamedata.monsters[i].id, gamedata.monsters[i]);
        }

        players.Clear();
        for (var i = 0; i < gamedata.players.Length; ++i) {
            players.Add(gamedata.players[i].id, gamedata.players[i]);
        }

        weapons.Clear();
        for (var i = 0; i < gamedata.weapons.Length; ++i) {
            weapons.Add(gamedata.weapons[i].id, gamedata.weapons[i]);
        }

        chats.Clear();
        for (var i = 0; i < gamedata.chats.Length; ++i) {
            chats.Add(gamedata.chats[i].id, gamedata.chats[i]);
        }

        choices.Clear();
        for (var i = 0; i < gamedata.choices.Length; ++i) {
            choices.Add(gamedata.choices[i].id, gamedata.choices[i]);
        }

        languages.Clear();
        for (var i = 0; i < gamedata.languages.Length; ++i) {
            languages.Add(gamedata.languages[i].id, gamedata.languages[i]);
        }

        // Reset string dictionary
        strings.Clear();
        for (var i = 0; i < gamedata.strings.Length; ++i) {
            strings.Add(gamedata.strings[i].key, gamedata.strings[i]);
        }

        StringInternational = new StringInternational();
    }

    /// <summary>
    /// 获得原始的游戏地图数据，如不在内存中，则读取配置文件
    /// </summary>
    /// <returns>The game map.</returns>
    /// <param name="index">Index.</param>
    public Constant.MapData GetGameMap(int index) {
        if (!mapdata.ContainsKey(index)) {
            string path = Constant.MAP_DATA_DIR;
            if (index < 9)
                path += "00" + (index + 1);
            else if (index < 99)
                path += "0" + (index + 1);
            else
                path += index + 1;
            var asset = UnityEngine.Resources.Load<UnityEngine.TextAsset>(path).text;

            mapdata[index] = UnityEngine.JsonUtility.FromJson<Constant.MapData>(asset);
        }
        return mapdata[index];
    }

    /// <summary>
    /// 将原始地图数据覆盖到游戏当前数据
    /// </summary>
    /// <param name="index">Index.</param>
    public void SaveMapTo(int index) {
        if (!mapdata.ContainsKey(index)) {
            UnityEngine.Debug.LogError("Saving map " + index + " failed, cannot find map old data.");
        } else {
            Game.Map.OverrideMapData(mapdata[index].mapId, mapdata[index]);
        }
    }

    /// <summary>
    /// 将原始地图数据深拷贝一份出来
    /// </summary>
    /// <returns>The copied map.</returns>
    /// <param name="index">Index.</param>
    public Constant.MapData GetCopiedMap(int index) {
        var dt = GetGameMap(index);
        var ret = new Constant.MapData {
            backThing = dt.backThing,
            mapId = dt.mapId,
            mapName = dt.mapName,
            music = dt.music,
            blocks = new Constant.MapBlockRaw[dt.blocks.Length]
        };
        for (var x = 0; x < ret.blocks.Length; ++x) {
            ret.blocks[x] = new Constant.MapBlockRaw { blocks = new Constant.MapBlock[dt.blocks[x].Length] };
            for (var y = 0; y < ret.blocks[x].Length; ++y) {
                ret.blocks[x][y] = new Constant.MapBlock {
                    thing = dt.blocks[x][y].thing,
                    eventId = dt.blocks[x][y].eventId,
                    eventData = dt.blocks[x][y].eventData
                };
            }
        }
        return ret;
    }

    /// <summary>
    /// 将修改过后的游戏配置保存到原始数据，只用于编辑器
    /// </summary>
    /// <returns>The data.</returns>
    public string SaveData() {
        return UnityEngine.JsonUtility.ToJson(gamedata, false);
    }

    public int MapsCount { get { return mapdata.Count; } }

    /// <summary>
    /// Gets the international strings.
    /// </summary>
    /// <value>The string international manager.</value>
    public StringInternational StringInternational { get;private set;}

    [System.Serializable]
    private class GameData {
        public Constant.ModalData[] modals = null;
        public Constant.Audio[] audios = null;
        public Constant.MonsterData[] monsters = null;
        public Constant.PlayerData[] players = null;
        public Constant.WeaponData[] weapons = null;
        public Constant.ChatData[] chats = null;
        public Constant.ChoiceData[] choices = null;
        public Constant.LanguageData[] languages = null;
        public Constant.InternationalString[] strings = null;
    }

    private Dictionary<int, Constant.MapData> mapdata = new Dictionary<int, Constant.MapData>();
    private GameData gamedata;
}
