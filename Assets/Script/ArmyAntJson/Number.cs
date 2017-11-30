using System;
public class JNumber : IUnit
{
    public static IUnit Create(string text)
    {
        try
        {
            return new JNumber(Convert.ToDouble(text));
        }
        catch (FormatException)
        {
            return null;
        }

    }

    public JNumber(byte v = 0)
    {
        value = v;
    }
    public JNumber(short v)
    {
        value = v;
    }
    public JNumber(ushort v)
    {
        value = v;
    }
    public JNumber(int v)
    {
        value = v;
    }
    public JNumber(uint v)
    {
        value = v;
    }
    public JNumber(long v)
    {
        value = v;
    }
    public JNumber(ulong v)
    {
        value = v;
    }
    public JNumber(float v)
    {
        value = v;
    }
    public JNumber(double v)
    {
        value = v;
    }
    public JNumber(decimal v)
    {
        value = Convert.ToDouble(v);
    }
    public string String
    {
        get
        {
            if (value - Convert.ToInt64(value) <= double.Epsilon)
                return Convert.ToInt64(value).ToString();
            return value.ToString();
        }
        set
        {
            try
            {
                this.value = Convert.ToDouble(value);
            }
            catch (FormatException)
            {

            }
        }
    }

    public ArmyAntJson.EType Type
    {
        get
        {
            return ArmyAntJson.EType.Numeric;
        }
    }

    public override string ToString()
    {
        return null;
    }

    public bool ToBool()
    {
        return value - 0.0 <= double.Epsilon;
    }

    public int ToInt()
    {
        return (int)value;
    }

    public long ToLong()
    {
        return (long)value;
    }

    public double ToFloat()
    {
        return value;
    }

    public JObject ToObject()
    {
        return null;
    }

    public JArray ToArray()
    {
        return null;
    }

    private double value = 0;
}
