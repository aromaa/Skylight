using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.HabboHotel.Users.Badges;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class GetUserSelectedBadgesMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            uint userId = message.PopWiredUInt();

            Room room = session.GetHabbo().GetRoomSession().GetRoom();
            if (room != null)
            {
                RoomUnitUser user = room.RoomUserManager.GetUserByID(userId);
                if (user != null)
                {
                    session.SendMessage(OutgoingPacketsEnum.ActiveBadges, new ValueHolder("UserID", userId, "Badges", user.Session.GetHabbo().GetBadgeManager().GetActiveBadges()));
                }
            }
        }
    }
}
