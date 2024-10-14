using System;
using System.Collections.Generic;

namespace Dancer.Modules
{
    internal static class ArrayHelper
    {
        public static T[] Append<T>(ref T[] array, List<T> list)
        {
            int num = array.Length;
            int count = list.Count;
            Array.Resize(ref array, num + count);
            list.CopyTo(array, num);
            return array;
        }

        public static Func<T[], T[]> AppendDel<T>(List<T> list)
        {
            return (T[] r) => Append(ref r, list);
        }
    }
}
