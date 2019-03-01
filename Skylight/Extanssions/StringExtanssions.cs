using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Extanssions
{
    public static class StringExtanssions
    {
        public static bool EqualsIgnoreCase(this string left, string right)
        {
            return String.Compare(left, right, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public static bool StartsWithIgnoreCase(this string left, string right)
        {
            return left.StartsWith(right, StringComparison.OrdinalIgnoreCase);
        }

        public static bool ContainsIgnoreCase(this string left, string right)
        {
            return left.IndexOf(right, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
