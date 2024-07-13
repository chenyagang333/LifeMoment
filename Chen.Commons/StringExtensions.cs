using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chen.Commons
{
    public static class StringExtensions
    {
        public static bool EqualsIgnoreCase(this string s1,string s2)
        {
            return string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase);
        }

        public static string Cut(this string s1, int maxLength)
        {
            if (s1 == null)
            {
                return string.Empty;
            }
            int len = s1.Length <= maxLength ? s1.Length : maxLength;
            return s1[0..len];
        }
    }
}
