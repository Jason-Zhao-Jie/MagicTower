
public class DataCenter
{
    public static DataCenter instance = null;

    public Constant.MapData[] NewGameMaps { get { return data.newGameMaps; } }
    public Constant.ModalData[] Modals { get { return data.modals; } }
    public Constant.ModalData GetModalById(int id)
    {
        for (int i = 0; i < data.modals.Length; ++i)
            if (data.modals[i].id == id)
                return data.modals[i];
        return null;
    }
    public Constant.Audio[] Audios { get { return data.audios; } }
    public string GetAudioById(int id)
    {
        for (int i = 0; i < data.audios.Length; ++i)
            if (data.audios[i].id == id)
                return data.audios[i].path;
        return "";
    }
    public Constant.MonsterData[] Monsters { get { return data.monsters; } }
    public Constant.MonsterData GetMonsterDataById(int id)
    {
        for (int i = 0; i < data.monsters.Length; ++i)
            if (data.monsters[i].id == id)
                return data.monsters[i];
        return null;
    }
    public Constant.PlayerData[] Players { get { return data.players; } }
    public Constant.PlayerData GetPlayerDataById(int id)
    {
        for (int i = 0; i < data.players.Length; ++i)
            if (data.players[i].id == id)
                return data.players[i];
        return new Constant.PlayerData();
    }
    public Constant.Event[] Events { get { return data.events; } }
    public Constant.Event GetEventDataById(int id)
    {
        for (int i = 0; i < data.events.Length; ++i)
            if (data.audios[i].id == id)
                return data.events[i];
        return null;
    }

    /// <summary>
    /// When the game loading begins, the data should be load and the instance should be initialized
    /// This function is used to replace the "Start" function
    /// </summary>
    public void LoadData()
    {
        status = Constant.EGameStatus.Start;

        // Reading json config files:
        var txt = UnityEngine.Resources.Load<UnityEngine.TextAsset>("GameData");
        //data = JsonUtility.FromJson<GameData>(txt.text);
        data = GetGameDataFromJsonString(txt.text);

    }

    public Constant.EGameStatus Status
    {
        get { return status; }
        set { status = value; }
    }

    public static string GetRuntimeDataJson()
    {
        var saver = new JObject();
        saver["player"] = GetJsonOfPlayerData(PlayerController.instance.data);
        var pos = new JObject();
        pos["x"] = new JNumber(PlayerController.instance.posx);
        pos["y"] = new JNumber(PlayerController.instance.posy);
        pos["mapId"] = new JNumber(MapManager.instance.CurrentFloorId);
        saver["pos"] = pos;
        saver["maps"] = GetJsonOfMapData(MapManager.instance.MapData);
        return saver.String;
    }

    public static bool LoadGameDataFromJson(string json)
    {
        var jo = JObject.isThis(json) as JObject;
        if (jo == null || !jo.ContainsKey("player") || !jo.ContainsKey("pos") || !jo.ContainsKey("maps"))
            return false;
        var pos = jo["pos"] as JObject;
        if (pos == null)
            return false;
        PlayerController.instance.PlayerData = GetPlayerDataOfJson(jo["player"] as JObject);
        MapManager.instance.SetData(pos["mapId"].ToInt(), GetMapDataOfJson(jo["maps"] as JArray));
        PlayerController.instance.posx = pos["x"].ToInt();
        PlayerController.instance.posy = pos["y"].ToInt();
        return true;
    }

    private Constant.EGameStatus status;
    internal Constant.GameData data;

