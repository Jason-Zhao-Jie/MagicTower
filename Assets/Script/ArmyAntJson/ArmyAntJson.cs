using System.Collections.Generic;

public static class ArmyAntJson
{

    public enum EType : byte
    {
        Undefined,
        Null,
        Boolean,
        Numeric,
        String,
        Array,
        Object
    }
    
    public class JException : System.Exception
    {
        public JException()
        {
        }
        public JException(string message) : base(message)
        {
        }

    }

    public static IUnit Create(string value)
    {
        IUnit ret = null;
        ret = Undefined.isThis(value);
        if (ret != null)
            return ret;
        ret = JNull.isThis(value);
        if (ret != null)
            return ret;
        ret = JBoolean.isThis(value);
        if (ret != null)
            return ret;
        ret = JNumber.isThis(value);
        if (ret != null)
            return ret;
        ret = JString.isThis(value);
        if (ret != null)
            return ret;
        ret = JObject.isThis(value);
        if (ret != null)
            return ret;
        ret = JArray.isThis(value);
        return ret;
    }

    internal static string[] CutByComma(string value)
    {
        value = value.Trim().Trim(new char[] { '\r', '\n' });
        var ret = new List<string>();
        var tmp = "";
        bool isInSingleString = false;
        bool isInDoubleString = false;
        int deepInArray = 0;
        int deepInObject = 0;
        for (int i = 0; i < value.Length; i++)
        {
            if (value[i] == '\'' && !isInDoubleString)
            {
                isInSingleString = !(isInSingleString && (i == 0 || value[i - 1] != '\\'));
            }
            else if (value[i] == '"' && !isInSingleString)
            {
                isInDoubleString = !(isInDoubleString && (i == 0 || value[i - 1] != '\\'));
            }
            else if (value[i] == '[' && !isInSingleString && !isInDoubleString)
                deepInArray++;
            else if (value[i] == '{' && !isInSingleString && !isInDoubleString)
                deepInObject++;
            else if (value[i] == ']' && !isInSingleString && !isInDoubleString)
                deepInArray--;
            else if (value[i] == '}' && !isInSingleString && !isInDoubleString)
                deepInObject--;
            if (deepInArray < 0 || deepInObject < 0)
                throw new JException();
            if (deepInArray == 0 && deepInObject == 0 && !isInSingleString && !isInDoubleString && value[i] == ',')
            {
                ret.Add(tmp);
                tmp = "";
                value = value.Remove(0, i + 1).Trim().Trim(new char[] { '\r', '\n' });
                i = -1;
            }
            else
                tmp += value[i];
        }
        ret.Add(tmp);
        return ret.ToArray();
    }
}



