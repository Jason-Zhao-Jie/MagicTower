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
        Input.multiTouchEnabled = true;

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

        DataCenter.instance = new DataCenter();
        DataCenter.instance.LoadData();
        InputController.instance = new InputController();
        InputController.Init();
        EventManager.instance = new EventManager();
        AudioController.instance = new AudioController();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
