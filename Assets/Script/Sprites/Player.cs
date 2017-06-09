using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    const int RUN_SPEED = 50;

	void Start()
	{
        runningTime = 0;
	}

	void Update()
	{
		switch (PlayerController.instance.Dir)
		{
			case PlayerController.Direction.Up:
                animator.Play(PlayerController.instance.IsRunning ? "Up" : "Up_Stand");
                break;
			case PlayerController.Direction.Down:
				animator.Play(PlayerController.instance.IsRunning ? "Down" : "Down_Stand"); 
                break;
			case PlayerController.Direction.Left:
				animator.Play(PlayerController.instance.IsRunning ? "Left" : "Left_Stand"); 
                break;
			case PlayerController.Direction.Right:
				animator.Play(PlayerController.instance.IsRunning ? "Right" : "Right_Stand"); 
                break;
		}
	}

    void FixedUpdate()
	{
        if (PlayerController.instance.IsRunning)
        {
            if (runningTime < RUN_SPEED)
                ++runningTime;
            else
            {
                runningTime = 0;
                if (PlayerController.instance.GoToNextBlock())
                {
                    var posController = GetComponent<RectTransform>();
                    switch (PlayerController.instance.Dir)
                    {
                        case PlayerController.Direction.Up:
                            posController.position = new Vector3(posController.position.x, posController.position.y + MainScene.instance.BlockSize.y);
                            break;
                        case PlayerController.Direction.Down:
                            posController.position = new Vector3(posController.position.x, posController.position.y - MainScene.instance.BlockSize.y);
                            break;
                        case PlayerController.Direction.Left:
                            posController.position = new Vector3(posController.position.x - MainScene.instance.BlockSize.x, posController.position.y);
                            break;
                        case PlayerController.Direction.Right:
                            posController.position = new Vector3(posController.position.x + MainScene.instance.BlockSize.x, posController.position.y);
                            break;
                    }
                }
            }
        }
        else
        {
            runningTime = 0;
        }
    }
    public Animator animator{ get { return GetComponent<Animator>(); }}

    private int runningTime;
}
