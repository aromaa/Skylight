using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Utilies;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class SitMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            RoomUnit user = session.GetHabbo().GetRoomSession().GetRoomUser();

            if (!user.HasStatus("lay"))
            {
                if (user.BodyRotation == 0 || user.BodyRotation == 2 || user.BodyRotation == 4 || user.BodyRotation == 6)
                {
                    user.AddStatus("sit", TextUtilies.DoubleWithDotDecimal((user.Z + 1) / 2 - user.Z * 0.5));
                }
            }
        }
    }
}
