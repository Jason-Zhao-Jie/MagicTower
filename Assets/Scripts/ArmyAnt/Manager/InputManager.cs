using System.Collections.Generic;
using UnityEngine;

namespace ArmyAnt.Manager {
    public abstract class InputManager {
        public static class AndroidKeyCode {
            public const KeyCode Back = (KeyCode)6;
        }
        public static class XboxJoysticsCode {
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

        protected static class Ps4JoysticsCode {
            public const KeyCode Block = KeyCode.Joystick1Button0;
            public const KeyCode X = KeyCode.Joystick1Button1;
            public const KeyCode Circle = KeyCode.Joystick1Button2;
            public const KeyCode Triangle = KeyCode.Joystick1Button3;
            public const KeyCode Left1 = KeyCode.Joystick1Button4;
            public const KeyCode Right1 = KeyCode.Joystick1Button5;
            public const KeyCode Left2 = KeyCode.Joystick1Button6;
            public const KeyCode Right2 = KeyCode.Joystick1Button7;
            public const KeyCode Share = KeyCode.Joystick1Button8;    // 左摇杆按下
            public const KeyCode Option = KeyCode.Joystick1Button9;   // 右摇杆按下
            public const KeyCode LeftRocker = KeyCode.Joystick1Button10;    // 左摇杆按下
            public const KeyCode RightRocker = KeyCode.Joystick1Button11;   // 右摇杆按下
            public const KeyCode PS = KeyCode.Joystick1Button12;    // 左摇杆按下
            public const KeyCode Touch = KeyCode.Joystick1Button13;   // 右摇杆按下
        }

        public enum JoysticsAxes {
            LeftHorizontal, // 左摇杆X轴
            LeftVertical,   // 左摇杆Y轴
            RightHorizontal,// 右摇杆X轴
            RightVertical,  // 右摇杆Y轴
            SpecialHorizontal,// 十字键X轴
            SpecialVertical,  // 十字键Y轴
            LeftTrigger,    // 左扳机
            RightTrigger,   // 右扳机
        }

        protected InputManager(IDictionary<JoysticsAxes, string> xboxAxesNames, IDictionary<JoysticsAxes, string> ps4WinAxesNames, IDictionary<JoysticsAxes, string> ps4MacAxesNames) {
            keyStatusMap = new bool[512];
            forceDownedKeyMap = new bool[512];
            axesStatusMap = new Dictionary<JoysticsAxes, float> {
                [JoysticsAxes.LeftHorizontal] = 0,
                [JoysticsAxes.LeftVertical] = 0,
                [JoysticsAxes.RightHorizontal] = 0,
                [JoysticsAxes.RightVertical] = 0,
                [JoysticsAxes.SpecialHorizontal] = 0,
                [JoysticsAxes.SpecialVertical] = 0,
                [JoysticsAxes.LeftTrigger] = 0,
                [JoysticsAxes.RightTrigger] = 0,
            };
            mouseButtonStatusMap = new Vector2?[] { null, null, null, null, null, null, null };
            touchDownListenerList = new Dictionary<int, OnTouchDown>();
            touchMovingListenerList = new Dictionary<int, OnTouchMoving>();
            touchUpListenerList = new Dictionary<int, OnTouchUp>();
            listenedXboxAxesNames = new Dictionary<JoysticsAxes, string>(xboxAxesNames);
            listenedPs4WinAxesNames = new Dictionary<JoysticsAxes, string>(ps4WinAxesNames);
            listenedPs4MacAxesNames = new Dictionary<JoysticsAxes, string>(ps4MacAxesNames);
            keyListenerList = new Dictionary<KeyCode, Dictionary<int, System.Action<bool>>>();
            joysticksAxisListenerList = new Dictionary<JoysticsAxes, Dictionary<int, OnJoysticksAxis>>();
        }


