public interface IUnit
{
    string String
    {
        get;
        set;
    }
    ArmyAntJson.EType Type
    {
        get;
    }
    bool ToBool();
    int ToInt();
    long ToLong();
    double ToFloat();
    string ToString();

    JObject ToObject();
    JArray ToArray();
}

