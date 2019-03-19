
using System.Collections.Generic;

public class PlayerData : AData
{
    public PlayerData(PlayerController controller) : base(controller)
    {
    }

    public void InitPlayerData(int posx = -1, int posy = -1, int playerId = 0)
    {
        if (posx < 0 || posx >= Constant.MAP_BLOCK_LENGTH)
            posx = PlayerPosX;
        else
            PlayerPosX = posx;
        if (posx < 0 || posx >= Constant.MAP_BLOCK_LENGTH)
        {
            posx = 0;
            PlayerPosX = 0;
        }
        if (posy < 0 || posy >= Constant.MAP_BLOCK_LENGTH)
            posy = PlayerPosY;
        else
            PlayerPosY = posy;
        if (posy < 0 || posy >= Constant.MAP_BLOCK_LENGTH)
        {
            posy = 0;
            PlayerPosY = 0;
        }
    }

    public void ChangePlayerData(Constant.ResourceType type, int count)
    {
        switch (type)
        {
            case Constant.ResourceType.Life:
                playerData.life += count;
                break;
            case Constant.ResourceType.Attack:
                playerData.attack += count;
                break;
            case Constant.ResourceType.Defense:
                playerData.defense += count;
                break;
            case Constant.ResourceType.Level:
                // TODO : 升级需要特殊处理
                break;
            case Constant.ResourceType.Experience:
                playerData.exp += count;
                break;
            case Constant.ResourceType.Speed:
                playerData.speed += count;
                break;
            case Constant.ResourceType.Critical:
                playerData.critical += count;
                break;
            case Constant.ResourceType.Gold:
                playerData.gold += count;
                break;
            case Constant.ResourceType.YellowKey:
                playerData.yellowKey += count;
                break;
            case Constant.ResourceType.BlueKey:
                playerData.blueKey += count;
                break;
            case Constant.ResourceType.RedKey:
                playerData.redKey += count;
                break;
            case Constant.ResourceType.GreenKey:
                playerData.greenKey += count;
                break;
            default:
                return;
        }
        (Controller as PlayerController).SyncPlayerData();
    }

    public int PlayerId
    {
        get { return playerData.id; }
    }

    public bool SetPlayerInfo(int id)
    {
        playerData = Game.Config.players[id];
        return true;
    }

    public int Level
    {
        get { return playerData.level; }
        set { playerData.level = value; }
    }
    public int Experience
    {
        get { return playerData.exp; }
        set { playerData.exp = value; }
    }
    public int Life
    {
        get { return playerData.life; }
        set { playerData.life = value; }
    }
    public int Attack
    {
        get { return playerData.attack; }
        set { playerData.attack = value; }
    }
    public int Defense
    {
        get { return playerData.defense; }
        set { playerData.defense = value; }
    }
    public int Speed
    {
        get { return playerData.speed; }
        set { playerData.speed = value; }
    }
    public double Critical
    {
        get { return playerData.critical; }
        set { playerData.critical = value; }
    }

    public int Gold
    {
        get { return playerData.gold; }
        set { playerData.gold = value; }
    }
    public int Weapon
    {
        get { return playerData.weaponId; }
        set { playerData.weaponId = value; }
    }
    public int YellowKey
    {
        get { return playerData.yellowKey; }
        set { playerData.yellowKey = value; }
    }
    public int BlueKey
    {
        get { return playerData.blueKey; }
        set { playerData.blueKey = value; }
    }
    public int RedKey
    {
        get { return playerData.redKey; }
        set { playerData.redKey = value; }
    }
    public int GreenKey
    {
        get { return playerData.greenKey; }
        set { playerData.greenKey = value; }
    }

    public Constant.PlayerData Data { get { return playerData; } set { playerData = value; } }
    public int PlayerPosX { get; set; }
    public int PlayerPosY { get; set; }

    private Constant.PlayerData playerData;
    private Dictionary<int, bool> items = new Dictionary<int, bool>();
}

