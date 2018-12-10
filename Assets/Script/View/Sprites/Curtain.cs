using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curtain : MonoBehaviour {
    private const int SHOW_HIDE_TIME = 30;

    private bool isShowing = false;
    private bool isMiddled = false;
    private int lastTime = -1;
    private Constant.EmptyCallBack midCallback = null;
    private Constant.EGameStatus lastStatue = Constant.EGameStatus.Start;

    public void StartShow(Constant.EmptyCallBack cb) {
        lastStatue = DataCenter.instance.Status;
        DataCenter.instance.Status = Constant.EGameStatus.OnMiddleLoading;
        gameObject.SetActive(true);
        midCallback = cb;
        lastTime = SHOW_HIDE_TIME;
        isShowing = true;
        // TODO: 完成幕布的动画后, 在此处播放show动画,以代替下列语句
        var color = GetComponent<UnityEngine.UI.Image>().color;
        color.a = 0;
        GetComponent<UnityEngine.UI.Image>().color = color;
    }

    // Use this for initialization
    void Start() {
        // 置为透明
        var color = GetComponent<UnityEngine.UI.Image>().color;
        color.a = 0;
        GetComponent<UnityEngine.UI.Image>().color = color;
    }

    // Update is called once per frame
    void Update() {

    }

    private void OnMiddle() {
        if (midCallback != null)
            midCallback();
        isMiddled = true;
        lastTime = SHOW_HIDE_TIME;
        // TODO: 完成幕布的动画后, 在此处播放hide动画,以代替下列语句
        var color = GetComponent<UnityEngine.UI.Image>().color;
        color.a = 255;
        GetComponent<UnityEngine.UI.Image>().color = color;
    }

    private void OnHide() {
        isShowing = false;
        isMiddled = false;
        DataCenter.instance.Status = lastStatue;
        // TODO: 完成幕布的动画后, 删除下列语句
        var color = GetComponent<UnityEngine.UI.Image>().color;
        color.a = 0;
        GetComponent<UnityEngine.UI.Image>().color = color;

        gameObject.SetActive(false);
    }

    private void OnCurtainOpacity() {
        // TODO: 完成幕布的动画后, 删除下列语句
        var color = GetComponent<UnityEngine.UI.Image>().color;
        if (isMiddled)
            color.a = 255 * lastTime / SHOW_HIDE_TIME;
        else
            color.a = 255 * (SHOW_HIDE_TIME - lastTime) / SHOW_HIDE_TIME;
        GetComponent<UnityEngine.UI.Image>().color = color;
    }

    private void FixedUpdate() {
        if (isShowing)
            if (lastTime > 0) {
                --lastTime;
                OnCurtainOpacity();
            } else if (isMiddled)
                OnHide();
            else
                OnMiddle();
    }
}
