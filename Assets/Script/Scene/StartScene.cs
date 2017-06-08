using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScene : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Input.multiTouchEnabled = true;

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

    public void OnStartGame()
    {
        MapManager.instance.SetData();
        PlayerController.instance.PlayerId = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }

    public void OnLoadGame()
    {
        
    }

    public void OnReadMe()
    {

    }

    public void OnGameEditor()
	{
		MapManager.instance.SetData();
        UnityEngine.SceneManagement.SceneManager.LoadScene("DataEditorScene");
    }
}