    private static Constant.GameData GetGameDataFromJsonString(string json)
    {
        var _root = ArmyAntJson.Create(json) as JObject;
        var data = new Constant.GameData();

        // parse map data
        data.newGameMaps = GetMapDataOfJson(_root["newGameMaps"] as JArray);

        // parse modal data
        var _modals = _root["modals"] as JArray;
        data.modals = new Constant.ModalData[_modals.Length];
        for (var i = 0; i < _modals.Length; ++i)
        {
            var __oneModal = _modals[i] as JObject;
            data.modals[i] = new Constant.ModalData();
            var __modalData = data.modals[i];
            __modalData.id = __oneModal["id"].ToInt();
            __modalData.typeId = __oneModal["typeId"].ToInt();
            __modalData.name = __oneModal["name"].ToString();
            __modalData.prefabPath = __oneModal["prefabPath"].ToString();
            __modalData.eventId = __oneModal["eventId"].ToInt();
        }

        // parse audios
        var _audios = _root["audios"] as JArray;
        data.audios = new Constant.Audio[_audios.Length];
        for (var i = 0; i < _audios.Length; ++i)
        {
            var __oneAudio = _audios[i] as JObject;
            data.audios[i] = new Constant.Audio();
            var __audioData = data.audios[i];
            __audioData.id = __oneAudio["id"].ToInt();
            __audioData.path = __oneAudio["path"].ToString();
        }

        // parse monster data
        var _monsters = _root["monsters"] as JArray;
        data.monsters = new Constant.MonsterData[_monsters.Length];
        for (var i = 0; i < _monsters.Length; ++i)
        {
            var __oneMonster = _monsters[i] as JObject;
            data.monsters[i] = new Constant.MonsterData();
            var __monsterData = data.monsters[i];
            __monsterData.id = __oneMonster["id"].ToInt();
            __monsterData.level = __oneMonster["level"].ToInt();
            __monsterData.exp = __oneMonster["exp"].ToInt();
            __monsterData.life = __oneMonster["life"].ToInt();
            __monsterData.attack = __oneMonster["attack"].ToInt();
            __monsterData.defense = __oneMonster["defense"].ToInt();
            __monsterData.speed = __oneMonster["speed"].ToInt();
            __monsterData.critical = __oneMonster["critical"].ToFloat();
            __monsterData.gold = __oneMonster["gold"].ToInt();
            __monsterData.weaponId = __oneMonster["weaponId"].ToInt();

            var __specials = __oneMonster["special"] as JArray;
            __monsterData.special = new int[__specials.Length];
            for (var n = 0; n < __specials.Length; ++n)
            {
                __monsterData.special[n] = __specials[i].ToInt();
            }
        }

        // parse player data
        var _players = _root["players"] as JArray;
        data.players = new Constant.PlayerData[_players.Length];
        for (var i = 0; i < _players.Length; ++i)
        {
            data.players[i] = GetPlayerDataOfJson(_players[i] as JObject);
        }

        // parse event data
        var _events = _root["events"] as JArray;
        data.events = new Constant.Event[_events.Length];
        for (var i = 0; i < _events.Length; ++i)
        {
            var __oneEvent = _events[i] as JObject;
            data.events[i] = new Constant.Event();
            var __eventData = data.events[i];
            __eventData.id = __oneEvent["id"].ToInt();
            __eventData.typeId = __oneEvent["typeId"].ToInt();
            __eventData.dataId = System.Convert.ToInt64(__oneEvent["dataId"].ToFloat());
        }

        // parse weapon data
        var _weapons = _root["weapons"] as JArray;
        data.weapons = new Constant.WeaponData[_weapons.Length];
        for (var i = 0; i < _weapons.Length; ++i)
        {
            var __oneWeapon = _weapons[i] as JObject;
            data.weapons[i] = new Constant.WeaponData();
            var __weaponData = data.weapons[i];
            __weaponData.id = __oneWeapon["id"].ToInt();
            __weaponData.name = __oneWeapon["name"].ToString();
            __weaponData.prefabPath = __oneWeapon["prefabPath"].ToString();
            __weaponData.audioId = __oneWeapon["audioId"].ToInt();
            __weaponData.critAudioId = __oneWeapon["critAudioId"].ToInt();
        }

        // parse chat data
        var _chats = _root["chats"] as JArray;
        data.chats = new Constant.ChatData[_chats.Length];
        for (var i = 0; i < _chats.Length; ++i)
        {
            var __oneChat = _chats[i] as JObject;
            data.chats[i] = new Constant.ChatData();
            var __chatData = data.chats[i];
            __chatData.id = __oneChat["id"].ToInt();
            __chatData.lastEventId = __oneChat["lastEventId"].ToInt();
            __chatData.canOn = __oneChat["canOn"].ToBool();

            var __data = __oneChat["data"] as JArray;
            __chatData.data = new Constant.OneChatData[__data.Length];
            for (int n = 0; n < __data.Length; ++n)
            {
                var ___oneData = __data[n] as JObject;
                __chatData.data[n].speakerId = ___oneData["speakerId"].ToInt();
                __chatData.data[n].content = ___oneData["content"].ToString();
            }
        }

        // parse choice data
        var _choices = _root["choices"] as JArray;
        data.choices = new Constant.ChoiceData[_choices.Length];
        for (var i = 0; i < _choices.Length; ++i)
        {
            var __oneChoice = _choices[i] as JObject;
            data.choices[i] = new Constant.ChoiceData();
            var __choiceData = data.choices[i];
            __choiceData.id = __oneChoice["id"].ToInt();
            __choiceData.speakerId = __oneChoice["speakerId"].ToInt();
            __choiceData.title = __oneChoice["title"].ToString();
            __choiceData.tail = __oneChoice["tail"].ToString();
            __choiceData.canOn = __oneChoice["canOn"].ToBool();

            var __data = __oneChoice["data"] as JArray;
            __choiceData.data = new Constant.OneChoiceData[__data.Length];
            for (int n = 0; n < __data.Length; ++n)
            {
                var ___oneData = __data[n] as JObject;
                __choiceData.data[n] = new Constant.OneChoiceData();
                __choiceData.data[n].eventId = ___oneData["eventId"].ToInt();
                __choiceData.data[n].content = ___oneData["content"].ToString();
            }
        }

        return data;
    }

