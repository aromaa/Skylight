using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Catalog;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Users.Subscription;

namespace SkylightEmulator.Communication.Messages.Incoming.r63c
{
    class ClubDataMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            int windowId = message.PopWiredInt32();

            List<CatalogItem> items = Skylight.GetGame().GetCatalogManager().GetCatalogPages().Where(p => p.PageLayout == "club_buy").SelectMany(p => p.Items.Values).Where(i => i.Name.StartsWith("DEAL_VIP")).ToList();

            Subscription subscription = session.GetHabbo().GetSubscriptionManager().TryGetSubscription("habbo_vip", true, false);
            if (subscription == null) //user isint vip
            {
                subscription = session.GetHabbo().GetSubscriptionManager().TryGetSubscription("habbo_club", true, false);

                if (subscription == null) //still null
                {
                    subscription = new Subscription(0, "", TimeUtilies.GetUnixTimestamp(), TimeUtilies.GetUnixTimestamp());
                }
            }

            bool isUpgrade = !session.GetHabbo().IsVIP() && session.GetHabbo().IsHC();

            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
            message_.Init(r63cOutgoing.ClubData);
            message_.AppendInt32(items.Count); //count
            foreach (CatalogItem item in items)
            {
                message_.AppendUInt(item.Id);
                message_.AppendString(item.Name);
                message_.AppendBoolean(true); //unused
                message_.AppendInt32(item.CostCredits);
                message_.AppendInt32(item.CostActivityPoints);
                message_.AppendInt32(item.ActivityPointsType);

                double vipUpgradeTime = 0;
                if (item.Name.StartsWith("DEAL_VIP"))
                {
                    message_.AppendBoolean(true);

                    if (isUpgrade)
                    {
                        vipUpgradeTime = subscription.SecoundsLeft() / 1.67;
                    }
                }
                else
                {
                    message_.AppendBoolean(false);
                }

                message_.AppendInt32(item.GetItems()[0].Item2); //months lenght
                message_.AppendInt32((int)Math.Ceiling(vipUpgradeTime / 86400) + (31 * item.GetItems()[0].Item2)); //days lenght
                message_.AppendBoolean(true); //unused
                message_.AppendInt32(0); //unused

                DateTime expires = TimeUtilies.UnixTimestampToDateTime(subscription.GetExpires() + (2678400.0 * item.GetItems()[0].Item2) + vipUpgradeTime);
                message_.AppendInt32(expires.Year); //exprie year
                message_.AppendInt32(expires.Month); //expire month
                message_.AppendInt32(expires.Day); //expire day
            }

            message_.AppendInt32(windowId);
            session.SendMessage(message_);
        }
    }
}
