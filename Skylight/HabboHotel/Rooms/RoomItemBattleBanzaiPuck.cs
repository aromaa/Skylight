using SkylightEmulator.HabboHotel.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class RoomItemBattleBanzaiPuck : RoomItem
    {
        public RoomUnit LastUserHitPuck;
        public int PuckDirection;
        public int PuckWaitTime;
        public int Power;

        public RoomItemBattleBanzaiPuck(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
            : base(id, roomId, userId, baseItem, extraData, x, y, z, rot, wallCoordinate, room)
        {

        }

        public override void OnWalkOn(RoomUnit user)
        {
            this.Room.RoomGameManager.RoomBattleBanzaiManager.UserKickPuck(user, this);
        }
    }
}
