using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class RoomWiredDelay
    {
        public readonly RoomItemWiredAction Wired;
        public readonly RoomUnitUser Triggerer;
        public int Delay;
        public HashSet<uint> Used;

        public RoomWiredDelay(RoomItemWiredAction wired, RoomUnitUser triggerer, int delay, HashSet<uint> used)
        {
            this.Wired = wired;
            this.Triggerer = triggerer;
            this.Delay = delay;
            this.Used = used;
        }
    }
}
