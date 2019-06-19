using System.Collections.Generic;
using System.Collections.ObjectModel;

using UnityEngine;

namespace MagicTower.Present.Manager {

    public class InputManager : ArmyAnt.Manager.InputManager {
        public static class VirtualKeyCode {
            public const KeyCode Up = (KeyCode)508;
            public const KeyCode Down = (KeyCode)509;
            public const KeyCode Left = (KeyCode)510;
            public const KeyCode Right = (KeyCode)511;
        }

        private static readonly ReadOnlyDictionary<JoysticsAxes, string> xboxAxesNames = new ReadOnlyDictionary<JoysticsAxes, string>(new Dictionary<JoysticsAxes, string> {
            { JoysticsAxes.LeftHorizontal, "AxisXJoystick" },
            { JoysticsAxes.LeftVertical, "AxisYJoystick" },
            { JoysticsAxes.RightHorizontal, "Axis4Joystick" },
            { JoysticsAxes.RightVertical, "Axis5Joystick" },
            { JoysticsAxes.SpecialHorizontal,"Axis6Joystick" },
            { JoysticsAxes.SpecialVertical,"Axis7Joystick" },
            { JoysticsAxes.LeftTrigger,"Axis9Joystick" },
            { JoysticsAxes.RightTrigger,"Axis10Joystick" },
        });

        private static readonly ReadOnlyDictionary<JoysticsAxes, string> ps4WinAxesNames = new ReadOnlyDictionary<JoysticsAxes, string>(new Dictionary<JoysticsAxes, string> {
            { JoysticsAxes.LeftHorizontal, "AxisXJoystick" },
            { JoysticsAxes.LeftVertical, "AxisYJoystick" },
            { JoysticsAxes.RightHorizontal, "Axis3Joystick" },
            { JoysticsAxes.RightVertical, "Axis6Joystick" },
            { JoysticsAxes.SpecialHorizontal,"Axis7Joystick" },
            { JoysticsAxes.SpecialVertical,"Axis8Joystick" },
            { JoysticsAxes.LeftTrigger,"Axis4Joystick" },
            { JoysticsAxes.RightTrigger,"Axis5Joystick" },
        });

        private static readonly ReadOnlyDictionary<JoysticsAxes, string> ps4MacAxesNames = new ReadOnlyDictionary<JoysticsAxes, string>(new Dictionary<JoysticsAxes, string> {
            { JoysticsAxes.LeftHorizontal, "AxisXJoystick" },
            { JoysticsAxes.LeftVertical, "AxisYJoystick" },
            { JoysticsAxes.RightHorizontal, "Axis3Joystick" },
            { JoysticsAxes.RightVertical, "Axis4Joystick" },
            { JoysticsAxes.SpecialHorizontal,"Axis7Joystick" },
            { JoysticsAxes.SpecialVertical,"Axis8Joystick" },
            { JoysticsAxes.LeftTrigger,"Axis5Joystick" },
            { JoysticsAxes.RightTrigger,"Axis6Joystick" },
        });

        public InputManager() : base(xboxAxesNames, ps4WinAxesNames, ps4MacAxesNames) {
            RegisterKeyListener(KeyCode.Return, OnKeySubmit);
            RegisterKeyListener(KeyCode.Space, OnKeySubmit);
            RegisterKeyListener(KeyCode.KeypadEnter, OnKeySubmit);
            RegisterKeyListener(XboxJoysticsCode.A, OnKeySubmit);
            RegisterKeyListener(KeyCode.Escape, OnKeyCancel);
            RegisterKeyListener(KeyCode.Backspace, OnKeyCancel);
            RegisterKeyListener(AndroidKeyCode.Back, OnKeyCancel);
            RegisterKeyListener(XboxJoysticsCode.B, OnKeyCancel);
            RegisterTouchUpListener(OnBaseTouchUp);
            forceVirtualing = false;
        }

