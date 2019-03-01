using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Catalog.Marketplace;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class RedeemCreditsMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null)
            {
                List<uint> redeemOffers = new List<uint>();

                int credits = 0;
                foreach(MarketplaceOffer offer in Skylight.GetGame().GetCatalogManager().GetMarketplaceManager().GetOffersByUserID(session.GetHabbo().ID))
                {
                    if (!offer.Redeem && offer.Sold)
                    {
                        offer.Redeem = true;
                        credits += offer.Price;

                        redeemOffers.Add(offer.ID);
                    }
                }

                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.ExecuteQuery("UPDATE catalog_marketplace_offers SET redeem = '1' WHERE id IN(" + string.Join(",", redeemOffers) + ") LIMIT " + redeemOffers.Count);
                }

                session.GetHabbo().Credits += credits;
                session.GetHabbo().UpdateCredits(true);
            }
        }
    }
}
