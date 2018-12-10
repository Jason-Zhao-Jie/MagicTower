using UnityEngine;
using System.Collections;

public static class StringInternational {
    public static string GetValue(string key, params string[] values) {
        if (!DataCenter.instance.strings.ContainsKey(key))
            return key;
        var str = DataCenter.instance.strings[key];
        var ret = str[Language];
        if (ret == null) {
            ret = str["en-us"];
            if (ret == null)
                ret = key;
        }
        if (values != null && values.Length > 0) {
            var index = 0;
            var lastLen = 0;
            for (var i = 0; i < values.Length; ++i) {
                index = ret.IndexOf("##", index + lastLen);
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

    public static bool SetLanguageById(int id) {
        if (id <= 0)
            return false;
        var ret = DataCenter.instance.languages[id];
        if (ret == null)
            return false;
        Language = ret.key;
        return true;
    }

    public static string Language {
        get; set;
    }

    static StringInternational() {
        Language = "en-us";
    }
}
