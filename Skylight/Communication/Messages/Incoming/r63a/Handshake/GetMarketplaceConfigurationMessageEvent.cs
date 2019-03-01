using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
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
    class GetMarketplaceConfigurationMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null)
            {
                ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                message_.Init(r63aOutgoing.MarketplaceConfiguration);
                message_.AppendBoolean(ServerConfiguration.EnableMarketplace); //allow marketplace
                message_.AppendInt32(ServerConfiguration.MarketplaceCompremission); //compremission
                message_.AppendInt32(ServerConfiguration.MarketplaceTokensPrice); //tokens price
                message_.AppendInt32(session.GetHabbo().IsHcOrVIP() ? ServerConfiguration.MarketplaceTokensPremium : ServerConfiguration.MarketplaceTokensNonPremium); //buy tokens at once
                message_.AppendInt32(ServerConfiguration.MarketplaceMinPrice); //min price
                message_.AppendInt32(ServerConfiguration.MarketplaceMaxPrice); //max price
                message_.AppendInt32(ServerConfiguration.MarketplaceOffersActiveHours); //ofer active (hours)
                message_.AppendInt32(ServerConfiguration.MarketplaceAvaragePriceDays); //avarage price (days)
                session.SendMessage(message_);
            }
        }
    }
}
