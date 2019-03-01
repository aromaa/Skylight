using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Users.Badges;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class BadgesMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            List<Badge> badges = valueHolder.GetValue<List<Badge>>("Badges");

            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
            message.Init(r63cOutgoing.AddBadges);
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
                    message.AppendInt32(BadgeManager.GetBadgeID(badge.BadgeID));
                    message.AppendString(badge.BadgeID);
                }
            }
            foreach (Badge badge in slotBadges.OrderBy(b => b.SlotID)) //we don't want order whole badge list, fixes issue badges not showing on correct slot id
            {
                message.AppendInt32(BadgeManager.GetBadgeID(badge.BadgeID));
                message.AppendString(badge.BadgeID);
            }

            message.AppendInt32(slotBadges.Count);
            foreach (Badge badge in slotBadges)
            {
                message.AppendInt32(badge.SlotID);
                message.AppendString(badge.BadgeID);
            }
            return message;
        }
    }
}