        public void UpdateScene() {
            // 监测键盘和手柄按键
            for(var i = 0; i < keyStatusMap.Length; ++i) {
                var key = (KeyCode)i;
                bool isDown = forceDownedKeyMap[(int)key] || Input.GetKey(key);
                if(Input.GetKey(key)) {
                    CallKeyDown(key);
                } else {
                    CallKeyUp(key);
                }
            }

            // 判断手柄类型
            var joysticks = Input.GetJoystickNames();
            if (joysticks != null) {
                for (var i = 0; i < joysticks.Length; ++i) {
                    switch (joysticks[i]) {
                        case "Controller (XBOX 360 For Windows)":
                            // 检测手柄摇杆状态
                            foreach(var key in listenedXboxAxesNames) {
                                var oldValue = axesStatusMap[key.Key];
                                var newValue = Input.GetAxis(key.Value + i);
                                axesStatusMap[key.Key] = newValue;
                                if(joysticksAxisListenerList.ContainsKey(key.Key)) {
                                    foreach(var funcs in joysticksAxisListenerList[key.Key]) {
                                        funcs.Value(i, newValue, oldValue);
                                    }
                                }
                            }
                            break;
                        case "Controller (PS4 For Windows)":
                            // 检测手柄摇杆状态
                            foreach(var key in listenedPs4WinAxesNames) {
                                var oldValue = axesStatusMap[key.Key];
                                var newValue = Input.GetAxis(key.Value + i);
                                axesStatusMap[key.Key] = newValue;
                                if(joysticksAxisListenerList.ContainsKey(key.Key)) {
                                    foreach(var funcs in joysticksAxisListenerList[key.Key]) {
                                        funcs.Value(i, newValue, oldValue);
                                    }
                                }

                            }
                            break;
                        case "Controller (PS4 For Mac)":
                            // 检测手柄摇杆状态
                            foreach(var key in listenedPs4MacAxesNames) {
                                var oldValue = axesStatusMap[key.Key];
                                var newValue = Input.GetAxis(key.Value + i);
                                axesStatusMap[key.Key] = newValue;
                                if(joysticksAxisListenerList.ContainsKey(key.Key)) {
                                    foreach(var funcs in joysticksAxisListenerList[key.Key]) {
                                        funcs.Value(i, newValue, oldValue);
                                    }
                                }

                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            // 监测鼠标和触屏
            if (Input.touchCount <= 0) {
                for(int i = 0; i < mouseButtonStatusMap.Length; ++i) {
                    if(Input.GetMouseButtonDown(i)) {
                        if(mouseButtonStatusMap[i] == null) {
                            var pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                            CallTouchDown(pos, i);
                            mouseButtonStatusMap[i] = pos;
                        } else {
                            CallTouchMoving(new Vector2(Input.mousePosition.x, Input.mousePosition.y), mouseButtonStatusMap[i].Value, i);
                        }
                    }
                    if(Input.GetMouseButtonUp(i) && mouseButtonStatusMap[i] != null) {
                        CallTouchUp(new Vector2(Input.mousePosition.x, Input.mousePosition.y), mouseButtonStatusMap[i].Value, i);
                        mouseButtonStatusMap[i] = null;
                    }
                }
            } else {
                for(int i = 0; i < Input.touchCount; ++i) {
                    var tc = Input.GetTouch(i);
                    switch(tc.phase) {
                        case TouchPhase.Began:
                            CallTouchDown(tc.position, 0);
                            break;
                        case TouchPhase.Moved:
                            CallTouchMoving(tc.position, new Vector2(tc.position.x - tc.deltaPosition.x, tc.position.y - tc.deltaPosition.y), 0);
                            break;
                        case TouchPhase.Canceled:
                        case TouchPhase.Ended:
                            CallTouchUp(tc.position, new Vector2(tc.position.x - tc.deltaPosition.x, tc.position.y - tc.deltaPosition.y), 0);
                            break;
                    }
                }
            }
        }

        public int RegisterKeyListener(KeyCode keyCode, System.Action<bool> action) {
            if(action == null) {
                return 0;
            }
            if(!keyListenerList.ContainsKey(keyCode)) {
                keyListenerList.Add(keyCode, new Dictionary<int, System.Action<bool>>());
            }
            int index = 0;
            while(true) {
                if(!keyListenerList[keyCode].ContainsKey(++index)) {
                    break;
                }
            }
            keyListenerList[keyCode].Add(index, action);
            return index;
        }

        public void UnregisterKeyListener(KeyCode keyCode, int tag) {
            keyListenerList[keyCode].Remove(tag);
        }

        public void CallKeyDown(KeyCode key, bool force = false) {
            bool hasDown = GetKeyStatus(key);
            if(force) {
                forceDownedKeyMap[(int)key] = true;
            }
            if(!hasDown) {
                keyStatusMap[(int)key] = true;
                if(keyListenerList.TryGetValue(key, out var listener) && listener != null) {
                    foreach(var sub in listener) {
                        sub.Value(true);
                    }
                }
            }
        }

        public void CallKeyUp(KeyCode key, bool force = false) {
            bool hasDown = GetKeyStatus(key);
            if(force) {
                forceDownedKeyMap[(int)key] = false;
            }
            if(hasDown) {
                keyStatusMap[(int)key] = false;
                if(keyListenerList.TryGetValue(key, out var listener) && listener != null) {
                    foreach(var sub in listener) {
                        sub.Value(false);
                    }
                }
            }
        }

        public delegate void OnJoysticksAxis(int joystickIndex, float value, float oldValue);

        public int RegisterJoysticksAxisListener(JoysticsAxes axisCode, OnJoysticksAxis action) {
            if(action == null) {
                return 0;
            }
            if(!joysticksAxisListenerList.ContainsKey(axisCode)) {
                joysticksAxisListenerList.Add(axisCode, new Dictionary<int, OnJoysticksAxis>());
            }
            int index = 0;
            while(true) {
                if(!joysticksAxisListenerList[axisCode].ContainsKey(++index)) {
                    break;
                }
            }
            joysticksAxisListenerList[axisCode].Add(index, action);
            return index;
        }

        public void UnregisterJoysticksAxisListener(JoysticsAxes axisCode, int tag) {
            joysticksAxisListenerList[axisCode].Remove(tag);
        }
        
        public delegate void OnTouchDown(Vector2 touchedPos, int mouseBtn);
        public delegate void OnTouchUp(Vector2 end, Vector2 begin, int mouseBtn);
        public delegate void OnTouchMoving(Vector2 now, Vector2 begin, int mouseBtn);

        public int RegisterTouchDownListener(OnTouchDown action) {
            int index = 0;
            while(true) {
                if(!touchDownListenerList.ContainsKey(++index)) {
                    break;
                }
            }
            touchDownListenerList.Add(index, action);
            return index;
        }

        public void UnregisterTouchDownListener(int tag) {
            touchDownListenerList.Remove(tag);
        }

        public void CallTouchDown(Vector2 touchedPos, int mouseBtn) {
            foreach(var func in touchDownListenerList) {
                func.Value(touchedPos, mouseBtn);
            }
        }

        public int RegisterTouchUpListener(OnTouchUp action) {
            int index = 0;
            while(true) {
                if(!touchUpListenerList.ContainsKey(++index)) {
                    break;
                }
            }
            touchUpListenerList.Add(index, action);
            return index;
        }

        public void UnregisterTouchUpListener(int tag) {
            touchUpListenerList.Remove(tag);
        }

        public void CallTouchUp(Vector2 end, Vector2 begin, int mouseBtn) {
            foreach(var func in touchUpListenerList) {
                func.Value(end, begin, mouseBtn);
            }
        }

        public int RegisterTouchMovingListener(OnTouchMoving action) {
            int index = 0;
            while(true) {
                if(!touchMovingListenerList.ContainsKey(++index)) {
                    break;
                }
            }
            touchMovingListenerList.Add(index, action);
            return index;
        }

        public void UnregisterTouchMovingListener(int tag) {
            touchMovingListenerList.Remove(tag);
        }

        private void CallTouchMoving(Vector2 now, Vector2 begin, int mouseBtn) {
            foreach(var func in touchMovingListenerList) {
                func.Value(now, begin, mouseBtn);
            }
        }

        public bool GetKeyStatus(KeyCode key) {
            return keyStatusMap[(int)key] || forceDownedKeyMap[(int)key];
        }

        public float GetAxesStatus(JoysticsAxes key) {
            return axesStatusMap[key];
        }

        private bool[] keyStatusMap;
        private bool[] forceDownedKeyMap;
        private Dictionary<JoysticsAxes, float> axesStatusMap;
        private Vector2?[] mouseButtonStatusMap;

        private Dictionary<int, OnTouchDown> touchDownListenerList;
        private Dictionary<int, OnTouchMoving> touchMovingListenerList;
        private Dictionary<int, OnTouchUp> touchUpListenerList;
        private Dictionary<KeyCode, Dictionary<int, System.Action<bool>>> keyListenerList;
        private Dictionary<JoysticsAxes, Dictionary<int, OnJoysticksAxis>> joysticksAxisListenerList;

        readonly private Dictionary<JoysticsAxes, string> listenedXboxAxesNames;
        readonly private Dictionary<JoysticsAxes, string> listenedPs4WinAxesNames;
        readonly private Dictionary<JoysticsAxes, string> listenedPs4MacAxesNames;
    }
}
