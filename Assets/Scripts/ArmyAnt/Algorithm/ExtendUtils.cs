using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmyAnt.Algorithm {
    public static class ExtendUtils
    {
        public static void Swap<T>(ref T a, ref T b)
        {
            var temp = a;
            a = b;
            b = temp;
        }
        public static (T, T) GetSwap<T>(T a, T b)
        {
            return (b, a);
        }

        public static T[] GetArray<T>(IEnumerable<T> input) {
            if(input == null) {
                return null;
            }
            var ret = new T[input.Count()];
            int index = -1;
            foreach(var i in input) {
                ret[++index] = i;
            }
            return ret;
        }

        public static string[] GetArray<T>(T input, int start, int end)where T:Enum {
            var ret = new string[end - start + 1];
            for(var i=start; i <= end; ++i) {
                ret[i - start] = input.ToString();
            }
            return ret;
        }

        public static T GetArrayElemSafe<T>(T[] arr, int key, T defaultValue = default(T)) {
            if(arr == null || arr.Length <= key) {
                return defaultValue;
            } else {
                return arr[key];
            }
        }

        public static List<T> GetList<T>(IEnumerable<T> input) {
            if(input == null) {
                return null;
            }
            var ret = new List<T>(input.Count());
            foreach(var i in input) {
                ret.Add(i);
            }
            return ret;
        }

        public static List<string> GetList<T>(T start, T end) where T : Enum {
            int startNum = Convert.ToInt32(start);
            int endNum = Convert.ToInt32(end);
            var ret = new List<string>(endNum - startNum + 1);
            for(var i = startNum; i <= endNum; ++i) {
                ret.Add(((T)Enum.ToObject(default(T).GetType(), i)).ToString());
            }
            return ret;
        }

    }
}
