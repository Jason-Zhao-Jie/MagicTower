using UnityEngine;
using System.Collections.Generic;

public class InputManager
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

    public InputManager()
    {
        keyStatusMap = new Dictionary<KeyCode, bool>();
        for (int i = 0; i < listenedKeys.Length; ++i)
        {
            Debug.Log("On key listener adding: " + listenedKeys[i].ToString());
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

    public void OnKeyDown(KeyCode keyCode)
    {
        keyStatusMap[keyCode] = true;

        switch (Game.Status)
        {
            case Constant.EGameStatus.Start:
                break;
            case Constant.EGameStatus.InGame:
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
            case Constant.EGameStatus.InEditor:
                break;
            case Constant.EGameStatus.OnCG:
                break;
            case Constant.EGameStatus.OnTipChat:
                switch (keyCode)
                {
                    case KeyCode.Return:
                    case KeyCode.KeypadEnter:
                    case KeyCode.Space:
                    case JoysticsCode.A:
                        (Game.CurrentScene as MainScene)?.ChatStepOn();
                        break;
                }
                break;
            case Constant.EGameStatus.OnDialog:
                break;
            case Constant.EGameStatus.OnMiddleLoading:
                break;
            case Constant.EGameStatus.OnBattle:
                break;
            case Constant.EGameStatus.OnBattleResult:
                break;
            case Constant.EGameStatus.OnSmallGame:
                break;
            case Constant.EGameStatus.AutoStepping:
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

    public void OnKeyUp(KeyCode keyCode)
    {
        keyStatusMap[keyCode] = false;

        switch (Game.Status)
        {
            case Constant.EGameStatus.Start:
                switch (keyCode)
                {
                    case KeyCode.Escape:
                    case KeyCode.Backspace:
                    case JoysticsCode.B:
                        StartScene.instance.OnExitGame();
                        break;
                }
                break;
            case Constant.EGameStatus.InGame:
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
            case Constant.EGameStatus.InEditor:
                break;
            case Constant.EGameStatus.OnCG:
                break;
            case Constant.EGameStatus.OnTipChat:
                break;
            case Constant.EGameStatus.OnDialog:
                break;
            case Constant.EGameStatus.OnMiddleLoading:
                break;
            case Constant.EGameStatus.OnBattle:
                break;
            case Constant.EGameStatus.OnBattleResult:
                switch (keyCode)
                {
                    case KeyCode.Return:
                    case KeyCode.KeypadEnter:
                    case KeyCode.Space:
                    case JoysticsCode.A:
                        (Game.CurrentScene as MainScene)?.StopBattle();
                        break;
                }
                break;
            case Constant.EGameStatus.OnSmallGame:
                break;
            case Constant.EGameStatus.AutoStepping:
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

    public void OnTouchDown(Vector2 touchedPos, bool changeMouseStatue)
    {
        OnTouchDown(touchedPos);
        isMouseLeftDown = changeMouseStatue;
    }
    public void OnTouchDown(Vector2 touchedPos)
    {
        switch (Game.Status)
        {
            case Constant.EGameStatus.Start:
                break;
            case Constant.EGameStatus.InGame:
            case Constant.EGameStatus.InEditor:
                //MainScene.instance.ShowTips("Click Called !");
                Game.Map.ClickMap(touchedPos);
                break;
            case Constant.EGameStatus.OnCG:
                break;
            case Constant.EGameStatus.OnTipChat:
                (Game.CurrentScene as MainScene)?.ChatStepOn();
                break;
            case Constant.EGameStatus.OnDialog:
                break;
            case Constant.EGameStatus.OnMiddleLoading:
                break;
            case Constant.EGameStatus.OnBattle:
                break;
            case Constant.EGameStatus.OnBattleResult:
                (Game.CurrentScene as MainScene)?.StopBattle();
                break;
            case Constant.EGameStatus.OnSmallGame:
                break;
            case Constant.EGameStatus.AutoStepping:
                Game.Player.StopAutoStep();
                break;
            default:
                break;
        }
    }

    public void OnTouchUp(Vector2 end, bool changeMouseStatue)
    {
        OnTouchUp(end, end, changeMouseStatue);
    }

    public void OnTouchUp(Vector2 end)
    {
        OnTouchUp(end, end);
    }

    public void OnTouchUp(Vector2 end, Vector2 begin, bool changeMouseStatue)
    {
        OnTouchUp(end, begin);
        isMouseLeftDown = changeMouseStatue;
    }

    public void OnTouchUp(Vector2 end, Vector2 begin)
    {
        // TODO
    }

    public void OnJoysticsRockerAxes(JoysticsAxes keyCode, float value)
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
                        case Constant.EGameStatus.InGame:
                            OnChangeWalkState();
                            break;
                        case Constant.EGameStatus.AutoStepping:
                            Game.Player.StopAutoStep();
                            break;
                    }
                }
                break;
        }
    }

    internal Dictionary<KeyCode, bool> keyStatusMap;
    internal Dictionary<JoysticsAxes, float> axesStatusMap;
    internal bool isMouseLeftDown;


    public void OnChangeWalkState()
    {
        if (Game.Status == Constant.EGameStatus.InEditor)
        {
            return;
        }
        else if (Game.Status == Constant.EGameStatus.InGame)
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
                Game.Player.StartWalk((PlayerController.Direction)(trueNum / 10));
        }
        else if (Game.Status != Constant.EGameStatus.AutoStepping)
        {
            Game.Player.StopWalk();
        }
    }
}
