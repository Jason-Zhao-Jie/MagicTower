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

    public struct MapBlock
    {
        public int thing;
        public int eventId;
    }

    public struct MapData
    {
        public int mapId;
        public string mapName;
        public string backgroundImage;
        public string backgroundAudio;
        public MapBlock[][] mapBlocks;
    }

    public class ModalData
    {
        public int id;
        public int typeId;
        public string name;
        public string prefabPath;
        public string keyAnimation;
        public int eventId;
    }

    public class Audio
    {
        public int id;
        public string path;
    }

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

    public MapData[] NewGameMaps { get { return newGameMaps; } }
    public ModalData[] Modals { get { return modals; } }
    public ModalData GetModalById(int id)
    {
        for (int i = 0; i < modals.Length; ++i)
            if (modals[i].id == id)
                return modals[i];
        return null;
    }
    public Audio[] Audios { get { return audios; } }
    public string GetAudioById(int id)
    {
        for (int i = 0; i < audios.Length; ++i)
            if (audios[i].id == id)
                return audios[i].path;
        return "";
    }
    public MonsterData[] Monsters { get { return monsters; } }
    public MonsterData GetMonsterDataById(int id)
    {
        for (int i = 0; i < monsters.Length; ++i)
            if (monsters[i].id == id)
                return monsters[i];
        return null;
    }
    public PlayerData[] Players { get { return players; } }
    public PlayerData GetPlayerDataById(int id)
    {
        for (int i = 0; i < players.Length; ++i)
            if (players[i].id == id)
                return players[i];
        return new PlayerData();
    }
    public Event[] Events { get { return events; } }
    public Event GetEventDataById(int id)
    {
        for (int i = 0; i < events.Length; ++i)
            if (audios[i].id == id)
                return events[i];
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
        var txt = Resources.Load<TextAsset>("mapInfo.json");
        newGameMaps = JsonUtility.FromJson<MapData[]>(txt.text);
        txt = Resources.Load<TextAsset>("modal.json");
        modals = JsonUtility.FromJson<ModalData[]>(txt.text);
        txt = Resources.Load<TextAsset>("audio.json");
        audios = JsonUtility.FromJson<Audio[]>(txt.text);
        txt = Resources.Load<TextAsset>("monsters.json");
        monsters = JsonUtility.FromJson<MonsterData[]>(txt.text);
        txt = Resources.Load<TextAsset>("players.json");
        players = JsonUtility.FromJson<PlayerData[]>(txt.text);
        txt = Resources.Load<TextAsset>("event.json");
        events = JsonUtility.FromJson<Event[]>(txt.text);

    }

    public EGameStatus Status
    {
        get { return status; }
        set { status = value; }
    }

    private EGameStatus status;
    private MapData[] newGameMaps;
    private ModalData[] modals;
    private Audio[] audios;
    private MonsterData[] monsters;
    private PlayerData[] players;
    private Event[] events;
}
