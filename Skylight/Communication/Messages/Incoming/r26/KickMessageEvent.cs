using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Core;

namespace SkylightEmulator.Communication.Messages.Incoming.r26
{
    class KickMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            Room room = Skylight.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().GetRoomSession().CurrentRoomID);
            if (room != null && room.GaveRoomRights(session))
            {
                RoomUnitUser user = room.RoomUserManager.GetUserByName(message.PopStringUntilBreak(null));
                if (user != null && !room.HaveOwnerRights(user.Session) && !user.Session.GetHabbo().HasPermission("acc_unkickable"))
                {
                    room.RoomUserManager.KickUser(user.Session, true);
                }
            }
        }
    }
}
