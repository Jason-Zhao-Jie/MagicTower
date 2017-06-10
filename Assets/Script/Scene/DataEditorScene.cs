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
        saveResult = GameObject.Find("SaveResult");
        saveResult.SetActive(false);

        backgroundImg = GetComponent<UnityEngine.UI.Image>();
        mapPartRect = MapManager.GetMapPosition(GameObject.Find("MapPanel").GetComponent<RectTransform>());
        blockSize = new Vector3(mapPartRect.width * 100 / Constant.MAP_BLOCK_BASE_SIZE / Constant.MAP_BLOCK_LENGTH, mapPartRect.height * 100 / Constant.MAP_BLOCK_BASE_SIZE / Constant.MAP_BLOCK_LENGTH);
        //TODO: 需要在四周添加填充墙，然后再MapManager构造地图时刷新墙

        // 载入所有资源
        allPrefabs = Resources.LoadAll<GameObject>(Constant.PREFAB_DIR);

        // 载入Map信息
        MapManager.instance.ShowMap();

        // 载入Modal信息
        {
            var modalId = modalMakerCanvas.transform.Find("ModalId").GetComponent<Dropdown>();
            var modalIdList = new List<string>();
            for (int i = 0; i < DataCenter.instance.data.modals.Length; ++i)
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
                weaponIdList.Add(DataCenter.instance.data.weapons[i].id + ". " + DataCenter.instance.data.weapons[i].name);
            }
            weaponIds.AddOptions(weaponIdList);
            modalWeaponIds.AddOptions(weaponIdList);

            var weaponHit = modalMakerCanvas.transform.Find("WeaponHit").GetComponent<Dropdown>();
            var weaponCrit = modalMakerCanvas.transform.Find("WeaponCrit").GetComponent<Dropdown>();
            var audioList = new List<string>();
            for (int i = 0; i < DataCenter.instance.data.audios.Length; ++i)
            {
                audioList.Add(DataCenter.instance.data.audios[i].id + ". " + DataCenter.instance.data.audios[i].path);
            }
            weaponHit.AddOptions(audioList);
            weaponCrit.AddOptions(audioList);

            modalId.value = 0;
            weaponIds.value = 0;
            OnModalSelected(0);
            OnWeaponSelected(0);
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
        if (saveResult.activeSelf == false)
        {
            string result = DataCenter.GetJsonStringFromGameData(DataCenter.instance.data);
            saveResult.SetActive(true);
            saveResult.GetComponent<InputField>().text = result;
            mapMakerCanvas.SetActive(false);
            modalMakerCanvas.SetActive(false);
            eventMakerCanvas.SetActive(false);
        }
        else
        {
            saveResult.SetActive(false);
        }
    }

    public void OnRefreshAudio()
    {
        var allAudios = Resources.LoadAll<AudioClip>(Constant.AUDIO_DIR);
        Constant.Audio[] audioData = new Constant.Audio[allAudios.Length];
        for(var i = 0; i < audioData.Length; ++i)
        {
            audioData[i] = new Constant.Audio();
            audioData[i].id = i + 1;
            audioData[i].path = allAudios[i].name;
        }
        DataCenter.instance.data.audios = audioData;
        PlatformUIManager.ShowMessageBox("音效表刷新成功，请立即保存配置然后重新启动游戏！");
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

    public void OnModalSelected(int index)
    {
        if (index == 0)
            index = modalMakerCanvas.transform.Find("ModalId").GetComponent<Dropdown>().value;
        modalMakerCanvas.transform.Find("ModalName").GetComponent<InputField>().text = DataCenter.instance.data.modals[index].name;
        modalMakerCanvas.transform.Find("ModalType").GetComponent<Dropdown>().value = DataCenter.instance.data.modals[index].typeId - 1;
        modalMakerCanvas.transform.Find("EventId").GetComponent<Dropdown>().value = DataCenter.instance.data.modals[index].eventId - 1;
        modalMakerCanvas.transform.Find("ModalPrefab").GetComponent<InputField>().text = DataCenter.instance.data.modals[index].prefabPath;
        if (DataCenter.instance.data.modals[index].typeId == (int)Modal.ModalType.Player)
        {
            var player = DataCenter.instance.GetPlayerDataById(DataCenter.instance.data.modals[index].id);
            modalMakerCanvas.transform.Find("Level").GetComponent<InputField>().text = player.level.ToString();
            modalMakerCanvas.transform.Find("Exp").GetComponent<InputField>().text = player.exp.ToString();
            modalMakerCanvas.transform.Find("Life").GetComponent<InputField>().text = player.life.ToString();
            modalMakerCanvas.transform.Find("Attack").GetComponent<InputField>().text = player.attack.ToString();
            modalMakerCanvas.transform.Find("Defense").GetComponent<InputField>().text = player.defense.ToString();
            modalMakerCanvas.transform.Find("Speed").GetComponent<InputField>().text = player.speed.ToString();
            modalMakerCanvas.transform.Find("Critical").GetComponent<InputField>().text = player.critical.ToString();
            modalMakerCanvas.transform.Find("Gold").GetComponent<InputField>().text = player.gold.ToString();
            modalMakerCanvas.transform.Find("ModalWeaponId").GetComponent<Dropdown>().value = player.weaponId - 1;
        }
        else if (DataCenter.instance.data.modals[index].typeId == (int)Modal.ModalType.Monster)
        {
            var monster = DataCenter.instance.GetMonsterDataById(DataCenter.instance.data.modals[index].id);
            modalMakerCanvas.transform.Find("Level").GetComponent<InputField>().text = monster.level.ToString();
            modalMakerCanvas.transform.Find("Exp").GetComponent<InputField>().text = monster.exp.ToString();
            modalMakerCanvas.transform.Find("Life").GetComponent<InputField>().text = monster.life.ToString();
            modalMakerCanvas.transform.Find("Attack").GetComponent<InputField>().text = monster.attack.ToString();
            modalMakerCanvas.transform.Find("Defense").GetComponent<InputField>().text = monster.defense.ToString();
            modalMakerCanvas.transform.Find("Speed").GetComponent<InputField>().text = monster.speed.ToString();
            modalMakerCanvas.transform.Find("Critical").GetComponent<InputField>().text = monster.critical.ToString();
            modalMakerCanvas.transform.Find("Gold").GetComponent<InputField>().text = monster.gold.ToString();
            modalMakerCanvas.transform.Find("ModalWeaponId").GetComponent<Dropdown>().value = monster.weaponId - 1;
        }
        else
        {
            modalMakerCanvas.transform.Find("Level").GetComponent<InputField>().text = "";
            modalMakerCanvas.transform.Find("Exp").GetComponent<InputField>().text = "";
            modalMakerCanvas.transform.Find("Life").GetComponent<InputField>().text = "";
            modalMakerCanvas.transform.Find("Attack").GetComponent<InputField>().text = "";
            modalMakerCanvas.transform.Find("Defense").GetComponent<InputField>().text = "";
            modalMakerCanvas.transform.Find("Speed").GetComponent<InputField>().text = "";
            modalMakerCanvas.transform.Find("Critical").GetComponent<InputField>().text = "";
            modalMakerCanvas.transform.Find("Gold").GetComponent<InputField>().text = "";
            modalMakerCanvas.transform.Find("ModalWeaponId").GetComponent<Dropdown>().value = 0;
        }
    }

    public void OnReset(int part)
    {
        switch (part)
        {
            case 0:
                break;
            case 1:
                OnModalSelected(0);
                break;
            case 2:
                OnWeaponSelected(0);
                break;
        }
    }

    public void OnModalApply()
    {
        var index = modalMakerCanvas.transform.Find("ModalId").GetComponent<Dropdown>().value;
        DataCenter.instance.data.modals[index].name = modalMakerCanvas.transform.Find("ModalName").GetComponent<InputField>().text;
        DataCenter.instance.data.modals[index].typeId = modalMakerCanvas.transform.Find("ModalType").GetComponent<Dropdown>().value + 1;
        DataCenter.instance.data.modals[index].eventId = modalMakerCanvas.transform.Find("EventId").GetComponent<Dropdown>().value + 1;
        DataCenter.instance.data.modals[index].prefabPath = modalMakerCanvas.transform.Find("ModalPrefab").GetComponent<InputField>().text;
        if (DataCenter.instance.data.modals[index].typeId == (int)Modal.ModalType.Player)
        {
            var playerIndex = DataCenter.instance.GetPlayerIndexById(DataCenter.instance.data.modals[index].id);
            if(playerIndex<0)
            {
                playerIndex = DataCenter.instance.AddPlayer(DataCenter.instance.data.modals[index].id);
            }
            DataCenter.instance.data.players[playerIndex].level = System.Convert.ToInt32(modalMakerCanvas.transform.Find("Level").GetComponent<InputField>().text);
            DataCenter.instance.data.players[playerIndex].exp = System.Convert.ToInt32(modalMakerCanvas.transform.Find("Exp").GetComponent<InputField>().text);
            DataCenter.instance.data.players[playerIndex].life = System.Convert.ToInt32(modalMakerCanvas.transform.Find("Life").GetComponent<InputField>().text);
            DataCenter.instance.data.players[playerIndex].attack = System.Convert.ToInt32(modalMakerCanvas.transform.Find("Attack").GetComponent<InputField>().text);
            DataCenter.instance.data.players[playerIndex].defense = System.Convert.ToInt32(modalMakerCanvas.transform.Find("Defense").GetComponent<InputField>().text);
            DataCenter.instance.data.players[playerIndex].speed = System.Convert.ToInt32(modalMakerCanvas.transform.Find("Speed").GetComponent<InputField>().text);
            DataCenter.instance.data.players[playerIndex].critical = System.Convert.ToDouble(modalMakerCanvas.transform.Find("Critical").GetComponent<InputField>().text);
            DataCenter.instance.data.players[playerIndex].gold = System.Convert.ToInt32(modalMakerCanvas.transform.Find("Gold").GetComponent<InputField>().text);
            DataCenter.instance.data.players[playerIndex].weaponId = modalMakerCanvas.transform.Find("ModalWeaponId").GetComponent<Dropdown>().value;
        }
        else if (DataCenter.instance.data.modals[index].typeId == (int)Modal.ModalType.Monster)
        {
            var monsterIndex = DataCenter.instance.GetMonsterIndexById(DataCenter.instance.data.modals[index].id);
            if (monsterIndex < 0)
            {
                monsterIndex = DataCenter.instance.AddMonster(DataCenter.instance.data.modals[index].id);
            }
            DataCenter.instance.data.players[monsterIndex].level = System.Convert.ToInt32(modalMakerCanvas.transform.Find("Level").GetComponent<InputField>().text);
            DataCenter.instance.data.players[monsterIndex].exp = System.Convert.ToInt32(modalMakerCanvas.transform.Find("Exp").GetComponent<InputField>().text);
            DataCenter.instance.data.players[monsterIndex].life = System.Convert.ToInt32(modalMakerCanvas.transform.Find("Life").GetComponent<InputField>().text);
            DataCenter.instance.data.players[monsterIndex].attack = System.Convert.ToInt32(modalMakerCanvas.transform.Find("Attack").GetComponent<InputField>().text);
            DataCenter.instance.data.players[monsterIndex].defense = System.Convert.ToInt32(modalMakerCanvas.transform.Find("Defense").GetComponent<InputField>().text);
            DataCenter.instance.data.players[monsterIndex].speed = System.Convert.ToInt32(modalMakerCanvas.transform.Find("Speed").GetComponent<InputField>().text);
            DataCenter.instance.data.players[monsterIndex].critical = System.Convert.ToDouble(modalMakerCanvas.transform.Find("Critical").GetComponent<InputField>().text);
            DataCenter.instance.data.players[monsterIndex].gold = System.Convert.ToInt32(modalMakerCanvas.transform.Find("Gold").GetComponent<InputField>().text);
            DataCenter.instance.data.players[monsterIndex].weaponId = modalMakerCanvas.transform.Find("ModalWeaponId").GetComponent<Dropdown>().value;
        }
    }

    public void OnWeaponSelected(int index)
    {
        if (index == 0)
            index = modalMakerCanvas.transform.Find("WeaponId").GetComponent<Dropdown>().value;
        modalMakerCanvas.transform.Find("WeaponName").GetComponent<InputField>().text = DataCenter.instance.data.weapons[index].name;
        modalMakerCanvas.transform.Find("WeaponHit").GetComponent<Dropdown>().value = DataCenter.instance.data.weapons[index].audioId - 1;
        modalMakerCanvas.transform.Find("WeaponCrit").GetComponent<Dropdown>().value = DataCenter.instance.data.weapons[index].critAudioId - 1;
        modalMakerCanvas.transform.Find("WeaponPrefab").GetComponent<InputField>().text = DataCenter.instance.data.weapons[index].prefabPath;
    }

    public void OnWeaponApply()
    {
        var index = modalMakerCanvas.transform.Find("WeaponId").GetComponent<Dropdown>().value;
        DataCenter.instance.data.weapons[index].name = modalMakerCanvas.transform.Find("WeaponName").GetComponent<InputField>().text;
        DataCenter.instance.data.weapons[index].audioId = modalMakerCanvas.transform.Find("WeaponHit").GetComponent<Dropdown>().value + 1;
        DataCenter.instance.data.weapons[index].critAudioId = modalMakerCanvas.transform.Find("WeaponCrit").GetComponent<Dropdown>().value + 1;
        DataCenter.instance.data.weapons[index].prefabPath = modalMakerCanvas.transform.Find("WeaponPrefab").GetComponent<InputField>().text;
    }

    public void OnAdd(int index)
    {
        switch (index)
        {
            case 0:
                break;
            case 1:
                {
                    var drop = modalMakerCanvas.transform.Find("ModalId").GetComponent<Dropdown>();
                    var ind = DataCenter.instance.AddModal();
                    var lst = new List<string>();
                    lst.Add(ind + 1 + ". (new)");
                    drop.AddOptions(lst);
                    drop.value = ind;
                    OnModalSelected(ind);
                }
                break;
            case 2:
                {
                    var drop = modalMakerCanvas.transform.Find("WeaponId").GetComponent<Dropdown>();
                    var ind = DataCenter.instance.AddWeapon();
                    var lst = new List<string>();
                    lst.Add(ind + 1 + ". (new)");
                    drop.AddOptions(lst);
                    drop.value = ind;
                    OnWeaponSelected(ind);
                }
                break;
        }
    }

    public void OnPlay(int index)
    {
        switch (index)
        {
            case 0:
                AudioController.instance.PlayMusicLoop(GameObject.Find("Music").GetComponent<Dropdown>().value + 1);
                break;
            case 1:
                AudioController.instance.StopMusic();
                AudioController.instance.PlaySound(GameObject.Find("WeaponHit").GetComponent<Dropdown>().value + 1);
                break;
            case 2:
                AudioController.instance.StopMusic();
                AudioController.instance.PlaySound(GameObject.Find("WeaponCrit").GetComponent<Dropdown>().value + 1);
                break;
        }
    }

    public string BackgroundImage
    {
        get { return backgroundImg.sprite.name; }
        set { backgroundImg.sprite = Resources.Load<GameObject>(Constant.PREFAB_DIR + value).GetComponent<SpriteRenderer>().sprite; }
    }


    private UnityEngine.UI.Image backgroundImg;
    private Rect mapPartRect;
	private Vector3 blockSize;
	private GameObject[] allPrefabs;
    private GameObject mapMakerCanvas;
    private GameObject modalMakerCanvas;
    private GameObject eventMakerCanvas;
    private GameObject saveResult;
}
