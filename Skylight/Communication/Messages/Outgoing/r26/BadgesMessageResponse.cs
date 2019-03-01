using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Users.Badges;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r26
{
    class BadgesMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            List<Badge> badges = valueHolder.GetValue<List<Badge>>("Badges");

            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
            message.Init(r26Outgoing.Badges);
            message.AppendInt32(badges.Count);

            List<Badge> slotBadges = new List<Badge>();
            foreach (Badge badge in badges)
            {
                if (badge.SlotID > 0)
                {
                    slotBadges.Add(badge);
                }
                else
                {
                    BadgeManager.GetBadgeID(badge.BadgeID);
                    message.AppendString(badge.BadgeID);
                }
            }
            foreach (Badge badge in slotBadges.OrderBy(b => b.SlotID)) //we don't want order whole badge list, fixes issue badges not showing on correct slot id
            {
                BadgeManager.GetBadgeID(badge.BadgeID);
                message.AppendString(badge.BadgeID);
            }

            foreach (Badge badge in slotBadges)
            {
                message.AppendInt32(badge.SlotID);
                message.AppendString(badge.BadgeID);
            }
            return message;
        }
    }
}
