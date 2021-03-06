using System.Collections;
using System.Collections.Generic;

namespace MagicTower.Model
{
    public static class Dirs {
        public const string STRING_DATA_DIR = "Strings/";
        public const string MAP_DATA_DIR = "MapData/";
    }

    public delegate void EmptyCallBack();
    public delegate bool EmptyBoolCallBack();
    public delegate bool IntegerBoolCallBack(int param);

    public enum ResourceType : byte{
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
        OnPlayingAds,
        InGame,
        InEditor,
        InEditorDialog,
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

    #region Game Config Data Structs

    [System.Serializable]
    public struct MapBlock {
        public int thing;
        public int eventId;
        public long[] eventData;   // optional
    }

    [System.Serializable]
    public class MapBlockRaw : IEnumerable<MapBlock> {
        public MapBlock[] blocks;
        public MapBlock this[int index] {
            get {
                return blocks[index];
            }
            set {
                blocks[index] = value;
            }
        }
        public int Length {
            get {
                if(blocks == null)
                    return 0;
                return blocks.Length;
            }
        }

        public IEnumerator<MapBlock> GetEnumerator() {
            if (blocks != null) {
                foreach (var i in blocks) {
                    yield return i;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }

    [System.Serializable]
    public class MapData {
        public int mapId;
        public string mapName;
        public int mapNameParam;
        public int backThing;
        public int music;
        public MapBlockRaw[] blocks;
    }

    [System.Serializable]
    public class InfoData {
        public string title;
        public string content;
    }

    [System.Serializable]
    public class ModalData {
        public int id;
        public int typeId;
        public string name;
        public string prefabPath;
        public int eventId;
        public string animator;
        public long[] eventData;   // optional
    }

    [System.Serializable]
    public class Audio {
        public int id;
        public string path;
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
    }

    [System.Serializable]
    public class WeaponData {
        public int id;
        public string name;
        public string prefabPath;
        public float prefabLocalScale;
        public string critPrefabPath;
        public float critPrefabLocalScale;
        public int audioId;
        public int critAudioId;
    }

    [System.Serializable]
    public struct OneChatData {
        public int speakerId;
        public string content;
        public bool tipSilent;
    }

    [System.Serializable]
    public class ChatData {
        public int id;
        public int eventId;
        public long[] eventData;
        public bool canOn;
        public OneChatData[] data;
    }

    [System.Serializable]
    public class OneChoiceData {
        public int contentType;
        public string content;
        public string[] contentData;
        public int eventId;
        public long[] eventData;
        public bool close;
    }

    [System.Serializable]
    public class ChoiceData {
        public int id;
        public int speakerId;
        public string title;
        public string tail;
        public bool canOn;
        public OneChoiceData[] data;
    }

    [System.Serializable]
    public class LanguageData {
        public int id;
        public string name;
        public string key;
        public string path;
    }

    [System.Serializable]
    public class InternationalString {
        public string key;
        public string content;
    }

    #endregion

    #region Save data structs

    [System.Serializable]
    public struct RuntimePositionData {
        public int mapId;
        public int x;
        public int y;
    }

    [System.Serializable]
    public struct RuntimeNumberData {
        public int id;
        public long value;
    }

    [System.Serializable]
    public class RuntimeGameData {
        public PlayerData player;
        public RuntimePositionData pos;
        public RuntimeNumberData[] numbers;
        public long lastTime = System.DateTime.Now.ToFileTime();
        public long totalTime = 0;
    }

    #endregion

}
