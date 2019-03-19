using UnityEngine;

public class Zzhit : ObjectPool.AViewUnit {
    private const int BASE_RUN_TIME = 15;   // 修改此项以控制攻击动画的停留时长, 但是具体攻击动画的单段时长需要配合修改Anim文件, 否则若此值超过实际动画长度, 会循环播放. 单位: 帧
    private int runTime = BASE_RUN_TIME;

    public void SetParam(Constant.WeaponData data, bool crit) {
        this.data = data;
        isCrit = crit;
    }

    public void PlaySound() {
        Game.Managers.Audio.PlaySound(isCrit ? data.critAudioId : data.audioId);
    }

    private void Awake() {
        runTime = BASE_RUN_TIME;
    }

    // Use this for initialization
    void Start() {
        runTime = BASE_RUN_TIME;
    }

    // Update is called once per frame
    void Update() {

    }

    public override ObjectPool.ElementType GetPoolTypeId() {
        return ObjectPool.ElementType.Hitter;
    }

    public override bool RecycleSelf() {
        return RecycleSelf<Zzhit>();
    }

    public string PrefabPath {
        get {
            return data.prefabPath;
        }
    }

    public override string ResourcePath {
        get {
            return GetResourcePath(PrefabPath);
        }
    }

    public Sprite BaseSprite {
        get {
            return GetComponent<SpriteRenderer>().sprite;
        }
    }

    public static string GetResourcePath(string prefabPath) {
        return Modal.GetResourcePath(prefabPath);
    }

    public override bool OnCreate(ObjectPool.ElementType tid, int elemId, string resourcePath) {
        OnReuse(tid, elemId);
        return true;
    }

    public override void OnReuse(ObjectPool.ElementType tid, int elemId) {
        runTime = BASE_RUN_TIME;
        GetComponent<Animator>().enabled = true;
    }

    public override bool OnUnuse(ObjectPool.ElementType tid, int elemId) {
        GetComponent<Animator>().enabled = false;
        return true;
    }

    private void FixedUpdate() {
        --runTime;
        if (runTime < 0)
        {
            Game.ObjPool.RecycleAnElement(this);
        }
    }

    private Constant.WeaponData data;
    private bool isCrit;
}
