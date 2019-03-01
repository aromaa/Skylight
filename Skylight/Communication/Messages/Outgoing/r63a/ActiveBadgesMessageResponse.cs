using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Users.Badges;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63a
{
    class ActiveBadgesMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            List<Badge> badges = valueHolder.GetValue<List<Badge>>("Badges");

            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            message_.Init(r63aOutgoing.SetActiveBadges);
            message_.AppendUInt(valueHolder.GetValue<uint>("UserID"));
            message_.AppendInt32(badges.Count);
            foreach (Badge badge in badges.OrderBy(s => s.SlotID))
            {
                message_.AppendInt32(badge.SlotID);
                message_.AppendString(badge.BadgeID);
            }
            return message_;
        }
    }
}
