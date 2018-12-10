using System.Collections.Generic;

public static class Constant {
    public const string AUDIO_DIR = "Audio/";
    public const string PREFAB_DIR = "Prefab/";
    public const string DIALOG_DIR = "Controls/";
    public const string MAP_DATA_DIR = "MapData/";

    public const int MISS_HITTER = 9;       // TODO: To set the right "MISS" icon
    public const int NOHURT_HITTER = 9;

    public const int MAP_BLOCK_LENGTH = 18;
    public const int MAP_BLOCK_BASE_SIZE = 32;

    public delegate bool EventCallback(Modal caller, long blockData);
    public delegate void EmptyCallBack();

    public enum ResourceType {
        Unknown = 0,
        Life = 1,
        Attack = 2,
        Defense = 3,
        Level = 4,
        Experience = 5,
        Speed = 6,
        Critical = 7,
        Gold = 8,
        YellowKey = 9,
        BlueKey = 10,
        RedKey = 11,
        GreenKey = 12,
    }

    public enum ChatType {
        None,
        Bubble,
        Tip,
        Center,
        Bottom,
        Top,
    }

    public enum EGameStatus {
        Start,
        InGame,
        InEditor,
        OnMiddleLoading,
        OnEvent,
        OnBattle,
        OnBattleResult,
        OnTipChat,
        OnChoice,
        OnDialog,
        OnCG,
        OnSmallGame,
        AutoStepping,
    }

    [System.Serializable]
    public struct MapBlock {
        public int thing;
        public int eventId;
        public long eventData;   // optional

        public JObject Json {
            get {
                return new JObject(new Dictionary<string, IUnit> {
                    { "thing", new JNumber(thing) },
                    { "eventId", new JNumber(eventId) },
                    { "eventData", new JNumber(eventData) }
                });
            }
            set {
                thing = value["thing"].ToInt();
                eventId = value["eventId"].ToInt();
                if (value.ContainsKey("eventData"))
                    eventData = value["eventData"].ToLong();
                else
                    eventData = 0;
            }
        }
    }

    [System.Serializable]
    public class MapData {
        public int mapId;
        public string mapName;
        public int backThing;
        public int music;
        public MapBlock[][] mapBlocks;

