using UnityEngine;
using System.Collections;

public class Player : ObjectPool.AElement {
    const int RUN_SPEED = 10;

    void Start() {
        runningTime = 0;
        movedLength = new Vector2(transform.lossyScale.x * Constant.MAP_BLOCK_BASE_SIZE / 100, transform.lossyScale.y * Constant.MAP_BLOCK_BASE_SIZE / 100);
    }

    void Update() {
        if (PlayerController.instance.dirChanged && mainPlayer) {
            switch (PlayerController.instance.Dir) {
                case PlayerController.Direction.Up:
                    Animator.Play(PlayerController.instance.IsRunning ? "Up" : "Up_Stand");
                    break;
                case PlayerController.Direction.Down:
                    Animator.Play(PlayerController.instance.IsRunning ? "Down" : "Down_Stand");
                    break;
                case PlayerController.Direction.Left:
                    Animator.Play(PlayerController.instance.IsRunning ? "Left" : "Left_Stand");
                    break;
                case PlayerController.Direction.Right:
                    Animator.Play(PlayerController.instance.IsRunning ? "Right" : "Right_Stand");
                    break;
            }
            PlayerController.instance.dirChanged = false;
        }
    }

    void FixedUpdate() {
        if (PlayerController.instance.IsRunning && mainPlayer) {
            if (runningTime < RUN_SPEED)
                ++runningTime;
            else {
                runningTime = 0;
                if (PlayerController.instance.GoToNextBlock()) {
                    var posController = transform;
                    switch (PlayerController.instance.Dir) {
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
        } else {
            runningTime = RUN_SPEED - 1;
        }
    }

    public bool MainPlayer {
        get { return mainPlayer; }
        set { mainPlayer = value; }
    }

    public void RemoveSelf() {
        ObjectPool.instance.RecycleAnElement(this);
    }

    public override ObjectPool.ElementType GetPoolTypeId() {
        return ObjectPool.ElementType.Sprite;
    }

    public override string ResourcePath {
        get {
            return Constant.PREFAB_DIR + DataCenter.instance.modals[playerId].prefabPath;
        }
    }

    public void SetPlayerData(int id) {
        playerId = id;
    }

    public override bool RecycleSelf() {
        return RecycleSelf<Modal>();
    }

    public override bool OnCreate(ObjectPool.ElementType tid, int elemId, string resourcePath) {
        SetPlayerData(elemId);
        return true;
    }

    public override void OnReuse(ObjectPool.ElementType tid, int elemId) {
    }

    public override bool OnUnuse(ObjectPool.ElementType tid, int elemId) {
        mainPlayer = false;
        return true;
    }

    public Animator Animator { get { return GetComponent<Animator>(); } }

    private int runningTime;
    private Vector2 movedLength;
    private int playerId;
    private bool mainPlayer = false;
}
