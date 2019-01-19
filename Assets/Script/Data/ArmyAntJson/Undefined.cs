public class Undefined : IUnit
{
    public static IUnit Create(string text)
    {
        return text.Trim().Trim('\r', '\n') == "undefined" ? new Undefined() : null;
    }

    public virtual string String
    {
        get
        {
            return "undefined";
        }
        set
        {
            throw new ArmyAntJson.JException("Cannot set value to undefined");
        }
    }

    public virtual ArmyAntJson.EType Type
    {
        get
        {
            return ArmyAntJson.EType.Undefined;
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

    public virtual long ToLong()
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
