﻿using System;
using System.Collections;
using System.Collections.Generic;

    public class JArray : UnityEngine.MonoBehaviour, ArmyAntJson.IJsonCollection, IList<ArmyAntJson.IUnit>, ICollection<ArmyAntJson.IUnit>
    {
        public static ArmyAntJson.IUnit isThis(string text)
        {
            try
            {
                var ret = new JArray();
                ret.String = text;
                return ret;
            }
            catch (ArmyAntJson.JException)
            {
                return null;
            }
        }

        public JArray(ArmyAntJson.IUnit[] v = null)
            : base()
        {
            if (v != null)
                value = new List<ArmyAntJson.IUnit>(v);
        }
        public string String
        {
            get
            {
                var ret = "[\n";
                for (var i = 0; value != null && i < value.Count - 1; i++)
                {
                    ret += "  " + value[i].String + ",\n";
                }
                if (value != null && value.Count > 0)
                    ret += "  " + value[value.Count - 1].String + "\n]";
                else
                    ret += "\n]";
                return ret;
            }
            set
            {
                var realValue = value.Trim().Trim(new char[] { '\r', '\n' });
                if (realValue[realValue.Length - 1] != '\0')
                    realValue += '\0';
                if (realValue[0] != '[' || realValue[realValue.Length - 2] != ']')
                {
                    return;
                }
                realValue = realValue.Remove(realValue.Length - 2).Remove(0, 1);
                realValue = realValue.Trim().Trim(new char[] { '\r', '\n' });
                if (realValue != "")
                    try
                    {
                        var res = ArmyAntJson.CutByComma(realValue);
                        this.value = new List<ArmyAntJson.IUnit>();
                        for (int i = 0; i < res.Length; i++)
                        {
                            this.value.Add(ArmyAntJson.Create(res[i]));
                        }
                    }
                    catch (ArmyAntJson.JException)
                    {
                    }
            }
        }
        public ArmyAntJson.EType Type
        {
            get
            {
                return ArmyAntJson.EType.Array;
            }
        }

        public int Length
        {
            get
            {
                return value.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        public int Count
        {
            get
            {
                return value == null ? 0 : value.Count;
            }
        }

        public ArmyAntJson.IUnit this[int index]
        {
            get
            {
                return value[index];
            }

            set
            {
                this.value[index] = value;
            }
        }

        public bool AddChild(ArmyAntJson.IUnit child, string tag = null)
        {
            value.Add(child);
            return true;
        }

        public bool RemoveChild(string tag)
        {
            int num;
            try
            {
                num = Convert.ToInt32(tag);
            }
            catch (FormatException)
            {
                return false;
            }
            value.RemoveAt(num);
            return true;
        }

        public ArmyAntJson.IUnit GetChild(string tag)
        {
            int num;
            try
            {
                num = Convert.ToInt32(tag);
            }
            catch (FormatException)
            {
                return null;
            }
            return value[num];
        }

        public void Add(ArmyAntJson.IUnit value)
        {
            this.value.Add(value);
        }

        public bool Contains(ArmyAntJson.IUnit value)
        {
            return this.value.Contains(value);
        }

        public void Clear()
        {
            value = null;
        }

        public int IndexOf(ArmyAntJson.IUnit value)
        {
            return this.value.IndexOf(value);
        }

        public void Insert(int index, ArmyAntJson.IUnit value)
        {
            this.value.Insert(index, value);
        }

        public bool Remove(ArmyAntJson.IUnit value)
        {
            return this.value.Remove(value);
        }

        public void RemoveAt(int index)
        {
            value.RemoveAt(index);
        }

        public void CopyTo(ArmyAntJson.IUnit[] array, int index)
        {
            value.CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return value.GetEnumerator();
        }

        public bool ToBool()
        {
            return value != null;
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
            return this;
        }

        public void CopyTo(Array array, int index)
        {
            for(int i = index; i < value.Count; ++i)
            {
                array.SetValue(value[i], new int[] { i - index });
            }
        }

        IEnumerator<ArmyAntJson.IUnit> IEnumerable<ArmyAntJson.IUnit>.GetEnumerator()
        {
            return value.GetEnumerator();
        }

        private IList<ArmyAntJson.IUnit> value = new List<ArmyAntJson.IUnit>();
    }
