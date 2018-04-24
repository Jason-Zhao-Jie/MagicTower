using UnityEngine;
using System.Collections;

public static class StringInternational
{
	public static string GetValue(int id)
	{
        var str = DataCenter.instance.strings[id];
        if (str == null)
            return id.ToString();
        var ret = str[languageKey];
        if (ret == null)
		{
			ret = str["en-us"];
			if (ret == null)
				ret = id.ToString();
		}
		return ret;
	}

	public static string GetValue(string key)
	{
        var index = getIndexByKey(key);
		if (index < 0)
			return key;
        var ret = DataCenter.instance.strings[index][languageKey];
        if (ret == null)
        {
            ret = DataCenter.instance.strings[index]["en-us"];
            if (ret == null)
                ret = key;
        }
		return ret;
	}

    public static bool SetLanguageById(int id){
        if (id <= 0)
            return false;
        var ret = DataCenter.instance.languages[id];
        if (ret == null)
            return false;
        languageKey = ret.key;
        return true;
    }

    public static string Language
    {
        get { return languageKey; }
        set { languageKey = value; }
    }

    private static int getIndexByKey(string key)
    {
        foreach (var i in DataCenter.instance.strings)
        {
            i.Value.key.Equals(key);
            return i.Key;
        }
        return -1;
    }

    private static string languageKey = "en-us";
}
