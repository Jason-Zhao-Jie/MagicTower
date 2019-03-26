using UnityEngine;
using System.Collections.Generic;

namespace MagicTower.Present.Manager
{

    public static class InputManager
    {
        public static class JoysticsCode
        {
            public const KeyCode A = KeyCode.Joystick1Button0;
            public const KeyCode B = KeyCode.Joystick1Button1;
            public const KeyCode X = KeyCode.Joystick1Button2;
            public const KeyCode Y = KeyCode.Joystick1Button3;
            public const KeyCode LeftBumper = KeyCode.Joystick1Button4;
            public const KeyCode RightBumper = KeyCode.Joystick1Button5;
            public const KeyCode Back = KeyCode.Joystick1Button6;
            public const KeyCode Start = KeyCode.Joystick1Button7;
            public const KeyCode LeftRocker = KeyCode.Joystick1Button8;    // 左摇杆按下
            public const KeyCode RightRocker = KeyCode.Joystick1Button9;   // 右摇杆按下
        };

        public enum JoysticsAxes
        {
            LeftHorizontal, // 左摇杆X轴
            LeftVertical,   // 左摇杆Y轴
            RightHorizontal,// 右摇杆X轴
            RightVertical,  // 右摇杆Y轴
            SpecialHorizontal,// 十字键X轴
            SpecialVertical,  // 十字键Y轴
        }

        public static readonly KeyCode[] listenedKeys = {
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

        static InputManager()
        {
            keyStatusMap = new Dictionary<KeyCode, bool>();
            for (int i = 0; i < listenedKeys.Length; ++i)
            {
                Game.DebugLog("On key listener adding: " + listenedKeys[i].ToString());
                keyStatusMap.Add(listenedKeys[i], false);
            }
            axesStatusMap = new Dictionary<JoysticsAxes, float>
            {
                [JoysticsAxes.LeftHorizontal] = 0,
                [JoysticsAxes.LeftVertical] = 0,
                [JoysticsAxes.RightHorizontal] = 0,
                [JoysticsAxes.RightVertical] = 0,
                [JoysticsAxes.SpecialHorizontal] = 0,
                [JoysticsAxes.SpecialVertical] = 0,
            };
        }

        public static void OnKeyDown(KeyCode keyCode)
        {
            keyStatusMap[keyCode] = true;

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
                            (Game.CurrentScene as Components.Scene.MainScene)?.ChatStepOn();
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

        public static void OnKeyUp(KeyCode keyCode)
        {
            keyStatusMap[keyCode] = false;

            switch (Game.Status)
            {
                case Model.EGameStatus.Start:
                    switch (keyCode)
                    {
                        case KeyCode.Escape:
                        case KeyCode.Backspace:
                        case JoysticsCode.B:
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
                        case JoysticsCode.B:
                            Game.CurrentScene.BackToStartScene();
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
                            (Game.CurrentScene as Components.Scene.MainScene)?.StopBattle();
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

        public static void OnTouchDown(Vector2 touchedPos, bool changeMouseStatue)
        {
            OnTouchDown(touchedPos);
            isMouseLeftDown = changeMouseStatue;
        }
        public static void OnTouchDown(Vector2 touchedPos)
        {
            switch (Game.Status)
            {
                case Model.EGameStatus.Start:
                    break;
                case Model.EGameStatus.InGame:
                case Model.EGameStatus.InEditor:
                    //MainScene.instance.ShowTips("Click Called !");
                    Game.Map.ClickMap(touchedPos);
                    break;
                case Model.EGameStatus.OnCG:
                    break;
                case Model.EGameStatus.OnTipChat:
                    (Game.CurrentScene as Components.Scene.MainScene)?.ChatStepOn();
                    break;
                case Model.EGameStatus.OnDialog:
                    break;
                case Model.EGameStatus.OnMiddleLoading:
                    break;
                case Model.EGameStatus.OnBattle:
                    break;
                case Model.EGameStatus.OnBattleResult:
                    (Game.CurrentScene as Components.Scene.MainScene)?.StopBattle();
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

        public static void OnTouchUp(Vector2 end, bool changeMouseStatue)
        {
            OnTouchUp(end, end, changeMouseStatue);
        }

        public static void OnTouchUp(Vector2 end)
        {
            OnTouchUp(end, end);
        }

        public static void OnTouchUp(Vector2 end, Vector2 begin, bool changeMouseStatue)
        {
            OnTouchUp(end, begin);
            isMouseLeftDown = changeMouseStatue;
        }

        public static void OnTouchUp(Vector2 end, Vector2 begin)
        {
            // TODO
        }

        public static void OnJoysticsRockerAxes(JoysticsAxes keyCode, float value)
        {
            var oldValue = axesStatusMap[keyCode];
            axesStatusMap[keyCode] = value;
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

        public static Dictionary<KeyCode, bool> keyStatusMap;
        private static Dictionary<JoysticsAxes, float> axesStatusMap;
        public static bool isMouseLeftDown;


        public static void OnChangeWalkState()
        {
            if (Game.Status == Model.EGameStatus.InEditor)
            {
                return;
            }
            else if (Game.Status == Model.EGameStatus.InGame)
            {
                ;
                bool up = keyStatusMap[KeyCode.UpArrow] || axesStatusMap[JoysticsAxes.LeftVertical] < -0.1 || axesStatusMap[JoysticsAxes.RightVertical] < -0.1 || axesStatusMap[JoysticsAxes.SpecialVertical] > 0.1;
                bool down = keyStatusMap[KeyCode.DownArrow] || axesStatusMap[JoysticsAxes.LeftVertical] > 0.1 || axesStatusMap[JoysticsAxes.RightVertical] > 0.1 || axesStatusMap[JoysticsAxes.SpecialVertical] < -0.1;
                bool right = keyStatusMap[KeyCode.RightArrow] || axesStatusMap[JoysticsAxes.LeftHorizontal] > 0.1 || axesStatusMap[JoysticsAxes.RightHorizontal] > 0.1 || axesStatusMap[JoysticsAxes.SpecialHorizontal] > 0.1;
                bool left = keyStatusMap[KeyCode.LeftArrow] || axesStatusMap[JoysticsAxes.LeftHorizontal] < -0.1 || axesStatusMap[JoysticsAxes.RightHorizontal] < -0.1 || axesStatusMap[JoysticsAxes.SpecialHorizontal] < -0.1;
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
