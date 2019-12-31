using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MagicTower.Model {

    public class StringInternational {
        private const int default_language_id = 2;
        private const string language_pref_key = "language";

        public StringInternational(Dictionary<int, LanguageData> languages) {
            this.languages = languages;
            languageId = default_language_id;
            if (PlayerPrefs.HasKey(language_pref_key)) {
                var id = PlayerPrefs.GetInt(language_pref_key);
                if(languages.ContainsKey(id)) {
                    languageId = id;
                } else {
                    PlayerPrefs.SetInt(language_pref_key, default_language_id);
                }
            } else {
                PlayerPrefs.SetInt(language_pref_key, default_language_id);
            }
            ReloadLanguage(languageId);
        }

        public string GetValue(string key, params string[] values) {
            if (key == null || !languageTable.ContainsKey(key)) {
                Game.DebugLogWarning("Undefined string key \"", key, "\" in any language");
                return key;
            }
            var str = languageTable[key];
            if (str == null) {
                str = key;
                Game.DebugLogWarning("Undefined string key \"", key, "\" in language ", Language);
            }
            if (values != null && values.Length > 0) {
                var index = 0;
                var lastLen = 0;
                for (var i = 0; i < values.Length; ++i) {
                    index = str.IndexOf("##", index + lastLen, System.StringComparison.InvariantCulture);
                    if (index < 0)
                        break;
                    var tarStr = values[i];
                    if (tarStr == null)
                        tarStr = "";
                    str = str.Remove(index, 2).Insert(index, tarStr);
                    lastLen = tarStr.Length;
                }
            }
            return str;
        }

        public Dictionary<string, Dictionary<int, string>> GetAllValues(bool isSavedData)
        {
            var ret = new Dictionary<string, Dictionary<int, string>>();
            foreach (var i in languages)
            {
                Strings data;
                if (isSavedData)
                {
                    var bytes = ArmyAnt.Manager.IOManager.LoadFromFile("Strings", i.Value.path + ".json");
                    if (bytes == null)
                    {
                        var text = Resources.Load<TextAsset>(Dirs.STRING_DATA_DIR + i.Value.path);
                        data = JsonUtility.FromJson<Strings>(text.text);
                        Resources.UnloadAsset(text);
                    }
                    else
                    {
                        data = JsonUtility.FromJson<Strings>(System.Text.Encoding.UTF8.GetString(bytes));
                    }
                }
                else
                {
                    var text = Resources.Load<TextAsset>(Dirs.STRING_DATA_DIR + i.Value.path);
                    data = JsonUtility.FromJson<Strings>(text.text);
                    Resources.UnloadAsset(text);
                }
                foreach (var k in data.strings)
                {
                    if (!string.IsNullOrEmpty(k.key))
                    {
                        if (!ret.ContainsKey(k.key))
                        {
                            ret.Add(k.key, new Dictionary<int, string>());
                        }
                        ret[k.key].Add(i.Value.id, k.content);
                    }
                }
            }
            return ret;
        }

        public string Language {
            get {
                return languages[languageId].key;
            }
        }

        public int LanguageId {
            get { return languageId; }
            set {
                if(languageId != value) {
                    ReloadLanguage(value);
                }
                languageId = value;
                PlayerPrefs.SetInt(language_pref_key, value);
            }
        }

        /// <summary>
        /// 将此项置为 true, 则从缓存文件夹而不是游戏原配置中读取字符串数据, 用于编辑器
        /// </summary>
        public bool UsingSaveData {
            get => UsingSaveData;
            set {
                usingSaveData = value;
                ReloadLanguage(LanguageId);
            }
        }
        private bool usingSaveData = false;

        private void ReloadLanguage(int languageId) {
            languageTable.Clear();
            Strings ret;
            if(usingSaveData) {
                var bytes = ArmyAnt.Manager.IOManager.LoadFromFile("Strings", languages[languageId].path + ".json");
                ret = JsonUtility.FromJson<Strings>(System.Text.Encoding.UTF8.GetString(bytes));
            } else {
                var text = Resources.Load<TextAsset>(Dirs.STRING_DATA_DIR + languages[languageId].path);
                ret = JsonUtility.FromJson<Strings>(text.text);
                Resources.UnloadAsset(text);
            }
            foreach(var i in ret.strings) {
                if(!string.IsNullOrEmpty(i.key)) {
                    languageTable[i.key] = i.content;
                }
            }
        }

        private int languageId;
        private Dictionary<int, LanguageData> languages;
        private Dictionary<string, string> languageTable = new Dictionary<string, string>();

        [System.Serializable]
        public struct Strings {
            public InternationalString[] strings;
        };
    }

}
