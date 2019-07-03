using UnityEngine;
using ArmyAnt.ViewUtil;
using MagicTower.Present.Player;
using ArmyAnt.Manager;

namespace MagicTower.Components.Unit {

    public class Player : ObjectPool.AViewUnit {
        const int RUN_SPEED = 10;

        void Start() {
            runningTime = 0;
            movedLength = transform.lossyScale * Present.Map.View.MAP_BLOCK_BASE_SIZE / 100;
            direction = Controller.Direction.Down;
        }

        void FixedUpdate() {
            if(UpdateKeyArrow() || UpdatePosition(1)) {
                // 调整动画
                GetComponent<Animator>().Play(direction.ToString() + (isRunning ? "" : "_Stand"));
            }
        }

        private void OnEnable() {
            listeningKeys = new int[] {
                Game.Input.RegisterKeyListener(KeyCode.UpArrow, OnKeyArrow),
                Game.Input.RegisterKeyListener(KeyCode.DownArrow, OnKeyArrow),
                Game.Input.RegisterKeyListener(KeyCode.LeftArrow, OnKeyArrow),
                Game.Input.RegisterKeyListener(KeyCode.RightArrow, OnKeyArrow),
                Game.Input.RegisterKeyListener(Present.Manager.InputManager.VirtualKeyCode.Up, OnKeyArrow),
                Game.Input.RegisterKeyListener(Present.Manager.InputManager.VirtualKeyCode.Down, OnKeyArrow),
                Game.Input.RegisterKeyListener(Present.Manager.InputManager.VirtualKeyCode.Left, OnKeyArrow),
                Game.Input.RegisterKeyListener(Present.Manager.InputManager.VirtualKeyCode.Right, OnKeyArrow),
            };
            listeningAxis = new int[] {
                Game.Input.RegisterJoysticksAxisListener(InputManager.JoysticsAxes.LeftHorizontal, OnJoysticsRockerAxes),
                Game.Input.RegisterJoysticksAxisListener(InputManager.JoysticsAxes.LeftVertical, OnJoysticsRockerAxes),
                Game.Input.RegisterJoysticksAxisListener(InputManager.JoysticsAxes.RightHorizontal, OnJoysticsRockerAxes),
                Game.Input.RegisterJoysticksAxisListener(InputManager.JoysticsAxes.RightVertical, OnJoysticsRockerAxes),
                Game.Input.RegisterJoysticksAxisListener(InputManager.JoysticsAxes.SpecialHorizontal, OnJoysticsRockerAxes),
                Game.Input.RegisterJoysticksAxisListener(InputManager.JoysticsAxes.SpecialVertical, OnJoysticsRockerAxes),
            };
        }

        private void OnDisable() {
            if(Game.Input != null) {
                Game.Input.UnregisterKeyListener(KeyCode.UpArrow, listeningKeys[0]);
                Game.Input.UnregisterKeyListener(KeyCode.DownArrow, listeningKeys[1]);
                Game.Input.UnregisterKeyListener(KeyCode.LeftArrow, listeningKeys[2]);
                Game.Input.UnregisterKeyListener(KeyCode.RightArrow, listeningKeys[3]);
                Game.Input.UnregisterKeyListener(Present.Manager.InputManager.VirtualKeyCode.Up, listeningKeys[4]);
                Game.Input.UnregisterKeyListener(Present.Manager.InputManager.VirtualKeyCode.Down, listeningKeys[5]);
                Game.Input.UnregisterKeyListener(Present.Manager.InputManager.VirtualKeyCode.Left, listeningKeys[6]);
                Game.Input.UnregisterKeyListener(Present.Manager.InputManager.VirtualKeyCode.Right, listeningKeys[7]);
                Game.Input.UnregisterJoysticksAxisListener(InputManager.JoysticsAxes.LeftHorizontal, listeningAxis[0]);
                Game.Input.UnregisterJoysticksAxisListener(InputManager.JoysticsAxes.LeftVertical, listeningAxis[1]);
                Game.Input.UnregisterJoysticksAxisListener(InputManager.JoysticsAxes.RightHorizontal, listeningAxis[2]);
                Game.Input.UnregisterJoysticksAxisListener(InputManager.JoysticsAxes.RightVertical, listeningAxis[3]);
                Game.Input.UnregisterJoysticksAxisListener(InputManager.JoysticsAxes.SpecialHorizontal, listeningAxis[4]);
                Game.Input.UnregisterJoysticksAxisListener(InputManager.JoysticsAxes.SpecialVertical, listeningAxis[5]);
            }
            Game.Status = Model.EGameStatus.InGame;
        }

