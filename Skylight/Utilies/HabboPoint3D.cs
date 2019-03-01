using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Utilies
{
    public class HabboPoint3D
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public double Z { get; private set; }

        public HabboPoint3D(int x, int y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public override int GetHashCode()
        {
            return this.X ^ this.Y ^ (int)this.Z;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }

            HabboPoint3D point = obj as HabboPoint3D;
            if (point != null && this.X == point.X && this.Y == point.Y && this.Z == point.Z)
            {
                return true;
            }

            return false;
        }
    }
}
