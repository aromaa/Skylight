using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Rooms;

namespace SkylightEmulator.Communication.Messages.Incoming.r26
{
    class KickBanMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            Room room = Skylight.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().GetRoomSession().CurrentRoomID);
            if (room != null && room.HaveOwnerRights(session))
            {
                uint userId = message.PopWiredUInt();

                RoomUnitUser user = room.RoomUserManager.GetUserByName(message.PopStringUntilBreak(null));
                if (user != null && !room.HaveOwnerRights(user.Session) && !user.Session.GetHabbo().HasPermission("acc_unbannable"))
                {
                    room.RoomUserManager.BanUser(user.Session);
                }
            }
        }
    }
}
