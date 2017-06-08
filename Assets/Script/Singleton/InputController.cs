using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputController : MonoBehaviour
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
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnKeyDown(KeyCode keyCode)
    {
        keyStatusMap[keyCode] = true;

        switch (DataCenter.instance.Status)
        {
			case DataCenter.EGameStatus.Start:
				break;
			case DataCenter.EGameStatus.InGame:
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
			case DataCenter.EGameStatus.OnCG:
				break;
			case DataCenter.EGameStatus.OnTipChat:
				break;
			case DataCenter.EGameStatus.OnDialog:
				break;
			case DataCenter.EGameStatus.OnMiddleLoading:
				break;
			case DataCenter.EGameStatus.OnBattle:
				break;
            case DataCenter.EGameStatus.OnSmallGame:
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
			case DataCenter.EGameStatus.Start:
				break;
			case DataCenter.EGameStatus.InGame:
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
			case DataCenter.EGameStatus.OnCG:
				break;
			case DataCenter.EGameStatus.OnTipChat:
				break;
			case DataCenter.EGameStatus.OnDialog:
				break;
			case DataCenter.EGameStatus.OnMiddleLoading:
				break;
			case DataCenter.EGameStatus.OnBattle:
				break;
			case DataCenter.EGameStatus.OnSmallGame:
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