        public void StartAutoStep() {
            Game.Status = Model.EGameStatus.AutoStepping;
            isRunning = true;
        }

        public void StopAutoStep() {
            isRunning = false;
            GetComponent<Animator>().Play(direction.ToString() + "_Stand");
            Game.Status = Model.EGameStatus.InGame;
        }

        private void OnKeyArrow(bool down) {
            switch(Game.Status) {
                case Model.EGameStatus.AutoStepping:
                    StopAutoStep();
                    break;
            }
        }

        private void OnJoysticsRockerAxes(int joystickIndex, float value, float oldValue) {
            switch(Game.Status) {
                case Model.EGameStatus.AutoStepping:
                    StopAutoStep();
                    break;
            }
        }

        private bool UpdateKeyArrow() {
            // 检测按键状态
            var dirChanged = false;
            switch(Game.Status) {
                case Model.EGameStatus.InGame:
                    var newdir = Game.Input.GetArrowState();
                    if(newdir == Controller.Direction.Default) {
                        dirChanged = isRunning;
                        isRunning = false;
                    } else if(!isRunning || newdir != direction) {
                        direction = newdir;
                        dirChanged = true;
                        isRunning = true;
                    }
                    break;
                case Model.EGameStatus.Start:
                case Model.EGameStatus.AutoStepping:
                case Model.EGameStatus.InEditor:
                    break;
                default:
                    dirChanged = isRunning;
                    isRunning = false;
                    break;
            }
            return dirChanged;
        }

        private bool UpdatePosition(int increase) {
            // 调整位置
            var dirChanged = false;
            if(!isRunning) {
                runningTime = RUN_SPEED - 1;
            } else if(runningTime < RUN_SPEED) {
                runningTime += increase;
            } else {
                runningTime = 0;
                var newDir = Game.Player.GoToNextBlock(direction);
                if(newDir != Controller.Direction.Default) {
                    dirChanged = newDir != direction;
                    direction = newDir;
                    switch(direction) {
                        case Controller.Direction.Up:
                            transform.position = new Vector3(transform.position.x, transform.position.y + movedLength.y, transform.position.z);
                            break;
                        case Controller.Direction.Down:
                            transform.position = new Vector3(transform.position.x, transform.position.y - movedLength.y, transform.position.z);
                            break;
                        case Controller.Direction.Left:
                            transform.position = new Vector3(transform.position.x - movedLength.x, transform.position.y, transform.position.z);
                            break;
                        case Controller.Direction.Right:
                            transform.position = new Vector3(transform.position.x + movedLength.x, transform.position.y, transform.position.z);
                            break;
                    }
                }
            }
            return dirChanged;
        }

        public override ObjectPool.ElementType GetPoolTypeId() {
            return ObjectPool.ElementType.Player;
        }

        public Sprite BaseSprite => GetComponent<SpriteRenderer>().sprite;
        
        public override bool OnCreate<T>(ObjectPool.ElementType tid, int elemId, T data, params object[] para) {
            OnInit(tid, elemId, data, para);
            return true;
        }

        public override void OnInit<T>(ObjectPool.ElementType tid, int elemId, T data, params object[] para) {
            playerId = elemId;
        }

        public override bool OnUnuse(ObjectPool.ElementType tid, int elemId) {
            return true;
        }
        
        private bool isRunning;
        private Controller.Direction direction;
        private int runningTime;
        private Vector2 movedLength;
        private int playerId;

        private int[] listeningKeys;
        private int[] listeningAxis;
    }

}
