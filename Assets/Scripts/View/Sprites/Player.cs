using UnityEngine;
using System.Collections;

public class Player : ObjectPool.AElement {
    const int RUN_SPEED = 10;

    void Start() {
        runningTime = 0;
        movedLength = new Vector2(transform.lossyScale.x * Constant.MAP_BLOCK_BASE_SIZE / 100, transform.lossyScale.y * Constant.MAP_BLOCK_BASE_SIZE / 100);
    }

    void Update() {
        if (Game.Player.DirChanged && mainPlayer) {
            switch (Game.Player.Dir) {
                case PlayerController.Direction.Up:
                    Animator.Play(Game.Player.IsRunning ? "Up" : "Up_Stand");
                    break;
                case PlayerController.Direction.Down:
                    Animator.Play(Game.Player.IsRunning ? "Down" : "Down_Stand");
                    break;
                case PlayerController.Direction.Left:
                    Animator.Play(Game.Player.IsRunning ? "Left" : "Left_Stand");
                    break;
                case PlayerController.Direction.Right:
                    Animator.Play(Game.Player.IsRunning ? "Right" : "Right_Stand");
                    break;
            }
            Game.Player.DirChanged = false;
        }
    }

    void FixedUpdate() {
        if (Game.Player.IsRunning && mainPlayer) {
            if (runningTime < RUN_SPEED)
                ++runningTime;
            else {
                runningTime = 0;
                if (Game.Player.GoToNextBlock()) {
                    var posController = transform;
                    switch (Game.Player.Dir) {
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
        Game.View.ObjPool.RecycleAnElement(this);
    }

    public override ObjectPool.ElementType GetPoolTypeId() {
        return ObjectPool.ElementType.Sprite;
    }

    public string PrefabPath {
        get {
            return Game.Data.Config.modals[playerId].prefabPath;
        }
    }

    public override string ResourcePath {
        get {
            return GetResourcePath(playerId);
        }
    }

    public Sprite BaseSprite {
        get {
            return GetResourceBaseSprite(playerId);
        }
    }

    public static string GetResourcePath(int playerId) {
        return Modal.GetResourcePath(playerId);
    }

    public static string GetResourcePath(string prefabPath) {
        return Modal.GetResourcePath(prefabPath);
    }

    public static Sprite GetResourceBaseSprite(int playerId) {
        return Modal.GetResourceBaseSprite(playerId);
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
