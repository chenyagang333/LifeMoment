using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic;
public static class EnumerableExtensions
{
    public static bool SequenceIgnoredEqual<T>(this IEnumerable<T> itme1, IEnumerable<T> itme2)
    {
        //有一个为null，就是false
        if (itme1 == null || itme2 == null)
        {
            return false;
        }
        else if (itme1 == itme2)
        {
            return true;
        }
        else
        {
            return itme1.OrderBy(x => x).SequenceEqual(itme2.OrderBy(x => x));
        }
    }
}
