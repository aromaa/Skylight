using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Utilies
{
    public class RoomUserRollerMovement
    {
        public int VirtualID { get; private set; }

        public int CurrentX { get; private set; }
        public int CurrentY { get; private set; }
        public double CurrentZ { get; private set; }

        public uint SourceID { get; private set; }

        public int NextX;
        public int NextY;
        public double NextZ;

        public RoomUserRollerMovement(int virtualId, int currentX, int currentY, double currentZ, uint sourceID, int nextX = 0, int nextY = 0, double nextZ = 0)
        {
            this.VirtualID = virtualId;

            this.CurrentX = currentX;
            this.CurrentY = currentY;
            this.CurrentZ = currentZ;

            this.SourceID = sourceID;

            this.NextX = nextX;
            this.NextY = nextY;
            this.NextZ = nextZ;
        }
    }
}
