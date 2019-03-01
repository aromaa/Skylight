using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class RoomItemBall : RoomItem
    {
        public int FootballWaitTime;
        public int FootballDirection;
        public RoomUnitUser LastUserHitFootball;

        public RoomItemBall(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
            : base(id, roomId, userId, baseItem, extraData, x, y, z, rot, wallCoordinate, room)
        {

        }

        public override void OnUse(GameClient session, RoomItem item, int request, bool userHasRights)
        {

        }

        public override void OnWalkOn(RoomUnit user)
        {
            this.Room.RoomGameManager.RoomFootballManager.UserKickFootball(user, this);
        }
    }
}
