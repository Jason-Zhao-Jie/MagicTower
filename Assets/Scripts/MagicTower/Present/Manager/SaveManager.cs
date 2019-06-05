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
            public long lastTime = System.DateTime.Now.ToFileTime();
            public long totalTime = 0;
        }

        public static bool Write(string saveName, System.DateTime lastTime, long totalTime, Model.MapData[] maps, IDictionary<int, long> numberData, int currentMapId, int playerPosX, int playerPosY, Model.PlayerData playerData) {
            if(!ArmyAnt.Manager.IOManager.MkdirIfNotExist("save", saveName) || !ArmyAnt.Manager.IOManager.MkdirIfNotExist("save", saveName, "MapData")) {
                return false;
            }
            var numbers = new RuntimeNumberData[numberData.Count];
            int index_numbers = 0;
            foreach(var i in numberData) {
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
                lastTime = lastTime.ToFileTime(),
                totalTime = totalTime,
            }, false);
            ;
            try {
                ArmyAnt.Manager.IOManager.SaveToFile(System.Text.Encoding.UTF8.GetBytes(json), "save", saveName, "RuntimeData.json");
                foreach(var i in maps) {
                    ArmyAnt.Manager.IOManager.SaveToFile(System.Text.Encoding.UTF8.GetBytes(json), "save", saveName, "MapData", i.mapId.ToString() + ".json");
                }
            } catch(System.IO.IOException) {
                return false;
            }
            return true;
        }

        public static (System.DateTime lastTime, long totalTime, Model.MapData[] maps, Dictionary<int, long> numberData, int currentMapId, int playerPosX, int playerPosY, Model.PlayerData playerData) Read(string saveName = null) {
            Model.MapData[] maps = null;
            RuntimeGameData data;
            if(saveName == "") {
                var json = UnityEngine.Resources.Load<UnityEngine.TextAsset>("RuntimeData").text;
                data = UnityEngine.JsonUtility.FromJson<RuntimeGameData>(json);
                data.player = Game.Config.players[Game.Config.newGamePlayerId];
            } else {
                var bin = ArmyAnt.Manager.IOManager.LoadFromFile("save", saveName, "RuntimeData.json");
                var json = System.Text.Encoding.UTF8.GetString(bin);
                data = UnityEngine.JsonUtility.FromJson<RuntimeGameData>(json);
                var mapFiles = ArmyAnt.Manager.IOManager.ListAllFiles("*.json", saveName, "MapData");
                maps = new Model.MapData[mapFiles.Length];
                int index = 0;
                foreach(var i in mapFiles) {
                    var mapBin = ArmyAnt.Manager.IOManager.LoadFromFileWholePath(i);
                    var mapStr = System.Text.Encoding.UTF8.GetString(mapBin);
                    var mapData = UnityEngine.JsonUtility.FromJson<Model.MapData>(mapStr);
                    maps[index++] = mapData;
                }
            }
            var numberData = new Dictionary<int, long>();
            foreach(var i in data.numbers) {
                numberData.Add(i.id, i.value);
            }
            return (System.DateTime.FromFileTime(data.lastTime), data.totalTime, maps, numberData, data.pos.mapId, data.pos.x, data.pos.y, data.player);
        }

        public static Dictionary<string, (System.DateTime lastTime, long totalTime, Model.MapData[] maps, Dictionary<int, long> numberData, int currentMapId, int playerPosX, int playerPosY, Model.PlayerData playerData)> ListAll() {
            var ret = new Dictionary<string, (System.DateTime lastTime, long totalTime, Model.MapData[] maps, Dictionary<int, long> numberData, int currentMapId, int playerPosX, int playerPosY, Model.PlayerData playerData)>();
            var names = ArmyAnt.Manager.IOManager.ListAllDirectories("save");
            foreach(var i in names) {
                ret.Add(i, Read(i));
            }
            return ret;
        }

    }

}
