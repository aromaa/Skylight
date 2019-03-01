using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Catalog.Marketplace;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class GetOwnOffersMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null)
            {
                List<MarketplaceOffer> offers = Skylight.GetGame().GetCatalogManager().GetMarketplaceManager().GetOffersByUserID(session.GetHabbo().ID).Where(o => o.Redeem == false).ToList();

                ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                message_.Init(r63aOutgoing.MarketplaceOwnOffers);
                message_.AppendInt32(offers.Where(o => o.Sold).Sum(o => o.Price)); //earned money
                message_.AppendInt32(offers.Count);
                foreach(MarketplaceOffer offer in offers)
                {
                    Item item = Skylight.GetGame().GetItemManager().TryGetItem(offer.ItemID);

                    int state = 1;
                    if (offer.Sold)
                    {
                        state = 2;
                    }
                    else if (offer.Expired)
                    {
                        state = 3;
                    }

                    message_.AppendUInt(offer.ID);
                    message_.AppendInt32(state); //state, state 1 = selling, 2 = sold, 3 = item was not sold
                    message_.AppendInt32(item.IsFloorItem ? 1 : 2);
                    message_.AppendInt32(item.SpriteID);
                    message_.AppendString("");
                    message_.AppendInt32(this.CalcCompremission(offer.Price) + offer.Price);
                    message_.AppendInt32((int)Math.Floor(offer.Timeleft / 60.0)); //time left as minutes
                    message_.AppendInt32(item.SpriteID); //unknown but phx says this.. so idk :s
                }

                session.SendMessage(message_);
            }
        }

        private int CalcCompremission(float price)
        {
            double num = price / 100.0;
            return (int)Math.Ceiling(num * ServerConfiguration.MarketplaceCompremission);
        }
    }
}
