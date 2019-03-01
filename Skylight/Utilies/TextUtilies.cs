using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Utilies
{
    public class TextUtilies
    {
        public static bool StringToBool(string s)
        {
            return s == "1";
        }

        public static string FilterString(string str)
        {
            return FilterString(str, true, false);
        }

        public static string FilterString(string Input, bool FilterBreak, bool FilterSlash)
        {
            Input = Input.Replace(Convert.ToChar(1), ' ');
            Input = Input.Replace(Convert.ToChar(2), ' ');
            Input = Input.Replace(Convert.ToChar(9), ' ');

            if (FilterBreak)
            {
                Input = Input.Replace(Convert.ToChar(13), ' ');
            }
            if (FilterSlash)
            {
                Input = Input.Replace('\'', ' ');
            }
            return Input;
        }

        public static string CheckBlacklistedWords(string str)
        {
            return str;
        }

        public static string DoubleWithDotDecimal(double d)
        {
            return d.ToString().Replace(',', '.');
        }
    }
}
