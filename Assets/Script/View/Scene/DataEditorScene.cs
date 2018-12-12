using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

public class DataEditorScene : MonoBehaviour {
    public static DataEditorScene instance;

    public static readonly ReadOnlyDictionary<string, string> UI_LANG_KEY = new ReadOnlyDictionary<string, string>(new Dictionary<string, string> {
        ["en-us"] = "ENUS",
        ["zh-cn"] = "ZHCN",
        ["zh-tw"] = "ZHTW",
        ["sp-sp"] = "SPSP",
        ["ge-ge"] = "GEGE",
        ["fr-fr"] = "FRFR",
        ["ru-ru"] = "RURU",
        ["jp-jp"] = "JPJP",
        ["kr-kr"] = "KRKR",
        ["hi-in"] = "HIIN",
    });

    // Use this for initialization
    void Start() {
        // 初始化游戏信息
        instance = this;
        Initializationer.InitBases(GetComponent<RectTransform>().rect.size);
        AudioController.instance.MusicSource = GetComponent<AudioSource>();
        AudioController.instance.ClearSoundSource();
        AudioController.instance.AddSoundSource(GameObject.Find("Main Camera").GetComponent<AudioSource>());

        // 设定canvas
        mapMakerPanel = GameObject.Find("MapMakerPanel");
        modalMakerPanel = GameObject.Find("ModalMakerPanel");
        stringChatsAndChoicesPanel = GameObject.Find("StringChatsAndChoicesPanel");
        saveResult = GameObject.Find("SaveResult");
        saveResult.SetActive(false);
        prefabList = modalMakerPanel.transform.Find("prefabs").GetComponent<ListView>();
        // 设定背景和地图区域
        backgroundImg = GetComponent<Image>();
        ScreenAdaptator.instance.LoadOnMainScene(mapMakerPanel.transform.Find("MapPanel").GetComponent<RectTransform>().rect);
        // 设定幕布和填充墙
        curtain = mapMakerPanel.transform.Find("MapPanel").transform.Find("Curtain").GetComponent<Curtain>();
        curtain.gameObject.SetActive(false);
        //UnityEngine.Debug.Log("The current map whole rect is: " + mapMakerPanel.transform.Find("MapPanel").GetComponent<RectTransform>().rect.width + ", " + mapMakerPanel.transform.Find("MapPanel").GetComponent<RectTransform>().rect.height);
        //UnityEngine.Debug.Log("The current map part rect is: " + ScreenAdaptator.instance.MapPartRect.x + ", " + ScreenAdaptator.instance.MapPartRect.y + ", " + ScreenAdaptator.instance.MapPartRect.width + ", " + ScreenAdaptator.instance.MapPartRect.height);
        //UnityEngine.Debug.Log("The current map block size is: " + ScreenAdaptator.instance.BlockSize.x + ", " + ScreenAdaptator.instance.BlockSize.y);
        //TODO: 需要在四周添加填充墙，然后再MapManager构造地图时刷新墙

        // 载入所有资源
        allPrefabs = Resources.LoadAll<GameObject>(Constant.PREFAB_DIR);

        // 载入Map信息
        {
            var mapId = mapMakerPanel.transform.Find("SetPanel").transform.Find("MapId").GetComponent<Dropdown>();
            var mapList = new List<string>();
            for (int i = 0; i < DataCenter.instance.mapLength; ++i) {
                mapList.Add(i.ToString());
            }
            mapId.AddOptions(mapList);

            var musicId = mapMakerPanel.transform.Find("SetPanel").transform.Find("Music").GetComponent<Dropdown>();
            var audioList = new List<string>();
            for (int i = 0; i < DataCenter.instance.audios.Count; ++i) {
                audioList.Add(DataCenter.instance.audios[i + 1].id + ". " + DataCenter.instance.audios[i + 1].path);
            }
            musicId.AddOptions(audioList);

            var backModal = mapMakerPanel.transform.Find("SetPanel").transform.Find("BackModal").GetComponent<Dropdown>();
            var currentModal = mapMakerPanel.transform.Find("SetPanel").transform.Find("CurrentModal").GetComponent<Dropdown>();
            var modalList = new List<Dropdown.OptionData>();
            for (int i = 0; i < DataCenter.instance.modals.Count; ++i) {
                modalList.Add(new Dropdown.OptionData(DataCenter.instance.modals[i + 1].id + ". " + DataCenter.instance.modals[i + 1].name, Modal.GetResourceBaseSprite(DataCenter.instance.modals[i + 1].id)));
            }
            backModal.AddOptions(modalList);
            modalList.Insert(0, new Dropdown.OptionData("None", null));
            currentModal.AddOptions(modalList);

            var eventId = mapMakerPanel.transform.Find("SetPanel").transform.Find("EventId").GetComponent<Dropdown>();
            var eventIdList = new List<string> {
                (EventManager.EventName.None).ToString()
            };
            foreach (var k in EventManager.instance.eventList) {
                eventIdList.Add(k.Key.ToString());
            }
            eventId.AddOptions(eventIdList);

            mapId.value = 0;
            OnMapSelected();
        }

        // 载入Modal信息
        {
            // modId列表
            var modalId = modalMakerPanel.transform.Find("ModalId").GetComponent<Dropdown>();
            var modalIdList = new List<string>();
            for (int i = 0; i < DataCenter.instance.modals.Count; ++i) {
                modalIdList.Add(DataCenter.instance.modals[i + 1].id + ". " + DataCenter.instance.modals[i + 1].name);
            }
            modalId.AddOptions(modalIdList);
            // eventId列表
            var eventId = modalMakerPanel.transform.Find("EventId").GetComponent<Dropdown>();
            var eventIdList = new List<string> {
                (EventManager.EventName.None).ToString()
            };
            foreach (var k in EventManager.instance.eventList) {
                eventIdList.Add(k.Key.ToString());
            }
            eventId.AddOptions(eventIdList);
            // weaponId 列表
            var weaponIds = modalMakerPanel.transform.Find("WeaponId").GetComponent<Dropdown>();
            var modalWeaponIds = modalMakerPanel.transform.Find("ModalWeaponId").GetComponent<Dropdown>();
            var weaponIdList = new List<string>();
            for (int i = 0; i < DataCenter.instance.weapons.Count; ++i) {
                weaponIdList.Add(DataCenter.instance.weapons[i + 1].id + ". " + DataCenter.instance.weapons[i + 1].name);
            }
            weaponIds.AddOptions(weaponIdList);
            modalWeaponIds.AddOptions(weaponIdList);

            var weaponHit = modalMakerPanel.transform.Find("WeaponHit").GetComponent<Dropdown>();
            var weaponCrit = modalMakerPanel.transform.Find("WeaponCrit").GetComponent<Dropdown>();
            var audioList = new List<string>();
            for (int i = 0; i < DataCenter.instance.audios.Count; ++i) {
                audioList.Add(DataCenter.instance.audios[i + 1].id + ". " + DataCenter.instance.audios[i + 1].path);
            }
            weaponHit.AddOptions(audioList);
            weaponCrit.AddOptions(audioList);

            modalId.value = 0;
            weaponIds.value = 0;

            // 显示所有prefabs
            var currentPrefab = GameObject.Find("CurrentPrefab");
            prefabList.DefaultElement = currentPrefab.GetComponent<RectTransform>();
            foreach(var v in allPrefabs) {
                if(v.GetComponent<SpriteRenderer>() != null) {
                    var item = prefabList.PushbackDefaultItem();
                    item.GetComponent<Image>().sprite = v.GetComponent<SpriteRenderer>().sprite;
                    item.GetComponent<Button>().onClick.AddListener(() => { OnPrefabSelected(item); });
                    if (v.GetComponent<Modal>() != null) {
                        item.GetComponent<UserData>().SetStringData(v.name);
                    }else if(v.GetComponent<Player>() != null) {
                        item.GetComponent<UserData>().SetStringData(v.name);
                    }else if(v.GetComponent<Zzhit>() != null) {
                        item.GetComponent<UserData>().SetStringData(v.name);
                    }
                }
            }
            currentPrefab.GetComponent<Button>().enabled = false;
            GameObject.Find("btnSetModalPrefab").GetComponent<Button>().enabled = false;

            OnModalSelected(0);
            OnWeaponSelected(0);
        }
        // 载入String Chats 和 Choice 信息
        {
            // 数据部分
            var StringList = new List<string>(DataCenter.instance.strings.Count);
            foreach (var v in DataCenter.instance.strings) {
                StringList.Add(v.Key);
            }

            // String 编辑部分
            var stringMakerPanel = stringChatsAndChoicesPanel.transform.Find("StringMakerPanel");
            var stringIds = stringMakerPanel.Find("StringId").GetComponent<Dropdown>();
            stringIds.AddOptions(StringList);
            OnStringSelected(-1);

            // Chat 编辑部分
        }

        // 初始化
        DataCenter.instance.Status = Constant.EGameStatus.InEditor;
        OnChangeToMaps();
    }

