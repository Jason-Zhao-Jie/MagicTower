using System.Collections.Generic;

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

    public int posx = 10;
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
            if (!EventManager.instance.DispatchEvent(block.eventId, ModalManager.GetModalByUuid(uuid)))
                return false;
        }
        if (block.thing != 0)
        {
            var thingData = DataCenter.instance.GetModalById(block.thing);
            if (thingData.eventId != 0)
            {
                if (!EventManager.instance.DispatchEvent(thingData.eventId, ModalManager.GetModalByUuid(uuid)))
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
        return true;
    }

    public int PlayerId
    {
        get { return data.id; }
        set { data = DataCenter.instance.GetPlayerDataById(value); }
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
                player.transform.SetParent(null);
            this.data = DataCenter.instance.GetPlayerDataById(playerId);
            player = null;
        }
        if (player == null || isNew)
        {
            var modalData = DataCenter.instance.GetModalById(playerId);
            var obj = UnityEngine.Object.Instantiate(UnityEngine.Resources.Load(Constant.PREFAB_DIR + modalData.prefabPath) as UnityEngine.GameObject);
            player = obj.GetComponent<Player>();
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
            MainScene.instance.AddObjectToMap(player.gameObject, posx, posy, -4);
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
