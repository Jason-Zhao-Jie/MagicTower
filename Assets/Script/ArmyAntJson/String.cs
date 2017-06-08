using System;

public class JString : IUnit
{
    public static IUnit isThis(string text)
    {
        var realValue = text.Trim().Trim(new char[] { '\r', '\n' });
        if (realValue[realValue.Length - 1] != '\0')
            realValue += '\0';
        if (realValue[0] != '"' || realValue[realValue.Length - 2] != '"')
        {
            return null;
        }
        return new JString(realValue.Remove(realValue.Length - 2).Remove(0, 1));
    }

    public JString(string v = "")
        : base()
    {
        value = v;
    }
    public string String
    {
        get
        {
            return '"' + value + '"';
        }
        set
        {
            var realValue = value.Trim().Trim(new char[] { '\r', '\n' });
            if (realValue[realValue.Length - 1] != '\0')
                realValue += '\0';
            if (realValue[0] != '"' || realValue[realValue.Length - 2] != '"')
            {
                return;
            }
            this.value = realValue.Remove(realValue.Length - 2).Remove(0, 1);
        }
    }
    public ArmyAntJson.EType Type
    {
        get
        {
            return ArmyAntJson.EType.String;
        }
    }

    public JArray ToArray()
    {
        return null;
    }

    public bool ToBool()
    {
        return value == "";
    }

    public double ToFloat()
    {
        return 0.0;
    }

    public int ToInt()
    {
        return 0;
    }

    public JObject ToObject()
    {
        return null;
    }

    public override string ToString()
    {
        return value;
    }

    private string value = "";
}
