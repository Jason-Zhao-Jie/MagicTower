using System.Collections.Generic;
using UnityEngine;
using MagicTower.Model;

namespace MagicTower.Present.Manager {

    public static class SaveManager {
        private struct SaveInnerData {
            public int captureWidth;
            public int captureHeight;
        }

        public static bool Write(string saveName, MapData[] maps, RuntimeGameData data, Texture2D capture) {
            if(!ArmyAnt.Manager.IOManager.MkdirIfNotExist("save", saveName) || !ArmyAnt.Manager.IOManager.MkdirIfNotExist("save", saveName, "MapData")) {
                return false;
            }
            var json = JsonUtility.ToJson(data, false);

            try {
                ArmyAnt.Manager.IOManager.SaveToFile(System.Text.Encoding.UTF8.GetBytes(json), "save", saveName, "RuntimeData.json");
                foreach(var i in maps) {
                    var mapJson = JsonUtility.ToJson(i, false);                    
                    ArmyAnt.Manager.IOManager.SaveToFile(System.Text.Encoding.UTF8.GetBytes(mapJson), "save", saveName, "MapData", i.mapId.ToString() + ".json");
                }
                ArmyAnt.Manager.IOManager.SaveToFile(capture.EncodeToPNG(), "save", saveName, "ScreenCapture.png");
                ArmyAnt.Manager.IOManager.SaveToFile(System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(new SaveInnerData() {
                    captureWidth = capture.width,
                    captureHeight = capture.height,
                }, false)), "save", saveName, "InnerData.json");
            } catch(System.IO.IOException) {
                return false;
            }
            return true;
        }

        public static (MapData[] maps, RuntimeGameData data) Read(string saveName = null, bool wholepath = false) {
            MapData[] maps = null;
            RuntimeGameData data;
            if(saveName == "") {
                var json = Resources.Load<TextAsset>("RuntimeData").text;
                data = JsonUtility.FromJson<RuntimeGameData>(json);
                data.player = Game.Config.players[Game.Config.newGamePlayerId];
            } else {
                var bin = wholepath ? ArmyAnt.Manager.IOManager.LoadFromFileWholePath(saveName + System.IO.Path.AltDirectorySeparatorChar + "RuntimeData.json") : ArmyAnt.Manager.IOManager.LoadFromFile("save", saveName, "RuntimeData.json");
                var json = System.Text.Encoding.UTF8.GetString(bin);
                data = JsonUtility.FromJson<RuntimeGameData>(json);
                var mapFiles = ArmyAnt.Manager.IOManager.ListAllFiles("*.json", "save", saveName, "MapData");
                maps = new MapData[mapFiles.Length];
                int index = 0;
                foreach(var i in mapFiles) {
                    var mapBin = ArmyAnt.Manager.IOManager.LoadFromFileWholePath(i);
                    var mapStr = System.Text.Encoding.UTF8.GetString(mapBin);
                    var mapData = JsonUtility.FromJson<MapData>(mapStr);
                    maps[index++] = mapData;
                }
            }
            return (maps, data);
        }

        public static bool Remove(string saveName) {
            return ArmyAnt.Manager.IOManager.RemoveFolder("save", saveName);
        }

        public static Texture2D GetSaveCapture(string saveName) {
            if(saveName == null) {
                return null;
            }
            var jsonInfo = ArmyAnt.Manager.IOManager.LoadFromFile("save", saveName, "InnerData.json");
            if(jsonInfo == null) {
                return null;
            }
            var json = JsonUtility.FromJson<SaveInnerData>(System.Text.Encoding.UTF8.GetString(jsonInfo));            
            var bytes = ArmyAnt.Manager.IOManager.LoadFromFile("save", saveName, "ScreenCapture.png");
            var ret = new Texture2D(json.captureWidth, json.captureHeight);
            ret.LoadImage(bytes);
            return ret;
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
