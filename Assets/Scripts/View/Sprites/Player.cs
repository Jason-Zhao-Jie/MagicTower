﻿using UnityEngine;
using System.Collections;

public class Player : ObjectPool.AViewUnit {
    const int RUN_SPEED = 10;

    void Start() {
        runningTime = 0;
        movedLength = new Vector2(transform.lossyScale.x * Constant.MAP_BLOCK_BASE_SIZE / 100, transform.lossyScale.y * Constant.MAP_BLOCK_BASE_SIZE / 100);
    }

    void Update() {
        if (dirChanged && MainPlayer) {
            switch (Dir) {
                case PlayerController.Direction.Up:
                    Animator.Play(IsRunning ? "Up" : "Up_Stand");
                    break;
                case PlayerController.Direction.Down:
                    Animator.Play(IsRunning ? "Down" : "Down_Stand");
                    break;
                case PlayerController.Direction.Left:
                    Animator.Play(IsRunning ? "Left" : "Left_Stand");
                    break;
                case PlayerController.Direction.Right:
                    Animator.Play(IsRunning ? "Right" : "Right_Stand");
                    break;
            }
            dirChanged = false;
        }
    }

    void FixedUpdate() {
        if (IsRunning && MainPlayer) {
            if (runningTime < RUN_SPEED)
                ++runningTime;
            else {
                runningTime = 0;
                if (Game.Player.GoToNextBlock()) {
                    var posController = transform;
                    switch (Dir) {
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

    public bool MainPlayer { get; set; }

    public void RemoveSelf() {
        Game.ObjPool.RecycleAnElement(this);
    }

    public override ObjectPool.ElementType GetPoolTypeId() {
        return ObjectPool.ElementType.Sprite;
    }

    public string PrefabPath {
        get {
            return Game.Config.modals[playerId].prefabPath;
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
        MainPlayer = false;
        return true;
    }

    public Animator Animator { get { return GetComponent<Animator>(); } }


    public PlayerController.Direction Dir
    {
        get { return dir; }
        set
        {
            dirChanged = dir != value;
            dir = value;
        }
    }

    public bool IsRunning
    {
        get { return isRunning; }
        set
        {
            if (isRunning != value)
                dirChanged = true;
            isRunning = value;
        }
    }

    private bool isRunning;
    private PlayerController.Direction dir;

    private bool dirChanged;
    private int runningTime;
    private Vector2 movedLength;
    private int playerId;
}
