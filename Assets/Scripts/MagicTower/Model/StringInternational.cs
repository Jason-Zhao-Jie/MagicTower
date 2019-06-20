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

        private void ReloadLanguage(int languageId) {
            languageTable.Clear();
            var text = Resources.Load<TextAsset>(Dirs.STRING_DATA_DIR + languages[languageId].path);
            var ret = JsonUtility.FromJson<Strings>(text.text);
            foreach(var i in ret.strings) {
                if(!string.IsNullOrEmpty(i.key)) {
                    languageTable[i.key] = i.content;
                }
            }
            Resources.UnloadAsset(text);
        }

        private int languageId;
        private Dictionary<int, LanguageData> languages;
        private Dictionary<string, string> languageTable = new Dictionary<string, string>();

        [System.Serializable]
        private struct Strings {
            public InternationalString[] strings;
        };
    }

}
