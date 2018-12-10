using UnityEngine;

public class Zzhit : ObjectPool.AElement {
    private int runTime = 15;

    public void SetParam(Constant.WeaponData data, bool crit) {
        this.data = data;
        isCrit = crit;
    }

    // Use this for initialization
    void Start() {
        AudioController.instance.PlaySound(isCrit ? data.critAudioId : data.audioId);
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

    public override string ResourcePath {
        get {
            return prefabPath;
        }
    }

    public override bool OnCreate(ObjectPool.ElementType tid, int elemId, string resourcePath) {
        prefabPath = resourcePath;
        return true;
    }

    public override void OnReuse(ObjectPool.ElementType tid, int elemId) {
        GetComponent<Animator>().enabled = true;
    }

    public override bool OnUnuse(ObjectPool.ElementType tid, int elemId) {
        GetComponent<Animator>().enabled = false;
        return true;
    }

    private void FixedUpdate() {
        --runTime;
        if (runTime < 0) {
            if (MainScene.instance) {
                ObjectPool.instance.RecycleAnElement(this);
            }
        }
    }

    private Constant.WeaponData data;
    private bool isCrit;
    private string prefabPath;
}
