using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class GetOffersMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            int minPrice = message.PopWiredInt32();
            int maxPrice = message.PopWiredInt32();
            string search = message.PopFixedString();
            int order = message.PopWiredInt32();

            session.SendMessage(Skylight.GetGame().GetCatalogManager().GetMarketplaceManager().GetOffers(minPrice, maxPrice, search, order));
        }
    }
}
