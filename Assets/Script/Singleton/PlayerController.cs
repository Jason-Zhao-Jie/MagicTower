using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance = null;
    public enum Direction{
        Default,
        Down,
        Up,
        Left,
        Right
    }

    public string playerName = "Jack";
    public int level = 0;
    public int exp = 0;
    public int life = 1000;
    public int attack = 10;
    public int defense = 10;
    public int speed = 0;
    public double critical = 0.0;
    public int gold = 0;
    public int yellowKey = 0;
    public int blueKey = 0;
    public int redKey = 0;

    public Dictionary<int, bool> items = new Dictionary<int, bool>();

    private int posx = -1;
    private int posy = -1;

    void Start()
    // Use this for initializationid Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public int PlayerId
    {
        get { return currentPlayerId; }
        set { currentPlayerId = value; }
    }

    public void ShowPlayer(int posx = -1, int posy = -1, int playerId = 0, bool isNew = false)
    {
        if (playerId == 0)
            PlayerId = currentPlayerId;
        else if (playerId != currentPlayerId)
        {
            if (player != null)
                player.transform.SetParent(null);
            currentPlayerId = playerId;
            player = null;
        }
        if (player == null)
        {
            var playerData = DataCenter.instance.GetPlayerDataById(playerId);
            var modalData = DataCenter.instance.GetModalById(playerId);
            var obj = Instantiate(Resources.Load(modalData.prefabPath) as GameObject);
            player = obj.GetComponent<Player>();

            playerName = modalData.name;
            if (isNew)
            {
                level = playerData.level;
                life = playerData.life;
                attack = playerData.attack;
                defense = playerData.defense;
                speed = playerData.speed;
                critical = playerData.critical;
                gold = playerData.gold;
                yellowKey = playerData.yellowKey;
                blueKey = playerData.blueKey;
                redKey = playerData.redKey;
            }
        }
        if (posx < 0)
            posx = this.posx;
        else
            this.posx = posx;
        if (posy < 0)
            posy = this.posy;
        else
            this.posy = posy;
        if (posx >= 0 && posy >= 0)
            MainScene.instance.AddObjectToMap(player.gameObject, posx, posy);
    }

    public void SetDirection(Direction dir){
        direction = dir;
        //TODO: set the direction in GameObject
    }

    public void StartWalk(Direction dir = Direction.Default){
        if (dir != Direction.Default)
            SetDirection(dir);
		switch (direction)
		{
			case Direction.Down:
			case Direction.Up:
            case Direction.Left:
			case Direction.Right:
                // TODO: Set the player's animator to run
                break;
        }
    }

    private Player player = null;
    private int currentPlayerId;
    private Direction direction;
}
