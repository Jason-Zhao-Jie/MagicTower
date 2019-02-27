using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curtain : MonoBehaviour {
    private Constant.EmptyBoolCallBack firstcallback = null;
    private Constant.EmptyBoolCallBack[] callbacks = null;

    public void StartShow(Constant.EmptyBoolCallBack hideCb, params Constant.EmptyBoolCallBack[] showCb) {
        gameObject.SetActive(true);
        firstcallback = hideCb;
        callbacks = showCb;
        Animator.enabled = true;

        Animator.Play("Curtain_show");
    }

    public void StartHide(Constant.EmptyBoolCallBack showCb, params Constant.EmptyBoolCallBack[] hideCb) {
        gameObject.SetActive(true);
        firstcallback = showCb;
        callbacks = hideCb;
        Animator.enabled = true;

        Animator.Play("Curtain_hide");
    }

    // Use this for initialization
    void Awake() {
    }

    private void Start()
    {
        Animator.enabled = false;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
    }

    private void OnShow() {
        Animator.enabled = false;
        if (firstcallback != null) {
            if (firstcallback()) {
                Constant.EmptyBoolCallBack cb = null;
                Constant.EmptyBoolCallBack[] cbs = null;
                if (callbacks != null && callbacks.Length > 0) {
                    cb = callbacks[0]; }
                if (callbacks != null && callbacks.Length > 1) {
                    cbs = new Constant.EmptyBoolCallBack[callbacks.Length-1];
                    for(var i=0;i< cbs.Length; ++i) {
                        cbs[i] = callbacks[i + 1];
                    }

                }
                StartHide(cb, cbs);
            }
        }
    }

    private void OnHide() {
        Animator.enabled = false;
        if (firstcallback != null) {
            if (firstcallback()) {
                Constant.EmptyBoolCallBack cb = null;
                Constant.EmptyBoolCallBack[] cbs = null;
                if (callbacks != null && callbacks.Length > 0) {
                    cb = callbacks[0];
                }
                if (callbacks != null && callbacks.Length > 1) {
                    cbs = new Constant.EmptyBoolCallBack[callbacks.Length - 1];
                    for (var i = 0; i < cbs.Length; ++i) {
                        cbs[i] = callbacks[i + 1];
                    }

                }
                StartShow(cb, cbs);
            }
        }
    }

    private void FixedUpdate() {

    }

    private Animator Animator { get { return GetComponent<Animator>(); } }
}
