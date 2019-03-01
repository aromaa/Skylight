using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Pathfinders;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class LookToMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null && session.GetHabbo().GetRoomSession() != null)
            {
                RoomUnitUser user = session.GetHabbo().GetRoomSession().CurrentRoomRoomUser;
                if (user != null)
                {
                    user.Unidle();

                    int x = message.PopWiredInt32();
                    int y = message.PopWiredInt32();
                    if (x != user.X || y != user.Y)
                    {
                        int rotation = WalkRotation.Walk(user.X, user.Y, x, y);
                        user.SetRotation(rotation, false);
                    }
                }
            }
        }
    }
}
