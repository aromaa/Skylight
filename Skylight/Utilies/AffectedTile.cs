using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Utilies
{
    public class AffectedTile
    {
        public int X;
        public int Y;
        public int Rot;

        public AffectedTile(int x, int y, int rot)
        {
            this.X = x;
            this.Y = y;
            this.Rot = rot;
        }

        public override bool Equals(object obj)
        {
            if (obj is AffectedTile)
            {
                AffectedTile obj_ = (AffectedTile)obj;
                return obj_.X == this.X && obj_.Y == this.Y && obj_.Rot == this.Rot;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.X ^ this.Y ^ this.Rot;
        }
    }
}
