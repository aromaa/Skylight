using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.HabboHotel.Users.Subscription;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.GameClients;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class ClubMembershipMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            GameClient session = valueHolder.GetValue<GameClient>("Session");
            string clubType = valueHolder.GetValue<string>("ClubType");

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

            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
            Message.Init(r63cOutgoing.UserClub);
            Message.AppendString(clubType == "habbo_vip" ? "habbo_club" : clubType);
            Message.AppendInt32(daysLeft - (monthsLeft * 31)); //club days left
            Message.AppendInt32(0); //un used
            Message.AppendInt32(monthsLeft); //club months left
            Message.AppendInt32(1); //response type
            Message.AppendBoolean(true); //unused
            Message.AppendBoolean(session.GetHabbo().IsVIP()); //is vip
            Message.AppendInt32(10); //normal price
            Message.AppendInt32(0); //promo price
            Message.AppendInt32((int)subscription.SecoundsLeft());
            //Message.AppendInt32(1); //Update timer, optional
            return Message;
        }
    }
}
