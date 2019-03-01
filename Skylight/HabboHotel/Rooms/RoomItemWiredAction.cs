using SkylightEmulator.HabboHotel.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public abstract class RoomItemWiredAction : RoomItem
    {
        public int Delay;

        public RoomItemWiredAction(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
            : base(id, roomId, userId, baseItem, extraData, x, y, z, rot, wallCoordinate, room)
        {
            this.Delay = 0;
        }

        public abstract void DoWiredAction(RoomUnitUser user, HashSet<uint> used);
    }
}