    private void OnDestroy() {
        MapManager.instance.ClearMap();
        DataCenter.instance.Status = Constant.EGameStatus.Start;
        instance = null;
        ObjectPool.instance.ClearAll();
    }

    // Update is called once per frame
    void Update() {
        for (int i = 0; i < Input.touchCount; ++i) {
            var tc = Input.GetTouch(i);
            switch (tc.phase) {
                case TouchPhase.Began:
                    InputController.instance.OnTouchDown(tc.position);
                    break;
                case TouchPhase.Canceled:
                case TouchPhase.Ended:
                    InputController.instance.OnTouchUp(tc.position, new Vector2(tc.position.x - tc.deltaPosition.x, tc.position.y - tc.deltaPosition.y));
                    break;
            }
        }

        if (Input.touchCount <= 0) {
            if (Input.GetMouseButtonDown(0) && !InputController.instance.isMouseLeftDown)
                InputController.instance.OnTouchDown(new Vector2(Input.mousePosition.x, Input.mousePosition.y), true);
            if (Input.GetMouseButtonUp(0) && InputController.instance.isMouseLeftDown)
                InputController.instance.OnTouchUp(new Vector2(Input.mousePosition.x, Input.mousePosition.y), false);
        }
    }

    // "Map"按钮回调
    public void OnChangeToMaps() {
        mapMakerPanel.SetActive(true);
        modalMakerPanel.SetActive(false);
        stringChatsAndChoicesPanel.SetActive(false);
        var panel = mapMakerPanel.transform.Find("SetPanel");
        var mapId = panel.transform.Find("MapId").GetComponent<Dropdown>().value + 1;
        MapManager.instance.ShowMap(mapId);
    }

