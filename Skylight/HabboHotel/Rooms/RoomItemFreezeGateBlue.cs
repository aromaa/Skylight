using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.HabboHotel.Rooms.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    class RoomItemFreezeGateBlue : RoomItem
    {
        public RoomItemFreezeGateBlue(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
            : base(id, roomId, userId, baseItem, extraData, x, y, z, rot, wallCoordinate, room)
        {

        }

        public override void OnWalkOn(RoomUnit user)
        {
            if (user.IsRealUser && user is RoomUnitUser user_)
            {
                if (user_.GameTeam == GameTeam.Blue && user_.GameType == GameType.Freeze)
                {
                    this.Room.RoomGameManager.LeaveTeam(user_);
                }
                else
                {
                    this.Room.RoomGameManager.JoinTeam(user_, GameTeam.Blue, GameType.Freeze);
                }

                this.Room.RoomGameManager.GiveEffect(user_);
            }
        }
    }
}