    internal static string GetJsonStringFromGameData(Constant.GameData data)
    {
        var json = new JObject();

        // Set map data
        json["newGameMaps"] = GetJsonOfMapData(data.newGameMaps);

        // Set modal data
        var modals = new JArray();
        for (int i = 0; i < data.modals.Length; ++i)
        {
            var _oneModal = new JObject();
            _oneModal["id"] = new JNumber(data.modals[i].id);
            _oneModal["typeId"] = new JNumber(data.modals[i].typeId);
            _oneModal["prefabPath"] = new JString(data.modals[i].prefabPath);
            _oneModal["eventId"] = new JNumber(data.modals[i].eventId);
            _oneModal["name"] = new JString(data.modals[i].name);
            modals.Add(_oneModal);
        }
        json["modals"] = modals;

        // Set audios
        var audios = new JArray();
        for (int i = 0; i < data.audios.Length; ++i)
        {
            var _oneAudio = new JObject();
            _oneAudio["id"] = new JNumber(data.audios[i].id);
            _oneAudio["path"] = new JString(data.audios[i].path);
            audios.Add(_oneAudio);
        }
        json["audios"] = audios;

        // Set monster data
        var monsters = new JArray();
        for (int i = 0; i < data.monsters.Length; ++i)
        {
            var _oneMonster = new JObject();
            _oneMonster["id"] = new JNumber(data.monsters[i].id);
            _oneMonster["level"] = new JNumber(data.monsters[i].level);
            _oneMonster["exp"] = new JNumber(data.monsters[i].exp);
            _oneMonster["life"] = new JNumber(data.monsters[i].life);
            _oneMonster["attack"] = new JNumber(data.monsters[i].attack);
            _oneMonster["defense"] = new JNumber(data.monsters[i].defense);
            _oneMonster["speed"] = new JNumber(data.monsters[i].speed);
            _oneMonster["critical"] = new JNumber(data.monsters[i].critical);
            _oneMonster["gold"] = new JNumber(data.monsters[i].gold);
            _oneMonster["weaponId"] = new JNumber(data.monsters[i].weaponId);
            var _special = new JArray();
            for (int n = 0; n < data.monsters[i].special.Length; ++n)
            {
                _special.Add(new JNumber(data.monsters[i].special[n]));
            }
            _oneMonster["special"] = _special;
            monsters.Add(_oneMonster);
        }
        json["monsters"] = monsters;

        // Set player data
        var players = new JArray();
        for (int i = 0; i < data.players.Length; ++i)
        {
            players.Add(GetJsonOfPlayerData(data.players[i]));
        }
        json["players"] = players;

        // Set event data
        var events = new JArray();
        for (int i = 0; i < data.events.Length; ++i)
        {
            var _oneEvent = new JObject();
            _oneEvent["id"] = new JNumber(data.events[i].id);
            _oneEvent["typeId"] = new JNumber(data.events[i].typeId);
            _oneEvent["dataId"] = new JNumber(data.events[i].dataId);
            events.Add(_oneEvent);
        }
        json["events"] = events;

        // Set weapon data
        var weapons = new JArray();
        for (int i = 0; i < data.weapons.Length; ++i)
        {
            var _oneWeapon = new JObject();
            _oneWeapon["id"] = new JNumber(data.weapons[i].id);
            _oneWeapon["name"] = new JString(data.weapons[i].name);
            _oneWeapon["prefabPath"] = new JString(data.weapons[i].prefabPath);
            _oneWeapon["audioId"] = new JNumber(data.weapons[i].audioId);
            _oneWeapon["critAudioId"] = new JNumber(data.weapons[i].critAudioId);
            weapons.Add(_oneWeapon);
        }
		json["weapons"] = weapons;

		// Set chat data
		var chats = new JArray();
		for (int i = 0; i < data.chats.Length; ++i)
		{
			var _oneChat = new JObject();
			_oneChat["id"] = new JNumber(data.chats[i].id);
            _oneChat["lastEventId"] = new JNumber(data.chats[i].lastEventId);
            _oneChat["canOn"] = new JBoolean(data.chats[i].canOn);

            var _datas = new JArray();
            for (int n = 0; n < data.chats[i].data.Length;++n){
				var __oneData = new JObject();
				__oneData["speakerId"] = new JNumber(data.chats[i].data[n].speakerId);
                __oneData["content"] = new JString(data.chats[i].data[n].content);
                _datas.Add(__oneData);
            }
            _oneChat["data"] = _datas;
            chats.Add(_oneChat);
		}
		json["chats"] = chats;

		// Set chat data
		var choices = new JArray();
		for (int i = 0; i < data.choices.Length; ++i)
		{
			var _oneChoice = new JObject();
			_oneChoice["id"] = new JNumber(data.choices[i].id);
			_oneChoice["speakerId"] = new JNumber(data.choices[i].speakerId);
            _oneChoice["title"] = new JString(data.choices[i].title);
            _oneChoice["tail"] = new JString(data.choices[i].tail);
			_oneChoice["canOn"] = new JBoolean(data.choices[i].canOn);

			var _datas = new JArray();
			for (int n = 0; n < data.choices[i].data.Length; ++n)
			{
				var __oneData = new JObject();
                __oneData["eventId"] = new JNumber(data.choices[i].data[n].eventId);
				__oneData["content"] = new JString(data.choices[i].data[n].content);
				_datas.Add(__oneData);
			}
			_oneChoice["data"] = _datas;
            choices.Add(_oneChoice);
		}
		json["choices"] = choices;

        return json.String;
    }

