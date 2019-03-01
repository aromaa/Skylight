using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Utilies
{
    public class CoordUtilies
    {
        public static bool InRange(int targetX, int targetY, int x, int y, int range = 1)
        {
            return (Math.Abs(targetX - x) <= range && Math.Abs(targetY - y) <= range) || (targetX == x && targetY == y);
        }
    }
}
