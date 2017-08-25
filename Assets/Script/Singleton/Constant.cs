public static class Constant
{
	public const string AUDIO_DIR = "Audio/";
    public const string PREFAB_DIR = "Prefab/";
    public const string MAP_DATA_DIR = "MapData/";

    public const int MISS_HITTER = 9;       // TODO: To set the right "MISS" icon
    public const int NOHURT_HITTER = 9;

    public const int MAP_BLOCK_LENGTH = 18;
	public const int MAP_BLOCK_BASE_SIZE = 32;

	public delegate bool EventCallback(Modal caller, long blockData);
    public delegate bool BattlePauseEventCheck();
    public delegate void EmptyCallBack();

    public enum ResourceType
    {
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

    public enum ChatType
	{
		None,
		Bubble,
		Tip,
		Center,
		Bottom,
		Top,
	}

	public enum EGameStatus
	{
		Start,
		InGame,
        InEditor,
		OnMiddleLoading,
        OnEvent,
		OnBattle,
        OnBattleResult,
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
        public long eventData;   // optional
	}

	[System.Serializable]
	public class MapData
	{
		public int mapId;
		public string mapName;
		public int backThing;
		public int music;
		public MapBlock[][] mapBlocks;

        public JObject GetJson()
        {
            var ob = new JObject();
            ob["mapId"] = new JNumber(mapId);
            ob["mapName"] = new JString(mapName);
            ob["backThing"] = new JNumber(backThing);
            ob["music"] = new JNumber(music);
            var blocks = new JArray();
            for (int x = 0; x < mapBlocks.Length; ++x)
            {
                var blockX = new JArray();
                for (int y = 0; y < mapBlocks[x].Length; ++y)
                {
                    var blockY = new JObject();
                    blockY["thing"] = new JNumber(mapBlocks[x][y].thing);
                    blockY["eventId"] = new JNumber(mapBlocks[x][y].eventId);
                    blockY["eventData"] = new JNumber(mapBlocks[x][y].eventData);
                    blockX.Add(blockY);
                }
                blocks.Add(blockX);
            }
            ob["mapBlocks"] = blocks;
            return ob;
        }
	}

	[System.Serializable]
	public class ModalData
	{
		public int id;
		public int typeId;
		public string name;
		public string prefabPath;
		public int eventId;
        public long eventData;   // optional
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
		public int weaponId;

        public MonsterData Clone()
        {
            return new MonsterData()
            {
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
		public int weaponId;
	}

	[System.Serializable]
	public class WeaponData
	{
		public int id;
		public string name;
        public string prefabPath;
        public string critPrefabPath;
        public int audioId;
		public int critAudioId;
	}

	[System.Serializable]
	public struct OneChatData
	{
		public int speakerId;
		public string content;
	}

	[System.Serializable]
	public class ChatData
	{
		public int id;
		public int lastEventId;
		public bool canOn;
		public OneChatData[] data;
	}

	[System.Serializable]
	public class OneChoiceData
	{
		public string content;
		public int eventId;
	}

	[System.Serializable]
	public class ChoiceData
	{
		public int id;
		public int speakerId;
		public string title;
		public string tail;
		public bool canOn;
		public OneChoiceData[] data;
	}

    [System.Serializable]
    public class LanguageData{
        public int id;
        public string name;
        public string key;
    }

    [System.Serializable]
    public class StringInOneLanguage{
        public string langKey;
        public string content;
    }

    [System.Serializable]
    public class InternationalString{
        public int id;
        public string key;
        public StringInOneLanguage[] strings;

        public string this[string key]{
            get{
                for (int i = 0; i < strings.Length;++i){
                    if (key.Equals(strings[i].langKey))
                        return strings[i].content;
                }
                return null;
            }
        }
    }

	[System.Serializable]
	public class GameData
	{
		private MapData[] newGameMaps;
		public ModalData[] modals;
		public Audio[] audios;
		public MonsterData[] monsters;
		public PlayerData[] players;
		public WeaponData[] weapons;
		public ChatData[] chats;
		public ChoiceData[] choices;
        public LanguageData[] languages;
        public InternationalString[] strings;

        public GameData(int mapNums)
        {
            newGameMaps = new MapData[mapNums];
        }

        public MapData GetGameMap(int index)
        {
            if (newGameMaps[index] == null)
            {
                string path = MAP_DATA_DIR;
                if (index < 9)
                    path += "00" + (index + 1);
                else if (index < 99)
                    path += "0" + (index + 1);
                else
                    path += index + 1;
                var asset = UnityEngine.Resources.Load<UnityEngine.TextAsset>(path).text;

                //UnityEngine.Debug.Log("Loading map:" + i + ", Time:" + UnityEngine.Time.realtimeSinceStartup);
                var __oneMap = ArmyAntJson.Create(asset) as JObject;
                newGameMaps[index] = new MapData();
                newGameMaps[index].mapId = __oneMap["mapId"].ToInt();
                newGameMaps[index].mapName = __oneMap["mapName"].ToString();
                newGameMaps[index].backThing = __oneMap["backThing"].ToInt();
                newGameMaps[index].music = __oneMap["music"].ToInt();
                var __mapBlocks = __oneMap["mapBlocks"] as JArray;
                newGameMaps[index].mapBlocks = new MapBlock[__mapBlocks.Length][];
                for (var x = 0; x < __mapBlocks.Length; ++x)
                {
                    var ___mapBlocksX = __mapBlocks[x] as JArray;
                    newGameMaps[index].mapBlocks[x] = new MapBlock[___mapBlocksX.Length];
                    for (var y = 0; y < ___mapBlocksX.Length; ++y)
                    {
                        var ____mapBlockY = ___mapBlocksX[y] as JObject;
                        newGameMaps[index].mapBlocks[x][y].thing = ____mapBlockY["thing"].ToInt();
                        newGameMaps[index].mapBlocks[x][y].eventId = ____mapBlockY["eventId"].ToInt();
                        if (____mapBlockY.ContainsKey("eventData"))
                            newGameMaps[index].mapBlocks[x][y].eventData = ____mapBlockY["eventData"].ToInt();
                    }
                }
            }
            return newGameMaps[index];
        }

        public MapData GetCopiedMap(int index)
        {
            var dt = GetGameMap(index);
            var ret = new MapData();
            ret.backThing = dt.backThing;
            ret.mapId = dt.mapId;
            ret.mapName = dt.mapName;
            ret.music = dt.music;
            ret.mapBlocks = new MapBlock[dt.mapBlocks.Length][];
            for (var x = 0; x < ret.mapBlocks.Length; ++x)
            {
                ret.mapBlocks[x] = new MapBlock[dt.mapBlocks[x].Length];
                for (var y = 0; y < ret.mapBlocks[x].Length; ++y)
                {
                    ret.mapBlocks[x][y].thing = dt.mapBlocks[x][y].thing;
                    ret.mapBlocks[x][y].eventId = dt.mapBlocks[x][y].eventId;
                    ret.mapBlocks[x][y].eventData = dt.mapBlocks[x][y].eventData;
                }
            }
            return ret;
        }

        public int MapLength { get { return newGameMaps.Length; } }
	}
}
