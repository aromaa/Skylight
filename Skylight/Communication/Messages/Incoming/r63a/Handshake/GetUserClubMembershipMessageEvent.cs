using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Users.Subscription;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class GetUserClubMembershipMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            string clubType = message.PopFixedString();

            Subscription subscription = null;
            if (clubType == "habbo_club")
            {
                subscription = session.GetHabbo().GetSubscriptionManager().TryGetSubscription("habbo_vip", true, false);
            }

            if (subscription == null)
            {
                subscription = session.GetHabbo().GetSubscriptionManager().TryGetSubscription(clubType, false, true);
            }

            int daysLeft = subscription.DaysLeft();
            int monthsLeft = daysLeft / 31;
            if (monthsLeft >= 1)
            {
                monthsLeft--;
            }

            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            Message.Init(r63aOutgoing.SendClubMembership);
            Message.AppendString(clubType == "habbo_vip" ? "habbo_club" : clubType);
            Message.AppendInt32(daysLeft - (monthsLeft * 31)); //club days left
            Message.AppendInt32(0); //un used
            Message.AppendInt32(monthsLeft); //club months left
            Message.AppendInt32(1); //response type
            Message.AppendBoolean(false); //unused
            Message.AppendBoolean(session.GetHabbo().IsVIP()); //is vip
            Message.AppendInt32(0); //hc club gifts(?)
            Message.AppendInt32(0); //vip club gifts(?)
            Message.AppendBoolean(false); //show promo
            Message.AppendInt32(10); //normal price
            Message.AppendInt32(0); //promo price
            session.SendMessage(Message);
        }
    }
}
