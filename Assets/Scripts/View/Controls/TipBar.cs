using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipBar : ObjectPool.AElement
{
    public const string PREFAB_DIR = "TipBar";
    public const int PREFAB_ID = 3;

    public static TipBar ShowTip()
    {
        var ret = Game.View.ObjPool.GetAnElement<TipBar>(PREFAB_ID, ObjectPool.ElementType.Dialog, Constant.DIALOG_DIR + PREFAB_DIR);
        return ret;
    }

    // Use this for initialization
    void Awake() {
        tipsText = transform.Find("Text").GetComponent<UnityEngine.UI.Text>();
        tipsText.fontSize = System.Convert.ToInt32(tipsText.fontSize * Game.View.ScreenAdaptorInst.RealFontSize);
    }

    void FixedUpdate() {
        if(hideTime > 0) {
            hideTime--;
        }else if(hideTime == 0) {
            hideTime--;
            RecycleSelf();
        }
    }

    public void SetTipText(string content)
    {
        tipsText.text = content;
        Game.Controller.Audio.PlaySound(20);
    }

    public void StartAutoRemove(int time) {
        hideTime = time;
    }

    public override ObjectPool.ElementType GetPoolTypeId()
    {
        return ObjectPool.ElementType.Dialog;
    }

    public override bool RecycleSelf()
    {
        return RecycleSelf<TipBar>();
    }

    public override string ResourcePath {
        get {
            return Constant.DIALOG_DIR + PREFAB_DIR;
        }
    }

    public override bool OnCreate(ObjectPool.ElementType tid, int elemId, string resourcePath)
    {
        return true;
    }

    public override void OnReuse(ObjectPool.ElementType tid, int elemId)
    {
    }

    public override bool OnUnuse(ObjectPool.ElementType tid, int elemId)
    {
        return true;
    }

    private UnityEngine.UI.Text tipsText;
    private int hideTime = -1;
}
