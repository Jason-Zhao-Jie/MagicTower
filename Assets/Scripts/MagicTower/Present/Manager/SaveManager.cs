using System.Collections.Generic;

namespace MagicTower.Present.Manager {

    public static class SaveManager {
        [System.Serializable]
        private struct RuntimePositionData {
            public int mapId;
            public int x;
            public int y;
        }

        [System.Serializable]
        private struct RuntimeNumberData {
            public int id;
            public long value;
        }

        [System.Serializable]
        private class RuntimeGameData {
            public Model.PlayerData player;
            public RuntimePositionData pos;
            public RuntimeNumberData[] numbers;
        }

        public static async System.Threading.Tasks.Task<bool> Write(string saveName, Model.MapData[] maps, IDictionary<int, long> numberData, int currentMapId, int playerPosX, int playerPosY, Model.PlayerData playerData) {
            if (!ArmyAnt.Manager.IOManager.MkdirIfNotExist("save", saveName) || !ArmyAnt.Manager.IOManager.MkdirIfNotExist("save", saveName, "MapData")) {
                return false;
            }
            var numbers = new RuntimeNumberData[numberData.Count];
            int index_numbers = 0;
            foreach (var i in numberData) {
                numbers[index_numbers++] = new RuntimeNumberData {
                    id = i.Key,
                    value = i.Value,
                };
            }
            var json = UnityEngine.JsonUtility.ToJson(new RuntimeGameData {
                player = playerData,
                pos = new RuntimePositionData {
                    mapId = currentMapId,
                    x = playerPosX,
                    y = playerPosY,
                },
                numbers = numbers,
            }, false);
            try {
                await ArmyAnt.Manager.IOManager.SaveToFile(System.Text.Encoding.UTF8.GetBytes(json), "save", saveName, "RuntimeData.json");
                foreach (var i in maps) {
                    await ArmyAnt.Manager.IOManager.SaveToFile(System.Text.Encoding.UTF8.GetBytes(json), "save", saveName, "MapData", i.mapId.ToString() + ".json");
                }
            } catch (System.IO.IOException) {
                return false;
            }
            return true;
        }

        public static async System.Threading.Tasks.Task<(Model.MapData[] maps, Dictionary<int, long> numberData, int currentMapId, int playerPosX, int playerPosY, Model.PlayerData playerData)> Read(string saveName = null) {
            RuntimeGameData data = null;
            Model.MapData[] maps = null;
            if (saveName == "") {
                var json = UnityEngine.Resources.Load<UnityEngine.TextAsset>("RuntimeData").text;
                data = UnityEngine.JsonUtility.FromJson<RuntimeGameData>(json);
                data.player = Game.Config.players[62];   // TODO : 这里是新游戏载入的player，由于需要进入MainScene之前作选择，因此这里的逻辑需要进一步拓展，目前先写死
            } else {
                var bin = await ArmyAnt.Manager.IOManager.LoadFromFile("save", saveName, "RuntimeData.json");
                var json = System.Text.Encoding.UTF8.GetString(bin);
                data = UnityEngine.JsonUtility.FromJson<RuntimeGameData>(json);
                var mapFiles = ArmyAnt.Manager.IOManager.ListAllFiles("*.json", saveName, "MapData");
                maps = new Model.MapData[mapFiles.Length];
                int index = 0;
                foreach (var i in mapFiles) {
                    var mapBin = await ArmyAnt.Manager.IOManager.LoadFromFileWholePath(i);
                    var mapStr = System.Text.Encoding.UTF8.GetString(mapBin);
                    var mapData = UnityEngine.JsonUtility.FromJson<Model.MapData>(mapStr);
                    maps[index++] = mapData;
                }
            }
            var numberData = new Dictionary<int, long>();
            foreach (var i in data.numbers) {
                numberData.Add(i.id, i.value);
            }
            return (maps, numberData, data.pos.mapId, data.pos.x, data.pos.y, data.player);
        }

        public static async System.Threading.Tasks.Task<Dictionary<string, Model.PlayerData>> ListAll() {
            var ret = new Dictionary<string, Model.PlayerData>();
            var names = ArmyAnt.Manager.IOManager.ListAllDirectories("save");
            foreach(var i in names) {
                ret.Add(i, (await Read(i)).playerData);
            }
            return ret;
        }

    }

}
