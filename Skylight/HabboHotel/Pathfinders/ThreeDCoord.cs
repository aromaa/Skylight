using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Pathfinders
{
    public class ThreeDCoord
    {
        internal int x;
        internal int y;

        internal ThreeDCoord(int _x, int _y)
        {
            this.x = _x;
            this.y = _y;
        }

        public static bool smethod_0(ThreeDCoord a, ThreeDCoord b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            if ((object)a == null || (object)b == null)
            {
                return false;
            }

            return a.x == b.x && a.y == b.y;
        }

        public static bool smethod_1(ThreeDCoord a, ThreeDCoord b)
        {
            return !ThreeDCoord.smethod_0(a, b);
        }

        public override int GetHashCode()
        {
            return this.x ^ this.y;
        }

        public override bool Equals(object obj)
        {
            return base.GetHashCode().Equals(obj.GetHashCode());
        }
    }
}
