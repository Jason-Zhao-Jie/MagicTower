using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    const int RUN_SPEED = 10;

	void Start()
	{
        runningTime = 0;
        movedLength = new Vector2(transform.lossyScale.x * Constant.MAP_BLOCK_BASE_SIZE / 100, transform.lossyScale.y * Constant.MAP_BLOCK_BASE_SIZE / 100);
	}

	void Update()
	{
        if (PlayerController.instance.dirChanged)
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
            PlayerController.instance.dirChanged = false;
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
                    var posController = transform;
                    switch (PlayerController.instance.Dir)
                    {
                        case PlayerController.Direction.Up:
                            posController.position = new Vector3(posController.position.x, posController.position.y + movedLength.y, posController.position.z);
                            break;
                        case PlayerController.Direction.Down:
                            posController.position = new Vector3(posController.position.x, posController.position.y - movedLength.y, posController.position.z);
                            break;
                        case PlayerController.Direction.Left:
                            posController.position = new Vector3(posController.position.x - movedLength.x, posController.position.y, posController.position.z);
                            break;
                        case PlayerController.Direction.Right:
                            posController.position = new Vector3(posController.position.x + movedLength.x, posController.position.y, posController.position.z);
                            break;
                    }
                }
            }
        }
        else
        {
            runningTime = RUN_SPEED - 1;
        }
	}

	public void RemoveSelf()
	{
		Destroy(gameObject);
	}

    public Animator animator{ get { return GetComponent<Animator>(); }}

    private int runningTime;
    private Vector2 movedLength;
}
