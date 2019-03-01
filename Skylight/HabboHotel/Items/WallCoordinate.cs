using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Items
{
    public class WallCoordinate
    {
        private readonly int WidthX;
        private readonly int WidthY;
        private readonly int LengthX;
        private readonly int LengthY;
        private readonly char Side;

        public WallCoordinate(string wallCoords)
        {
            string[] splittedData = wallCoords.Split(' ');

            string[] widthCoords = splittedData[0].Substring(3).Split(',');
            this.WidthX = int.Parse(widthCoords[0]);
            this.WidthY = int.Parse(widthCoords[1]);

            string[] lengthCoords = splittedData[1].Substring(2).Split(',');
            this.LengthX = int.Parse(lengthCoords[0]);
            this.LengthY = int.Parse(lengthCoords[1]);

            this.Side = splittedData[2] == "l" ? 'l' : 'r';
        }

        public override string ToString()
        {
            return ":w=" + this.WidthX + "," + this.WidthY + " " + "l=" + this.LengthX + "," + this.LengthY + " " + this.Side;
        }
    }
}
