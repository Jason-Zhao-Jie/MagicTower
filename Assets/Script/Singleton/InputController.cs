using UnityEngine;
using System.Collections.Generic;

public class InputController
{
    public static InputController instance = null;

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
        (KeyCode)6

    };

    public void OnKeyDown(KeyCode keyCode)
    {
        keyStatusMap[keyCode] = true;

        switch (DataCenter.instance.Status)
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
            case Constant.EGameStatus.OnSmallGame:
                break;
			default:
				break;
        }
    }

    public void OnKeyUp(KeyCode keyCode)
    {
        keyStatusMap[keyCode] = false;

		switch (DataCenter.instance.Status)
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
			case Constant.EGameStatus.OnSmallGame:
				break;
			default:
				break;
		}
    }

    public void OnTouchDown(Vector2 touchedPos)
    {

	}

    public void OnTouchUp(Vector2 end)
    {
        OnTouchUp(end, end);
    }

	public void OnTouchUp(Vector2 end, Vector2 begin)
	{

	}

    public void Init()
    {
        keyStatusMap = new Dictionary<KeyCode, bool>();
        for (int i = 0; i < listenedKeys.Length; ++i)
        {
            System.Console.Out.WriteLine("On key listener adding: " + listenedKeys[i].ToString());
            keyStatusMap.Add(listenedKeys[i], false);
        }
    }

    internal Dictionary<KeyCode, bool> keyStatusMap;
    internal bool isMouseLeftDown = false;


	private void OnChangeWalkState()
	{
		bool up = keyStatusMap[KeyCode.UpArrow];
		bool down = keyStatusMap[KeyCode.DownArrow];
		bool right = keyStatusMap[KeyCode.RightArrow];
		bool left = keyStatusMap[KeyCode.LeftArrow];
        int trueNum = (up ? 11 : 0) + (down ? 21 : 0) + (right ? 31 : 0) + (left ? 41 : 0);
        if (trueNum % 10 != 1)
            PlayerController.instance.StopWalk();
        else
            PlayerController.instance.StartWalk((PlayerController.Direction)(trueNum / 10));
	}

}
