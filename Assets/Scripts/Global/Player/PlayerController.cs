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

    public PlayerController()
    {
        InitDataAndView(new PlayerData(this), new PlayerView(this));
    }

    public void ShowPlayer(bool isNew)
    {
        ShowPlayer(9, 1, 0, isNew);
    }

    public void ShowPlayer(int posx, int posy, int playerId = 0, bool isNew = false)
    {

        if (playerId == 0)
            playerId = Data.PlayerId;
        if (playerId == 0)
        {
            Data.SetPlayerInfo(DEFALUT_PLAYER_ID);
            playerId = DEFALUT_PLAYER_ID;
        }
        else if (playerId != Data.PlayerId)
        {
            if (player != null)
                player.RemoveSelf();
            Data.SetPlayerInfo(playerId);
            player = null;
        }
        Data.InitPlayerData(posx, posy, playerId);
        var modalData = Game.Data.Config.modals[playerId];
        if (player == null || isNew)
        {
            player = Game.View.ObjPool.GetAnElement<Player>(modalData.id, ObjectPool.ElementType.Sprite, Constant.PREFAB_DIR + modalData.prefabPath);
            player.MainPlayer = true;
        }

        // Set Scene Texts
        MainScene.instance.AddObjectToMap(player.gameObject, posx, posy, -4);
        MainScene.instance.RoleName = Game.Data.Config.StringInternational.GetValue(modalData.name);
        MainScene.instance.Portrait = player.BaseSprite;

        if (isNew)
        {
            Data.SyncPlayerData();
        }
    }

    public bool GoToNextBlock()
    {
        return Data.GoToNextBlock();
    }

    public void StartWalk(PlayerController.Direction dir = PlayerController.Direction.Default)
    {
        Data.StartWalk(dir);
    }

    public void StopWalk()
    {
        Data.StopWalk();
    }

    public bool StartAutoStep(int targetPosx, int targetPosy)
    {
        return Data.StartAutoStep(targetPosx, targetPosy);
    }

    public void StopAutoStepping()
    {
        Data.StopAutoStepping();
    }

    public void ChangePlayerData(Constant.ResourceType type, int count)
    {
        Data.ChangePlayerData(type, count);
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
            MainScene.instance.Level = value.ToString();
        }
    }

    public int Experience
    {
        get { return Data.Experience; }
        set
        {
            Data.Experience = value;
            MainScene.instance.Experience = value.ToString();
        }
    }

    public int Life
    {
        get { return Data.Life; }
        set
        {
            Data.Life = value;
            MainScene.instance.Life = value.ToString();
        }
    }

    public int Attack
    {
        get { return Data.Attack; }
        set
        {
            Data.Attack = value;
            MainScene.instance.Attack = value.ToString();
        }
    }

    public int Defense
    {
        get { return Data.Defense; }
        set
        {
            Data.Defense = value;
            MainScene.instance.Defense = value.ToString();
        }
    }

    public int Speed
    {
        get { return Data.Speed; }
        set
        {
            Data.Speed = value;
            MainScene.instance.Speed = value.ToString();
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
            MainScene.instance.Gold = value.ToString();
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
            MainScene.instance.YellowKey = value.ToString();
        }
    }
    public int BlueKey
    {
        get { return Data.BlueKey; }
        set
        {
            Data.BlueKey = value;
            MainScene.instance.BlueKey = value.ToString();
        }
    }
    public int RedKey
    {
        get { return Data.RedKey; }
        set
        {
            Data.RedKey = value;
            MainScene.instance.RedKey = value.ToString();
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

    public bool DirChanged { get { return Data.dirChanged; } set { Data.dirChanged = value; } }
    public bool IsRunning { get { return Data.IsRunning; } set { Data.IsRunning = value; } }
    public Direction Dir { get { return Data.Dir; } set { Data.Dir = value; } }

    private Player player;
}
