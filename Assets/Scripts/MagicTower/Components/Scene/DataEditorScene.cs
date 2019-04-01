using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;
using ArmyAnt.ViewUtil.Components;
using MagicTower.Components.Unit;
using MagicTower.Present.Manager;

namespace MagicTower.Components.Scene
{
    public class DataEditorScene : AScene
    {
        public static readonly ReadOnlyDictionary<string, string> UI_LANG_KEY = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
        {
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

        override public SceneType Type { get { return SceneType.DataEditorScene; } }

        // Use this for initialization
        override protected System.Threading.Tasks.Task Start()
        {
            var ret = base.Start();
            // 初始化游戏信息
            Game.Player = null;
            AudioManager.MusicSource = GetComponent<AudioSource>();
            AudioManager.ClearSoundSource();
            AudioManager.AddSoundSource(GameObject.Find("Main Camera").GetComponent<AudioSource>());

            // 设定canvas
            mapMakerPanel = GameObject.Find("MapMakerPanel");
            modalMakerPanel = GameObject.Find("ModalMakerPanel");
            stringChatsAndChoicesPanel = GameObject.Find("StringChatsAndChoicesPanel");
            prefabList = modalMakerPanel.transform.Find("prefabs").GetComponent<ListView>();
            dataList = stringChatsAndChoicesPanel.transform.Find("DataList").GetComponent<ListView>();
            // 初始化地图并设定背景
            Game.Map = new Present.Map.Controller(GameObject.Find("MapMakerPanel").transform.Find("MapPanel").GetComponent<Present.Map.View>());

            // 载入所有资源
            allPrefabs = Resources.LoadAll<GameObject>(Model.Dirs.PREFAB_DIR);

            // dropdown 公共数据列表
            var eventIdList = new List<string> {
                (EventManager.EventName.None).ToString()
            };
            foreach (var k in EventManager.EventKeyStrings)
            {
                eventIdList.Add(k.ToString());
            }
            var modalIdList = new List<string>();
            for (int i = 0; i < Game.Config.modals.Count; ++i)
            {
                modalIdList.Add(Game.Config.modals[i + 1].id + ". " + Game.Config.modals[i + 1].name);
            }
            var modalList = new List<Dropdown.OptionData>();
            for (int i = 0; i < Game.Config.modals.Count; ++i)
            {
                modalList.Add(new Dropdown.OptionData(Game.Config.modals[i + 1].id + ". " + Game.Config.modals[i + 1].name, Modal.GetResourceBaseSprite(Game.Config.modals[i + 1].id)));
            }

            // 载入Map信息
            {
                var mapId = mapMakerPanel.transform.Find("SetPanel").transform.Find("MapId").GetComponent<Dropdown>();
                var mapList = new List<string>();
                for (int i = 0; i < Game.Config.mapLength; ++i)
                {
                    mapList.Add(i.ToString());
                }
                mapId.AddOptions(mapList);

                var musicId = mapMakerPanel.transform.Find("SetPanel").transform.Find("Music").GetComponent<Dropdown>();
                var audioList = new List<string>();
                for (int i = 0; i < Game.Config.audios.Count; ++i)
                {
                    audioList.Add(Game.Config.audios[i + 1].id + ". " + Game.Config.audios[i + 1].path);
                }
                musicId.AddOptions(audioList);

                var eventId = mapMakerPanel.transform.Find("SetPanel").transform.Find("EventId").GetComponent<Dropdown>();
                eventId.AddOptions(eventIdList);

                mapId.value = 0;
                OnMapSelected();
            }

            // 载入Modal信息
            {
                // modId列表
                var modalId = modalMakerPanel.transform.Find("ModalId").GetComponent<Dropdown>();
                modalId.AddOptions(modalIdList);
                // eventId列表
                var eventId = modalMakerPanel.transform.Find("EventId").GetComponent<Dropdown>();
                eventId.AddOptions(eventIdList);
                // weaponId 列表
                var weaponIds = modalMakerPanel.transform.Find("WeaponId").GetComponent<Dropdown>();
                var modalWeaponIds = modalMakerPanel.transform.Find("ModalWeaponId").GetComponent<Dropdown>();
                var weaponIdList = new List<string>();
                for (int i = 0; i < Game.Config.weapons.Count; ++i)
                {
                    weaponIdList.Add(Game.Config.weapons[i + 1].id + ". " + Game.Config.weapons[i + 1].name);
                }
                weaponIds.AddOptions(weaponIdList);
                modalWeaponIds.AddOptions(weaponIdList);

                var weaponHit = modalMakerPanel.transform.Find("WeaponHit").GetComponent<Dropdown>();
                var weaponCrit = modalMakerPanel.transform.Find("WeaponCrit").GetComponent<Dropdown>();
                var audioList = new List<string>();
                for (int i = 0; i < Game.Config.audios.Count; ++i)
                {
                    audioList.Add(Game.Config.audios[i + 1].id + ". " + Game.Config.audios[i + 1].path);
                }
                weaponHit.AddOptions(audioList);
                weaponCrit.AddOptions(audioList);

                modalId.value = 0;
                weaponIds.value = 0;

                // 显示所有prefabs
                var currentPrefab = GameObject.Find("CurrentPrefab");
                foreach (var v in allPrefabs)
                {
                    if (v.GetComponent<SpriteRenderer>() != null)
                    {
                        var item = prefabList.PushbackDefaultItem();
                        item.GetComponent<Image>().sprite = v.GetComponent<SpriteRenderer>().sprite;
                        item.GetComponent<Button>().onClick.AddListener(() => { OnPrefabSelected(item); });
                        if (v.GetComponent<Modal>() != null)
                        {
                            item.GetComponent<UserData>().SetStringData(v.name);
                        }
                        else if (v.GetComponent<Player>() != null)
                        {
                            item.GetComponent<UserData>().SetStringData(v.name);
                        }
                        else if (v.GetComponent<Zzhit>() != null)
                        {
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
                var stringList = new List<string>(Game.Config.strings.Count);
                foreach (var v in Game.Config.strings)
                {
                    stringList.Add(v.Key);
                }
                var chatList = new List<string>(Game.Config.chats.Count);
                foreach (var v in Game.Config.chats)
                {
                    chatList.Add(v.Value.id.ToString());
                }
                var choiceList = new List<string>(Game.Config.choices.Count);
                foreach (var v in Game.Config.choices)
                {
                    choiceList.Add(v.Value.id.ToString());
                }

                // String 编辑部分
                var stringMakerPanel = stringChatsAndChoicesPanel.transform.Find("StringMakerPanel");
                var stringIds = stringMakerPanel.Find("StringId").GetComponent<Dropdown>();
                stringIds.AddOptions(stringList);
                OnStringSelected(-1);

                // Chat 编辑部分
                var chatDataPanel = stringChatsAndChoicesPanel.transform.Find("ChatDataPanel");
                var chatIds = chatDataPanel.Find("ChatId").GetComponent<Dropdown>();
                chatIds.AddOptions(chatList);
                var chatlastEventIds = chatDataPanel.Find("LastEventId").GetComponent<Dropdown>();
                chatlastEventIds.AddOptions(eventIdList);
                OnChatSelected(-1);

                // Choice 编辑部分
                var choiceDataPanel = stringChatsAndChoicesPanel.transform.Find("ChoiceDataPanel");
                var choiceIds = choiceDataPanel.Find("ChoiceId").GetComponent<Dropdown>();
                choiceIds.AddOptions(choiceList);
                var choiceSpeakers = choiceDataPanel.Find("Speaker").GetComponent<Dropdown>();
                choiceSpeakers.AddOptions(new List<string>() { "-200 None", "-1 ChatTarget", "0 SelfPlayer" });
                choiceSpeakers.AddOptions(modalList);
                var choiceTitleStrings = choiceDataPanel.Find("TitleString").GetComponent<Dropdown>();
                var choiceTailStrings = choiceDataPanel.Find("TailString").GetComponent<Dropdown>();
                choiceTitleStrings.AddOptions(new List<string>() { "0 (None)" });
                choiceTitleStrings.AddOptions(stringList);
                choiceTailStrings.AddOptions(new List<string>() { "0 (None)" });
                choiceTailStrings.AddOptions(stringList);
                OnChoiceSelected(-1);

                // DataList 部分
                var chatSpeaker = chatDataPanel.Find("DataSpeaker").GetComponent<Dropdown>();
                chatSpeaker.AddOptions(new List<string>() { "-200 None", "-1 ChatTarget", "0 SelfPlayer" });
                chatSpeaker.AddOptions(modalList);
                var chatContent = chatDataPanel.Find("DataContent").GetComponent<Dropdown>();
                chatContent.AddOptions(stringList);
                var choiceEventId = choiceDataPanel.Find("DataEventId").GetComponent<Dropdown>();
                choiceEventId.AddOptions(eventIdList);
                var choiceContent = choiceDataPanel.Find("DataContent").GetComponent<Dropdown>();
                choiceContent.AddOptions(stringList);
            }

            // 初始化
            Game.Status = Model.EGameStatus.InEditor;
            OnPanelSelected(0);
            return ret;
        }

        // Update is called once per frame
        async System.Threading.Tasks.Task Update() {
            Game.SceneUpdate();

            while (ioTaskLine.Count > 0)
            {
                var ret = ioTaskLine.Dequeue();
                ret.Value(await ret.Key);
            }
        }

        public void OnPanelSelected(int value = -1)
        {
            if(value < 0)
            {
                value = panelSelect.value;
            }
            else
            {
                panelSelect.value = value;
            }
            switch (value)
            {
                case 0:
                    mapMakerPanel.SetActive(true);
                    modalMakerPanel.SetActive(false);
                    stringChatsAndChoicesPanel.SetActive(false);
                    var panel = mapMakerPanel.transform.Find("SetPanel");
                    var mapId = panel.transform.Find("MapId").GetComponent<Dropdown>().value + 1;
                    Game.Map.ShowMap(mapId);
                    break;
                case 1:
                    mapMakerPanel.SetActive(false);
                    modalMakerPanel.SetActive(true);
                    stringChatsAndChoicesPanel.SetActive(false);
                    Game.Map.ChangeBack("BrownWall");
                    break;
                case 2:
                    mapMakerPanel.SetActive(false);
                    modalMakerPanel.SetActive(false);
                    stringChatsAndChoicesPanel.SetActive(true);
                    Game.Map.ChangeBack("BrownWall");
                    break;
            }
        }

        // GetDataJson按钮回调
        public void OnSave()
        {
            string result = Game.Config.SaveData();
            ioTaskLine.Enqueue(new KeyValuePair<System.Threading.Tasks.Task<string>, IOTaskResult>(ArmyAnt.Manager.IOManager.SaveToFile(System.Text.Encoding.UTF8.GetBytes(result), "GameData.json"), (filepath) => ShowTips("已保存成功！路径：" + filepath)));
        }

        // GetMapJson 按钮回调
        public void OnMapSave()
        {
            var panel = mapMakerPanel.transform.Find("SetPanel");
            var mapId = panel.transform.Find("MapId").GetComponent<Dropdown>().value + 1;
            Game.Config.SaveMapTo(mapId - 1);

            string result = JsonUtility.ToJson(Game.Config.GetGameMap(mapId - 1), false);

            string number = "";
            if (mapId < 10)
            {
                number = "00" + mapId.ToString();
            }
            else if (mapId < 100)
            {
                number = "0" + mapId.ToString();
            }
            else
            {
                number = mapId.ToString();
            }
            ArmyAnt.Manager.IOManager.MkdirIfNotExist("MapData");
            ioTaskLine.Enqueue(new KeyValuePair<System.Threading.Tasks.Task<string>, IOTaskResult>(ArmyAnt.Manager.IOManager.SaveToFile(System.Text.Encoding.UTF8.GetBytes(result), "MapData", number + ".json"), (filepath) => ShowTips("已保存成功！路径：" + filepath)));
        }

        public void OnMapNameChanged()
        {
            var panel = mapMakerPanel.transform.Find("SetPanel");
            var mapId = panel.transform.Find("MapId").GetComponent<Dropdown>().value;
            var mapName = panel.transform.Find("MapName").GetComponent<InputField>().text;
            Game.Config.GetGameMap(mapId).mapName = mapName;
        }

        public void OnMapMusicChanged()
        {
            var panel = mapMakerPanel.transform.Find("SetPanel");
            var mapId = panel.transform.Find("MapId").GetComponent<Dropdown>().value;
            var bgMusic = panel.transform.Find("Music").GetComponent<Dropdown>().value + 1;
            Game.Config.GetGameMap(mapId).music = bgMusic;
        }

        // Map地图块物品选择框回调
        public void OnMapBackModalClick()
        {
            var panel = mapMakerPanel.transform.Find("SetPanel");
            var mapId = panel.transform.Find("MapId").GetComponent<Dropdown>().value;
            Control.ModalSelectorDlg.ShowDialog(dialogCanvas.transform, Game.Config.GetGameMap(mapId).backThing, (int id) => {
                Game.Map.ChangeBack(Game.Config.modals[id].prefabPath);
                Game.Config.GetGameMap(mapId).backThing = id;
                panel.transform.Find("btnBackModal").Find("Image").GetComponent<Image>().sprite = Modal.GetResourceBaseSprite(id);
                panel.transform.Find("btnBackModal").Find("Text").GetComponent<Text>().text = Game.Config.StringInternational.GetValue(Game.Config.modals[id].name);
            });
        }

        public void OnMapCurrentModalClick()
        {
            var panel = mapMakerPanel.transform.Find("SetPanel");
            var mapId = panel.transform.Find("MapId").GetComponent<Dropdown>().value;
            Control.ModalSelectorDlg.ShowDialog(dialogCanvas.transform, Game.Config.GetGameMap(mapId).blocks[posx][posy].thing, (int id) => {
                Game.Map.ChangeThingOnMap(id, posx, posy);
                var block = Game.Config.GetGameMap(mapId).blocks[posx][posy];
                block.thing = id;
                Game.Config.GetGameMap(mapId).blocks[posx][posy] = block;
                if (id == 0) {
                    panel.transform.Find("btnCurrentModal").Find("Image").GetComponent<Image>().sprite = null;
                    panel.transform.Find("btnCurrentModal").Find("Text").GetComponent<Text>().text = Game.Config.StringInternational.GetValue("none");
                } else {
                    panel.transform.Find("btnCurrentModal").Find("Image").GetComponent<Image>().sprite = Modal.GetResourceBaseSprite(id);
                    panel.transform.Find("btnCurrentModal").Find("Text").GetComponent<Text>().text = Game.Config.StringInternational.GetValue(Game.Config.modals[id].name);
                }
            });
        }

        public void OnMapCurrentModalClear()
        {
            var panel = mapMakerPanel.transform.Find("SetPanel");
            var mapId = panel.transform.Find("MapId").GetComponent<Dropdown>().value;
            Game.Map.ChangeThingOnMap(0, posx, posy);
            var block = Game.Config.GetGameMap(mapId).blocks[posx][posy];
            block.thing = 0;
            Game.Config.GetGameMap(mapId).blocks[posx][posy] = block;
            panel.transform.Find("btnCurrentModal").Find("Image").GetComponent<Image>().sprite = null;
            panel.transform.Find("btnCurrentModal").Find("Text").GetComponent<Text>().text = Game.Config.StringInternational.GetValue("none");
        }

        public void OnMapModalEventIdSelected()
        {
            var panel = mapMakerPanel.transform.Find("SetPanel");
            var mapId = panel.transform.Find("MapId").GetComponent<Dropdown>().value;
            var eventId = panel.transform.Find("EventId").GetComponent<Dropdown>().value;
            var block = Game.Config.GetGameMap(mapId).blocks[posx][posy];
            block.eventId = eventId;
            Game.Config.GetGameMap(mapId).blocks[posx][posy] = block;
        }

        // Map选择框回调
        void OnMapSelected()
        {
            var panel = mapMakerPanel.transform.Find("SetPanel");
            var mapId = panel.transform.Find("MapId").GetComponent<Dropdown>().value + 1;
            panel.transform.Find("MapName").GetComponent<InputField>().text = Game.Config.GetGameMap(mapId - 1).mapName;
            panel.transform.Find("Music").GetComponent<Dropdown>().value = Game.Config.GetGameMap(mapId - 1).music - 1;
            panel.transform.Find("btnBackModal").Find("Image").GetComponent<Image>().sprite = Modal.GetResourceBaseSprite(Game.Config.GetGameMap(mapId - 1).backThing);
            panel.transform.Find("btnBackModal").Find("Text").GetComponent<Text>().text = Game.Config.StringInternational.GetValue(Game.Config.modals[Game.Config.GetGameMap(mapId - 1).backThing].name);
            Game.Map.ShowMap(mapId);
            OnMapClicked(0, 0);
            AudioManager.StopMusic();
        }

        // Map选定地图块位置的回调
        override public void OnMapClicked(int posx, int posy)
        {
            if (!mapMakerPanel.activeSelf)
                return;
            this.posx = posx;
            this.posy = posy;
            var panel = mapMakerPanel.transform.Find("SetPanel");
            var mapId = panel.transform.Find("MapId").GetComponent<Dropdown>().value + 1;
            var map = Game.Config.GetGameMap(mapId - 1);
            panel.transform.Find("CurrentPosition").GetComponent<Text>().text = "(" + posx + ", " + posy + ")";
            if (map.blocks[posx][posy].thing == 0)
            {
                panel.transform.Find("btnCurrentModal").Find("Image").GetComponent<Image>().sprite = null;
                panel.transform.Find("btnCurrentModal").Find("Text").GetComponent<Text>().text = "none";
            }
            else
            {
                panel.transform.Find("btnCurrentModal").Find("Image").GetComponent<Image>().sprite = Modal.GetResourceBaseSprite(map.blocks[posx][posy].thing);
                panel.transform.Find("btnCurrentModal").Find("Text").GetComponent<Text>().text = Game.Config.StringInternational.GetValue(Game.Config.modals[map.blocks[posx][posy].thing].name);
            }
            panel.transform.Find("EventId").GetComponent<Dropdown>().value = map.blocks[posx][posy].eventId;
            ResetEventData(EventDataSourceType.MapBlock);
        }

        public enum EventDataSourceType
        {
            MapBlock = 1,
            Modal = 2,
            Chat = 3,
            Choice = 4,
        }
        private delegate long[] ResolveEventDataFunc(long[] eventData);
        private long[] ResolveEventDataByType(EventDataSourceType type, ResolveEventDataFunc func)
        {
            switch (type)
            {
                case EventDataSourceType.MapBlock:
                    {
                        var panel = mapMakerPanel.transform.Find("SetPanel");
                        var mapId = panel.transform.Find("MapId").GetComponent<Dropdown>().value + 1;
                        var map = Game.Config.GetGameMap(mapId - 1);
                        var block = map.blocks[posx][posy];
                        block.eventData = func(block.eventData);
                        map.blocks[posx][posy] = block;
                        return block.eventData;
                    }
                case EventDataSourceType.Modal:
                case EventDataSourceType.Chat:
                case EventDataSourceType.Choice:
                default:
                    return null;
            }
        }

        private ListView GetEventDataList(EventDataSourceType type)
        {
            switch (type)
            {
                case EventDataSourceType.MapBlock:
                    return mapMakerPanel.transform.Find("SetPanel").transform.Find("EventDataList").GetComponent<ListView>();
                case EventDataSourceType.Modal:
                case EventDataSourceType.Chat:
                case EventDataSourceType.Choice:
                default:
                    return null;
            }
        }

        private InputField GetEventDataInput(EventDataSourceType type)
        {
            switch (type)
            {
                case EventDataSourceType.MapBlock:
                    return mapMakerPanel.transform.Find("SetPanel").transform.Find("EventData").GetComponent<InputField>();
                case EventDataSourceType.Modal:
                case EventDataSourceType.Chat:
                case EventDataSourceType.Choice:
                default:
                    return null;
            }
        }

        private Button GetEventDataBtn(EventDataSourceType type, int btnNumber /* 0: OK, 1: Add, 2: Delete, 3: Up, 4: Down*/)
        {
            switch (type)
            {
                case EventDataSourceType.MapBlock:
                    switch (btnNumber)
                    {
                        case 0:
                            return mapMakerPanel.transform.Find("SetPanel").transform.Find("btnEventDataOK").GetComponent<Button>();
                        case 1:
                            return mapMakerPanel.transform.Find("SetPanel").transform.Find("btnEventDataAdd").GetComponent<Button>();
                        case 2:
                            return mapMakerPanel.transform.Find("SetPanel").transform.Find("btnEventDataDelete").GetComponent<Button>();
                        case 3:
                            return mapMakerPanel.transform.Find("SetPanel").transform.Find("btnEventDataUp").GetComponent<Button>();
                        case 4:
                            return mapMakerPanel.transform.Find("SetPanel").transform.Find("btnEventDataDown").GetComponent<Button>();
                    }
                    return null;
                case EventDataSourceType.Modal:
                case EventDataSourceType.Chat:
                case EventDataSourceType.Choice:
                    return null;
            }
            return null;
        }

        private void ResetEventData(EventDataSourceType type)
        {
            ResolveEventDataByType(type, (long[] eventData) =>
            {
                ResetEventData(eventData, GetEventDataList(type), GetEventDataInput(type), GetEventDataBtn(type, 0), GetEventDataBtn(type, 2), GetEventDataBtn(type, 3), GetEventDataBtn(type, 4));
                return eventData;
            });
        }

        private void ResetEventData(long[] eventData, ListView eventDataList, InputField eventDataInput, Button btnOK, Button btnDelete, Button btnUp, Button btnDown)
        {
            eventDataList.Clear();
            if (eventData != null && eventData.Length > 0)
            {
                for (var i = 0; i < eventData.Length; ++i)
                {
                    var item = eventDataList.PushbackDefaultItem().GetComponent<Button>();
                    item.transform.Find("Text").GetComponent<Text>().text = eventData[i].ToString();
                    item.onClick.AddListener(() => { OnEventDataClicked(item, eventDataList, eventDataInput, btnOK, btnDelete, btnUp, btnDown); });
                }
            }
            eventDataInput.text = "";
            eventDataInput.enabled = false;
            btnOK.enabled = false;
            btnDelete.enabled = false;
            btnUp.enabled = false;
            btnDown.enabled = false;
        }

        // Map上地图块的EventData列表项点击回调
        public void OnEventDataClicked(Button item, int typeNum)
        {
            EventDataSourceType type = (EventDataSourceType)typeNum;
            OnEventDataClicked(item, GetEventDataList(type), GetEventDataInput(type), GetEventDataBtn(type, 0), GetEventDataBtn(type, 2), GetEventDataBtn(type, 3), GetEventDataBtn(type, 4));
        }

        private void OnEventDataClicked(Button item, ListView eventDataList, InputField eventDataInput, Button btnOK, Button btnDelete, Button btnUp, Button btnDown)
        {
            foreach (var i in eventDataList)
            {
                i.GetComponent<Button>().interactable = true;
            }
            if (item == null)
            {
                eventDataInput.text = "";
                eventDataInput.enabled = false;
                btnOK.enabled = false;
                btnDelete.enabled = false;
                btnUp.enabled = false;
                btnDown.enabled = false;
                return;
            }
            var index = eventDataList.GetItemIndex(item.GetComponent<RectTransform>());
            item.interactable = false;
            eventDataInput.text = item.transform.Find("Text").GetComponent<Text>().text;
            eventDataInput.enabled = true;
            btnOK.enabled = true;
            btnDelete.enabled = true;
            btnUp.enabled = index > 0;
            btnDown.enabled = index < eventDataList.ItemCount - 1;
        }

        // Map上地图块的btnEventDataOK键的回调
        public void OnEventDataOKClicked(int typeNum)
        {
            EventDataSourceType type = (EventDataSourceType)typeNum;
            OnEventDataOKClicked(type, GetEventDataList(type), GetEventDataInput(type));
        }

        private void OnEventDataOKClicked(EventDataSourceType type, ListView eventDataList, InputField eventDataInput)
        {
            var index = eventDataList.GetItemIndex((RectTransform i) => { return !i.GetComponent<Button>().interactable; });
            var item = eventDataList[index].GetComponent<Button>();
            ResolveEventDataByType(type, (long[] eventData) =>
            {
                eventData[index] = System.Convert.ToInt64(eventDataInput.text);
                item.transform.Find("Text").GetComponent<Text>().text = eventData[index].ToString();
                return eventData;
            });
            ShowTips("Successful!");
        }

        // Map上地图块的EventData列表 + 键回调
        public void OnEventDataAddClicked(int typeNum)
        {
            EventDataSourceType type = (EventDataSourceType)typeNum;
            OnEventDataAddClicked(type, GetEventDataList(type), GetEventDataInput(type), GetEventDataBtn(type, 0), GetEventDataBtn(type, 2), GetEventDataBtn(type, 3), GetEventDataBtn(type, 4));
        }

        private void OnEventDataAddClicked(EventDataSourceType type, ListView eventDataList, InputField eventDataInput, Button btnOK, Button btnDelete, Button btnUp, Button btnDown)
        {
            var index = eventDataList.ItemCount;
            var item = eventDataList.PushbackDefaultItem().GetComponent<Button>();
            ResolveEventDataByType(type, (long[] eventData) =>
            {
                var newEventData = new long[index + 1];
                for (var i = 0; i < index; ++i)
                {
                    newEventData[i] = eventData[i];
                }
                newEventData[index] = 0;
                return newEventData;
            });
            eventDataInput.text = "0";
            item.GetComponent<Button>().onClick.AddListener(() => { OnEventDataClicked(item, eventDataList, eventDataInput, btnOK, btnDelete, btnUp, btnDown); });
            OnEventDataClicked(item, eventDataList, eventDataInput, btnOK, btnDelete, btnUp, btnDown);
        }

        // Map上地图块的EventData列表 - 键回调
        public void OnEventDataRemoveClicked(int typeNum)
        {
            EventDataSourceType type = (EventDataSourceType)typeNum;
            OnEventDataRemoveClicked(type, GetEventDataList(type), GetEventDataInput(type), GetEventDataBtn(type, 0), GetEventDataBtn(type, 2), GetEventDataBtn(type, 3), GetEventDataBtn(type, 4));
        }

        private void OnEventDataRemoveClicked(EventDataSourceType type, ListView eventDataList, InputField eventDataInput, Button btnOK, Button btnDelete, Button btnUp, Button btnDown)
        {
            var index = eventDataList.GetItemIndex((RectTransform i) => { return !i.GetComponent<Button>().interactable; });
            if (index >= 0)
            {
                var item = eventDataList[index];
                if (item != null)
                {
                    // 移除数据
                    ResolveEventDataByType(type, (long[] eventData) =>
                    {
                        var newEventData = new long[eventData.Length - 1];
                        for (var i = 0; i < index; ++i)
                        {
                            newEventData[i] = eventData[i];
                        }
                        for (var i = index; i < eventData.Length - 1; ++i)
                        {
                            newEventData[i] = eventData[i + 1];
                        }
                        return newEventData;
                    });
                    // 移除界面列表中的项
                    eventDataList.DeleteItem(index);
                }
            }
            OnEventDataClicked(null, eventDataList, eventDataInput, btnOK, btnDelete, btnUp, btnDown);
        }

        public void OnEventDataUp(int typeNum)
        {
            OnEventDataUpDown(true, typeNum);
        }

        public void OnEventDataDown(int typeNum)
        {
            OnEventDataUpDown(false, typeNum);
        }

        // Map上地图块的EventData列表 ↑ 以及 ↓ 键回调
        public void OnEventDataUpDown(bool isUp, int typeNum)
        {
            EventDataSourceType type = (EventDataSourceType)typeNum;
            OnEventDataUpDown(isUp, type, GetEventDataList(type), GetEventDataInput(type), GetEventDataBtn(type, 0), GetEventDataBtn(type, 2), GetEventDataBtn(type, 3), GetEventDataBtn(type, 4));
        }

        private void OnEventDataUpDown(bool isUp, EventDataSourceType type, ListView eventDataList, InputField eventDataInput, Button btnOK, Button btnDelete, Button btnUp, Button btnDown)
        {
            var index = eventDataList.GetItemIndex((RectTransform i) => { return !i.GetComponent<Button>().interactable; });
            if (index < 0 || (isUp ? index == 0 : index == eventDataList.ItemCount - 1))
            {
                OnEventDataClicked(null, eventDataList, eventDataInput, btnOK, btnDelete, btnUp, btnDown);
                return;
            }
            var item = eventDataList[index];
            if (item == null)
            {
                OnEventDataClicked(null, eventDataList, eventDataInput, btnOK, btnDelete, btnUp, btnDown);
                return;
            }
            var changedItem = eventDataList[isUp ? (index - 1) : (index + 1)];
            ResolveEventDataByType(type, (long[] eventData) =>
            {
                var temp = eventData[isUp ? (index - 1) : (index + 1)];
                item.transform.Find("Text").GetComponent<Text>().text = temp.ToString();
                eventData[isUp ? (index - 1) : (index + 1)] = eventData[index];
                changedItem.transform.Find("Text").GetComponent<Text>().text = eventData[index].ToString();
                eventData[index] = temp;
                return eventData;
            });
            OnEventDataClicked(changedItem.GetComponent<Button>(), eventDataList, eventDataInput, btnOK, btnDelete, btnUp, btnDown);
        }

        // 音乐播放键回调
        public void OnPlay(int index)
        {
            switch (index)
            {
                case 0:
                    AudioManager.PlayMusicLoop(GameObject.Find("Music").GetComponent<Dropdown>().value + 1);
                    break;
                case 1:
                    AudioManager.StopMusic();
                    AudioManager.PlaySound(GameObject.Find("WeaponHit").GetComponent<Dropdown>().value + 1);
                    break;
                case 2:
                    AudioManager.StopMusic();
                    AudioManager.PlaySound(GameObject.Find("WeaponCrit").GetComponent<Dropdown>().value + 1);
                    break;
            }
        }

        // Refresh audios 按钮回调
        public void OnRefreshAudio()
        {
            var allAudios = Resources.LoadAll<AudioClip>(Model.Dirs.AUDIO_DIR);
            Model.Audio[] audioData = new Model.Audio[allAudios.Length];
            Game.Config.audios.Clear();
            for (var i = 0; i < audioData.Length; ++i)
            {
                Game.Config.audios.Add(audioData[i].id, new Model.Audio()
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
            Model.ModalData[] prefabData = new Model.ModalData[allPrefabs.Length];
            Game.Config.modals.Clear();
            for (var i = 0; i < prefabData.Length; ++i)
            {
                Game.Config.modals.Add(prefabData[i + 1].id, new Model.ModalData()
                {
                    id = i + 1,
                    name = allPrefabs[i].name,
                    prefabPath = allPrefabs[i].name,
                    eventId = 0,
                    eventData = null,
                    typeId = 2
                });
            }
            //PlatformUIManager.ShowMessageBox("模型表刷新成功，请立即保存配置然后重新启动游戏！");
        }

        // 关闭(X)按钮回调
        public void OnExitEditor()
        {
            BackToStartScene();
        }

        // modal部分modal选择框
        public void OnModalSelected(int index)
        {
            if (index == 0)
                index = modalMakerPanel.transform.Find("ModalId").GetComponent<Dropdown>().value;
            ++index;
            modalMakerPanel.transform.Find("ModalName").GetComponent<InputField>().text = Game.Config.modals[index].name;
            modalMakerPanel.transform.Find("ModalType").GetComponent<Dropdown>().value = Game.Config.modals[index].typeId - 1;
            modalMakerPanel.transform.Find("EventId").GetComponent<Dropdown>().value = Game.Config.modals[index].eventId;
            modalMakerPanel.transform.Find("EventData").GetComponent<InputField>().text = Game.Config.modals[index].eventData[0].ToString();
            modalMakerPanel.transform.Find("ModalPrefabText").GetComponent<InputField>().text = Game.Config.modals[index].prefabPath;
            modalMakerPanel.transform.Find("ModalPrefabImage").GetComponent<Image>().sprite = Resources.Load<GameObject>(Modal.GetResourcePath(index)).GetComponent<SpriteRenderer>().sprite;
            if (Game.Config.modals[index].typeId == (int)ModalType.Player)
            {
                var player = Game.Config.players[Game.Config.modals[index].id];
                modalMakerPanel.transform.Find("Level").GetComponent<InputField>().text = player.level.ToString();
                modalMakerPanel.transform.Find("Exp").GetComponent<InputField>().text = player.exp.ToString();
                modalMakerPanel.transform.Find("Life").GetComponent<InputField>().text = player.life.ToString();
                modalMakerPanel.transform.Find("Attack").GetComponent<InputField>().text = player.attack.ToString();
                modalMakerPanel.transform.Find("Defense").GetComponent<InputField>().text = player.defense.ToString();
                modalMakerPanel.transform.Find("Speed").GetComponent<InputField>().text = player.speed.ToString();
                modalMakerPanel.transform.Find("Critical").GetComponent<InputField>().text = player.critical.ToString();
                modalMakerPanel.transform.Find("Gold").GetComponent<InputField>().text = player.gold.ToString();
                modalMakerPanel.transform.Find("ModalWeaponId").GetComponent<Dropdown>().value = player.weaponId - 1;
            }
            else if (Game.Config.modals[index].typeId == (int)ModalType.Monster)
            {
                var monster = Game.Config.monsters[Game.Config.modals[index].id];
                modalMakerPanel.transform.Find("Level").GetComponent<InputField>().text = monster.level.ToString();
                modalMakerPanel.transform.Find("Exp").GetComponent<InputField>().text = monster.exp.ToString();
                modalMakerPanel.transform.Find("Life").GetComponent<InputField>().text = monster.life.ToString();
                modalMakerPanel.transform.Find("Attack").GetComponent<InputField>().text = monster.attack.ToString();
                modalMakerPanel.transform.Find("Defense").GetComponent<InputField>().text = monster.defense.ToString();
                modalMakerPanel.transform.Find("Speed").GetComponent<InputField>().text = monster.speed.ToString();
                modalMakerPanel.transform.Find("Critical").GetComponent<InputField>().text = monster.critical.ToString();
                modalMakerPanel.transform.Find("Gold").GetComponent<InputField>().text = monster.gold.ToString();
                modalMakerPanel.transform.Find("ModalWeaponId").GetComponent<Dropdown>().value = monster.weaponId - 1;
            }
            else
            {
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
        public void OnPrefabSelected(RectTransform sender)
        {
            var currentPrefab = GameObject.Find("CurrentPrefab");
            currentPrefab.GetComponent<Image>().sprite = sender.GetComponent<Image>().sprite;
            currentPrefab.GetComponent<UserData>().SetStringData(sender.GetComponent<UserData>().GetStringData());
            var prefabPath = currentPrefab.GetComponent<UserData>().GetStringData();
            modalMakerPanel.transform.Find("SelectedModalPrefab").GetComponent<InputField>().text = prefabPath;
            modalMakerPanel.transform.Find("CurrentPrefab").GetComponent<Image>().sprite = Resources.Load<GameObject>(Modal.GetResourcePath(prefabPath)).GetComponent<SpriteRenderer>().sprite;

            GameObject.Find("btnSetModalPrefab").GetComponent<Button>().enabled = true;
        }

        // btnSetModalPrefab 按钮回调
        public void OnSetModalPrefab()
        {
            modalMakerPanel.transform.Find("ModalPrefabText").GetComponent<InputField>().text = modalMakerPanel.transform.Find("SelectedModalPrefab").GetComponent<InputField>().text;
            modalMakerPanel.transform.Find("ModalPrefabImage").GetComponent<Image>().sprite = modalMakerPanel.transform.Find("CurrentPrefab").GetComponent<Image>().sprite;
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
            var index = modalMakerPanel.transform.Find("ModalId").GetComponent<Dropdown>().value + 1;
            Game.Config.modals[index].name = modalMakerPanel.transform.Find("ModalName").GetComponent<InputField>().text;
            Game.Config.modals[index].typeId = modalMakerPanel.transform.Find("ModalType").GetComponent<Dropdown>().value + 1;
            Game.Config.modals[index].eventId = modalMakerPanel.transform.Find("EventId").GetComponent<Dropdown>().value;
            Game.Config.modals[index].eventData[0] = System.Convert.ToInt64(modalMakerPanel.transform.Find("EventData").GetComponent<InputField>().text);
            Game.Config.modals[index].prefabPath = modalMakerPanel.transform.Find("ModalPrefabText").GetComponent<InputField>().text;
            if (Game.Config.modals[index].typeId == (int)ModalType.Player)
            {
                var playerId = Game.Config.modals[index].id;
                if (!Game.Config.players.ContainsKey(playerId))
                {
                    Game.Config.players.Add(playerId, new Model.PlayerData()
                    {
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
            else if (Game.Config.modals[index].typeId == (int)ModalType.Monster)
            {
                var monsterId = Game.Config.modals[index].id;
                if (!Game.Config.monsters.ContainsKey(monsterId))
                {
                    Game.Config.monsters.Add(monsterId, new Model.MonsterData()
                    {
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
        public void OnWeaponSelected(int index)
        {
            if (index == 0)
                index = modalMakerPanel.transform.Find("WeaponId").GetComponent<Dropdown>().value;
            ++index;
            modalMakerPanel.transform.Find("WeaponName").GetComponent<InputField>().text = Game.Config.weapons[index].name;
            modalMakerPanel.transform.Find("WeaponHit").GetComponent<Dropdown>().value = Game.Config.weapons[index].audioId - 1;
            modalMakerPanel.transform.Find("WeaponCrit").GetComponent<Dropdown>().value = Game.Config.weapons[index].critAudioId - 1;
            modalMakerPanel.transform.Find("WeaponPrefab").GetComponent<InputField>().text = Game.Config.weapons[index].prefabPath;
        }

        // Apply Weapon 按钮回调
        public void OnWeaponApply()
        {
            var index = modalMakerPanel.transform.Find("WeaponId").GetComponent<Dropdown>().value;
            Game.Config.weapons[index].name = modalMakerPanel.transform.Find("WeaponName").GetComponent<InputField>().text;
            Game.Config.weapons[index].audioId = modalMakerPanel.transform.Find("WeaponHit").GetComponent<Dropdown>().value + 1;
            Game.Config.weapons[index].critAudioId = modalMakerPanel.transform.Find("WeaponCrit").GetComponent<Dropdown>().value + 1;
            Game.Config.weapons[index].prefabPath = modalMakerPanel.transform.Find("WeaponPrefab").GetComponent<InputField>().text;
        }

        // Add map, Add modal, Add weapon 按钮的回调
        public void OnAdd(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        var drop = mapMakerPanel.transform.Find("MapPanel").transform.Find("MapId").GetComponent<Dropdown>();
                        //var ind = Game.Config.AddMap();
                        var lst = new List<string>();
                        //lst.Add(ind + 1 + ". (new)");
                        drop.AddOptions(lst);
                        //drop.value = ind;
                        OnMapSelected();
                    }
                    break;
                case 1:
                    {
                        var drop = modalMakerPanel.transform.Find("ModalId").GetComponent<Dropdown>();
                        var ind = Game.Config.modals.Count;
                        Game.Config.modals.Add(ind, new Model.ModalData() { id = ind });
                        var lst = new List<string> {
                        ind + 1 + ". (new)"
                    };
                        drop.AddOptions(lst);
                        drop.value = ind;
                        OnModalSelected(ind);
                    }
                    break;
                case 2:
                    {
                        var drop = modalMakerPanel.transform.Find("WeaponId").GetComponent<Dropdown>();
                        var ind = Game.Config.weapons.Count;
                        Game.Config.weapons.Add(ind, new Model.WeaponData() { id = ind });
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
        public void OnStringSelected(int index)
        {
            var stringMakerPanel = stringChatsAndChoicesPanel.transform.Find("StringMakerPanel");
            var dropdown = stringMakerPanel.transform.Find("StringId").GetComponent<Dropdown>();
            if (index == -1)
            {
                index = dropdown.value;
            }
            else
            {
                dropdown.value = index;
            }
            var str = dropdown.options[index].text;
            var data = Game.Config.strings[str];
            //显示数据
            stringMakerPanel.Find("StringKey").GetComponent<InputField>().text = str;
            foreach (var v in UI_LANG_KEY)
            {
                stringMakerPanel.Find(v.Value).GetComponent<InputField>().text = "";
            }
            foreach (var v in data.strings)
            {
                stringMakerPanel.Find(UI_LANG_KEY[v.langKey]).GetComponent<InputField>().text = v.content;
            }
        }

        // Add string 按钮回调
        public void OnAddString()
        {
            if (Game.Config.strings.ContainsKey("#####"))
            {
                ShowTips("请先将之前添加的字符串编辑完成并保存!");
                return;
            }
            Game.Config.strings.Add("#####", new Model.InternationalString { id = Game.Config.strings.Count + 1, key = "#####", strings = new Model.StringInOneLanguage[] { new Model.StringInOneLanguage { langKey = "en-us", content = "" } } });

            var stringMakerPanel = stringChatsAndChoicesPanel.transform.Find("StringMakerPanel");
            var dropdown = stringMakerPanel.transform.Find("StringId").GetComponent<Dropdown>();
            dropdown.AddOptions(new List<string> { "#####" });
            OnStringSelected(dropdown.options.Count - 1);
        }

        // Save String 按钮回调
        public void OnSaveString()
        {
            var stringMakerPanel = stringChatsAndChoicesPanel.transform.Find("StringMakerPanel");
            var dropdown = stringMakerPanel.transform.Find("StringId").GetComponent<Dropdown>();
            var key = dropdown.options[dropdown.value].text;
            var oldValue = Game.Config.strings[key];
            var newKey = stringMakerPanel.transform.Find("StringKey").GetComponent<InputField>().text;
            if (key != newKey && Game.Config.strings.ContainsKey(newKey))
            {
                ShowTips("保存失败, key值重复!");
                return;
            }
            if (newKey == "")
            {
                ShowTips("保存失败, key不能为空!");
                return;
            }
            var stringList = new List<Model.StringInOneLanguage>();
            foreach (var v in UI_LANG_KEY)
            {
                if (stringMakerPanel.Find(v.Value).GetComponent<InputField>().text != "")
                {
                    stringList.Add(new Model.StringInOneLanguage { langKey = v.Key, content = stringMakerPanel.Find(v.Value).GetComponent<InputField>().text });
                }
            }
            if (stringList.Count < 1)
            {
                ShowTips("保存失败, 至少需要有一种语言拥有字符串值");
                return;
            }
            if (key == newKey)
            {
                Game.Config.strings[key].strings = stringList.ToArray();
            }
            else
            {
                var newValue = new Model.InternationalString { id = oldValue.id, key = newKey, strings = stringList.ToArray() };
                Game.Config.strings.Remove(key);
                Game.Config.strings.Add(newKey, newValue);
                var dropdownValue = dropdown.value;
                dropdown.options[dropdownValue].text = newKey;
                dropdown.captionText.text = newKey;
            }
        }

        public void OnChatSelected(int index)
        {
            var chatDataPanel = stringChatsAndChoicesPanel.transform.Find("ChatDataPanel");
            var dropdown = chatDataPanel.Find("ChatId").GetComponent<Dropdown>();
            if (index == -1)
            {
                index = dropdown.value;
            }
            else
            {
                dropdown.value = index;
            }
            var id = index + 1;
            var chatLastEventIds = chatDataPanel.Find("LastEventId").GetComponent<Dropdown>();
            chatLastEventIds.value = Game.Config.chats[id].eventId;
            var chatLastEventData = chatDataPanel.Find("LastEventData").GetComponent<InputField>();
            chatLastEventData.text = Game.Config.chats[id].eventData.ToString();
            var chatCanOn = chatDataPanel.Find("CanOn").GetComponent<Dropdown>();
            chatCanOn.value = Game.Config.chats[id].canOn ? 0 : 1;
        }

        // Add chat 按钮回调
        public void OnAddChat()
        {
            var id = Game.Config.chats.Count + 1;
            Game.Config.chats.Add(id, new Model.ChatData { id = id, eventId = 0, eventData = null, canOn = true, data = new Model.OneChatData[0] });

            var chatDataPanel = stringChatsAndChoicesPanel.transform.Find("ChatDataPanel");
            var dropdown = chatDataPanel.transform.Find("ChatId").GetComponent<Dropdown>();
            dropdown.AddOptions(new List<string> { id.ToString() });
            OnStringSelected(dropdown.options.Count - 1);
        }

        // Save chat 按钮回调
        public void OnSaveChat()
        {
            var chatDataPanel = stringChatsAndChoicesPanel.transform.Find("ChatDataPanel");
            var dropdown = chatDataPanel.transform.Find("ChatId").GetComponent<Dropdown>();
            var key = System.Convert.ToInt32(dropdown.options[dropdown.value].text);
            var chatLastEventIds = chatDataPanel.Find("LastEventId").GetComponent<Dropdown>();
            Game.Config.chats[key].eventId = chatLastEventIds.value;
            var chatLastEventData = chatDataPanel.Find("LastEventData").GetComponent<InputField>();
            Game.Config.chats[key].eventData[0] = System.Convert.ToInt64(chatLastEventData.text);
            var chatCanOn = chatDataPanel.Find("CanOn").GetComponent<Dropdown>();
            Game.Config.chats[key].canOn = chatCanOn.value == 0 ? true : false;
        }

        public void OnChoiceSelected(int index)
        {
            var choiceDataPanel = stringChatsAndChoicesPanel.transform.Find("ChoiceDataPanel");
            var dropdown = choiceDataPanel.Find("ChoiceId").GetComponent<Dropdown>();
            if (index == -1)
            {
                index = dropdown.value;
            }
            else
            {
                dropdown.value = index;
            }
            var id = index + 1;
            var choiceSpeaker = choiceDataPanel.Find("Speaker").GetComponent<Dropdown>();
            if (Game.Config.choices[id].speakerId < -100)
            {
                choiceSpeaker.value = 0;
            }
            else if (Game.Config.choices[id].speakerId < 0)
            {
                choiceSpeaker.value = 1;
            }
            else if (Game.Config.choices[id].speakerId == 0)
            {
                choiceSpeaker.value = 2;
            }
            else
            {
                choiceSpeaker.value = Game.Config.choices[id].speakerId + 2;
            }
            var choiceTitleString = choiceDataPanel.Find("TitleString").GetComponent<Dropdown>();
            choiceTitleString.value = choiceTitleString.options.IndexOf(new Dropdown.OptionData(Game.Config.choices[id].title));
            var choiceTailString = choiceDataPanel.Find("TailString").GetComponent<Dropdown>();
            choiceTailString.value = choiceTailString.options.IndexOf(new Dropdown.OptionData(Game.Config.choices[id].tail));
            var choiceCanOn = choiceDataPanel.Find("CanOn").GetComponent<Dropdown>();
            choiceCanOn.value = Game.Config.choices[id].canOn ? 0 : 1;
        }

        // Add choice 按钮回调
        public void OnAddChoice()
        {
            var id = Game.Config.choices.Count + 1;
            Game.Config.choices.Add(id, new Model.ChoiceData { id = id, speakerId = 0, title = "", tail = "", canOn = true, data = new Model.OneChoiceData[0] });

            var choiceDataPanel = stringChatsAndChoicesPanel.transform.Find("ChoiceDataPanel");
            var dropdown = choiceDataPanel.Find("ChoiceId").GetComponent<Dropdown>();
            dropdown.AddOptions(new List<string> { id.ToString() });
            OnStringSelected(dropdown.options.Count - 1);
        }

        // Save choice 按钮回调
        public void OnSaveChoice()
        {
            var choiceDataPanel = stringChatsAndChoicesPanel.transform.Find("ChoiceDataPanel");
            var dropdown = choiceDataPanel.Find("ChoiceId").GetComponent<Dropdown>();
            var key = System.Convert.ToInt32(dropdown.options[dropdown.value].text);
            var choiceSpeakerId = choiceDataPanel.Find("Speaker").GetComponent<Dropdown>();
            if (choiceSpeakerId.value == 0)
            {
                Game.Config.choices[key].speakerId = -200;
            }
            else if (choiceSpeakerId.value == 1)
            {
                Game.Config.choices[key].speakerId = -1;
            }
            else if (choiceSpeakerId.value == 2)
            {
                Game.Config.choices[key].speakerId = 0;
            }
            else
            {
                Game.Config.choices[key].speakerId = choiceSpeakerId.value - 2;
            }
            var choiceTitleString = choiceDataPanel.Find("TitleString").GetComponent<Dropdown>();
            Game.Config.choices[key].title = choiceTitleString.captionText.text;
            var choiceTailString = choiceDataPanel.Find("TailString").GetComponent<Dropdown>();
            Game.Config.choices[key].tail = choiceTailString.captionText.text;
            var choiceCanOn = choiceDataPanel.Find("CanOn").GetComponent<Dropdown>();
            Game.Config.choices[key].canOn = choiceCanOn.value == 0 ? true : false;
        }

        // 弹出Tips提示, 并在一定时间后消失
        public override void ShowTips(params string[] texts)
        {
            string text = "";
            for (var i = 0; i < texts.Length; ++i)
            {
                text += texts[i];
            }
            var tipbar = Control.TipBar.ShowTip();
            tipbar.transform.SetParent(dialogCanvas.transform, false);
            tipbar.SetTipText(text);
            tipbar.StartAutoRemove(200);
        }

        private void RefreshDataList()
        {
            var chatDataPanel = stringChatsAndChoicesPanel.transform.Find("ChatDataPanel");
            var choiceDataPanel = stringChatsAndChoicesPanel.transform.Find("ChoiceDataPanel");

            var chatSpeaker = chatDataPanel.Find("DataSpeaker").GetComponent<Dropdown>();
            var chatContent = chatDataPanel.Find("DataContent").GetComponent<Dropdown>();
            var chatSaveDataList = chatDataPanel.Find("btnSaveChatDatas").GetComponent<Button>();
            var choiceEventId = choiceDataPanel.Find("DataEventId").GetComponent<Dropdown>();
            var choiceEventData = choiceDataPanel.Find("DataEventData").GetComponent<InputField>();
            var choiceContent = choiceDataPanel.Find("DataContent").GetComponent<Dropdown>();
            var choiceSaveDataList = chatDataPanel.Find("btnSaveChoiceDatas").GetComponent<Button>();

            // TODO : 以下部分需要理清
            chatSpeaker.enabled = isDataListForChat && dataList.ItemCount > 0;
            chatContent.enabled = isDataListForChat && dataList.ItemCount > 0;
            chatSaveDataList.enabled = isDataListForChat && dataList.ItemCount > 0;
            choiceEventId.enabled = !isDataListForChat && dataList.ItemCount > 0;
            choiceEventData.enabled = !isDataListForChat && dataList.ItemCount > 0;
            choiceContent.enabled = !isDataListForChat && dataList.ItemCount > 0;
            choiceSaveDataList.enabled = !isDataListForChat && dataList.ItemCount > 0;


        }

        public GameObject dialogCanvas;
        public Dropdown panelSelect;

        private GameObject[] allPrefabs;
        private GameObject mapMakerPanel;
        private GameObject modalMakerPanel;
        private GameObject stringChatsAndChoicesPanel;
        private int posx = 0;
        private int posy = 0;
        private ListView prefabList;
        private ListView dataList;
        private bool isDataListForChat = true;

        private delegate void IOTaskResult(string returnValue);
        private Queue<KeyValuePair<System.Threading.Tasks.Task<string>, IOTaskResult>> ioTaskLine = new Queue<KeyValuePair<System.Threading.Tasks.Task<string>, IOTaskResult>>();
    }

}