using System;
public class JNumber : IUnit
{
    public static IUnit isThis(string text)
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
        : base()
    {
        value = v;
    }
    public JNumber(short v)
        : base()
    {
        value = v;
    }
    public JNumber(ushort v)
        : base()
    {
        value = v;
    }
    public JNumber(int v)
        : base()
    {
        value = v;
    }
    public JNumber(uint v)
        : base()
    {
        value = v;
    }
    public JNumber(long v)
        : base()
    {
        value = v;
    }
    public JNumber(ulong v)
        : base()
    {
        value = v;
    }
    public JNumber(float v)
        : base()
    {
        value = v;
    }
    public JNumber(double v)
        : base()
    {
        value = v;
    }
    public JNumber(decimal v)
        : base()
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
