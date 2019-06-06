using System.Collections.Generic;
using MagicTower.Model;

namespace MagicTower.Present.Manager {

    public static class SaveManager {

        public static bool Write(string saveName, MapData[] maps, RuntimeGameData data) {
            if(!ArmyAnt.Manager.IOManager.MkdirIfNotExist("save", saveName) || !ArmyAnt.Manager.IOManager.MkdirIfNotExist("save", saveName, "MapData")) {
                return false;
            }
            var json = UnityEngine.JsonUtility.ToJson(data, false);

            try {
                ArmyAnt.Manager.IOManager.SaveToFile(System.Text.Encoding.UTF8.GetBytes(json), "save", saveName, "RuntimeData.json");
                foreach(var i in maps) {
                    var mapJson = UnityEngine.JsonUtility.ToJson(i, false);                    
                    ArmyAnt.Manager.IOManager.SaveToFile(System.Text.Encoding.UTF8.GetBytes(mapJson), "save", saveName, "MapData", i.mapId.ToString() + ".json");
                }
            } catch(System.IO.IOException) {
                return false;
            }
            return true;
        }

        public static (MapData[] maps, RuntimeGameData data) Read(string saveName = null, bool wholepath = false) {
            MapData[] maps = null;
            RuntimeGameData data;
            if(saveName == "") {
                var json = UnityEngine.Resources.Load<UnityEngine.TextAsset>("RuntimeData").text;
                data = UnityEngine.JsonUtility.FromJson<RuntimeGameData>(json);
                data.player = Game.Config.players[Game.Config.newGamePlayerId];
            } else {
                var bin = wholepath ? ArmyAnt.Manager.IOManager.LoadFromFileWholePath(saveName + System.IO.Path.AltDirectorySeparatorChar + "RuntimeData.json") : ArmyAnt.Manager.IOManager.LoadFromFile("save", saveName, "RuntimeData.json");
                var json = System.Text.Encoding.UTF8.GetString(bin);
                data = UnityEngine.JsonUtility.FromJson<RuntimeGameData>(json);
                var mapFiles = ArmyAnt.Manager.IOManager.ListAllFiles("*.json", "save", saveName, "MapData");
                maps = new MapData[mapFiles.Length];
                int index = 0;
                foreach(var i in mapFiles) {
                    var mapBin = ArmyAnt.Manager.IOManager.LoadFromFileWholePath(i);
                    var mapStr = System.Text.Encoding.UTF8.GetString(mapBin);
                    var mapData = UnityEngine.JsonUtility.FromJson<MapData>(mapStr);
                    maps[index++] = mapData;
                }
            }
            return (maps, data);
        }

        public static Dictionary<string, (MapData[] maps, RuntimeGameData data)> ListAll() {
            var ret = new Dictionary<string, (MapData[] maps, RuntimeGameData data)>();
            var names = ArmyAnt.Manager.IOManager.ListAllDirectories("save");
            if(names != null) {
                foreach(var i in names) {
                    var name = i.Remove(0, UnityEngine.Application.persistentDataPath.Length + 6);
                    ret.Add(name, Read(name));
                }
            }
            return ret;
        }

    }

}
