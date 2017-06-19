using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zzhit : MonoBehaviour
{
    public int runTime = 30;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        --runTime;
        if (runTime < 0)
            Destroy(gameObject, 0);
    }
}
