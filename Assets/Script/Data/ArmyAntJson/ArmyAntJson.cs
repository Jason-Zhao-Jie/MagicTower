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
        ret = Undefined.Create(value);
        if (ret != null)
            return ret;
        ret = JNull.Create(value);
        if (ret != null)
            return ret;
        ret = JBoolean.Create(value);
        if (ret != null)
            return ret;
        ret = JNumber.Create(value);
        if (ret != null)
            return ret;
        ret = JString.Create(value);
        if (ret != null)
            return ret;
        ret = JObject.Create(value);
        if (ret != null)
            return ret;
        ret = JArray.Create(value);
        return ret;
    }

    internal static string[] CutByComma(string value)
    {
        value = value.Trim().Trim('\r', '\n');
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
                value = value.Remove(0, i + 1).Trim().Trim('\r', '\n');
                i = -1;
            }
            else
                tmp += value[i];
        }
        ret.Add(tmp);
        return ret.ToArray();
    }
}



