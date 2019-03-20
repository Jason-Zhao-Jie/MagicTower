using System.Collections.Generic;
using UnityEngine;

public class PlayerController : AController<PlayerData, PlayerView>
{
    public const int DEFALUT_PLAYER_ID = 62;

    public enum Direction
    {
        Default,
        Up,
        Down,
        Right,
        Left
    }

    public PlayerController(PlayerView playerPanel)
    {
        InitDataAndView(new PlayerData(this), playerPanel);
        View.Controller = this;
    }

    public void ShowPlayer(bool isNew)
    {
        ShowPlayer(9, 1, 0, isNew);
    }

    public void ShowPlayer(int posx, int posy, int playerId = 0, bool isNew = false)
    {

        if (playerId == 0)
        {
            playerId = Data.PlayerId;
        }
        if (playerId == 0)
        {
            playerId = DEFALUT_PLAYER_ID;
        }
        isNew = isNew || playerId != Data.PlayerId;
        if (isNew)
        {
            Data.SetPlayerInfo(playerId);
        }
        Data.InitPlayerData(posx, posy, playerId);
        var player = View.ShowPlayer(playerId, isNew);
        Game.Map.AddObjectToMap(player.gameObject, posx, posy, -17);

        if (isNew)
        {
            SyncPlayerData();
        }
    }

    public void SyncPlayerData()
    {
        if (!View.HasStarted || Data.PlayerId == 0)
        {
            return;
        }
        View.MapName = Game.Config.StringInternational.GetValue(Game.Map.CurrentMap.mapName, Game.Map.CurrentMap.mapId.ToString());
        View.RoleName = Game.Config.StringInternational.GetValue(Game.Config.modals[Data.PlayerId].name);
        View.Portrait = View.Player.BaseSprite;
        View.Level = Data.Level.ToString();
        View.Experience = Data.Experience.ToString();
        View.Life = Data.Life.ToString();
        View.Attack = Data.Attack.ToString();
        View.Defense = Data.Defense.ToString();
        View.Speed = Data.Speed.ToString();
        View.Gold = Data.Gold.ToString();
        View.YellowKey = Data.YellowKey.ToString();
        View.BlueKey = Data.BlueKey.ToString();
        View.RedKey = Data.RedKey.ToString();
    }

