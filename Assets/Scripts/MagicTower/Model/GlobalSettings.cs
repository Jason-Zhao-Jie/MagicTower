using System.Collections;
using ArmyAnt.Manager;

namespace MagicTower.Model {

    public enum PopAdsFreq : sbyte {
        High,
        Midium,
        Low,
        OnlyOnce,
        None,
    }

    [System.Serializable]
    public struct Setting{
        public float musicVolume;
        public float soundVolume;
        public bool virtualJoysticksOn;
        public bool autoFindBestRoad;
        public sbyte popAdsFreq;
        public bool rewardAdsOn;
        public bool analyticsOn;

        public Setting(PopAdsFreq popAdsFreq = PopAdsFreq.Low) {
            musicVolume = 100;
            soundVolume = 100;
            virtualJoysticksOn = true;
            autoFindBestRoad = false;
            this.popAdsFreq = (sbyte)popAdsFreq;
            rewardAdsOn = true;
            analyticsOn = true;
        }
    }

    public class GlobalSettings {
        private const string SETTING_FILE_PATH = "GlobalSettings.json";
        
        public void Load() {
            if (IOManager.ExistFile(SETTING_FILE_PATH)) {
                var bytes = IOManager.LoadFromFile(SETTING_FILE_PATH);
                var json = System.Text.Encoding.UTF8.GetString(bytes);
                Settings = UnityEngine.JsonUtility.FromJson<Setting>(json);
            } else {
                Settings = new Setting(PopAdsFreq.Low);
                Save();
            }
        }

        public void Save() {
            var json = UnityEngine.JsonUtility.ToJson(Settings);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            IOManager.SaveToFile(bytes, SETTING_FILE_PATH);
        }

        public Setting Settings {
            get; set;
        }

    }

}