    // Modal&Audio按钮回调
    public void OnChangeToModals() {
        mapMakerPanel.SetActive(false);
        modalMakerPanel.SetActive(true);
        stringChatsAndChoicesPanel.SetActive(false);
        BackgroundImage = "BrownWall";
    }

    // Event 按钮回调
    public void OnChangeToAudioAndEvents() {
        mapMakerPanel.SetActive(false);
        modalMakerPanel.SetActive(false);
        stringChatsAndChoicesPanel.SetActive(true);
        BackgroundImage = "BrownWall";
    }

    // GetDataJson按钮回调
    public void OnSave() {
        if (saveResult.activeSelf == false) {
            string result = DataCenter.instance.SaveData();
            saveResult.SetActive(true);
            saveResult.GetComponent<InputField>().text = result;
            mapMakerPanel.SetActive(false);
            modalMakerPanel.SetActive(false);
            stringChatsAndChoicesPanel.SetActive(false);
        } else {
            saveResult.SetActive(false);
        }
    }

    // GetMapJson 按钮回调
    public void OnMapSave() {
        if (saveResult.activeSelf == false) {
            string result = DataCenter.instance.GetGameMap(mapMakerPanel.transform.Find("SetPanel").transform.Find("MapId").GetComponent<Dropdown>().value).Json.String;
            saveResult.SetActive(true);
            saveResult.GetComponent<InputField>().text = result;
            mapMakerPanel.SetActive(false);
            modalMakerPanel.SetActive(false);
            stringChatsAndChoicesPanel.SetActive(false);
        } else {
            saveResult.SetActive(false);
        }
    }

