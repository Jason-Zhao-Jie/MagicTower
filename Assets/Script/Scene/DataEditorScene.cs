using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataEditorScene : MonoBehaviour
{
    public static DataEditorScene instance;
    // Use this for initialization
    void Start()
    {
        instance = this;
        AudioController.instance.MusicSource = GetComponent<AudioSource>();
        AudioController.instance.SoundSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        // 设定canvas
        mapMakerCanvas = GameObject.Find("MapMakerCanvas");
        modalMakerCanvas = GameObject.Find("ModalMakerCanvas");
        eventMakerCanvas = GameObject.Find("AudioAndEventsCanvas");

        backgroundImg = GetComponent<UnityEngine.UI.Image>();
        mapPartRect = MapManager.GetMapPosition(GameObject.Find("MapPanel").GetComponent<RectTransform>());
        blockSize = new Vector3(mapPartRect.width * 100 / Constant.MAP_BLOCK_BASE_SIZE / Constant.MAP_BLOCK_LENGTH, mapPartRect.height * 100 / Constant.MAP_BLOCK_BASE_SIZE / Constant.MAP_BLOCK_LENGTH);
        //TODO: 需要在四周添加填充墙，然后再MapManager构造地图时刷新墙

        // 载入所有资源
        allAudios = Resources.LoadAll<AudioClip>(Constant.AUDIO_DIR);
        allPrefabs = Resources.LoadAll<GameObject>(Constant.PREFAB_DIR);

        // 载入Map信息
        MapManager.instance.ShowMap();

        // 载入Modal信息
        {
            var modalId = modalMakerCanvas.transform.Find("ModalId").GetComponent<Dropdown>();
            var modalIdList = new List<string>();
            for(int i = 0; i < DataCenter.instance.data.modals.Length; ++i)
            {
                modalIdList.Add(DataCenter.instance.data.modals[i].id + ". " + DataCenter.instance.data.modals[i].name);
            }
            modalId.AddOptions(modalIdList);

            var eventId = modalMakerCanvas.transform.Find("EventId").GetComponent<Dropdown>();
            var eventIdList = new List<string>();
            for (int i = 0; i < DataCenter.instance.data.events.Length; ++i)
            {
                eventIdList.Add(DataCenter.instance.data.events[i].id.ToString());
            }
            eventId.AddOptions(eventIdList);

            var weaponIds = modalMakerCanvas.transform.Find("WeaponId").GetComponent<Dropdown>();
            var modalWeaponIds = modalMakerCanvas.transform.Find("ModalWeaponId").GetComponent<Dropdown>();
            var weaponIdList = new List<string>();
            for (int i = 0; i < DataCenter.instance.data.weapons.Length; ++i)
            {
                weaponIdList.Add(DataCenter.instance.data.weapons[i].id+". "+ DataCenter.instance.data.weapons[i].name);
            }
            weaponIds.AddOptions(weaponIdList);
            modalWeaponIds.AddOptions(weaponIdList);

        }
        // 初始化
        OnChangeToModals();


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
        mapMakerCanvas.SetActive(true);
        modalMakerCanvas.SetActive(false);
        eventMakerCanvas.SetActive(false);
    }

    public void OnChangeToModals()
	{
        mapMakerCanvas.SetActive(false);
        modalMakerCanvas.SetActive(true);
        eventMakerCanvas.SetActive(false);
    }

    public void OnChangeToAudioAndEvents()
	{
        mapMakerCanvas.SetActive(false);
        modalMakerCanvas.SetActive(false);
        eventMakerCanvas.SetActive(true);
    }

    public void OnSave()
    {
        string result = DataCenter.GetJsonStringFromGameData(DataCenter.instance.data);

    }

    public void OnRefreshAudio()
    {


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
        set { backgroundImg.sprite = Resources.Load<GameObject>(Constant.PREFAB_DIR + value).GetComponent<SpriteRenderer>().sprite; }
    }


    private UnityEngine.UI.Image backgroundImg;
    private Rect mapPartRect;
	private Vector3 blockSize;
	private AudioClip[] allAudios;
	private GameObject[] allPrefabs;
    private GameObject mapMakerCanvas;
    private GameObject modalMakerCanvas;
    private GameObject eventMakerCanvas;
}