    private static JArray GetJsonOfMapData(Constant.MapData[] data)
    {
        var ret = new JArray();
        for (var i = 0; i < data.Length; ++i)
        {
            var ob = new JObject();
            ob["mapId"] = new JNumber(data[i].mapId);
            ob["mapName"] = new JString(data[i].mapName);
            ob["backThing"] = new JNumber(data[i].backThing);
            ob["music"] = new JNumber(data[i].music);
            var blocks = new JArray();
            for (int x = 0; x < data[i].mapBlocks.Length; ++x)
            {
                var blockX = new JArray();
                for (int y = 0; y < data[i].mapBlocks[x].Length; ++y)
                {
                    var blockY = new JObject();
                    blockY["eventId"] = new JNumber(data[i].mapBlocks[x][y].eventId);
                    blockY["thing"] = new JNumber(data[i].mapBlocks[x][y].thing);
                    blockX.Add(blockY);
                }
                blocks.Add(blockX);
            }
            ob["mapBlocks"] = blocks;
            ret.Add(ob);
        }
        return ret;
    }

    private static Constant.MapData[] GetMapDataOfJson(JArray json)
    {
        var ret = new Constant.MapData[json.Length];
        for (var i = 0; i < json.Length; ++i)
        {
            var __oneMap = json[i] as JObject;
            ret[i].mapId = __oneMap["mapId"].ToInt();
            ret[i].mapName = __oneMap["mapName"].ToString();
            ret[i].backThing = __oneMap["backThing"].ToInt();
            ret[i].music = __oneMap["music"].ToInt();
            var __mapBlocks = __oneMap["mapBlocks"] as JArray;
            ret[i].mapBlocks = new Constant.MapBlock[__mapBlocks.Length][];
            for (var x = 0; x < __mapBlocks.Length; ++x)
            {
                var ___mapBlocksX = __mapBlocks[x] as JArray;
                ret[i].mapBlocks[x] = new Constant.MapBlock[___mapBlocksX.Length];
                for (var y = 0; y < ___mapBlocksX.Length; ++y)
                {
                    var ____mapBlockY = ___mapBlocksX[y] as JObject;
                    ret[i].mapBlocks[x][y].eventId = ____mapBlockY["eventId"].ToInt();
                    ret[i].mapBlocks[x][y].thing = ____mapBlockY["thing"].ToInt();
                }
            }
        }
        return ret;
    }