        public Player.Controller.Direction GetArrowState() {
            int horizontal = 0;
            int vertical = 0;
            if(GetKeyStatus(KeyCode.UpArrow) || GetKeyStatus(VirtualKeyCode.Up) || GetAxesStatus(JoysticsAxes.LeftVertical) < -0.1 || GetAxesStatus(JoysticsAxes.RightVertical) < -0.1 || GetAxesStatus(JoysticsAxes.SpecialVertical) > 0.1) {
                ++horizontal;
            }
            if(GetKeyStatus(KeyCode.DownArrow) || GetKeyStatus(VirtualKeyCode.Down) || GetAxesStatus(JoysticsAxes.LeftVertical) > 0.1 || GetAxesStatus(JoysticsAxes.RightVertical) > 0.1 || GetAxesStatus(JoysticsAxes.SpecialVertical) < -0.1) {
                --horizontal;
            }
            if(GetKeyStatus(KeyCode.RightArrow) || GetKeyStatus(VirtualKeyCode.Right) || GetAxesStatus(JoysticsAxes.LeftHorizontal) > 0.1 || GetAxesStatus(JoysticsAxes.RightHorizontal) > 0.1 || GetAxesStatus(JoysticsAxes.SpecialHorizontal) > 0.1) {
                ++vertical;
            }
            if(GetKeyStatus(KeyCode.LeftArrow) || GetKeyStatus(VirtualKeyCode.Left) || GetAxesStatus(JoysticsAxes.LeftHorizontal) < -0.1 || GetAxesStatus(JoysticsAxes.RightHorizontal) < -0.1 || GetAxesStatus(JoysticsAxes.SpecialHorizontal) < -0.1) {
                --vertical;
            }
            if(horizontal > 0) {
                return Player.Controller.Direction.Up;
            } else if(horizontal < 0) {
                return Player.Controller.Direction.Down;
            } else if(vertical > 0) {
                return Player.Controller.Direction.Right;
            } else if(vertical < 0) {
                return Player.Controller.Direction.Left;
            }
            return Player.Controller.Direction.Default;
        }

        public bool CheckVirtualRockerState() {
            Game.DebugLogNote("Virtual key status: ", forceVirtualing, GetKeyStatus(VirtualKeyCode.Up), GetKeyStatus(VirtualKeyCode.Down), GetKeyStatus(VirtualKeyCode.Right), GetKeyStatus(VirtualKeyCode.Left));
            return forceVirtualing || GetKeyStatus(VirtualKeyCode.Up) || GetKeyStatus(VirtualKeyCode.Down) || GetKeyStatus(VirtualKeyCode.Right) || GetKeyStatus(VirtualKeyCode.Left);
        }

        private void OnKeySubmit(bool down) {
            if(!down) {
                return;
            }
            switch(Game.Status) {
                case Model.EGameStatus.Start:
                    Game.HideUI(Components.UIType.TipBar);
                    break;
                case Model.EGameStatus.OnTipChat:
                    Game.ChatStepOn();
                    break;
                case Model.EGameStatus.OnBattleResult:
                    Game.StopBattle();
                    break;
            }
        }

        private void OnKeyCancel(bool down) {
            if(down) {
                return;
            }
            switch(Game.Status) {
                case Model.EGameStatus.Start:
                    Game.ExitGame();
                    break;
                case Model.EGameStatus.InGame:
                    Game.StopAndBackToStart();
                    break;
            }
        }

        private void OnBaseTouchUp(Vector2 end, Vector2 begin, int mouseBtn) {
            if(CheckVirtualRockerState()) {
                return;
            }
            switch(Game.Status) {
                case Model.EGameStatus.Start:
                    Game.HideUI(Components.UIType.TipBar);
                    break;
                case Model.EGameStatus.InGame:
                case Model.EGameStatus.InEditor:
                    Game.Map.ClickMap(end);
                    break;
                case Model.EGameStatus.OnTipChat:
                    Game.ChatStepOn();
                    break;
                case Model.EGameStatus.OnBattleResult:
                    Game.StopBattle();
                    break;
            }
        }

        public bool forceVirtualing;
    }

}
