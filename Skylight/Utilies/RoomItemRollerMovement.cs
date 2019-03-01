using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Utilies
{
    public class RoomItemRollerMovement
    {
        public uint ItemId { get; private set; }

        public Point CurrentXY { get; private set; }
        public double CurrentZ { get; private set; }

        public uint SourceID { get; private set; }

        public Point NextXY { get; private set; }
        public double NextZ;

        public RoomItemRollerMovement(uint itemId, int currentX, int currentY, double currentZ, uint sourceId, int nextX = 0, int nextY = 0, double nextZ = 0)
        {
            this.ItemId = itemId;

            this.CurrentXY = new Point(currentX, currentY);
            this.CurrentZ = currentZ;

            this.SourceID = sourceId;

            this.NextXY = new Point(nextX, nextY);
            this.NextZ = nextZ;
        }
    }
}