    public bool GoToNextBlock()
    {
        int targetPosX = PlayerPosX;
        int targetPosY = PlayerPosY;
        if (Game.Status == Constant.EGameStatus.AutoStepping)
        {
            if (AutoSteppingRoad.Count <= 0)
            {
                StopAutoStep();
                return false;
            }
            var target = AutoSteppingRoad.Pop();
            targetPosX = target.x;
            targetPosY = target.y;
            if (targetPosX == PlayerPosX)
            {
                if (targetPosY > PlayerPosY)
                    View.Player.Dir = Direction.Up;
                else
                    View.Player.Dir = Direction.Down;
            }
            else
            {
                if (targetPosX > PlayerPosX)
                    View.Player.Dir = Direction.Right;
                else
                    View.Player.Dir = Direction.Left;
            }
        }
        else
        {
            switch (View.Player.Dir)
            {
                case Direction.Up:
                    ++targetPosY;
                    break;
                case Direction.Down:
                    --targetPosY;
                    break;
                case Direction.Right:
                    ++targetPosX;
                    break;
                case Direction.Left:
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
            if (!Game.Managers.EventMgr.DispatchEvent(block.eventId, Game.Map.GetModalByUuid(uuid), block.eventData))
                return false;
        }
        if (block.thing != 0)
        {
            var thingData = Game.Config.modals[block.thing];
            if (thingData.eventId != 0)
            {
                if (Game.Status == Constant.EGameStatus.AutoStepping)
                    Game.Status = Constant.EGameStatus.InGame;
                if (!Game.Managers.EventMgr.DispatchEvent(thingData.eventId, Game.Map.GetModalByUuid(uuid), thingData.eventData))
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
        Game.Managers.Audio.PlaySound(AudioManager.stepSound);
        return true;
    }

    public void StartWalk(Direction dir = Direction.Default)
    {
        if (dir != Direction.Default)
            View.Player.Dir = dir;
        View.Player.IsRunning = true;
    }

    public void StopWalk()
    {
        View.Player.IsRunning = false;
    }

    public bool StartAutoStep(int targetPosx, int targetPosy)
    {
        var findedRoad = MathHelper.AutoFindBestRoad(Game.Map.ConvertCurrentMapToFinderArray(), PlayerPosX, PlayerPosY, targetPosx, targetPosy);
        if (findedRoad == null || findedRoad.Length <= 0)
        {
            Game.Managers.Audio.PlaySound(AudioManager.disableSound);
            return false;
        }
        Game.Status = Constant.EGameStatus.AutoStepping;
        TargetAutoStep = new Vector2Int(targetPosx, targetPosy);
        AutoSteppingRoad = new Stack<Vector2Int>();
        for (int i = findedRoad.Length - 1; i > 0; --i)
        {
            AutoSteppingRoad.Push(findedRoad[i]);
        }
        View.Player.IsRunning = true;
        return true;
    }

    public void StopAutoStep()
    {
        View.Player.IsRunning = false;
        Game.Status = Constant.EGameStatus.InGame;
    }

    public void ChangePlayerData(Constant.ResourceType type, int count)
    {
        Data.ChangePlayerData(type, count);
    }

    public string MapName
    {
        get { return View.MapName; }
        set { View.MapName = value; }
    }

    public int PlayerId
    {
        get { return Data.PlayerId; }
    }

    public int Level
    {
        get { return Data.Level; }
        set
        {
            Data.Level = value;
            View.Level = value.ToString();
        }
    }

    public int Experience
    {
        get { return Data.Experience; }
        set
        {
            Data.Experience = value;
            View.Experience = value.ToString();
        }
    }

    public int Life
    {
        get { return Data.Life; }
        set
        {
            Data.Life = value;
            View.Life = value.ToString();
        }
    }

    public int Attack
    {
        get { return Data.Attack; }
        set
        {
            Data.Attack = value;
            View.Attack = value.ToString();
        }
    }

    public int Defense
    {
        get { return Data.Defense; }
        set
        {
            Data.Defense = value;
            View.Defense = value.ToString();
        }
    }

    public int Speed
    {
        get { return Data.Speed; }
        set
        {
            Data.Speed = value;
            View.Speed = value.ToString();
        }
    }

    public double Critical
    {
        get { return Data.Critical; }
        set { Data.Critical = value; }
    }

    public int Gold
    {
        get { return Data.Gold; }
        set
        {
            Data.Gold = value;
            View.Gold = value.ToString();
        }
    }

    public int Weapon
    {
        get { return Data.Weapon; }
        set { Data.Weapon = value; }
    }

    public int YellowKey
    {
        get { return Data.YellowKey; }
        set
        {
            Data.YellowKey = value;
            View.YellowKey = value.ToString();
        }
    }
    public int BlueKey
    {
        get { return Data.BlueKey; }
        set
        {
            Data.BlueKey = value;
            View.BlueKey = value.ToString();
        }
    }
    public int RedKey
    {
        get { return Data.RedKey; }
        set
        {
            Data.RedKey = value;
            View.RedKey = value.ToString();
        }
    }
    public int GreenKey
    {
        get { return Data.GreenKey; }
        set { Data.GreenKey = value; }
    }

    public Constant.PlayerData PlayerData { get { return Data.Data; } set { Data.Data = value; } }
    public int PlayerPosX { get { return Data.PlayerPosX; } set { Data.PlayerPosX = value; } }
    public int PlayerPosY { get { return Data.PlayerPosY; } set { Data.PlayerPosY = value; } }

    public Vector2Int TargetAutoStep { get; private set; }
    public Stack<Vector2Int> AutoSteppingRoad { get; private set; }
}
