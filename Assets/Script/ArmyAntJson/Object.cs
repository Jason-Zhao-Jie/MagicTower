using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class JObject : IUnit, IJsonCollection, IDictionary<string, IUnit>
{
    public static IUnit Create(string text)
    {
        try
        {
            return new JObject()
            {
                String = text
            };
        }
        catch (ArmyAntJson.JException)
        {
            return null;
        }
    }

    public JObject(Dictionary<string, IUnit> v = null)
    {
        if (v != null)
        {
            keys = v.Keys.ToList();
            values = v.Values.ToList();
        }

    }
    public virtual string String
    {
        get
        {
            var ret = "{\n";
            for (var i = 0; i < keys.Count - 1; i++)
            {
                ret += "  \"" + keys[i] + '"' + " : " + values[i].String + ",\n";
            }
            if (keys.Count > 0)
                ret += "  \"" + keys[keys.Count - 1] + '"' + " : " + values[keys.Count - 1].String + "\n}";
            else
                ret += "\n}";
            return ret;
        }
        set
        {
            var realValue = value.Trim().Trim(new char[] { '\r', '\n' });
            if (realValue.Length<=0 || realValue[realValue.Length - 1] != '\0')
                realValue += '\0';
            if (realValue[0] != '{' || realValue[realValue.Length - 2] != '}')
            {
                throw new ArmyAntJson.JException();
            }
            realValue = realValue.Remove(realValue.Length - 2).Remove(0, 1);
            realValue = realValue.Trim().Trim(new char[] { '\r', '\n' });
            keys = new List<string>();
            values = new List<IUnit>();
            if (realValue != "")
            {
                var res = ArmyAntJson.CutByComma(realValue);
                for (int i = 0; i < res.Length; i++)
                {
                    var ins = CutKeyValue(res[i]);
                    keys.Add(ins.Key);
                    values.Add(ArmyAntJson.Create(ins.Value));
                }
            }
        }
    }
    public virtual ArmyAntJson.EType Type
    {
        get
        {
            return ArmyAntJson.EType.Object;
        }
    }

    public virtual ICollection<string> Keys
    {
        get
        {
            return keys;
        }
    }

    public virtual ICollection<IUnit> Values
    {
        get
        {
            return values;
        }
    }

    public virtual int Count
    {
        get
        {
            return keys.Count;
        }
    }

    public virtual bool IsReadOnly
    {
        get
        {
            return false;
        }
    }

    public virtual int Length
    {
        get
        {
            return keys.Count;
        }
    }

    public virtual object SyncRoot
    {
        get
        {
            return keys;
        }
    }

    public virtual bool IsSynchronized
    {
        get
        {
            return false;
        }
    }

    public virtual IUnit this[string key]
    {
        get
        {
            return GetChild(key);
        }

        set
        {
            for (var i = 0; i < Count; ++i)
            {
                if (keys[i].Equals(key))
                {
                    values[i] = value;
                    return;
                }
            }
            AddChild(key, value);
        }
    }

    public virtual bool AddChild(string key, IUnit value)
    {
        if (keys.Contains(key))
            return false;
        keys.Add(key);
        values.Add(value);
        return true;
    }

    public virtual bool RemoveChild(string key)
    {
        for (var i = 0; i < Count; ++i)
        {
            if (keys[i].Equals(key))
            {
                keys.RemoveAt(i);
                values.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    public virtual IUnit GetChild(string key)
	{
		for (var i = 0; i < Count; ++i)
		{
			if (keys[i].Equals(key))
			{
				return values[i];
			}
		}
		return null;
    }
    private static KeyValuePair<string, string> CutKeyValue(string str)
    {
        str = str.Trim().Trim(new char[] { '\r', '\n' });
        char isSingleKey = str[0];
        if (str[0] != '"' && str[0] != '\'')
            throw new Exception();
        var key = "";
        var count = 1;
        while (count < str.Length)
        {
            if (str[count] == isSingleKey)
                break;
            else
                key += str[count++];
        }
        str = str.Remove(0, count + 1).Trim().Trim(new char[] { '\r', '\n' });
        if (str[0] != ':')
            throw new Exception();
        return new KeyValuePair<string, string>(key, str.Remove(0, 1).Trim().Trim(new char[] { '\r', '\n' }));
    }

    public virtual bool ContainsKey(string key)
	{
		for (var i = 0; i < Count; ++i)
		{
			if (keys[i].Equals(key))
			{
				return true;
			}
		}
		return false;
    }

    public virtual void Add(string key, IUnit value)
    {
        AddChild(key, value);
    }

    public virtual bool Remove(string key)
    {
        return RemoveChild(key);
    }

    public virtual bool TryGetValue(string key, out IUnit value)
    {
        value = GetChild(key);
        return value != null;
    }

    public virtual void Add(KeyValuePair<string, IUnit> item)
    {
        AddChild(item.Key, item.Value);
    }

    public virtual void Clear()
    {
        keys.Clear();
        values.Clear();
    }

    public virtual bool Contains(KeyValuePair<string, IUnit> item)
    {
        return ContainsKey(item.Key) && this[item.Key] == item.Value;
    }

    public virtual void CopyTo(KeyValuePair<string, IUnit>[] array, int arrayIndex)
    {
        for (var i = 0; i < values.Count; i++)
        {
            array[arrayIndex++] = new KeyValuePair<string, IUnit>(keys[i], values[i]);
        }
    }

    public virtual bool Remove(KeyValuePair<string, IUnit> item)
    {
        return RemoveChild(item.Key);
    }

    public virtual IEnumerator<KeyValuePair<string, IUnit>> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public virtual void CopyTo(Array array, int index)
    {
        CopyTo((KeyValuePair<string, IUnit>[])array, index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return keys.GetEnumerator();
    }

    public virtual bool ToBool()
    {
        return Count > 0;
    }

    public virtual int ToInt()
    {
        return 0;
    }

    public virtual long ToLong()
    {
        return 0;
    }

    public virtual double ToFloat()
    {
        return 0.0;
    }

    public virtual JObject ToObject()
    {
        return this;
    }

    public virtual JArray ToArray()
    {
        return null;
    }

    public override string ToString()
    {
        return null;
    }

    IEnumerator<IUnit> IEnumerable<IUnit>.GetEnumerator()
    {
        return values.GetEnumerator();
    }

    public bool AddChild(IUnit child, string tag)
    {
        return AddChild(tag, child);
    }

    protected List<string> keys = new List<string>();
    protected List<IUnit> values = new List<IUnit>();
}
