public static class Constant
{
	public const string AUDIO_DIR = "Audio/";
	public const string PREFAB_DIR = "Prefab/";
	public const int MAP_BLOCK_LENGTH = 18;
	public const int MAP_BLOCK_BASE_SIZE = 32;

	public delegate bool EventCallback(Modal caller);

	public enum EventType
	{
		Unknown,
		Send,       // Send player to another place in this or another floor, most stairs include this event.
		GetItem,    // Get an item, most items include this event
		Battle,     // Only have a fight, most normal monsters have this event.
		Chat,       // Chat with somebody
		Choice,     // Need to make a choice and will call another event.
		Game,       // Will play a small game
		Others,     // Call a self-determine function to do something, like "OpenDoor"
	}

	public enum ChatType
	{
		None,
		Bubble,
		Tip,
		Center,
		Bottom,
		Top

	}

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
		public int backThing;
		public int music;
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
		public int weaponId;
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
	public class Event
	{
		public int id;
		public int typeId;
		public long dataId;
	}

	[System.Serializable]
	public class WeaponData
	{
		public int id;
		public string name;
		public string prefabPath;
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
    }

	[System.Serializable]
	public class GameData
	{
		public MapData[] newGameMaps;
		public ModalData[] modals;
		public Audio[] audios;
		public MonsterData[] monsters;
		public PlayerData[] players;
		public Event[] events;
		public WeaponData[] weapons;
		public ChatData[] chats;
		public ChoiceData[] choices;
        public LanguageData[] languages;
        public InternationalString[] strings;
	}
}
