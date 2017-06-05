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
        (KeyCode)6,
        (KeyCode)8,

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

    }

    public void OnKeyUp(KeyCode keyCode)
    {
        keyStatusMap[keyCode] = false;

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

    public static void Init()
    {
        instance.keyStatusMap = new Dictionary<KeyCode, bool>();
        for (int i = 0; i < listenedKeys.Length; ++i)
        {
            instance.keyStatusMap.Add(listenedKeys[i], false);
        }
    }

    internal Dictionary<KeyCode, bool> keyStatusMap;
    internal bool isMouseLeftDown = false;

}
