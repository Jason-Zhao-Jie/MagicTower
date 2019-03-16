
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

    public bool GoToNextBlock()
    {
        int targetPosX = PlayerPosX;
        int targetPosY = PlayerPosY;
        if (Game.Status == Constant.EGameStatus.AutoStepping)
        {
            if (AutoSteppingRoad.Count <= 0)
            {
                StopAutoStepping();
                return false;
            }
            var target = AutoSteppingRoad.Pop();
            targetPosX = target.x;
            targetPosY = target.y;
            if (targetPosX == PlayerPosX)
            {
                if (targetPosY > PlayerPosY)
                    Dir = PlayerController.Direction.Up;
                else
                    Dir = PlayerController.Direction.Down;
            }
            else
            {
                if (targetPosX > PlayerPosX)
                    Dir = PlayerController.Direction.Right;
                else
                    Dir = PlayerController.Direction.Left;
            }
        }
        else
        {
            switch (Dir)
            {
                case PlayerController.Direction.Up:
                    ++targetPosY;
                    break;
                case PlayerController.Direction.Down:
                    --targetPosY;
                    break;
                case PlayerController.Direction.Right:
                    ++targetPosX;
                    break;
                case PlayerController.Direction.Left:
                    --targetPosX;
                    break;
            }
        }

        // Check if the player is at the condition
        if (targetPosY >= Constant.MAP_BLOCK_LENGTH || targetPosY < 0 || targetPosX >= Constant.MAP_BLOCK_LENGTH || targetPosX < 0)
        {
            return false;
        }

        // Check map event and thing event
        var block = Game.Map.CurrentMap.blocks[targetPosX][targetPosY];
        long uuid = Game.Map.CurrentMap.mapId * 10000 + targetPosY + targetPosX * 100;
        if (block.eventId != 0)
        {
            if (Game.Status == Constant.EGameStatus.AutoStepping)
                Game.Status = Constant.EGameStatus.InGame;
            if (!Game.Controller.EventMgr.DispatchEvent(block.eventId, Game.Map.GetModalByUuid(uuid), block.eventData))
                return false;
        }
        if (block.thing != 0)
        {
            var thingData = Game.Config.modals[block.thing];
            if (thingData.eventId != 0)
            {
                if (Game.Status == Constant.EGameStatus.AutoStepping)
                    Game.Status = Constant.EGameStatus.InGame;
                if (!Game.Controller.EventMgr.DispatchEvent(thingData.eventId, Game.Map.GetModalByUuid(uuid), thingData.eventData))
                    return false;
            }
            switch ((Modal.ModalType)thingData.typeId)
            {
                case Modal.ModalType.Walkable:
                    break;
                default:
                    return false;
            }
        }
        Game.Map.CurrentMap.blocks[targetPosX][targetPosY] = block;

        PlayerPosX = targetPosX;
        PlayerPosY = targetPosY;
        Game.Controller.Audio.PlaySound(AudioController.stepSound);
        return true;
    }

    public void StartWalk(PlayerController.Direction dir = PlayerController.Direction.Default)
    {
        if (dir != PlayerController.Direction.Default)
            Dir = dir;
        IsRunning = true;
    }

    public bool StartAutoStep(int targetPosx, int targetPosy)
    {
        var findedRoad = MathHelper.AutoFindBestRoad(Game.Map.ConvertCurrentMapToFinderArray(), PlayerPosX, PlayerPosY, targetPosx, targetPosy);
        if (findedRoad == null || findedRoad.Length <= 0)
        {
            Game.Controller.Audio.PlaySound(AudioController.disableSound);
            return false;
        }
        Game.Status = Constant.EGameStatus.AutoStepping;
        TargetAutoStep = new UnityEngine.Vector2Int(targetPosx, targetPosy);
        AutoSteppingRoad = new Stack<UnityEngine.Vector2Int>();
        for (int i = findedRoad.Length - 1; i > 0; --i)
        {
            AutoSteppingRoad.Push(findedRoad[i]);
        }
        IsRunning = true;
        return true;
    }

    public void StopAutoStepping()
    {
        IsRunning = false;
        Game.Status = Constant.EGameStatus.InGame;
    }

    public void StopWalk()
    {
        IsRunning = false;
    }

    public void SyncPlayerData()
    {
        MainScene.instance.Level = playerData.level.ToString();
        MainScene.instance.Experience = playerData.exp.ToString();
        MainScene.instance.Life = playerData.life.ToString();
        MainScene.instance.Attack = playerData.attack.ToString();
        MainScene.instance.Defense = playerData.defense.ToString();
        MainScene.instance.Speed = playerData.speed.ToString();
        MainScene.instance.Gold = playerData.gold.ToString();
        MainScene.instance.YellowKey = playerData.yellowKey.ToString();
        MainScene.instance.BlueKey = playerData.blueKey.ToString();
        MainScene.instance.RedKey = playerData.redKey.ToString();
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
        SyncPlayerData();
    }

    public PlayerController.Direction Dir
    {
        get { return dir; }
        set
        {
            dirChanged = dir != value;
            dir = value;
        }
    }
    public UnityEngine.Vector2Int TargetAutoStep { get; private set; }
    public Stack<UnityEngine.Vector2Int> AutoSteppingRoad { get; private set; }

    public bool IsRunning
    {
        get { return isRunning; }
        set
        {
            if (isRunning != value)
                dirChanged = true;
            isRunning = value;
        }
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

    public bool dirChanged = false;

    private bool isRunning;
    private PlayerController.Direction dir;
    private Constant.PlayerData playerData;
    private Dictionary<int, bool> items = new Dictionary<int, bool>();
}

