using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScene : MonoBehaviour
{
    Button btnStart;
    Button btnLoad;
    Button btnReadMe;

    // Use this for initialization
    void Start()
    {
        btnStart = GameObject.Find("btnStart").GetComponent<Button>();
        btnLoad = GameObject.Find("btnLoad").GetComponent<Button>();
        btnReadMe = GameObject.Find("btnReadMe").GetComponent<Button>();

        btnStart.onClick.AddListener(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
        });

        btnLoad.onClick.AddListener(() =>
        {

        });

        btnReadMe.onClick.AddListener(() =>
        {

        });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