    private static JObject GetJsonOfPlayerData(Constant.PlayerData data)
    {
        var player = new JObject();
        player["id"] = new JNumber(data.id);
        player["level"] = new JNumber(data.level);
        player["exp"] = new JNumber(data.exp);
        player["life"] = new JNumber(data.life);
        player["attack"] = new JNumber(data.attack);
        player["defense"] = new JNumber(data.defense);
        player["speed"] = new JNumber(data.speed);
        player["critical"] = new JNumber(data.critical);
        player["gold"] = new JNumber(data.gold);
        player["yellowKey"] = new JNumber(data.yellowKey);
        player["blueKey"] = new JNumber(data.blueKey);
        player["redKey"] = new JNumber(data.redKey);
        player["greenKey"] = new JNumber(data.greenKey);
        player["weaponId"] = new JNumber(data.weaponId);
        return player;
    }

    private static Constant.PlayerData GetPlayerDataOfJson(JObject json)
    {
        return new Constant.PlayerData()
        {
            id = json["id"].ToInt(),
            level = json["level"].ToInt(),
            exp = json["exp"].ToInt(),
            life = json["life"].ToInt(),
            attack = json["attack"].ToInt(),
            defense = json["defense"].ToInt(),
            speed = json["speed"].ToInt(),
            critical = json["critical"].ToFloat(),
            gold = json["gold"].ToInt(),
            yellowKey = json["yellowKey"].ToInt(),
            blueKey = json["blueKey"].ToInt(),
            redKey = json["redKey"].ToInt(),
            greenKey = json["greenKey"].ToInt(),
            weaponId = json["weaponId"].ToInt()
        };
    }
}
