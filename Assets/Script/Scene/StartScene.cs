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
			MapManager.instance.SetData();
            PlayerController.instance.PlayerId = 1;
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
        InputController.instance.Init();
        EventManager.instance = new EventManager();
        EventManager.instance.InitEvents();
        AudioController.instance = new AudioController();
        AudioController.instance.MusicSource = GetComponent<AudioSource>();
        AudioController.instance.SoundSource = transform.Find("imgBack").GetComponent<AudioSource>();
        ModalManager.instance = new ModalManager();
        MapManager.instance = new MapManager();
        PlayerController.instance = new PlayerController();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
