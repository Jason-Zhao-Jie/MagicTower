public class JNull : IUnit
{
    public static IUnit isThis(string text)
    {
        return text.Trim().Trim(new char[] { '\r', '\n' }) == "null" ? new Undefined() : null;
    }

    public string String
    {
        get
        {
            return "null";
        }
        set
        {
            throw new ArmyAntJson.JException("Cannot set value to null");
        }
    }
    public ArmyAntJson.EType Type
    {
        get
        {
            return ArmyAntJson.EType.Null;
        }
    }

    public bool ToBool()
    {
        return false;
    }

    public int ToInt()
    {
        return 0;
    }

    public double ToFloat()
    {
        return 0.0;
    }

    public JObject ToObject()
    {
        return null;
    }

    public JArray ToArray()
    {
        return null;
    }

    public override string ToString()
    {
        return null;
    }
}

