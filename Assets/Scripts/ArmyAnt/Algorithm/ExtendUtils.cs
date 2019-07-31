using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmyAnt.Algorithm {
    public static class ExtendUtils {
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
    }
}