    // Apply Map 回调
    public void OnMapApply() {
        var panel = mapMakerPanel.transform.Find("SetPanel");
        var mapId = panel.transform.Find("MapId").GetComponent<Dropdown>().value;
        var mapName = panel.transform.Find("MapName").GetComponent<InputField>().text;
        var bgMusic = panel.transform.Find("Music").GetComponent<Dropdown>().value + 1;
        var bgModal = panel.transform.Find("BackModal").GetComponent<Dropdown>().value + 1;
        var currModal = panel.transform.Find("CurrentModal").GetComponent<Dropdown>().value;
        var eventId = panel.transform.Find("EventId").GetComponent<Dropdown>().value;
        long eventData = 0;
        try {
            eventData = System.Convert.ToInt64(panel.transform.Find("EventData").GetComponent<InputField>().text);
        } catch (System.FormatException) {
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
    void OnMapSelected() {
        var panel = mapMakerPanel.transform.Find("SetPanel");
        var mapId = panel.transform.Find("MapId").GetComponent<Dropdown>().value + 1;
        panel.transform.Find("MapName").GetComponent<InputField>().text = DataCenter.instance.GetGameMap(mapId - 1).mapName;
        panel.transform.Find("Music").GetComponent<Dropdown>().value = DataCenter.instance.GetGameMap(mapId - 1).music - 1;
        panel.transform.Find("BackModal").GetComponent<Dropdown>().value = DataCenter.instance.GetGameMap(mapId - 1).backThing - 1;
        MapManager.instance.ShowMap(mapId);
        OnMapClicked(new Vector2(0, 0));
        AudioController.instance.StopMusic();
    }

    // Map地图块物品选择框回调
    public void OnMapModalSelected(bool isBack) {
        if (isBack) {
            var index = mapMakerPanel.transform.Find("SetPanel").transform.Find("BackModal").GetComponent<Dropdown>().value;
            MapManager.instance.ChangeBack(DataCenter.instance.modals[index + 1].prefabPath);
        } else {
            var index = mapMakerPanel.transform.Find("SetPanel").transform.Find("CurrentModal").GetComponent<Dropdown>().value;
            MapManager.instance.ChangeThingOnMap(index, posx, posy);
        }
    }

    // Map选定地图块位置的回调
    public void OnMapClicked(Vector2 pos) {
        if (!mapMakerPanel.activeSelf)
            return;
        var mapPanel = mapMakerPanel.transform.Find("MapPanel").GetComponent<RectTransform>();
        var panelPos = mapMakerPanel.transform.InverseTransformPoint(mapPanel.position);
        pos.x -= panelPos.x + ScreenAdaptator.instance.MapPartRect.x + mapMakerPanel.GetComponent<RectTransform>().rect.width / 2;
        pos.y -= panelPos.y + ScreenAdaptator.instance.MapPartRect.y + mapMakerPanel.GetComponent<RectTransform>().rect.height / 2;
        if (pos.x >= 0 && pos.y >= 0) {
            var _posx = (int)(pos.x * Constant.MAP_BLOCK_LENGTH / ScreenAdaptator.instance.MapPartRect.width);
            var _posy = (int)(pos.y * Constant.MAP_BLOCK_LENGTH / ScreenAdaptator.instance.MapPartRect.height);
            if (_posx >= Constant.MAP_BLOCK_LENGTH || _posy >= Constant.MAP_BLOCK_LENGTH)
                return;
            posx = _posx;
            posy = _posy;
            var panel = mapMakerPanel.transform.Find("SetPanel");
            var mapId = panel.transform.Find("MapId").GetComponent<Dropdown>().value + 1;
            panel.transform.Find("CurrentPosition").GetComponent<Text>().text = "(" + posx + ", " + posy + ")";
            panel.transform.Find("CurrentModal").GetComponent<Dropdown>().value = DataCenter.instance.GetGameMap(mapId - 1).mapBlocks[posx][posy].thing;
            panel.transform.Find("EventId").GetComponent<Dropdown>().value = DataCenter.instance.GetGameMap(mapId - 1).mapBlocks[posx][posy].eventId;
            panel.transform.Find("EventData").GetComponent<InputField>().text = DataCenter.instance.GetGameMap(mapId - 1).mapBlocks[posx][posy].eventData.ToString();
        }
    }

    // 音乐播放键回调
    public void OnPlay(int index) {
        switch (index) {
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

    // Refresh audios 按钮回调
    public void OnRefreshAudio() {
        var allAudios = Resources.LoadAll<AudioClip>(Constant.AUDIO_DIR);
        Constant.Audio[] audioData = new Constant.Audio[allAudios.Length];
        DataCenter.instance.audios.Clear();
        for (var i = 0; i < audioData.Length; ++i) {
            DataCenter.instance.audios.Add(audioData[i].id, new Constant.Audio() {
                id = i + 1,
                path = allAudios[i].name
            });
        }
        //PlatformUIManager.ShowMessageBox("音效表刷新成功，请立即保存配置然后重新启动游戏！");
    }

    // Auto collect prefabs 按钮回调
    public void OnRefreshPrefab() {
        Constant.ModalData[] prefabData = new Constant.ModalData[allPrefabs.Length];
        DataCenter.instance.modals.Clear();
        for (var i = 0; i < prefabData.Length; ++i) {
            DataCenter.instance.modals.Add(prefabData[i + 1].id, new Constant.ModalData() {
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

    // 关闭(X)按钮回调
    public void OnExitEditor() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("StartScene");
    }

    // modal部分modal选择框
    public void OnModalSelected(int index) {
        if (index == 0)
            index = modalMakerPanel.transform.Find("ModalId").GetComponent<Dropdown>().value;
        ++index;
        modalMakerPanel.transform.Find("ModalName").GetComponent<InputField>().text = DataCenter.instance.modals[index].name;
        modalMakerPanel.transform.Find("ModalType").GetComponent<Dropdown>().value = DataCenter.instance.modals[index].typeId - 1;
        modalMakerPanel.transform.Find("EventId").GetComponent<Dropdown>().value = DataCenter.instance.modals[index].eventId;
        modalMakerPanel.transform.Find("EventData").GetComponent<InputField>().text = DataCenter.instance.modals[index].eventData.ToString();
        modalMakerPanel.transform.Find("ModalPrefabText").GetComponent<InputField>().text = DataCenter.instance.modals[index].prefabPath;
        modalMakerPanel.transform.Find("ModalPrefabImage").GetComponent<Image>().sprite = Resources.Load<GameObject>(Modal.GetResourcePath(index)).GetComponent<SpriteRenderer>().sprite;
        if (DataCenter.instance.modals[index].typeId == (int)Modal.ModalType.Player) {
            var player = DataCenter.instance.players[DataCenter.instance.modals[index].id];
            modalMakerPanel.transform.Find("Level").GetComponent<InputField>().text = player.level.ToString();
            modalMakerPanel.transform.Find("Exp").GetComponent<InputField>().text = player.exp.ToString();
            modalMakerPanel.transform.Find("Life").GetComponent<InputField>().text = player.life.ToString();
            modalMakerPanel.transform.Find("Attack").GetComponent<InputField>().text = player.attack.ToString();
            modalMakerPanel.transform.Find("Defense").GetComponent<InputField>().text = player.defense.ToString();
            modalMakerPanel.transform.Find("Speed").GetComponent<InputField>().text = player.speed.ToString();
            modalMakerPanel.transform.Find("Critical").GetComponent<InputField>().text = player.critical.ToString();
            modalMakerPanel.transform.Find("Gold").GetComponent<InputField>().text = player.gold.ToString();
            modalMakerPanel.transform.Find("ModalWeaponId").GetComponent<Dropdown>().value = player.weaponId - 1;
        } else if (DataCenter.instance.modals[index].typeId == (int)Modal.ModalType.Monster) {
            var monster = DataCenter.instance.monsters[DataCenter.instance.modals[index].id];
            modalMakerPanel.transform.Find("Level").GetComponent<InputField>().text = monster.level.ToString();
            modalMakerPanel.transform.Find("Exp").GetComponent<InputField>().text = monster.exp.ToString();
            modalMakerPanel.transform.Find("Life").GetComponent<InputField>().text = monster.life.ToString();
            modalMakerPanel.transform.Find("Attack").GetComponent<InputField>().text = monster.attack.ToString();
            modalMakerPanel.transform.Find("Defense").GetComponent<InputField>().text = monster.defense.ToString();
            modalMakerPanel.transform.Find("Speed").GetComponent<InputField>().text = monster.speed.ToString();
            modalMakerPanel.transform.Find("Critical").GetComponent<InputField>().text = monster.critical.ToString();
            modalMakerPanel.transform.Find("Gold").GetComponent<InputField>().text = monster.gold.ToString();
            modalMakerPanel.transform.Find("ModalWeaponId").GetComponent<Dropdown>().value = monster.weaponId - 1;
        } else {
            modalMakerPanel.transform.Find("Level").GetComponent<InputField>().text = "";
            modalMakerPanel.transform.Find("Exp").GetComponent<InputField>().text = "";
            modalMakerPanel.transform.Find("Life").GetComponent<InputField>().text = "";
            modalMakerPanel.transform.Find("Attack").GetComponent<InputField>().text = "";
            modalMakerPanel.transform.Find("Defense").GetComponent<InputField>().text = "";
            modalMakerPanel.transform.Find("Speed").GetComponent<InputField>().text = "";
            modalMakerPanel.transform.Find("Critical").GetComponent<InputField>().text = "";
            modalMakerPanel.transform.Find("Gold").GetComponent<InputField>().text = "";
            modalMakerPanel.transform.Find("ModalWeaponId").GetComponent<Dropdown>().value = 0;
        }
    }

    // modal 部分 prefab 列表选项框的 element 点击回调
    public void OnPrefabSelected(RectTransform sender) {
        var currentPrefab = GameObject.Find("CurrentPrefab");
        currentPrefab.GetComponent<Image>().sprite = sender.GetComponent<Image>().sprite;
        currentPrefab.GetComponent<UserData>().SetStringData(sender.GetComponent<UserData>().GetStringData());
        var prefabPath = currentPrefab.GetComponent<UserData>().GetStringData();
        modalMakerPanel.transform.Find("SelectedModalPrefab").GetComponent<InputField>().text = prefabPath;
        modalMakerPanel.transform.Find("CurrentPrefab").GetComponent<Image>().sprite = Resources.Load<GameObject>(Modal.GetResourcePath(prefabPath)).GetComponent<SpriteRenderer>().sprite;

        GameObject.Find("btnSetModalPrefab").GetComponent<Button>().enabled = true;
    }

    // btnSetModalPrefab 按钮回调
    public void OnSetModalPrefab() {
        modalMakerPanel.transform.Find("ModalPrefabText").GetComponent<InputField>().text = modalMakerPanel.transform.Find("SelectedModalPrefab").GetComponent<InputField>().text;
        modalMakerPanel.transform.Find("ModalPrefabImage").GetComponent<Image>().sprite = modalMakerPanel.transform.Find("CurrentPrefab").GetComponent<Image>().sprite;
    }

    // Reset 按钮回调
    public void OnReset(int part) {
        switch (part) {
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
    public void OnModalApply() {
        var index = modalMakerPanel.transform.Find("ModalId").GetComponent<Dropdown>().value + 1;
        DataCenter.instance.modals[index].name = modalMakerPanel.transform.Find("ModalName").GetComponent<InputField>().text;
        DataCenter.instance.modals[index].typeId = modalMakerPanel.transform.Find("ModalType").GetComponent<Dropdown>().value + 1;
        DataCenter.instance.modals[index].eventId = modalMakerPanel.transform.Find("EventId").GetComponent<Dropdown>().value;
        DataCenter.instance.modals[index].eventData = System.Convert.ToInt64(modalMakerPanel.transform.Find("EventData").GetComponent<InputField>().text);
        DataCenter.instance.modals[index].prefabPath = modalMakerPanel.transform.Find("ModalPrefabText").GetComponent<InputField>().text;
        if (DataCenter.instance.modals[index].typeId == (int)Modal.ModalType.Player) {
            var playerId = DataCenter.instance.modals[index].id;
            if (!DataCenter.instance.players.ContainsKey(playerId)) {
                DataCenter.instance.players.Add(playerId, new Constant.PlayerData() {
                    level = System.Convert.ToInt32(modalMakerPanel.transform.Find("Level").GetComponent<InputField>().text),
                    exp = System.Convert.ToInt32(modalMakerPanel.transform.Find("Exp").GetComponent<InputField>().text),
                    life = System.Convert.ToInt32(modalMakerPanel.transform.Find("Life").GetComponent<InputField>().text),
                    attack = System.Convert.ToInt32(modalMakerPanel.transform.Find("Attack").GetComponent<InputField>().text),
                    defense = System.Convert.ToInt32(modalMakerPanel.transform.Find("Defense").GetComponent<InputField>().text),
                    speed = System.Convert.ToInt32(modalMakerPanel.transform.Find("Speed").GetComponent<InputField>().text),
                    critical = System.Convert.ToDouble(modalMakerPanel.transform.Find("Critical").GetComponent<InputField>().text),
                    gold = System.Convert.ToInt32(modalMakerPanel.transform.Find("Gold").GetComponent<InputField>().text),
                    weaponId = modalMakerPanel.transform.Find("ModalWeaponId").GetComponent<Dropdown>().value + 1
                });
            }
        } else if (DataCenter.instance.modals[index].typeId == (int)Modal.ModalType.Monster) {
            var monsterId = DataCenter.instance.modals[index].id;
            if (!DataCenter.instance.monsters.ContainsKey(monsterId)) {
                DataCenter.instance.monsters.Add(monsterId, new Constant.MonsterData() {
                    level = System.Convert.ToInt32(modalMakerPanel.transform.Find("Level").GetComponent<InputField>().text),
                    exp = System.Convert.ToInt32(modalMakerPanel.transform.Find("Exp").GetComponent<InputField>().text),
                    life = System.Convert.ToInt32(modalMakerPanel.transform.Find("Life").GetComponent<InputField>().text),
                    attack = System.Convert.ToInt32(modalMakerPanel.transform.Find("Attack").GetComponent<InputField>().text),
                    defense = System.Convert.ToInt32(modalMakerPanel.transform.Find("Defense").GetComponent<InputField>().text),
                    speed = System.Convert.ToInt32(modalMakerPanel.transform.Find("Speed").GetComponent<InputField>().text),
                    critical = System.Convert.ToDouble(modalMakerPanel.transform.Find("Critical").GetComponent<InputField>().text),
                    gold = System.Convert.ToInt32(modalMakerPanel.transform.Find("Gold").GetComponent<InputField>().text),
                    weaponId = modalMakerPanel.transform.Find("ModalWeaponId").GetComponent<Dropdown>().value + 1
                });
            }
        }
    }

    // Weapon选择框回调
    public void OnWeaponSelected(int index) {
        if (index == 0)
            index = modalMakerPanel.transform.Find("WeaponId").GetComponent<Dropdown>().value;
        ++index;
        modalMakerPanel.transform.Find("WeaponName").GetComponent<InputField>().text = DataCenter.instance.weapons[index].name;
        modalMakerPanel.transform.Find("WeaponHit").GetComponent<Dropdown>().value = DataCenter.instance.weapons[index].audioId - 1;
        modalMakerPanel.transform.Find("WeaponCrit").GetComponent<Dropdown>().value = DataCenter.instance.weapons[index].critAudioId - 1;
        modalMakerPanel.transform.Find("WeaponPrefab").GetComponent<InputField>().text = DataCenter.instance.weapons[index].prefabPath;
    }

    // Apply Weapon 按钮回调
    public void OnWeaponApply() {
        var index = modalMakerPanel.transform.Find("WeaponId").GetComponent<Dropdown>().value;
        DataCenter.instance.weapons[index].name = modalMakerPanel.transform.Find("WeaponName").GetComponent<InputField>().text;
        DataCenter.instance.weapons[index].audioId = modalMakerPanel.transform.Find("WeaponHit").GetComponent<Dropdown>().value + 1;
        DataCenter.instance.weapons[index].critAudioId = modalMakerPanel.transform.Find("WeaponCrit").GetComponent<Dropdown>().value + 1;
        DataCenter.instance.weapons[index].prefabPath = modalMakerPanel.transform.Find("WeaponPrefab").GetComponent<InputField>().text;
    }

    // Add map, Add modal, Add weapon 按钮的回调
    public void OnAdd(int index) {
        switch (index) {
            case 0: {
                    var drop = mapMakerPanel.transform.Find("MapPanel").transform.Find("MapId").GetComponent<Dropdown>();
                    //var ind = DataCenter.instance.AddMap();
                    var lst = new List<string>();
                    //lst.Add(ind + 1 + ". (new)");
                    drop.AddOptions(lst);
                    //drop.value = ind;
                    OnMapSelected();
                }
                break;
            case 1: {
                    var drop = modalMakerPanel.transform.Find("ModalId").GetComponent<Dropdown>();
                    var ind = DataCenter.instance.modals.Count;
                    DataCenter.instance.modals.Add(ind, new Constant.ModalData() { id = ind });
                    var lst = new List<string> {
                        ind + 1 + ". (new)"
                    };
                    drop.AddOptions(lst);
                    drop.value = ind;
                    OnModalSelected(ind);
                }
                break;
            case 2: {
                    var drop = modalMakerPanel.transform.Find("WeaponId").GetComponent<Dropdown>();
                    var ind = DataCenter.instance.weapons.Count;
                    DataCenter.instance.weapons.Add(ind, new Constant.WeaponData() { id = ind });
                    var lst = new List<string> {
                        ind + 1 + ". (new)"
                    };
                    drop.AddOptions(lst);
                    drop.value = ind;
                    OnWeaponSelected(ind);
                }
                break;
        }
    }

    // StringId 选择框回调
    public void OnStringSelected(int index) {
        var stringMakerPanel = stringChatsAndChoicesPanel.transform.Find("StringMakerPanel");
        var dropdown = stringMakerPanel.transform.Find("StringId").GetComponent<Dropdown>();
        if (index == -1) {
            index = dropdown.value;
        } else {
            dropdown.value = index;
        }
        var str = dropdown.options[index].text;
        var data = DataCenter.instance.strings[str];
        //显示数据
        stringMakerPanel.Find("StringKey").GetComponent<InputField>().text = str;
        foreach (var v in UI_LANG_KEY) {
            stringMakerPanel.Find(v.Value).GetComponent<InputField>().text = "";
        }
        foreach (var v in data.strings) {
            stringMakerPanel.Find(UI_LANG_KEY[v.langKey]).GetComponent<InputField>().text = v.content;
        }
    }

    // Add string 按钮回调
    public void OnAddString() {
        if (DataCenter.instance.strings.ContainsKey("#####")) {
            ShowTips("请先将之前添加的字符串编辑完成并保存!");
            return;
        }
        DataCenter.instance.strings.Add("#####", new Constant.InternationalString { id = DataCenter.instance.strings.Count, key = "#####", strings = new Constant.StringInOneLanguage[] { new Constant.StringInOneLanguage { langKey = "en-us", content = "" } } });

        var stringMakerPanel = stringChatsAndChoicesPanel.transform.Find("StringMakerPanel");
        var dropdown = stringMakerPanel.transform.Find("StringId").GetComponent<Dropdown>();
        dropdown.AddOptions(new List<string> { "#####" });
        OnStringSelected(dropdown.options.Count - 1);
    }

    // Save String 按钮回调
    public void OnSaveString() {
        var stringMakerPanel = stringChatsAndChoicesPanel.transform.Find("StringMakerPanel");
        var dropdown = stringMakerPanel.transform.Find("StringId").GetComponent<Dropdown>();
        var key = dropdown.options[dropdown.value].text;
        var oldValue = DataCenter.instance.strings[key];
        var newKey = stringMakerPanel.transform.Find("StringKey").GetComponent<InputField>().text;
        if (key != newKey && DataCenter.instance.strings.ContainsKey(newKey)) {
            ShowTips("保存失败, key值重复!");
            return;
        }
        if (newKey == "") {
            ShowTips("保存失败, key不能为空!");
            return;
        }
        var stringList = new List<Constant.StringInOneLanguage>();
        foreach (var v in UI_LANG_KEY) {
            if (stringMakerPanel.Find(v.Value).GetComponent<InputField>().text != "") {
                stringList.Add(new Constant.StringInOneLanguage { langKey = v.Key, content = stringMakerPanel.Find(v.Value).GetComponent<InputField>().text });
            }
        }
        if (stringList.Count < 1) {
            ShowTips("保存失败, 至少需要有一种语言拥有字符串值");
            return;
        }
        if (key == newKey) {
            DataCenter.instance.strings[key].strings = stringList.ToArray();
        } else {
            var newValue = new Constant.InternationalString { id = oldValue.id, key = newKey, strings = stringList.ToArray() };
            DataCenter.instance.strings.Remove(key);
            DataCenter.instance.strings.Add(newKey, newValue);
            var dropdownValue = dropdown.value;
            dropdown.options[dropdownValue].text = newKey;
            dropdown.captionText.text = newKey;
        }
    }

    // 弹出Tips提示, 并在一定时间后消失
    public void ShowTips(string text) {
        var tipbar = TipBar.ShowTip();
        tipbar.transform.SetParent(transform, false);
        tipbar.SetTipText(text);
        tipbar.StartAutoRemove(200);
    }

    // 添加物体到 map panel
    public void AddObjectToMap(GameObject obj, int posx, int posy, int posz = -2) {
        obj.transform.SetParent(mapMakerPanel.transform.Find("MapPanel"), false);
        obj.transform.position = mapMakerPanel.transform.Find("MapPanel").transform.
            TransformPoint(new Vector3((posx + (float)0.5) * Constant.MAP_BLOCK_BASE_SIZE * ScreenAdaptator.instance.BlockSize.x / 100 + ScreenAdaptator.instance.MapPartRect.x,
                                       (posy + (float)0.5) * Constant.MAP_BLOCK_BASE_SIZE * ScreenAdaptator.instance.BlockSize.y / 100 + ScreenAdaptator.instance.MapPartRect.y,
                                       posz));
        obj.transform.localScale = ScreenAdaptator.instance.BlockSize;
    }


    public string BackgroundImage {
        get { return backgroundImg.sprite.name; }
        set { backgroundImg.sprite = Resources.Load<GameObject>(Constant.PREFAB_DIR + value).GetComponent<SpriteRenderer>().sprite; }
    }
    public Curtain Curtain { get { return curtain; } }


    private Image backgroundImg;
    private Curtain curtain;
    private GameObject[] allPrefabs;
    private GameObject mapMakerPanel;
    private GameObject modalMakerPanel;
    private GameObject stringChatsAndChoicesPanel;
    private GameObject saveResult;
    private int posx = 0;
    private int posy = 0;
    private ListView prefabList;
}
