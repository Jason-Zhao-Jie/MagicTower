using System.Collections.Generic;
using UnityEngine;

namespace MagicTower.Present.Player
{

    public class Controller : ArmyAnt.Base.AController<Data, View>
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

        public Controller(View playerPanel)
        {
            InitDataAndView(new Data(this), playerPanel);
            View.Controller = this;
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
            Data.InitPlayerData(posx, posy);
            var player = View.ShowPlayer(playerId, isNew);
            Game.Map.AddPlayerToMap(player, posx, posy);

            if (isNew)
            {
                SyncPlayerData();
            }
        }

        public void ShowPlayer(int posx, int posy, Model.PlayerData playerData, bool isNew = false)
        {
            Data.PlayerData = playerData;
            Data.InitPlayerData(posx, posy);
            var player = View.ShowPlayer(playerData.id, isNew);
            Game.Map.AddPlayerToMap(player, posx, posy);
            SyncPlayerData();
        }

        public void SyncPlayerData()
        {
            if (!View.HasStarted || Data.PlayerId == 0)
            {
                return;
            }
            View.MapName = Game.Config.StringInternational.GetValue(Game.Map.CurrentMap.mapName, Game.Map.CurrentMap.mapNameParam.ToString());
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

        public Direction GoToNextBlock(Direction dir)
        {
            int targetPosX = PlayerPosX;
            int targetPosY = PlayerPosY;
            if (Game.Status == Model.EGameStatus.AutoStepping)
            {
                if (AutoSteppingRoad.Count <= 0)
                {
                    View.Player.StopAutoStep();
                    return Direction.Default;
                }
                var target = AutoSteppingRoad.Pop();
                targetPosX = target.x;
                targetPosY = target.y;
                if (targetPosX == PlayerPosX)
                {
                    if (targetPosY > PlayerPosY)
                        dir = Direction.Up;
                    else
                        dir = Direction.Down;
                }
                else
                {
                    if (targetPosX > PlayerPosX)
                        dir = Direction.Right;
                    else
                        dir = Direction.Left;
                }
            }
            else
            {
                switch (dir)
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
            if (targetPosY >= Map.View.MAP_BLOCK_LENGTH || targetPosY < 0 || targetPosX >= Map.View.MAP_BLOCK_LENGTH || targetPosX < 0)
            {
                return Direction.Default;
            }

            // Check map event and thing event
            var block = Game.Map.CurrentMap.blocks[targetPosX][targetPosY];
            long uuid = Components.Unit.Modal.GetUuid(Game.Map.CurrentMap.mapId, targetPosX, targetPosY);
            if (block.eventId != 0)
            {
                if (Game.Status == Model.EGameStatus.AutoStepping)
                    Game.Status = Model.EGameStatus.InGame;
                if (!Manager.EventManager.DispatchEvent(block.eventId, Game.Map.GetModalByUuid(uuid), block.eventData))
                    return Direction.Default;
            }
            if (block.thing != 0)
            {
                var thingData = Game.Config.modals[block.thing];
                if (thingData.eventId != 0)
                {
                    if (Game.Status == Model.EGameStatus.AutoStepping)
                        Game.Status = Model.EGameStatus.InGame;
                    if (!Manager.EventManager.DispatchEvent(thingData.eventId, Game.Map.GetModalByUuid(uuid), thingData.eventData))
                        return Direction.Default;
                }
                switch ((Components.Unit.ModalType)thingData.typeId)
                {
                    case Components.Unit.ModalType.Walkable:
                        break;
                    default:
                        return Direction.Default;
                }
            }
            Game.Map.CurrentMap.blocks[targetPosX][targetPosY] = block;

            PlayerPosX = targetPosX;
            PlayerPosY = targetPosY;
            Manager.AudioManager.PlaySound(Manager.AudioManager.stepSound);
            return dir;
        }

        public bool StartAutoStep(int targetPosx, int targetPosy)
        {
            var findedRoad = Model.MathHelper.AutoFindBestRoad(Game.Map.ConvertCurrentMapToFinderArray(), PlayerPosX, PlayerPosY, targetPosx, targetPosy);
            if (findedRoad == null || findedRoad.Length <= 0)
            {
                Manager.AudioManager.PlaySound(Manager.AudioManager.disableSound);
                return false;
            }
            TargetAutoStep = new Vector2Int(targetPosx, targetPosy);
            AutoSteppingRoad = new Stack<Vector2Int>();
            for (int i = findedRoad.Length - 1; i > 0; --i)
            {
                AutoSteppingRoad.Push(findedRoad[i]);
            }
            View.Player.StartAutoStep();
            return true;
        }

        public bool CheckPlayerData(Model.ResourceType type, int minValue)
        {
            return Data.CheckPlayerData(type, minValue);
        }

        public bool CheckPlayerData(Model.ResourceType type, Model.IntegerBoolCallBack checkFunc)
        {
            return Data.CheckPlayerData(type, checkFunc);
        }

        public void ChangePlayerData(Model.ResourceType type, int count)
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

        public Model.PlayerData PlayerData { get { return Data.PlayerData; } set { Data.PlayerData = value; } }
        public int PlayerPosX { get { return Data.PlayerPosX; } set { Data.PlayerPosX = value; } }
        public int PlayerPosY { get { return Data.PlayerPosY; } set { Data.PlayerPosY = value; } }


        public Vector2Int TargetAutoStep { get; private set; }
        public Stack<Vector2Int> AutoSteppingRoad { get; private set; }
    }

}