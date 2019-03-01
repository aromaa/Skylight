using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Utilies
{
    public class MatchFurniData
    {
        public string ExtraData;
        public int Rot;
        public int X;
        public int Y;
        public double Z;

        public MatchFurniData(string extraData, int rot, int x, int y, double z)
        {
            this.ExtraData = extraData;
            this.Rot = rot;
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }
}
