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

namespace SkylightEmulator.Communication.Messages.Incoming.r63b
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
                    ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
                    message_.Init(r63bOutgoing.SetActiveBadges);
                    message_.AppendUInt(userId);
                    message_.AppendInt32(user.Session.GetHabbo().GetBadgeManager().GetActiveBadges().Count);
                    foreach (Badge badge in user.Session.GetHabbo().GetBadgeManager().GetActiveBadges())
                    {
                        message_.AppendInt32(badge.SlotID);
                        message_.AppendString(badge.BadgeID);
                    }
                    session.SendMessage(message_);
                }
            }
        }
    }
}
