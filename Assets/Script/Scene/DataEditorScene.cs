using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataEditorScene : MonoBehaviour
{
    public static DataEditorScene instance;
    // Use this for initialization
    void Start()
    {
        instance = this;
        AudioController.instance.MusicSource = GetComponent<AudioSource>();
        AudioController.instance.SoundSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        MapManager.instance.ShowMap();

        backgroundImg = GetComponent<UnityEngine.UI.Image>();
        mapPartRect = MapManager.GetMapPosition(transform.Find("MapPanel").GetComponent<RectTransform>());
        blockSize = new Vector3(mapPartRect.width * 100 / Constant.MAP_BLOCK_BASE_SIZE, mapPartRect.height * 100 / Constant.MAP_BLOCK_BASE_SIZE);
        //TODO: 需要在四周添加填充墙，然后再MapManager构造地图时刷新墙

        allAudios = Resources.LoadAll<AudioClip>(Constant.AUDIO_DIR);
        allPrefabs = Resources.LoadAll<GameObject>(Constant.PREFAB_DIR);
        OnChangeToAudioAndEvents();


    }

    private void OnDestroy()
    {
        instance = null;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnChangeToMaps()
	{
		GameObject.Find("MapMakerCanvas").SetActive(true);
		GameObject.Find("ModalMakerCanvas").SetActive(false);
		GameObject.Find("AudioAndEventsCanvas").SetActive(false);
    }

    public void OnChangeToModals()
	{
		GameObject.Find("MapMakerCanvas").SetActive(false);
		GameObject.Find("ModalMakerCanvas").SetActive(true);
		GameObject.Find("AudioAndEventsCanvas").SetActive(false);
    }

    public void OnChangeToAudioAndEvents()
	{
		GameObject.Find("MapMakerCanvas").SetActive(false);
		GameObject.Find("ModalMakerCanvas").SetActive(false);
		GameObject.Find("AudioAndEventsCanvas").SetActive(true);
    }

    public void OnSave()
    {
        string result = DataCenter.GetJsonStringFromGameData(DataCenter.instance.data);

    }

    public void AddObjectToMap(GameObject obj, int posx, int posy)
    {
        obj.transform.SetParent(transform.Find("MapPanel"));
        obj.transform.position = new Vector3(posx * Constant.MAP_BLOCK_BASE_SIZE + Constant.MAP_BLOCK_BASE_SIZE / 2 + mapPartRect.x, posy * Constant.MAP_BLOCK_BASE_SIZE + Constant.MAP_BLOCK_BASE_SIZE / 2 + mapPartRect.y);
        obj.transform.localScale = blockSize;
    }

    public void OnExitEditor()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("StartScene");
    }

    public string BackgroundImage
    {
        get { return backgroundImg.sprite.name; }
        set { backgroundImg.sprite = Resources.Load<GameObject>(value).GetComponent<Sprite>(); }
    }


    private UnityEngine.UI.Image backgroundImg;
    private Rect mapPartRect;
	private Vector3 blockSize;
	private AudioClip[] allAudios;
	private GameObject[] allPrefabs;
}
