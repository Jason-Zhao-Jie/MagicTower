using UnityEngine;
using System.Collections;

namespace MagicTower.Model {

    public class StringInternational {
        private const int default_language_id = 2;
        private const string language_pref_key = "language";

        public StringInternational() {
            var id = default_language_id;
            if (PlayerPrefs.HasKey(language_pref_key)) {
                id = PlayerPrefs.GetInt(language_pref_key);
                languageId = id;
            }
        }

        public string GetValue(string key, params string[] values) {
            if (key == null || !Game.Config.strings.ContainsKey(key)) {
                Debug.LogWarning("Undefined string key \"" + key + "\" in any language");
                return key;
            }
            var str = Game.Config.strings[key];
            var ret = str[Language];
            if (ret == null) {
                ret = str["en-us"];
                Debug.LogWarning("Undefined string key \"" + key + "\" in language " + Language);
                if (ret == null) {
                    ret = key;
                    Debug.LogWarning("Undefined string key \"" + key + "\" in language en-us");
                }
            }
            if (values != null && values.Length > 0) {
                var index = 0;
                var lastLen = 0;
                for (var i = 0; i < values.Length; ++i) {
                    index = ret.IndexOf("##", index + lastLen, System.StringComparison.InvariantCulture);
                    if (index < 0)
                        break;
                    var tarStr = values[i];
                    if (tarStr == null)
                        tarStr = "";
                    ret = ret.Remove(index, 2).Insert(index, tarStr);
                    lastLen = tarStr.Length;
                }
            }
            return ret;
        }

        public string Language {
            get {
                return Game.Config.languages[LanguageId].key;
            }
        }

        public int LanguageId {
            get { return languageId; }
            set {
                languageId = value;
                PlayerPrefs.SetInt(language_pref_key, value);
            }
        }

        private int languageId;
    }

}
