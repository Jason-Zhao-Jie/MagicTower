using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zzhit : MonoBehaviour
{
    private int runTime = 15;

    public void SetParam(Constant.WeaponData data, bool crit){
        this.data = data;
        isCrit = crit;
    }

    // Use this for initialization
    void Start()
    {
        AudioController.instance.PlaySound(isCrit ? data.critAudioId : data.audioId);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        --runTime;
        if (runTime < 0)
        {
            if (MainScene.instance)
            {
                MainScene.instance.hitter = null;
                GetComponent<Animator>().enabled = false;
                transform.SetParent(null);
                Destroy(gameObject, 0);
            }
        }
    }

    private Constant.WeaponData data;
    private bool isCrit;
}
