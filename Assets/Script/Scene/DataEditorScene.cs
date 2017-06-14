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
        {
            var mapId = mapMakerCanvas.transform.Find("SetPanel").transform.Find("MapId").GetComponent<Dropdown>();
            var mapList = new List<string>();
            for(int i = 0; i < DataCenter.instance.data.newGameMaps.Length; ++i)
            {
                mapList.Add(DataCenter.instance.data.newGameMaps[i].mapId + ". " + DataCenter.instance.data.newGameMaps[i].mapName);
            }
            mapId.AddOptions(mapList);

            var musicId = mapMakerCanvas.transform.Find("SetPanel").transform.Find("Music").GetComponent<Dropdown>();
            var audioList = new List<string>();
            for (int i = 0; i < DataCenter.instance.data.audios.Length; ++i)
            {
                audioList.Add(DataCenter.instance.data.audios[i].id + ". " + DataCenter.instance.data.audios[i].path);
            }
            musicId.AddOptions(audioList);

            var backModal = mapMakerCanvas.transform.Find("SetPanel").transform.Find("BackModal").GetComponent<Dropdown>();
            var currentModal = mapMakerCanvas.transform.Find("SetPanel").transform.Find("CurrentModal").GetComponent<Dropdown>();
            var modalList = new List<string>();
            for (int i = 0; i < DataCenter.instance.data.modals.Length; ++i)
            {
                modalList.Add(DataCenter.instance.data.modals[i].id + ". " + DataCenter.instance.data.modals[i].name);
            }
            backModal.AddOptions(modalList);
            currentModal.AddOptions(modalList);

            var eventId = mapMakerCanvas.transform.Find("SetPanel").transform.Find("EventId").GetComponent<Dropdown>();
            var eventIdList = new List<string>();
            for (int i = 0; i < DataCenter.instance.data.events.Length; ++i)
            {
                eventIdList.Add(DataCenter.instance.data.events[i].id.ToString());
            }
            eventId.AddOptions(eventIdList);

            mapId.value = 0;
            OnMapSelected();
        }

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

            // 显示所有prefabs
            var prefabList = modalMakerCanvas.transform.Find("prefabs").GetComponent<ScrollRect>();

            DataCenter.instance.Status = Constant.EGameStatus.InEditor;
        }

        // 初始化
        OnChangeToModals();


    }

    private void OnDestroy()
    {
        DataCenter.instance.Status = Constant.EGameStatus.Start;
        instance = null;
    }

    // Update is called once per frame
    void Update()
	{
		for (int i = 0; i < Input.touchCount; ++i)
		{
			var tc = Input.GetTouch(i);
			switch (tc.phase)
			{
				case TouchPhase.Began:
					InputController.instance.OnTouchDown(tc.position);
					break;
				case TouchPhase.Canceled:
				case TouchPhase.Ended:
					InputController.instance.OnTouchUp(tc.position, new Vector2(tc.position.x - tc.deltaPosition.x, tc.position.y - tc.deltaPosition.y));
					break;
			}
		}

		if (Input.GetMouseButtonDown(0) && !InputController.instance.isMouseLeftDown)
			InputController.instance.OnTouchDown(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
		if (Input.GetMouseButtonUp(0) && InputController.instance.isMouseLeftDown)
			InputController.instance.OnTouchUp(new Vector2(Input.mousePosition.x, Input.mousePosition.y));

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
        for (var i = 0; i < audioData.Length; ++i)
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
                OnMapSelected();
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
            if (playerIndex < 0)
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
            DataCenter.instance.data.players[playerIndex].weaponId = modalMakerCanvas.transform.Find("ModalWeaponId").GetComponent<Dropdown>().value + 1;
        }
        else if (DataCenter.instance.data.modals[index].typeId == (int)Modal.ModalType.Monster)
        {
            var monsterIndex = DataCenter.instance.GetMonsterIndexById(DataCenter.instance.data.modals[index].id);
            if (monsterIndex < 0)
            {
                monsterIndex = DataCenter.instance.AddMonster(DataCenter.instance.data.modals[index].id);
            }
            DataCenter.instance.data.monsters[monsterIndex].level = System.Convert.ToInt32(modalMakerCanvas.transform.Find("Level").GetComponent<InputField>().text);
            DataCenter.instance.data.monsters[monsterIndex].exp = System.Convert.ToInt32(modalMakerCanvas.transform.Find("Exp").GetComponent<InputField>().text);
            DataCenter.instance.data.monsters[monsterIndex].life = System.Convert.ToInt32(modalMakerCanvas.transform.Find("Life").GetComponent<InputField>().text);
            DataCenter.instance.data.monsters[monsterIndex].attack = System.Convert.ToInt32(modalMakerCanvas.transform.Find("Attack").GetComponent<InputField>().text);
            DataCenter.instance.data.monsters[monsterIndex].defense = System.Convert.ToInt32(modalMakerCanvas.transform.Find("Defense").GetComponent<InputField>().text);
            DataCenter.instance.data.monsters[monsterIndex].speed = System.Convert.ToInt32(modalMakerCanvas.transform.Find("Speed").GetComponent<InputField>().text);
            DataCenter.instance.data.monsters[monsterIndex].critical = System.Convert.ToDouble(modalMakerCanvas.transform.Find("Critical").GetComponent<InputField>().text);
            DataCenter.instance.data.monsters[monsterIndex].gold = System.Convert.ToInt32(modalMakerCanvas.transform.Find("Gold").GetComponent<InputField>().text);
            DataCenter.instance.data.monsters[monsterIndex].weaponId = modalMakerCanvas.transform.Find("ModalWeaponId").GetComponent<Dropdown>().value + 1;
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
                {
                    var drop = mapMakerCanvas.transform.Find("MapPanel").transform.Find("MapId").GetComponent<Dropdown>();
                    var ind = DataCenter.instance.AddMap();
                    var lst = new List<string>();
                    lst.Add(ind + 1 + ". (new)");
                    drop.AddOptions(lst);
                    drop.value = ind;
                    OnMapSelected();
                }
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

    public void OnMapApply()
    {
        var panel = mapMakerCanvas.transform.Find("SetPanel");
        var mapId = panel.transform.Find("MapId").GetComponent<Dropdown>().value + 1;
        var mapName = panel.transform.Find("MapName").GetComponent<InputField>().text;
        var bgMusic = panel.transform.Find("Music").GetComponent<Dropdown>().value + 1;
		var bgModal = panel.transform.Find("BackModal").GetComponent<Dropdown>().value + 1;
		var currModal = panel.transform.Find("CurrentModal").GetComponent<Dropdown>().value + 1;
		var eventId = panel.transform.Find("EventId").GetComponent<Dropdown>().value + 1;

		DataCenter.instance.data.newGameMaps[mapId - 1].mapName = mapName;
        DataCenter.instance.data.newGameMaps[mapId - 1].backThing = bgModal;
		DataCenter.instance.data.newGameMaps[mapId - 1].music = bgMusic;
		DataCenter.instance.data.newGameMaps[mapId - 1].mapBlocks[posx][posy].thing = currModal;
		DataCenter.instance.data.newGameMaps[mapId - 1].mapBlocks[posx][posy].eventId = eventId;
    }


	void OnMapSelected()
	{
		var panel = mapMakerCanvas.transform.Find("SetPanel");
		var mapId = panel.transform.Find("MapId").GetComponent<Dropdown>().value + 1;
        panel.transform.Find("MapName").GetComponent<InputField>().text = DataCenter.instance.data.newGameMaps[mapId - 1].mapName;
        panel.transform.Find("Music").GetComponent<Dropdown>().value = DataCenter.instance.data.newGameMaps[mapId - 1].music - 1;
        panel.transform.Find("BackModal").GetComponent<Dropdown>().value = DataCenter.instance.data.newGameMaps[mapId - 1].backThing - 1;
        OnMapModalSelected(false);
		OnMapClicked(new Vector3(0, 0));
		MapManager.instance.ShowMap();
    }

    public void OnMapModalSelected(bool isBack)
    {
        if (isBack)
        {
            var index = mapMakerCanvas.transform.Find("SetPanel").transform.Find("BackModal").GetComponent<Dropdown>().value;
            MapManager.instance.ChangeBack(DataCenter.instance.GetModalById(index + 1).prefabPath);
        }
        else
        {
            var index = mapMakerCanvas.transform.Find("SetPanel").transform.Find("CurrentModal").GetComponent<Dropdown>().value;
            MapManager.instance.ChangeOneBlock(DataCenter.instance.GetModalById(index + 1).prefabPath, posy, posx);
        }
    }

    public void OnMapClicked(Vector3 pos)
    {
        pos = mapMakerCanvas.transform.Find("MapPanel").transform.InverseTransformPoint(pos);
        if (pos.x >= 0 && pos.y >= 0 && pos.x <= Constant.MAP_BLOCK_LENGTH * 32 && pos.y <= Constant.MAP_BLOCK_LENGTH)
        {
            posx = ((int)pos.x) / 32;
            posy = ((int)pos.y) / 32;
            var panel = mapMakerCanvas.transform.Find("SetPanel");
            var mapId = panel.transform.Find("MapId").GetComponent<Dropdown>().value + 1;
            panel.transform.Find("CurrentPosition").GetComponent<Text>().text = "(" + posx + ", " + posy + ")";
            panel.transform.Find("CurrentModal").GetComponent<Dropdown>().value = DataCenter.instance.data.newGameMaps[mapId - 1].mapBlocks[posx][posy].thing - 1;
            panel.transform.Find("EventId").GetComponent<Dropdown>().value = DataCenter.instance.data.newGameMaps[mapId - 1].mapBlocks[posx][posy].eventId - 1;
            OnMapModalSelected(true);
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
    private int posx = 0;
    private int posy = 0;
}
