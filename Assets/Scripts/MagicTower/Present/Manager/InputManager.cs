using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MagicTower.Present.Manager
{

    public class InputManager : ArmyAnt.Manager.InputManager
    {
        private static readonly KeyCode[] listenedKeys = {
            KeyCode.LeftArrow,
            KeyCode.UpArrow,
            KeyCode.RightArrow,
            KeyCode.DownArrow,
            KeyCode.Space,
            KeyCode.Escape,
            KeyCode.P,
            KeyCode.Q,
            KeyCode.Return,
            KeyCode.KeypadEnter,
            KeyCode.Backspace,
            (KeyCode)6, // Android back button
            // TODO: 以下手柄按键目前是Xbox的, 需要改为兼容PS4的
            JoysticsCode.A,
            JoysticsCode.B,
            JoysticsCode.X,
            JoysticsCode.Y,
            JoysticsCode.LeftBumper,
            JoysticsCode.RightBumper,
            JoysticsCode.Back,
            JoysticsCode.Start,
            JoysticsCode.LeftRocker,
            JoysticsCode.RightRocker,
        };

        private static readonly ReadOnlyDictionary<JoysticsAxes, string> axesNames = new ReadOnlyDictionary<JoysticsAxes, string>(new Dictionary<JoysticsAxes, string> {
            { JoysticsAxes.LeftHorizontal, "Horizontal_Left_" },
            { JoysticsAxes.LeftVertical, "Vertical_Left_" },
            { JoysticsAxes.RightHorizontal, "Horizontal_XBoxRight_" },
            { JoysticsAxes.RightVertical, "Vertical_XBoxRight_" },
            { JoysticsAxes.SpecialHorizontal,"Horizontal_XBoxSpecial_" },
            { JoysticsAxes.SpecialVertical,"Vertical_XBoxSpecial_" },
        });

        public InputManager() : base(listenedKeys, axesNames)
        {
        }

        override protected void OnKeyDown(KeyCode keyCode)
        {
            switch (Game.Status)
            {
                case Model.EGameStatus.Start:
                    break;
                case Model.EGameStatus.InGame:
                    switch (keyCode)
                    {
                        case KeyCode.UpArrow:
                        case KeyCode.DownArrow:
                        case KeyCode.LeftArrow:
                        case KeyCode.RightArrow:
                            OnChangeWalkState();
                            break;
                    }
                    break;
                case Model.EGameStatus.InEditor:
                    break;
                case Model.EGameStatus.OnCG:
                    break;
                case Model.EGameStatus.OnTipChat:
                    switch (keyCode)
                    {
                        case KeyCode.Return:
                        case KeyCode.KeypadEnter:
                        case KeyCode.Space:
                        case JoysticsCode.A:
                            Game.ChatStepOn();
                            break;
                    }
                    break;
                case Model.EGameStatus.OnDialog:
                    break;
                case Model.EGameStatus.OnMiddleLoading:
                    break;
                case Model.EGameStatus.OnBattle:
                    break;
                case Model.EGameStatus.OnBattleResult:
                    break;
                case Model.EGameStatus.OnSmallGame:
                    break;
                case Model.EGameStatus.AutoStepping:
                    switch (keyCode)
                    {
                        case KeyCode.UpArrow:
                        case KeyCode.DownArrow:
                        case KeyCode.LeftArrow:
                        case KeyCode.RightArrow:
                            Game.Player.StopAutoStep();
                            OnChangeWalkState();
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        override protected void OnKeyUp(KeyCode keyCode)
        {
            switch (Game.Status)
            {
                case Model.EGameStatus.Start:
                    switch (keyCode)
                    {
                        case KeyCode.Escape:
                        case KeyCode.Backspace:
                        case JoysticsCode.Back:
                            Game.ExitGame();
                            break;
                    }
                    break;
                case Model.EGameStatus.InGame:
                    switch (keyCode)
                    {
                        case KeyCode.UpArrow:
                        case KeyCode.DownArrow:
                        case KeyCode.LeftArrow:
                        case KeyCode.RightArrow:
                            OnChangeWalkState();
                            break;
                        case KeyCode.Escape:
                        case KeyCode.Backspace:
                        case JoysticsCode.Back:
                            Game.StopAndBackToStart();
                            break;
                    }
                    break;
                case Model.EGameStatus.InEditor:
                    break;
                case Model.EGameStatus.OnCG:
                    break;
                case Model.EGameStatus.OnTipChat:
                    break;
                case Model.EGameStatus.OnDialog:
                    break;
                case Model.EGameStatus.OnMiddleLoading:
                    break;
                case Model.EGameStatus.OnBattle:
                    break;
                case Model.EGameStatus.OnBattleResult:
                    switch (keyCode)
                    {
                        case KeyCode.Return:
                        case KeyCode.KeypadEnter:
                        case KeyCode.Space:
                        case JoysticsCode.A:
                            Game.StopBattle();
                            break;
                    }
                    break;
                case Model.EGameStatus.OnSmallGame:
                    break;
                case Model.EGameStatus.AutoStepping:
                    switch (keyCode)
                    {
                        case KeyCode.UpArrow:
                        case KeyCode.DownArrow:
                        case KeyCode.LeftArrow:
                        case KeyCode.RightArrow:
                            Game.Player.StopAutoStep();
                            OnChangeWalkState();
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        override protected void OnTouchDown(Vector2 touchedPos)
        {
            switch (Game.Status)
            {
                case Model.EGameStatus.Start:
                    break;
                case Model.EGameStatus.InGame:
                case Model.EGameStatus.InEditor:
                    Game.Map.ClickMap(touchedPos);
                    break;
                case Model.EGameStatus.OnCG:
                    break;
                case Model.EGameStatus.OnTipChat:
                    Game.ChatStepOn();
                    break;
                case Model.EGameStatus.OnDialog:
                    break;
                case Model.EGameStatus.OnMiddleLoading:
                    break;
                case Model.EGameStatus.OnBattle:
                    break;
                case Model.EGameStatus.OnBattleResult:
                    Game.StopBattle();
                    break;
                case Model.EGameStatus.OnSmallGame:
                    break;
                case Model.EGameStatus.AutoStepping:
                    Game.Player.StopAutoStep();
                    break;
                default:
                    break;
            }
        }

        override protected void OnTouchUp(Vector2 end, Vector2 begin)
        {
            // TODO
        }

        override protected void OnJoysticsRockerAxes(JoysticsAxes keyCode, float value, float oldValue)
        {
            switch (keyCode)
            {
                case JoysticsAxes.LeftHorizontal:
                case JoysticsAxes.LeftVertical:
                case JoysticsAxes.RightHorizontal:
                case JoysticsAxes.RightVertical:
                case JoysticsAxes.SpecialHorizontal:
                case JoysticsAxes.SpecialVertical:
                    if ((oldValue > 0.1 && value < 0.1) || (oldValue < -0.1 && value > -0.1) || (Mathf.Abs(oldValue) < 0.1 && Mathf.Abs(value) > 0.1))
                    {
                        switch (Game.Status)
                        {
                            case Model.EGameStatus.InGame:
                                OnChangeWalkState();
                                break;
                            case Model.EGameStatus.AutoStepping:
                                Game.Player.StopAutoStep();
                                break;
                        }
                    }
                    break;
            }
        }

        public void OnChangeWalkState()
        {
            if (Game.Status == Model.EGameStatus.InEditor)
            {
                return;
            }
            else if (Game.Status == Model.EGameStatus.InGame)
            {
                ;
                bool up = GetKeyStatus(KeyCode.UpArrow) || GetAxesStatus(JoysticsAxes.LeftVertical) < -0.1 || GetAxesStatus(JoysticsAxes.RightVertical) < -0.1 || GetAxesStatus(JoysticsAxes.SpecialVertical) > 0.1;
                bool down = GetKeyStatus(KeyCode.DownArrow) || GetAxesStatus(JoysticsAxes.LeftVertical) > 0.1 || GetAxesStatus(JoysticsAxes.RightVertical) > 0.1 || GetAxesStatus(JoysticsAxes.SpecialVertical) < -0.1;
                bool right = GetKeyStatus(KeyCode.RightArrow) || GetAxesStatus(JoysticsAxes.LeftHorizontal) > 0.1 || GetAxesStatus(JoysticsAxes.RightHorizontal) > 0.1 || GetAxesStatus(JoysticsAxes.SpecialHorizontal) > 0.1;
                bool left = GetKeyStatus(KeyCode.LeftArrow) || GetAxesStatus(JoysticsAxes.LeftHorizontal) < -0.1 || GetAxesStatus(JoysticsAxes.RightHorizontal) < -0.1 || GetAxesStatus(JoysticsAxes.SpecialHorizontal) < -0.1;
                int trueNum = (up ? 11 : 0) + (down ? 21 : 0) + (right ? 31 : 0) + (left ? 41 : 0);
                if (trueNum % 10 != 1)
                    Game.Player.StopWalk();
                else
                    Game.Player.StartWalk((Player.Controller.Direction)(trueNum / 10));
            }
            else if (Game.Status != Model.EGameStatus.AutoStepping)
            {
                Game.Player.StopWalk();
            }
        }
    }

}
