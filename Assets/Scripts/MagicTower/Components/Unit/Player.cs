using UnityEngine;
using ArmyAnt.ViewUtil;
using MagicTower.Present.Player;

namespace MagicTower.Components.Unit
{

    public class Player : ObjectPool.AViewUnit
    {
        const int RUN_SPEED = 10;

        void Start()
        {
            runningTime = 0;
            movedLength = new Vector2(transform.lossyScale.x * Present.Map.View.MAP_BLOCK_BASE_SIZE / 100, transform.lossyScale.y * Present.Map.View.MAP_BLOCK_BASE_SIZE / 100);
        }

        void Update()
        {
            if (dirChanged && MainPlayer)
            {
                switch (Dir)
                {
                    case Controller.Direction.Up:
                        Animator.Play(IsRunning ? "Up" : "Up_Stand");
                        break;
                    case Controller.Direction.Down:
                        Animator.Play(IsRunning ? "Down" : "Down_Stand");
                        break;
                    case Controller.Direction.Left:
                        Animator.Play(IsRunning ? "Left" : "Left_Stand");
                        break;
                    case Controller.Direction.Right:
                        Animator.Play(IsRunning ? "Right" : "Right_Stand");
                        break;
                }
                dirChanged = false;
            }
        }

        void FixedUpdate()
        {
            if (IsRunning && MainPlayer)
            {
                if (runningTime < RUN_SPEED)
                    ++runningTime;
                else
                {
                    runningTime = 0;
                    if (Game.Player.GoToNextBlock())
                    {
                        var posController = transform;
                        switch (Dir)
                        {
                            case Controller.Direction.Up:
                                posController.position = new Vector3(posController.position.x, posController.position.y + movedLength.y, posController.position.z);
                                break;
                            case Controller.Direction.Down:
                                posController.position = new Vector3(posController.position.x, posController.position.y - movedLength.y, posController.position.z);
                                break;
                            case Controller.Direction.Left:
                                posController.position = new Vector3(posController.position.x - movedLength.x, posController.position.y, posController.position.z);
                                break;
                            case Controller.Direction.Right:
                                posController.position = new Vector3(posController.position.x + movedLength.x, posController.position.y, posController.position.z);
                                break;
                        }
                    }
                }
            }
            else
            {
                runningTime = RUN_SPEED - 1;
            }
        }

        public bool MainPlayer { get; set; }

        public void RemoveSelf()
        {
            Game.ObjPool.RecycleAnElement(this);
        }

        public override ObjectPool.ElementType GetPoolTypeId()
        {
            return ObjectPool.ElementType.Sprite;
        }

        public string PrefabPath
        {
            get
            {
                return Game.Config.modals[playerId].prefabPath;
            }
        }

        public override string ResourcePath
        {
            get
            {
                return GetResourcePath(playerId);
            }
        }

        public Sprite BaseSprite
        {
            get
            {
                return GetResourceBaseSprite(playerId);
            }
        }

        public static string GetResourcePath(int playerId)
        {
            return Modal.GetResourcePath(playerId);
        }

        public static string GetResourcePath(string prefabPath)
        {
            return Modal.GetResourcePath(prefabPath);
        }

        public static Sprite GetResourceBaseSprite(int playerId)
        {
            return Modal.GetResourceBaseSprite(playerId);
        }

        public void SetPlayerData(int id)
        {
            playerId = id;
        }

        public override bool RecycleSelf()
        {
            return Game.ObjPoolRecycleSelf(this);
        }

        public override bool OnCreate(ObjectPool.ElementType tid, int elemId, string resourcePath)
        {
            SetPlayerData(elemId);
            return true;
        }

        public override void OnReuse(ObjectPool.ElementType tid, int elemId)
        {
        }

        public override bool OnUnuse(ObjectPool.ElementType tid, int elemId)
        {
            MainPlayer = false;
            return true;
        }

        public Animator Animator { get { return GetComponent<Animator>(); } }


        public Controller.Direction Dir
        {
            get { return dir; }
            set
            {
                dirChanged = dir != value;
                dir = value;
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

        private bool isRunning;
        private Controller.Direction dir;

        private bool dirChanged;
        private int runningTime;
        private Vector2 movedLength;
        private int playerId;
    }

}