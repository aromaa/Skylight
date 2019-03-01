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
    class GetMarketplaceCanMakeOfferMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (ServerConfiguration.EnableMarketplace)
            {
                if (session.GetHabbo().GetUserSettings().AcceptTrading)
                {
                    if (session.GetHabbo().MarketplaceTokens > 0)
                    {
                        ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                        message_.Init(r63aOutgoing.MarketplaceCanMakeOffer);
                        message_.AppendInt32(1); //result, 1 = sell, 2 = no rights, 3 = no trade access, 4 = buy tickets
                        message_.AppendInt32(session.GetHabbo().MarketplaceTokens); //token count
                        session.SendMessage(message_);
                    }
                    else
                    {
                        ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                        message_.Init(r63aOutgoing.MarketplaceCanMakeOffer);
                        message_.AppendInt32(4); //result, 1 = sell, 2 = no rights, 3 = no trade access, 4 = buy tickets
                        message_.AppendInt32(session.GetHabbo().MarketplaceTokens); //token count
                        session.SendMessage(message_);
                    }
                }
                else
                {
                    ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                    message_.Init(r63aOutgoing.MarketplaceCanMakeOffer);
                    message_.AppendInt32(3); //result, 1 = sell, 2 = no rights, 3 = no trade access, 4 = buy tickets
                    message_.AppendInt32(session.GetHabbo().MarketplaceTokens); //token count
                    session.SendMessage(message_);
                }
            }
            else
            {
                ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                message_.Init(r63aOutgoing.MarketplaceCanMakeOffer);
                message_.AppendInt32(2); //result, 1 = sell, 2 = no rights, 3 = no trade access, 4 = buy tickets
                message_.AppendInt32(session.GetHabbo().MarketplaceTokens); //token count
                session.SendMessage(message_);
            }
        }
    }
}
