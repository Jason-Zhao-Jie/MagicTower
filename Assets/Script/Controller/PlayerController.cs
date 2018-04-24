using System.Collections.Generic;
using UnityEngine;

public class PlayerController
{
    public static PlayerController instance = null;

    public enum Direction
    {
        Default,
        Up,
        Down,
        Right,
        Left
    }

    public Constant.PlayerData data;

    public Dictionary<int, bool> items = new Dictionary<int, bool>();

    public int posx = 9;
    public int posy = 1;

    public bool GoToNextBlock()
    {
        int targetPosX = posx;
        int targetPosY = posy;
        switch (direction)
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

        // Check if the player is at the condition
        if (targetPosY >= Constant.MAP_BLOCK_LENGTH || targetPosY < 0 || targetPosX >= Constant.MAP_BLOCK_LENGTH || targetPosX < 0)
        {
            return false;
        }

        // Check map event and thing event
        var block = MapManager.instance.CurrentMap.mapBlocks[targetPosX][targetPosY];
        long uuid = MapManager.instance.CurrentMap.mapId * 10000 + targetPosY + targetPosX * 100;
        if (block.eventId != 0)
        {
            if (!EventManager.instance.DispatchEvent(block.eventId, MapManager.instance.GetModalByUuid(uuid), block.eventData))
                return false;
        }
        if (block.thing != 0)
        {
            var thingData = DataCenter.instance.modals[block.thing];
            if (thingData.eventId != 0)
            {
                if (!EventManager.instance.DispatchEvent(thingData.eventId, MapManager.instance.GetModalByUuid(uuid), thingData.eventData))
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

        posx = targetPosX;
        posy = targetPosY;
        AudioController.instance.PlaySound(AudioController.stepSound);
        return true;
    }

    public int PlayerId
    {
        get { return data.id; }
    }

    public bool SetPlayerInfo(int id)
    {
        data = DataCenter.instance.players[id];
        return true;
    }

    public int Level
    {
        get { return data.level; }
        set
        {
            data.level = value;
            MainScene.instance.Level = value.ToString();
        }
    }
    public int Experience
    {
        get { return data.exp; }
        set
        {
            data.exp = value;
            MainScene.instance.Experience = value.ToString();
        }
    }
    public int Life
    {
        get { return data.life; }
        set
        {
            data.life = value;
            MainScene.instance.Life = value.ToString();
        }
    }
    public int Attack
    {
        get { return data.attack; }
        set
        {
            data.attack = value;
            MainScene.instance.Attack = value.ToString();
        }
    }
    public int Defense
    {
        get { return data.defense; }
        set
        {
            data.defense = value;
            MainScene.instance.Defense = value.ToString();
        }
    }
    public int Speed
    {
        get { return data.speed; }
        set
        {
            data.speed = value;
            MainScene.instance.Speed = value.ToString();
        }
    }
    public double Critical
    {
        get { return data.critical; }
        set { data.critical = value; }
    }

    public int Gold
    {
        get { return data.gold; }
        set
        {
            data.gold = value;
            MainScene.instance.Gold = value.ToString();
        }
    }
    public int Weapon
    {
        get { return data.weaponId; }
        set { data.weaponId = value; }
    }
    public int YellowKey
    {
        get { return data.yellowKey; }
        set
        {
            data.yellowKey = value;
            MainScene.instance.YellowKey = value.ToString();
        }
    }
    public int BlueKey
    {
        get { return data.blueKey; }
        set
        {
            data.blueKey = value;
            MainScene.instance.BlueKey = value.ToString();
        }
    }
    public int RedKey
    {
        get { return data.redKey; }
        set
        {
            data.redKey = value;
            MainScene.instance.RedKey = value.ToString();
        }
    }
    public int GreenKey
    {
        get { return data.greenKey; }
        set { data.greenKey = value; }
    }


    public Constant.PlayerData PlayerData
    {
        get
        {
            return data;
        }
        set
        {
            data = value;
        }
    }

    public void ShowPlayer(bool isNew)
    {
        ShowPlayer(-1, -1, 0, isNew);
    }

    public void ShowPlayer(int posx = -1, int posy = -1, int playerId = 0, bool isNew = false)
    {
        if (playerId == 0)
            playerId = this.data.id;
        else if (playerId != this.data.id)
        {
            if (player != null)
                player.RemoveSelf();
            SetPlayerInfo(playerId);
            player = null;
        }
        var modalData = DataCenter.instance.modals[playerId];
        if (player == null || isNew)
        {
            player = ObjectPool.instance.GetAnElement<Player>(modalData.id, ObjectPool.ElementType.Sprite, Constant.PREFAB_DIR + modalData.prefabPath);
            player.MainPlayer = true;
        }
        if (posx < 0 || posx >= Constant.MAP_BLOCK_LENGTH)
            posx = this.posx;
        else
            this.posx = posx;
        if (posx < 0 || posx >= Constant.MAP_BLOCK_LENGTH)
        {
            posx = 0;
            this.posx = 0;
        }
        if (posy < 0 || posy >= Constant.MAP_BLOCK_LENGTH)
            posy = this.posy;
        else
            this.posy = posy;
        if (posy < 0 || posy >= Constant.MAP_BLOCK_LENGTH)
        {
            posy = 0;
            this.posy = 0;
        }

        // Set Scene Texts
        MainScene.instance.AddObjectToMap(player.gameObject, posx, posy, -4);
        MainScene.instance.RoleName = modalData.name;
        MainScene.instance.Portrait = player.GetComponent<SpriteRenderer>().sprite;
    }

    public void SyncPlayerData()
    {
        MainScene.instance.Level = data.level.ToString();
        MainScene.instance.Experience = data.exp.ToString();
        MainScene.instance.Life = data.life.ToString();
        MainScene.instance.Attack = data.attack.ToString();
        MainScene.instance.Defense = data.defense.ToString();
        MainScene.instance.Speed = data.speed.ToString();
        MainScene.instance.Gold = data.gold.ToString();
        MainScene.instance.YellowKey = data.yellowKey.ToString();
        MainScene.instance.BlueKey = data.blueKey.ToString();
        MainScene.instance.RedKey = data.redKey.ToString();
    }

    public void ChangePlayerData(Constant.ResourceType type, int count)
    {
        switch (type)
        {
            case Constant.ResourceType.Life:
                data.life += count;
                break;
            case Constant.ResourceType.Attack:
                data.attack += count;
                break;
            case Constant.ResourceType.Defense:
                data.defense += count;
                break;
            case Constant.ResourceType.Level:
                // 升级需要特殊处理
                break;
            case Constant.ResourceType.Experience:
                data.exp += count;
                break;
            case Constant.ResourceType.Speed:
                data.speed += count;
                break;
            case Constant.ResourceType.Critical:
                data.critical += count;
                break;
            case Constant.ResourceType.Gold:
                data.gold += count;
                break;
            case Constant.ResourceType.YellowKey:
                data.yellowKey += count;
                break;
            case Constant.ResourceType.BlueKey:
                data.blueKey += count;
                break;
            case Constant.ResourceType.RedKey:
                data.redKey += count;
                break;
            case Constant.ResourceType.GreenKey:
                data.greenKey += count;
                break;
            default:
                return;
        }
        SyncPlayerData();
    }

    public void StartWalk(Direction dir = Direction.Default)
    {
        if (dir != Direction.Default)
            Dir = dir;
        IsRunning = true;
    }

    public void StopWalk()
    {
        IsRunning = false;
    }


    public Direction Dir
    {
        get { return direction; }
        set
        {
            direction = value;
        }
    }

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

    public bool dirChanged = false;

    private bool isRunning;
    private Direction direction;
    private Player player;
}
