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
        Initializationer.InitBases(GetComponent<RectTransform>().rect.size);

        AudioController.instance.MusicSource = GetComponent<AudioSource>();
        AudioController.instance.ClearSoundSource();
        AudioController.instance.AddSoundSource(GameObject.Find("Main Camera").GetComponent<AudioSource>());
        // 设定canvas
        mapMakerCanvas = GameObject.Find("MapMakerCanvas");
        modalMakerCanvas = GameObject.Find("ModalMakerCanvas");
        eventMakerCanvas = GameObject.Find("AudioAndEventsCanvas");
        saveResult = GameObject.Find("SaveResult");
        saveResult.SetActive(false);

        backgroundImg = GetComponent<UnityEngine.UI.Image>();
        ScreenAdaptator.instance.LoadOnMainScene(mapMakerCanvas.transform.Find("MapPanel").GetComponent<RectTransform>().rect);

        curtain = mapMakerCanvas.transform.Find("MapPanel").transform.Find("Curtain").GetComponent<Curtain>();
        curtain.gameObject.SetActive(false);
        UnityEngine.Debug.Log("The current map whole rect is: " + mapMakerCanvas.transform.Find("MapPanel").GetComponent<RectTransform>().rect.width + ", " +
            mapMakerCanvas.transform.Find("MapPanel").GetComponent<RectTransform>().rect.height);
		UnityEngine.Debug.Log("The current map part rect is: " + ScreenAdaptator.instance.MapPartRect.x + ", " + ScreenAdaptator.instance.MapPartRect.y + ", " + ScreenAdaptator.instance.MapPartRect.width + ", " + ScreenAdaptator.instance.MapPartRect.height);
		UnityEngine.Debug.Log("The current map block size is: " + ScreenAdaptator.instance.BlockSize.x + ", " + ScreenAdaptator.instance.BlockSize.y);
		//TODO: 需要在四周添加填充墙，然后再MapManager构造地图时刷新墙

		// 载入所有资源
		allPrefabs = Resources.LoadAll<GameObject>(Constant.PREFAB_DIR);

        // 载入Map信息
        {
            var mapId = mapMakerCanvas.transform.Find("SetPanel").transform.Find("MapId").GetComponent<Dropdown>();
            var mapList = new List<string>();
            for(int i = 0; i < DataCenter.instance.mapLength; ++i)
            {
                mapList.Add(i.ToString());
            }
            mapId.AddOptions(mapList);

            var musicId = mapMakerCanvas.transform.Find("SetPanel").transform.Find("Music").GetComponent<Dropdown>();
            var audioList = new List<string>();
            for (int i = 0; i < DataCenter.instance.audios.Count; ++i)
            {
                audioList.Add(DataCenter.instance.audios[i + 1].id + ". " + DataCenter.instance.audios[i + 1].path);
            }
            musicId.AddOptions(audioList);

            var backModal = mapMakerCanvas.transform.Find("SetPanel").transform.Find("BackModal").GetComponent<Dropdown>();
            var currentModal = mapMakerCanvas.transform.Find("SetPanel").transform.Find("CurrentModal").GetComponent<Dropdown>();
            var modalList = new List<string>();
            for (int i = 0; i < DataCenter.instance.modals.Count; ++i)
            {
                modalList.Add(DataCenter.instance.modals[i + 1].id + ". " + DataCenter.instance.modals[i + 1].name);
            }
            backModal.AddOptions(modalList);
            modalList.Insert(0, "None");
            currentModal.AddOptions(modalList);

            var eventId = mapMakerCanvas.transform.Find("SetPanel").transform.Find("EventId").GetComponent<Dropdown>();
            var eventIdList = new List<string>();
            eventIdList.Add((EventManager.EventName.None).ToString());
            foreach (var k in EventManager.instance.eventList)
            {
                eventIdList.Add(k.Key.ToString());
            }
            eventId.AddOptions(eventIdList);

            mapId.value = 0;
            OnMapSelected();
        }

        // 载入Modal信息
        {
            var modalId = modalMakerCanvas.transform.Find("ModalId").GetComponent<Dropdown>();
            var modalIdList = new List<string>();
            for (int i = 0; i < DataCenter.instance.modals.Count; ++i)
            {
                modalIdList.Add(DataCenter.instance.modals[i + 1].id + ". " + DataCenter.instance.modals[i + 1].name);
            }
            modalId.AddOptions(modalIdList);

            var eventId = modalMakerCanvas.transform.Find("EventId").GetComponent<Dropdown>();
            var eventIdList = new List<string>();
            eventIdList.Add((EventManager.EventName.None).ToString());
            foreach (var k in EventManager.instance.eventList)
            {
                eventIdList.Add(k.Key.ToString());
            }
            eventId.AddOptions(eventIdList);

            var weaponIds = modalMakerCanvas.transform.Find("WeaponId").GetComponent<Dropdown>();
            var modalWeaponIds = modalMakerCanvas.transform.Find("ModalWeaponId").GetComponent<Dropdown>();
            var weaponIdList = new List<string>();
            for (int i = 0; i < DataCenter.instance.weapons.Count; ++i)
            {
                weaponIdList.Add(DataCenter.instance.weapons[i + 1].id + ". " + DataCenter.instance.weapons[i + 1].name);
            }
            weaponIds.AddOptions(weaponIdList);
            modalWeaponIds.AddOptions(weaponIdList);

            var weaponHit = modalMakerCanvas.transform.Find("WeaponHit").GetComponent<Dropdown>();
            var weaponCrit = modalMakerCanvas.transform.Find("WeaponCrit").GetComponent<Dropdown>();
            var audioList = new List<string>();
            for (int i = 0; i < DataCenter.instance.audios.Count; ++i)
            {
                audioList.Add(DataCenter.instance.audios[i + 1].id + ". " + DataCenter.instance.audios[i + 1].path);
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
        OnChangeToMaps();


    }

    private void OnDestroy()
    {
        MapManager.instance.ClearMap();
        DataCenter.instance.Status = Constant.EGameStatus.Start;
        instance = null;
        ObjectPool.instance.ClearAll();
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

        if (Input.touchCount <= 0)
        {
            if (Input.GetMouseButtonDown(0) && !InputController.instance.isMouseLeftDown)
                InputController.instance.OnTouchDown(new Vector2(Input.mousePosition.x, Input.mousePosition.y), true);
            if (Input.GetMouseButtonUp(0) && InputController.instance.isMouseLeftDown)
                InputController.instance.OnTouchUp(new Vector2(Input.mousePosition.x, Input.mousePosition.y), false);
        }
	}

    // "Map"按钮回调
    public void OnChangeToMaps()
    {
        mapMakerCanvas.SetActive(true);
        modalMakerCanvas.SetActive(false);
        eventMakerCanvas.SetActive(false);
        var panel = mapMakerCanvas.transform.Find("SetPanel");
        var mapId = panel.transform.Find("MapId").GetComponent<Dropdown>().value + 1;
        MapManager.instance.ShowMap(mapId);
    }

    // Modal&Audio按钮回调
    public void OnChangeToModals()
    {
        mapMakerCanvas.SetActive(false);
        modalMakerCanvas.SetActive(true);
        eventMakerCanvas.SetActive(false);
        BackgroundImage = "BrownWall";
    }

    // Event 按钮回调
    public void OnChangeToAudioAndEvents()
    {
        mapMakerCanvas.SetActive(false);
        modalMakerCanvas.SetActive(false);
        eventMakerCanvas.SetActive(true);
        BackgroundImage = "BrownWall";
    }

    // GetDataJson按钮回调
    public void OnSave()
    {
        if (saveResult.activeSelf == false)
        {
            string result = DataCenter.instance.SaveData();
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

    // GetMapJson 按钮回调
    public void OnMapSave()
    {
        if (saveResult.activeSelf == false)
        {
            string result = DataCenter.instance.GetGameMap(mapMakerCanvas.transform.Find("SetPanel").transform.Find("MapId").GetComponent<Dropdown>().value).Json.String;
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

    // Refresh audios 按钮回调
	public void OnRefreshAudio()
	{
		var allAudios = Resources.LoadAll<AudioClip>(Constant.AUDIO_DIR);
		Constant.Audio[] audioData = new Constant.Audio[allAudios.Length];
        DataCenter.instance.audios.Clear();
        for (var i = 0; i < audioData.Length; ++i)
        {
            DataCenter.instance.audios.Add(audioData[i].id, new Constant.Audio()
            {
                id = i + 1,
                path = allAudios[i].name
            });
        }
		//PlatformUIManager.ShowMessageBox("音效表刷新成功，请立即保存配置然后重新启动游戏！");
	}

    // Auto collect prefabs 按钮回调
	public void OnRefreshPrefab()
	{
        Constant.ModalData[] prefabData = new Constant.ModalData[allPrefabs.Length];
        DataCenter.instance.modals.Clear();
        for (var i = 0; i < prefabData.Length; ++i)
        {
            DataCenter.instance.modals.Add(prefabData[i + 1].id, new Constant.ModalData()
            {
                id = i + 1,
                name = allPrefabs[i].name,
                prefabPath = allPrefabs[i].name,
                eventId = 0,
                eventData = 0,
                typeId = 2
            });
        }
		//PlatformUIManager.ShowMessageBox("模型表刷新成功，请立即保存配置然后重新启动游戏！");
	}

    // 添加物体到map panel
    public void AddObjectToMap(GameObject obj, int posx, int posy, int posz = -2)
	{
		obj.transform.SetParent(mapMakerCanvas.transform.Find("MapPanel"), false);
		obj.transform.position = mapMakerCanvas.transform.Find("MapPanel").transform.
            TransformPoint(new Vector3((posx + (float)0.5) * Constant.MAP_BLOCK_BASE_SIZE * ScreenAdaptator.instance.BlockSize.x / 100 + ScreenAdaptator.instance.MapPartRect.x,
                                       (posy + (float)0.5) * Constant.MAP_BLOCK_BASE_SIZE * ScreenAdaptator.instance.BlockSize.y / 100 + ScreenAdaptator.instance.MapPartRect.y,
                                       posz));
        obj.transform.localScale = ScreenAdaptator.instance.BlockSize;
    }

    // 关闭(X)按钮回调
    public void OnExitEditor()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("StartScene");
    }

    // modal部分modal选择框
    public void OnModalSelected(int index)
    {
        if (index == 0)
            index = modalMakerCanvas.transform.Find("ModalId").GetComponent<Dropdown>().value;
        ++index;
        modalMakerCanvas.transform.Find("ModalName").GetComponent<InputField>().text = DataCenter.instance.modals[index].name;
        modalMakerCanvas.transform.Find("ModalType").GetComponent<Dropdown>().value = DataCenter.instance.modals[index].typeId - 1;
        modalMakerCanvas.transform.Find("EventId").GetComponent<Dropdown>().value = DataCenter.instance.modals[index].eventId;
        modalMakerCanvas.transform.Find("EventData").GetComponent<InputField>().text = DataCenter.instance.modals[index].eventData.ToString();
        modalMakerCanvas.transform.Find("ModalPrefab").GetComponent<InputField>().text = DataCenter.instance.modals[index].prefabPath;
        if (DataCenter.instance.modals[index].typeId == (int)Modal.ModalType.Player)
        {
            var player = DataCenter.instance.players[DataCenter.instance.modals[index].id];
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
        else if (DataCenter.instance.modals[index].typeId == (int)Modal.ModalType.Monster)
        {
            var monster = DataCenter.instance.monsters[DataCenter.instance.modals[index].id];
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

    // Reset 按钮回调
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

    // Apply modal 按钮回调
    public void OnModalApply()
    {
        var index = modalMakerCanvas.transform.Find("ModalId").GetComponent<Dropdown>().value;
        DataCenter.instance.modals[index].name = modalMakerCanvas.transform.Find("ModalName").GetComponent<InputField>().text;
        DataCenter.instance.modals[index].typeId = modalMakerCanvas.transform.Find("ModalType").GetComponent<Dropdown>().value + 1;
        DataCenter.instance.modals[index].eventId = modalMakerCanvas.transform.Find("EventId").GetComponent<Dropdown>().value;
        DataCenter.instance.modals[index].eventData =System.Convert.ToInt64(modalMakerCanvas.transform.Find("EventData").GetComponent<InputField>().text);
        DataCenter.instance.modals[index].prefabPath = modalMakerCanvas.transform.Find("ModalPrefab").GetComponent<InputField>().text;
        if (DataCenter.instance.modals[index].typeId == (int)Modal.ModalType.Player)
        {
            var playerId = DataCenter.instance.modals[index].id;
            if (!DataCenter.instance.players.ContainsKey(playerId))
            {
                DataCenter.instance.players.Add(playerId, new Constant.PlayerData()
                {
                    level = System.Convert.ToInt32(modalMakerCanvas.transform.Find("Level").GetComponent<InputField>().text),
                    exp = System.Convert.ToInt32(modalMakerCanvas.transform.Find("Exp").GetComponent<InputField>().text),
                    life = System.Convert.ToInt32(modalMakerCanvas.transform.Find("Life").GetComponent<InputField>().text),
                    attack = System.Convert.ToInt32(modalMakerCanvas.transform.Find("Attack").GetComponent<InputField>().text),
                    defense = System.Convert.ToInt32(modalMakerCanvas.transform.Find("Defense").GetComponent<InputField>().text),
                    speed = System.Convert.ToInt32(modalMakerCanvas.transform.Find("Speed").GetComponent<InputField>().text),
                    critical = System.Convert.ToDouble(modalMakerCanvas.transform.Find("Critical").GetComponent<InputField>().text),
                    gold = System.Convert.ToInt32(modalMakerCanvas.transform.Find("Gold").GetComponent<InputField>().text),
                    weaponId = modalMakerCanvas.transform.Find("ModalWeaponId").GetComponent<Dropdown>().value + 1
                });
            }
        }
        else if (DataCenter.instance.modals[index].typeId == (int)Modal.ModalType.Monster)
        {
            var monsterId = DataCenter.instance.modals[index].id;
            if (!DataCenter.instance.monsters.ContainsKey(monsterId))
            {
                DataCenter.instance.monsters.Add(monsterId, new Constant.MonsterData()
                {
                    level = System.Convert.ToInt32(modalMakerCanvas.transform.Find("Level").GetComponent<InputField>().text),
                    exp = System.Convert.ToInt32(modalMakerCanvas.transform.Find("Exp").GetComponent<InputField>().text),
                    life = System.Convert.ToInt32(modalMakerCanvas.transform.Find("Life").GetComponent<InputField>().text),
                    attack = System.Convert.ToInt32(modalMakerCanvas.transform.Find("Attack").GetComponent<InputField>().text),
                    defense = System.Convert.ToInt32(modalMakerCanvas.transform.Find("Defense").GetComponent<InputField>().text),
                    speed = System.Convert.ToInt32(modalMakerCanvas.transform.Find("Speed").GetComponent<InputField>().text),
                    critical = System.Convert.ToDouble(modalMakerCanvas.transform.Find("Critical").GetComponent<InputField>().text),
                    gold = System.Convert.ToInt32(modalMakerCanvas.transform.Find("Gold").GetComponent<InputField>().text),
                    weaponId = modalMakerCanvas.transform.Find("ModalWeaponId").GetComponent<Dropdown>().value + 1
                });
            }
        }
    }

    // Weapon选择框回调
    public void OnWeaponSelected(int index)
    {
        if (index == 0)
            index = modalMakerCanvas.transform.Find("WeaponId").GetComponent<Dropdown>().value;
        ++index;
        modalMakerCanvas.transform.Find("WeaponName").GetComponent<InputField>().text = DataCenter.instance.weapons[index].name;
        modalMakerCanvas.transform.Find("WeaponHit").GetComponent<Dropdown>().value = DataCenter.instance.weapons[index].audioId - 1;
        modalMakerCanvas.transform.Find("WeaponCrit").GetComponent<Dropdown>().value = DataCenter.instance.weapons[index].critAudioId - 1;
        modalMakerCanvas.transform.Find("WeaponPrefab").GetComponent<InputField>().text = DataCenter.instance.weapons[index].prefabPath;
    }

    // Apply Weapon 按钮回调
    public void OnWeaponApply()
    {
        var index = modalMakerCanvas.transform.Find("WeaponId").GetComponent<Dropdown>().value;
        DataCenter.instance.weapons[index].name = modalMakerCanvas.transform.Find("WeaponName").GetComponent<InputField>().text;
        DataCenter.instance.weapons[index].audioId = modalMakerCanvas.transform.Find("WeaponHit").GetComponent<Dropdown>().value + 1;
        DataCenter.instance.weapons[index].critAudioId = modalMakerCanvas.transform.Find("WeaponCrit").GetComponent<Dropdown>().value + 1;
        DataCenter.instance.weapons[index].prefabPath = modalMakerCanvas.transform.Find("WeaponPrefab").GetComponent<InputField>().text;
    }

    // Add map, Add modal, Add weapon 按钮的回调
    public void OnAdd(int index)
    {
        switch (index)
        {
            case 0:
                {
                    var drop = mapMakerCanvas.transform.Find("MapPanel").transform.Find("MapId").GetComponent<Dropdown>();
                    //var ind = DataCenter.instance.AddMap();
                    var lst = new List<string>();
                    //lst.Add(ind + 1 + ". (new)");
                    drop.AddOptions(lst);
                    //drop.value = ind;
                    OnMapSelected();
                }
                break;
            case 1:
                {
                    var drop = modalMakerCanvas.transform.Find("ModalId").GetComponent<Dropdown>();
                    var ind = DataCenter.instance.modals.Count;
                    DataCenter.instance.modals.Add(ind, new Constant.ModalData() { id = ind });
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
                    var ind = DataCenter.instance.weapons.Count;
                    DataCenter.instance.weapons.Add(ind, new Constant.WeaponData() { id = ind });
                    var lst = new List<string>();
                    lst.Add(ind + 1 + ". (new)");
                    drop.AddOptions(lst);
                    drop.value = ind;
                    OnWeaponSelected(ind);
                }
                break;
        }
    }

    // Apply Map 回调
    public void OnMapApply()
    {
        var panel = mapMakerCanvas.transform.Find("SetPanel");
        var mapId = panel.transform.Find("MapId").GetComponent<Dropdown>().value;
        var mapName = panel.transform.Find("MapName").GetComponent<InputField>().text;
        var bgMusic = panel.transform.Find("Music").GetComponent<Dropdown>().value + 1;
		var bgModal = panel.transform.Find("BackModal").GetComponent<Dropdown>().value + 1;
		var currModal = panel.transform.Find("CurrentModal").GetComponent<Dropdown>().value;
        var eventId = panel.transform.Find("EventId").GetComponent<Dropdown>().value;
        long eventData = 0;
        try
        {
            eventData = System.Convert.ToInt64(panel.transform.Find("EventData").GetComponent<InputField>().text);
        }catch (System.FormatException e)
        {
            eventData = 0;
        }
        DataCenter.instance.GetGameMap(mapId).mapName = mapName;
        DataCenter.instance.GetGameMap(mapId).backThing = bgModal;
		DataCenter.instance.GetGameMap(mapId).music = bgMusic;
		DataCenter.instance.GetGameMap(mapId).mapBlocks[posx][posy].thing = currModal;
        DataCenter.instance.GetGameMap(mapId).mapBlocks[posx][posy].eventId = eventId;
        DataCenter.instance.GetGameMap(mapId).mapBlocks[posx][posy].eventData = eventData;
        DataCenter.instance.SaveMapTo(mapId);
    }

    // Map选择框回调
	void OnMapSelected()
	{
		var panel = mapMakerCanvas.transform.Find("SetPanel");
		var mapId = panel.transform.Find("MapId").GetComponent<Dropdown>().value + 1;
        panel.transform.Find("MapName").GetComponent<InputField>().text = DataCenter.instance.GetGameMap(mapId - 1).mapName;
        panel.transform.Find("Music").GetComponent<Dropdown>().value = DataCenter.instance.GetGameMap(mapId - 1).music - 1;
        panel.transform.Find("BackModal").GetComponent<Dropdown>().value = DataCenter.instance.GetGameMap(mapId - 1).backThing - 1;
		MapManager.instance.ShowMap(mapId);
        OnMapClicked(new Vector2(0, 0));
        AudioController.instance.StopMusic();
    }

    // Map地图块物品选择框回调
    public void OnMapModalSelected(bool isBack)
    {
        if (isBack)
        {
            var index = mapMakerCanvas.transform.Find("SetPanel").transform.Find("BackModal").GetComponent<Dropdown>().value;
            MapManager.instance.ChangeBack(DataCenter.instance.modals[index + 1].prefabPath);
        }
        else
        {
            var index = mapMakerCanvas.transform.Find("SetPanel").transform.Find("CurrentModal").GetComponent<Dropdown>().value;
            MapManager.instance.ChangeThingOnMap(index, posx, posy);
        }
    }

    // Map选定地图块位置的回调
    public void OnMapClicked(Vector2 pos)
    {
        if (!mapMakerCanvas.activeSelf)
            return;
        var mapPanel = mapMakerCanvas.transform.Find("MapPanel").GetComponent<RectTransform>();
        var panelPos = mapMakerCanvas.transform.InverseTransformPoint(mapPanel.position);
        pos.x -= panelPos.x + ScreenAdaptator.instance.MapPartRect.x + mapMakerCanvas.GetComponent<RectTransform>().rect.width / 2;
        pos.y -= panelPos.y + ScreenAdaptator.instance.MapPartRect.y + mapMakerCanvas.GetComponent<RectTransform>().rect.height / 2;
        if (pos.x >= 0 && pos.y >= 0)
        {
            var _posx = (int)(pos.x * Constant.MAP_BLOCK_LENGTH / ScreenAdaptator.instance.MapPartRect.width);
            var _posy = (int)(pos.y * Constant.MAP_BLOCK_LENGTH / ScreenAdaptator.instance.MapPartRect.height);
            if (_posx >= Constant.MAP_BLOCK_LENGTH || _posy >= Constant.MAP_BLOCK_LENGTH)
                return;
            posx = _posx;
            posy = _posy;
            var panel = mapMakerCanvas.transform.Find("SetPanel");
            var mapId = panel.transform.Find("MapId").GetComponent<Dropdown>().value + 1;
            panel.transform.Find("CurrentPosition").GetComponent<Text>().text = "(" + posx + ", " + posy + ")";
            panel.transform.Find("CurrentModal").GetComponent<Dropdown>().value = DataCenter.instance.GetGameMap(mapId - 1).mapBlocks[posx][posy].thing;
            panel.transform.Find("EventId").GetComponent<Dropdown>().value = DataCenter.instance.GetGameMap(mapId - 1).mapBlocks[posx][posy].eventId;
            panel.transform.Find("EventData").GetComponent<InputField>().text = DataCenter.instance.GetGameMap(mapId - 1).mapBlocks[posx][posy].eventData.ToString();
        }
    }

    // 音乐播放键回调
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
    public Curtain Curtain { get { return curtain; } }


    private UnityEngine.UI.Image backgroundImg;
    private Curtain curtain;
    private GameObject[] allPrefabs;
    private GameObject mapMakerCanvas;
    private GameObject modalMakerCanvas;
    private GameObject eventMakerCanvas;
    private GameObject saveResult;
    private int posx = 0;
    private int posy = 0;
}
