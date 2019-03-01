using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Extanssions
{
    public static class IntExtanssions
    {
        public static int Limit(this int value, int min, int max)
        {
            return value < min ? min : value > max ? max : value;
        }
    }
}
