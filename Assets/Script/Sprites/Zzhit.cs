using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zzhit : MonoBehaviour
{
    public int runTime = 30;

    public void SetParam(Constant.WeaponData data){
        this.data = data;
    }

    // Use this for initialization
    void Start()
    {
        AudioController.instance.PlaySound(data.audioId);
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
                Destroy(gameObject, 0);
            }
        }
    }

    private Constant.WeaponData data;
}
