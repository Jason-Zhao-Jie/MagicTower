using UnityEngine;
using System.Collections;

public class DataCenter : MonoBehaviour
{
    public static DataCenter instance;

    public enum EGameStatus{
        Start,
        InGame,
        OnMiddleLoading,
        OnBattle,
        OnTipChat,
        OnDialog,
        OnCG,
        OnSmallGame,
    }

    public class Audio{
        int audioId;
        string audioName;
        string audioUrl;
    }

    public class MapBlock{
        string backgroundAvatar;
        string foreheadType;
        string foreheadThing;
        int[] Event;
    }

    public class Map{
        int mapId;
        string mapName;
        string backgroundImage;
        string backgroundAudio;
        MapBlock[][] mapBlocks;
    }

    public int playerModalId = 1;
    public string playerName = "Braves";
    public int level = 0;
    public int exp = 0;
    public int life = 1000;
    public int attack = 10;
    public int defense = 10;
    public int speed = 0;
    public int gold = 0;
    public int yellowKey = 0;
    public int blueKey = 0;
    public int redKey = 0;

    public int floorId = 0;
    public Vector2 pos = new Vector2(1, 1);

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
			
	}

	/// <summary>
	/// When the game loading begins, the data should be load and the instance should be initialized
	/// This function is used to replace the "Start" function
	/// </summary>
    public void LoadData(){
        status = EGameStatus.Start;

        // Read audio list
        var f = System.IO.File.OpenText(Application.dataPath + "/Data/audioList.json");
        var json = f.ReadToEnd();
        f.Close();
        audioList = JsonUtility.FromJson<Audio[]>(json);

        // Read new game's map info
        f = System.IO.File.OpenText(Application.dataPath + "/Data/mapInfo.json");
        json = f.ReadToEnd();
        f.Close();
        newGameMaps = JsonUtility.FromJson<Map[]>(json);

        //

    }

    private EGameStatus status;
    private Audio[] audioList;
    private Map[] newGameMaps;
}
