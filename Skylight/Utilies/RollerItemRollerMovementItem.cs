using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Utilies
{
    public class RollerItemRollerMovementItem
    {
        public uint RoomID;
        public double CurrentZ;
        public double NextZ;

        public RollerItemRollerMovementItem(uint roomId, double currentZ, double nextZ)
        {
            this.RoomID = roomId;
            this.CurrentZ = currentZ;
            this.NextZ = nextZ;
        }
    }
}
