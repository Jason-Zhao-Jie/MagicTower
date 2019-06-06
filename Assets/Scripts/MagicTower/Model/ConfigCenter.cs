using System.Collections.Generic;

namespace MagicTower.Model
{

    /// <summary>
    /// 游戏核心数据处理类
    /// </summary>
    public class ConfigCenter
    {
        public readonly int mapLength;
        public readonly bool debug;
        public readonly int gameoverBackTime;
        public readonly int newGamePlayerId;

        public readonly Dictionary<int, ModalData> modals = new Dictionary<int, ModalData>();
        public readonly Dictionary<int, Audio> audios = new Dictionary<int, Audio>();
        public readonly Dictionary<int, MonsterData> monsters = new Dictionary<int, MonsterData>();
        public readonly Dictionary<int, PlayerData> players = new Dictionary<int, PlayerData>();
        public readonly Dictionary<int, WeaponData> weapons = new Dictionary<int, WeaponData>();
        public readonly Dictionary<int, ChatData> chats = new Dictionary<int, ChatData>();
        public readonly Dictionary<int, ChoiceData> choices = new Dictionary<int, ChoiceData>();
        public readonly Dictionary<int, LanguageData> languages = new Dictionary<int, LanguageData>();
        public readonly Dictionary<string, InternationalString> strings = new Dictionary<string, InternationalString>();

        public ConfigCenter()
        {
            mapLength = UnityEngine.Resources.LoadAll<UnityEngine.TextAsset>(Dirs.MAP_DATA_DIR).Length;

            // Reading json config files:
            var json = UnityEngine.Resources.Load<UnityEngine.TextAsset>("GameData").text;
            gamedata = UnityEngine.JsonUtility.FromJson<GameData>(json);

            debug = gamedata.debug;
            gameoverBackTime = gamedata.gameoverBackTime;
            newGamePlayerId = gamedata.newGamePlayerId;

            // Reset references
            modals.Clear();
            for (var i = 0; i < gamedata.modals.Length; ++i)
            {
                modals.Add(gamedata.modals[i].id, gamedata.modals[i]);
            }

            audios.Clear();
            for (var i = 0; i < gamedata.audios.Length; ++i)
            {
                audios.Add(gamedata.audios[i].id, gamedata.audios[i]);
            }

            monsters.Clear();
            for (var i = 0; i < gamedata.monsters.Length; ++i)
            {
                monsters.Add(gamedata.monsters[i].id, gamedata.monsters[i]);
            }

            players.Clear();
            for (var i = 0; i < gamedata.players.Length; ++i)
            {
                players.Add(gamedata.players[i].id, gamedata.players[i]);
            }

            weapons.Clear();
            for (var i = 0; i < gamedata.weapons.Length; ++i)
            {
                weapons.Add(gamedata.weapons[i].id, gamedata.weapons[i]);
            }

            chats.Clear();
            for (var i = 0; i < gamedata.chats.Length; ++i)
            {
                chats.Add(gamedata.chats[i].id, gamedata.chats[i]);
            }

            choices.Clear();
            for (var i = 0; i < gamedata.choices.Length; ++i)
            {
                choices.Add(gamedata.choices[i].id, gamedata.choices[i]);
            }

            languages.Clear();
            for (var i = 0; i < gamedata.languages.Length; ++i)
            {
                languages.Add(gamedata.languages[i].id, gamedata.languages[i]);
            }

            // Reset string dictionary
            strings.Clear();
            for (var i = 0; i < gamedata.strings.Length; ++i)
            {
                if (debug)
                {
                    if (strings.ContainsKey(gamedata.strings[i].key))
                    {
                        Game.DebugLogError("Repeated international string key: ", gamedata.strings[i].key);
                    }
                    foreach (var item in strings)
                    {
                        if (item.Value.id == gamedata.strings[i].id)
                        {
                            Game.DebugLogError("Repeated international string id: ", gamedata.strings[i].id);
                        }
                    }
                }
                strings.Add(gamedata.strings[i].key, gamedata.strings[i]);
            }

            StringInternational = new StringInternational(languages);
        }

        /// <summary>
        /// 获得原始的游戏地图数据，如不在内存中，则读取配置文件
        /// </summary>
        /// <returns>The game map.</returns>
        /// <param name="index">Index.</param>
        public MapData GetGameMap(int index)
        {
            if (!mapdata.ContainsKey(index))
            {
                string path = Dirs.MAP_DATA_DIR;
                if (index < 9)
                    path += "00" + (index + 1);
                else if (index < 99)
                    path += "0" + (index + 1);
                else
                    path += index + 1;
                var asset = UnityEngine.Resources.Load<UnityEngine.TextAsset>(path).text;

                mapdata.Add(index, UnityEngine.JsonUtility.FromJson<MapData>(asset));
            }
            return mapdata[index];
        }

        /// <summary>
        /// 将原始地图数据覆盖到游戏当前数据
        /// </summary>
        /// <param name="index">Index.</param>
        public void SaveMapTo(int index)
        {
            if (!mapdata.ContainsKey(index))
            {
                Game.DebugLogError("Saving map ", index, " failed, cannot find map old data.");
            }
            else
            {
                Game.Map.OverrideMapData(mapdata[index].mapId, mapdata[index]);
            }
        }

        /// <summary>
        /// 将原始地图数据深拷贝一份出来
        /// </summary>
        /// <returns>The copied map.</returns>
        /// <param name="index">Index.</param>
        public MapData GetCopiedMap(int index)
        {
            var dt = GetGameMap(index);
            var ret = new MapData
            {
                backThing = dt.backThing,
                mapId = dt.mapId,
                mapName = dt.mapName,
                mapNameParam = dt.mapNameParam,
                music = dt.music,
                blocks = new MapBlockRaw[dt.blocks.Length]
            };
            for (var x = 0; x < ret.blocks.Length; ++x)
            {
                ret.blocks[x] = new MapBlockRaw { blocks = new MapBlock[dt.blocks[x].Length] };
                for (var y = 0; y < ret.blocks[x].Length; ++y)
                {
                    ret.blocks[x][y] = new MapBlock
                    {
                        thing = dt.blocks[x][y].thing,
                        eventId = dt.blocks[x][y].eventId,
                        eventData = dt.blocks[x][y].eventData == null ? null : new long[dt.blocks[x][y].eventData.Length],
                    };
                    for (var i = 0; dt.blocks[x][y].eventData != null && i < ret.blocks[x][y].eventData.Length; ++i)
                    {
                        ret.blocks[x][y].eventData[i] = dt.blocks[x][y].eventData[i];
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// 将修改过后的游戏配置保存到原始数据，只用于编辑器
        /// </summary>
        /// <returns>The data.</returns>
        public string SaveData()
        {
            return UnityEngine.JsonUtility.ToJson(gamedata, false);
        }

        public int MapsCount { get { return mapdata.Count; } }

        /// <summary>
        /// Gets the international strings.
        /// </summary>
        /// <value>The string international manager.</value>
        public StringInternational StringInternational { get; private set; }

        [System.Serializable]
        private class GameData
        {
            public bool debug = true;
            public int gameoverBackTime;
            public int newGamePlayerId;
            public ModalData[] modals = null;
            public Audio[] audios = null;
            public MonsterData[] monsters = null;
            public PlayerData[] players = null;
            public WeaponData[] weapons = null;
            public ChatData[] chats = null;
            public ChoiceData[] choices = null;
            public LanguageData[] languages = null;
            public InternationalString[] strings = null;
        }

        private readonly Dictionary<int, MapData> mapdata = new Dictionary<int, MapData>();
        private GameData gamedata;
    }

}