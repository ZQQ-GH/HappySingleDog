using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappySingleDog.Utils
{
    public static class StringHandler
    {
        public static string QuotedStr(this string a_str)
        {
            return "'" + a_str + "'";
        }

        public static bool IsNullOrEmpty(this string a_str)
        {
            return string.IsNullOrEmpty(a_str);
        }
    }
}
