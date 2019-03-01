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
    }
}
