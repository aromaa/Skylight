using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Catalog.Marketplace;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class GetMarketplaceItemStatsMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (ServerConfiguration.EnableMarketplace)
            {
                if (session != null && session.GetHabbo() != null)
                {
                    int itemType = message.PopWiredInt32(); //1 = floor, 2 = wall
                    uint itemId = message.PopWiredUInt();

                    ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                    message_.Init(r63aOutgoing.MarketplaceItemStats);
                    message_.AppendInt32(1); //unknown
                    message_.AppendInt32(Skylight.GetGame().GetCatalogManager().GetMarketplaceManager().GetOffersCountByItemID(itemId));
                    message_.AppendInt32(30); //days
                    message_.AppendInt32(29); //count

                    List<IGrouping<DateTime, MarketplaceOffer>> soldItems = Skylight.GetGame().GetCatalogManager().GetMarketplaceManager().GetSoldsByItemID(itemId).OrderByDescending(s => s.SoldTimestamp).GroupBy(s => TimeUtilies.UnixTimestampToDateTime(s.SoldTimestamp).Date).ToList();
                    for (int i = -29; i < 0; i++)
                    {
                        message_.AppendInt32(i);
                        DateTime date = DateTime.Now.AddDays(i).Date;
                        if (soldItems.Any(o => o.Key == date))
                        {
                            IEnumerable<MarketplaceOffer> items = soldItems.Where(o => o.Key == date).SelectMany(g => g);
                            int price = items.Sum(s => this.CalcCompremission(s.Price) + s.Price) / items.Count();
                            message_.AppendInt32(price); //price
                            message_.AppendInt32(items.Count()); //trade
                        }
                        else
                        {
                            message_.AppendInt32(0);
                            message_.AppendInt32(0);
                        }
                    }

                    message_.AppendInt32(itemType);
                    message_.AppendUInt(itemId);
                    session.SendMessage(message_);
                }
            }
        }

        private int CalcCompremission(float price)
        {
            double num = price / 100.0;
            return (int)Math.Ceiling(num * ServerConfiguration.MarketplaceCompremission);
        }
    }
}
