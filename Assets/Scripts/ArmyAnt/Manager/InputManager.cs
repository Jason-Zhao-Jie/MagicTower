using System.Collections.Generic;
using UnityEngine;

namespace ArmyAnt.Manager
{
    public abstract class InputManager
    {
        protected static class JoysticsCode
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

        protected enum JoysticsAxes
        {
            LeftHorizontal, // 左摇杆X轴
            LeftVertical,   // 左摇杆Y轴
            RightHorizontal,// 右摇杆X轴
            RightVertical,  // 右摇杆Y轴
            SpecialHorizontal,// 十字键X轴
            SpecialVertical,  // 十字键Y轴
        }

        protected InputManager(KeyCode[] listenedKeys, Dictionary<JoysticsAxes, string> axesNames)
        {
            keyStatusMap = new Dictionary<KeyCode, bool>();
            for (int i = 0; i < listenedKeys.Length; ++i)
            {
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
            listenedAxesNames = new Dictionary<JoysticsAxes, string>
            {
                [JoysticsAxes.LeftHorizontal] = axesNames[JoysticsAxes.LeftHorizontal],
                [JoysticsAxes.LeftVertical] = axesNames[JoysticsAxes.LeftVertical],
                [JoysticsAxes.RightHorizontal] = axesNames[JoysticsAxes.RightHorizontal],
                [JoysticsAxes.RightVertical] = axesNames[JoysticsAxes.RightVertical],
                [JoysticsAxes.SpecialHorizontal] = axesNames[JoysticsAxes.SpecialHorizontal],
                [JoysticsAxes.SpecialVertical] = axesNames[JoysticsAxes.SpecialVertical],
            };
        }


        public void UpdateScene()
        {
            // 监测键盘和手柄按键
            foreach (var i in keyStatusMap)
            {
                bool isDown = Input.GetKey(i.Key);
                bool hasDown = i.Value;
                if (isDown && !hasDown)
                {
                    keyStatusMap[i.Key] = true;
                    OnKeyDown(i.Key);
                }
                else if (hasDown && !isDown)
                {
                    keyStatusMap[i.Key] = false;
                    OnKeyUp(i.Key);
                }
            }

            // 判断手柄类型
            var joysticks = Input.GetJoystickNames();
            if (joysticks != null)
            {
                for (var i = 0; i < joysticks.Length; ++i)
                {
                    switch (joysticks[i])
                    {
                        case "Controller (XBOX 360 For Windows)":
                            // 检测手柄摇杆状态
                            foreach(var key in listenedAxesNames)
                            {
                                var oldValue = axesStatusMap[key.Key];
                                var newValue = Input.GetAxis(key.Value + i);
                                axesStatusMap[key.Key] = newValue;
                                OnJoysticsRockerAxes(key.Key, newValue, oldValue);

                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            // 监测鼠标和触屏
            for (int i = 0; i < Input.touchCount; ++i)
            {
                var tc = Input.GetTouch(i);
                switch (tc.phase)
                {
                    case TouchPhase.Began:
                        OnTouchDown(tc.position);
                        break;
                    case TouchPhase.Canceled:
                    case TouchPhase.Ended:
                        OnTouchUp(tc.position, new Vector2(tc.position.x - tc.deltaPosition.x, tc.position.y - tc.deltaPosition.y));
                        break;
                }
            }

            if (Input.touchCount <= 0)
            {
                if (Input.GetMouseButtonDown(0) && !isMouseLeftDown)
                {
                    OnTouchDown(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
                    isMouseLeftDown = true;
                }
                if (Input.GetMouseButtonUp(0) && isMouseLeftDown)
                {
                    var pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    OnTouchUp(pos, pos);
                    isMouseLeftDown = false;
                }
            }
        }

        abstract protected void OnKeyDown(KeyCode keyCode);
        abstract protected void OnKeyUp(KeyCode keyCode);
        abstract protected void OnTouchDown(Vector2 touchedPos);
        abstract protected void OnTouchUp(Vector2 end, Vector2 begin);
        abstract protected void OnJoysticsRockerAxes(JoysticsAxes keyCode, float value, float oldValue);

        protected bool GetKeyStatus(KeyCode key)
        {
            return keyStatusMap[key];
        }

        protected float GetAxesStatus(JoysticsAxes key)
        {
            return axesStatusMap[key];
        }

        private Dictionary<KeyCode, bool> keyStatusMap;
        private Dictionary<JoysticsAxes, float> axesStatusMap;
        readonly private Dictionary<JoysticsAxes, string> listenedAxesNames;
        private bool isMouseLeftDown;
    }
}