        public JObject Json {
            get {
                var blocks = new JArray();
                for (int x = 0; x < mapBlocks.Length; ++x) {
                    var blockX = new JArray();
                    for (int y = 0; y < mapBlocks[x].Length; ++y) {
                        blockX.Add(mapBlocks[x][y].Json);
                    }
                    blocks.Add(blockX);
                }
                return new JObject(new Dictionary<string, IUnit> {
                    { "mapId", new JNumber(mapId) },
                    { "mapName", new JString(mapName) },
                    { "backThing", new JNumber(backThing) },
                    { "music", new JNumber(music) },
                    { "mapBlocks", blocks }
                });
            }
            set {
                mapId = value["mapId"].ToInt();
                mapName = value["mapName"].ToString();
                backThing = value["backThing"].ToInt();
                music = value["music"].ToInt();
                var blocks = value["mapBlocks"] as JArray;
                mapBlocks = new MapBlock[blocks.Length][];
                for (var x = 0; x < mapBlocks.Length; ++x) {
                    var blockX = blocks[x] as JArray;
                    mapBlocks[x] = new MapBlock[blockX.Length];
                    for (var y = 0; y < blockX.Length; ++y) {
                        mapBlocks[x][y] = new MapBlock() { Json = blockX[y] as JObject };
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class ModalData {
        public int id;
        public int typeId;
        public string name;
        public string prefabPath;
        public int eventId;
        public long eventData;   // optional

        public JObject Json {
            get {
                return new JObject(new Dictionary<string, IUnit>()
                {
                    {"id",new JNumber(id) },
                    {"typeId",new JNumber(typeId) },
                    {"name",new JString(name) },
                    {"prefabPath",new JString(prefabPath) },
                    {"eventId",new JNumber(eventId) },
                    {"eventData",new JNumber(eventData) },
                });
            }
            set {
                id = value["id"].ToInt();
                typeId = value["typeId"].ToInt();
                name = value["name"].ToString();
                prefabPath = value["prefabPath"].ToString();
                eventId = value["eventId"].ToInt();
                eventData = value["eventData"].ToLong();
            }
        }
    }

    [System.Serializable]
    public class Audio {
        public int id;
        public string path;

        public JObject Json {
            get {
                return new JObject(new Dictionary<string, IUnit>()
                {
                    {"id",new JNumber(id) },
                    {"path",new JString(path) }
                });
            }
            set {
                id = value["id"].ToInt();
                path = value["path"].ToString();
            }
        }
    }

    [System.Serializable]
    public class MonsterData {
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
        public int weaponId;

        public JObject Json {
            get {
                var special_list = new JArray();
                if (special != null)
                    for (var i = 0; i < special.Length; ++i) {
                        special_list.Add(new JNumber(special[i]));
                    }
                return new JObject(new Dictionary<string, IUnit>()
                {
                    {"id",new JNumber(id) },
                    {"level",new JNumber(level) },
                    {"exp",new JNumber(exp) },
                    {"life",new JNumber(life) },
                    {"attack",new JNumber(attack) },
                    {"defense",new JNumber(defense) },
                    {"speed",new JNumber(speed) },
                    {"critical",new JNumber(critical) },
                    {"gold",new JNumber(gold) },
                    {"special",special_list },
                    {"weaponId",new JNumber(weaponId) },
                });
            }
            set {
                id = value["id"].ToInt();
                level = value["level"].ToInt();
                exp = value["exp"].ToInt();
                life = value["life"].ToInt();
                attack = value["attack"].ToInt();
                defense = value["defense"].ToInt();
                speed = value["speed"].ToInt();
                critical = value["critical"].ToFloat();
                gold = value["gold"].ToInt();
                var special_list = value["special"] as JArray;
                if (special_list.Length <= 0)
                    special = null;
                else {
                    special = new int[special_list.Length];
                    for (int i = 0; i < special.Length; ++i) {
                        special[i] = special_list[i].ToInt();
                    }
                }
                weaponId = value["weaponId"].ToInt();
            }
        }

        public MonsterData Clone() {
            return new MonsterData() {
                id = id,
                level = level,
                exp = exp,
                life = life,
                attack = attack,
                defense = defense,
                speed = speed,
                gold = gold,
                special = special,
                weaponId = weaponId,
            };
        }
    }

    [System.Serializable]
    public struct PlayerData {
        public int id;
        public int level;
        public int exp;
        public int life;
        public int attack;
        public int defense;
        public int speed;
        public double critical;
        public int gold;
        public int weaponId;
        public int yellowKey;
        public int blueKey;
        public int redKey;
        public int greenKey;

        public JObject Json {
            get {
                return new JObject(new Dictionary<string, IUnit>()
                {
                    {"id",new JNumber(id) },
                    {"level",new JNumber(level) },
                    {"exp",new JNumber(exp) },
                    {"life",new JNumber(life) },
                    {"attack",new JNumber(attack) },
                    {"defense",new JNumber(defense) },
                    {"speed",new JNumber(speed) },
                    {"critical",new JNumber(critical) },
                    {"gold",new JNumber(gold) },
                    {"weaponId",new JNumber(weaponId) },
                    {"yellowKey",new JNumber(yellowKey) },
                    {"blueKey",new JNumber(blueKey) },
                    {"redKey",new JNumber(redKey) },
                    {"greenKey",new JNumber(greenKey) },
                });
            }
            set {
                id = value["id"].ToInt();
                level = value["level"].ToInt();
                exp = value["exp"].ToInt();
                life = value["life"].ToInt();
                attack = value["attack"].ToInt();
                defense = value["defense"].ToInt();
                speed = value["speed"].ToInt();
                critical = value["critical"].ToFloat();
                gold = value["gold"].ToInt();
                weaponId = value["weaponId"].ToInt();
                yellowKey = value["yellowKey"].ToInt();
                blueKey = value["blueKey"].ToInt();
                redKey = value["redKey"].ToInt();
                greenKey = value["greenKey"].ToInt();
            }
        }
    }

    [System.Serializable]
    public class WeaponData {
        public int id;
        public string name;
        public string prefabPath;
        public string critPrefabPath;
        public int audioId;
        public int critAudioId;
        public JObject Json {
            get {
                return new JObject(new Dictionary<string, IUnit>()
                {
                    {"id",new JNumber(id) },
                    {"name",new JString(name) },
                    {"prefabPath",new JString(prefabPath) },
                    {"critPrefabPath",new JString(critPrefabPath) },
                    {"audioId",new JNumber(audioId) },
                    {"critAudioId",new JNumber(critAudioId) },
                });
            }
            set {
                id = value["id"].ToInt();
                name = value["name"].ToString();
                prefabPath = value["prefabPath"].ToString();
                critPrefabPath = value["critPrefabPath"].ToString();
                audioId = value["audioId"].ToInt();
                critAudioId = value["critAudioId"].ToInt();
            }
        }
    }

    [System.Serializable]
    public struct OneChatData {
        public int speakerId;
        public string content;

        public JObject Json {
            get {
                return new JObject(new Dictionary<string, IUnit>()
                {
                    {"speakerId",new JNumber(speakerId) },
                    {"content",new JString(content) }
                });
            }
            set {
                speakerId = value["speakerId"].ToInt();
                content = value["content"].ToString();
            }
        }
    }

    [System.Serializable]
    public class ChatData {
        public int id;
        public int lastEventId;
        public long lastEventData;
        public bool canOn;
        public OneChatData[] data;

        public JObject Json {
            get {
                var data_list = new JArray();
                for (var i = 0; i < data.Length; ++i) {
                    data_list.Add(data[i].Json);
                }
                return new JObject(new Dictionary<string, IUnit>()
                {
                    {"id",new JNumber(id) },
                    {"lastEventId",new JNumber(lastEventId) },
                    {"lastEventData",new JNumber(lastEventData) },
                    {"canOn",new JBoolean(canOn) },
                    {"data", data_list }
                });
            }
            set {
                id = value["id"].ToInt();
                lastEventId = value["lastEventId"].ToInt();
                if (value.ContainsKey("lastEventData"))
                    lastEventData = value["lastEventData"].ToLong();
                canOn = value["canOn"].ToBool();
                var data_list = value["data"] as JArray;
                data = new OneChatData[data_list.Length];
                for (var i = 0; i < data.Length; ++i) {
                    data[i] = new OneChatData() { Json = data_list[i] as JObject };
                }
            }
        }
    }

    [System.Serializable]
    public class OneChoiceData {
        public string content;
        public int eventId;
        public long eventData;

        public JObject Json {
            get {
                return new JObject(new Dictionary<string, IUnit>()
                {
                    {"content",new JString(content) },
                    {"eventId",new JNumber(eventId) },
                    {"eventData",new JNumber(eventData) }
                });
            }
            set {
                content = value["content"].ToString();
                eventId = value["eventId"].ToInt();
                if (value.ContainsKey("eventData"))
                    eventData = value["eventData"].ToLong();
            }
        }
    }

    [System.Serializable]
    public class ChoiceData {
        public int id;
        public int speakerId;
        public string title;
        public string tail;
        public bool canOn;
        public OneChoiceData[] data;

        public JObject Json {
            get {
                var data_list = new JArray();
                for (var i = 0; i < data.Length; ++i) {
                    data_list.Add(data[i].Json);
                }
                return new JObject(new Dictionary<string, IUnit>()
                {
                    {"id",new JNumber(id) },
                    {"speakerId",new JNumber(speakerId) },
                    {"title",new JString(title) },
                    {"tail",new JString(tail) },
                    {"canOn",new JBoolean(canOn) },
                    {"data", data_list }
                });
            }
            set {
                id = value["id"].ToInt();
                speakerId = value["speakerId"].ToInt();
                title = value["title"].ToString();
                tail = value["tail"].ToString();
                canOn = value["canOn"].ToBool();
                var data_list = value["data"] as JArray;
                data = new OneChoiceData[data_list.Length];
                for (var i = 0; i < data.Length; ++i) {
                    data[i] = new OneChoiceData() { Json = data_list[i] as JObject };
                }
            }
        }
    }

    [System.Serializable]
    public class LanguageData {
        public int id;
        public string name;
        public string key;

        public JObject Json {
            get {
                return new JObject(new Dictionary<string, IUnit>()
                {
                    {"id",new JNumber(id) },
                    {"name",new JString(name) },
                    {"key",new JString(key) },
                });
            }
            set {
                id = value["id"].ToInt();
                name = value["name"].ToString();
                key = value["key"].ToString();
            }
        }
    }

    [System.Serializable]
    public class StringInOneLanguage {
        public string langKey;
        public string content;

        public JObject Json {
            get {
                return new JObject(new Dictionary<string, IUnit>()
                {
                    {"langKey",new JString(langKey) },
                    {"content",new JString(content) },
                });
            }
            set {
                langKey = value["langKey"].ToString();
                content = value["content"].ToString();
            }
        }
    }

    [System.Serializable]
    public class InternationalString {
        public int id;
        public string key;
        public StringInOneLanguage[] strings;

        public string this[string key] {
            get {
                for (int i = 0; i < strings.Length; ++i) {
                    if (key.Equals(strings[i].langKey))
                        return strings[i].content;
                }
                return null;
            }
        }

        public JObject Json {
            get {
                var strings_list = new JArray();
                for (var i = 0; i < strings.Length; ++i) {
                    strings_list.Add(strings[i].Json);
                }
                return new JObject(new Dictionary<string, IUnit>()
                {
                    {"id",new JNumber(id) },
                    {"key",new JString(key) },
                    {"strings", strings_list }
                });
            }
            set {
                id = value["id"].ToInt();
                key = value["key"].ToString();
                var strings_list = value["strings"] as JArray;
                strings = new StringInOneLanguage[strings_list.Length];
                for (var i = 0; i < strings.Length; ++i) {
                    strings[i] = new StringInOneLanguage() { Json = strings_list[i] as JObject };
                }
            }
        }
    }
}
