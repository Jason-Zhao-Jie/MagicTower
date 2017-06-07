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
        public string backgroundAudio;
        public MapBlock[][] mapBlocks;
    }

    [System.Serializable]
    public class ModalData
    {
        public int id;
        public int typeId;
        public string name;
        public string prefabPath;
        public string keyAnimation;
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
        data = JsonUtility.FromJson<GameData>(txt.text);

    }

    public EGameStatus Status
    {
        get { return status; }
        set { status = value; }
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
}
