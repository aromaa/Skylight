using SkylightEmulator.HabboHotel.Data.Enums;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    class RoomItemWiredSwitch : RoomItem
    {
        public RoomItemWiredSwitch(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
            : base(id, roomId, userId, baseItem, extraData, x, y, z, rot, wallCoordinate, room)
        {

        }

        public override void OnUse(GameClient session, RoomItem item, int request, bool userHasRights)
        {
            if (session != null)
            {
                if (session.GetHabbo().GetRoomSession().GetRoomUser().X - item.X > 1 || session.GetHabbo().GetRoomSession().GetRoomUser().Y - item.Y > 1)
                {
                    if ((session.GetHabbo().GetRoomSession().GetRoomUser().RestrictMovementType & RestrictMovementType.Client) == 0)
                    {
                        session.GetHabbo().GetRoomSession().GetRoomUser().MoveTo(this.X, this.Y);
                    }
                }
                else
                {
                    base.OnUse(session, item, request, true);
                }
            }
        }
    }
}
