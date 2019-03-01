using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Catalog.Marketplace;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class CancelOfferMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null)
            {
                uint itemId = message.PopWiredUInt();

                Skylight.GetGame().GetCatalogManager().GetMarketplaceManager().CancelOffer(session, itemId);
            }
        }
    }
}
