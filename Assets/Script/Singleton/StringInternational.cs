using UnityEngine;
using System.Collections;

public static class StringInternational
{
	public static string GetValue(int id)
	{
        var index = getIndexById(id);
        if (index < 0)
			return id.ToString();
		var ret = DataCenter.instance.data.strings[index][languageKey];
		if (ret == null)
		{
			ret = DataCenter.instance.data.strings[index]["en-us"];
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
        var ret = DataCenter.instance.data.strings[index][languageKey];
        if (ret == null)
        {
            ret = DataCenter.instance.data.strings[index]["en-us"];
            if (ret == null)
                ret = key;
        }
		return ret;
	}

    public static bool SetLanguageById(int id){
        if (id <= 0)
            return false;
        var ret = DataCenter.instance.GetLanguageById(id);
        if (ret == null)
            return false;
        languageKey = ret;
        return true;
    }

    public static string Language
    {
        get { return languageKey; }
        set { languageKey = value; }
    }

	private static int getIndexById(int id)
	{
		for (int i = 0; i < DataCenter.instance.data.strings.Length; ++i)
		{
			if (id == DataCenter.instance.data.strings[i].id)
				return i;
		}
		return -1;
	}

    private static int getIndexByKey(string key)
    {
        for (int i = 0; i < DataCenter.instance.data.strings.Length; ++i)
        {
            if (key.Equals(DataCenter.instance.data.strings[i].key))
                return i;
        }
        return -1;
    }

    private static string languageKey = "en-us";
}
