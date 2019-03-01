using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Catalog;
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
    class GetClubOffersMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            ServerMessage club = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            club.Init(r63aOutgoing.ClubOffers);

            List<CatalogItem> items = Skylight.GetGame().GetCatalogManager().GetCatalogPages().Where(p => p.PageLayout == "club_buy").SelectMany(p => p.Items.Values).ToList();

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

            club.AppendInt32(items.Count); //count
            foreach(CatalogItem item in items)
            {
                double vipUpgradeTime = 0;

                club.AppendUInt(item.Id); //base id
                club.AppendString(item.Name); //name
                club.AppendInt32(item.CostCredits); //price

                if (item.Name.StartsWith("DEAL_VIP"))
                {
                    club.AppendBoolean(isUpgrade); //is upgrade, vip thing

                    if (isUpgrade)
                    {
                        vipUpgradeTime = subscription.SecoundsLeft() / 1.67;
                    }
                }
                else
                {
                    club.AppendBoolean(false); //is upgrade, vip thing
                }

                club.AppendBoolean(item.Name.StartsWith("DEAL_VIP")); //is vip packgage
                club.AppendInt32(item.GetItems()[0].Item2); //months lenght
                club.AppendInt32((int)Math.Ceiling(vipUpgradeTime / 86400) + (31 * item.GetItems()[0].Item2)); //days lenght, vip upgrade thing

                DateTime expires = TimeUtilies.UnixTimestampToDateTime(subscription.GetExpires() + (2678400.0 * item.GetItems()[0].Item2) + vipUpgradeTime);
                club.AppendInt32(expires.Year); //exprie year
                club.AppendInt32(expires.Month); //expire month
                club.AppendInt32(expires.Day); //expire day
            }

            session.SendMessage(club);
        }
    }
}
