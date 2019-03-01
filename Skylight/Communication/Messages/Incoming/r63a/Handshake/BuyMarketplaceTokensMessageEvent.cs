using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class BuyMarketplaceTokensMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (ServerConfiguration.EnableMarketplace)
            {
                if (session != null && session.GetHabbo() != null)
                {
                    if (session.GetHabbo().Credits > 0)
                    {
                        if (session.GetHabbo().IsHcOrVIP())
                        {
                            session.GetHabbo().MarketplaceTokens += ServerConfiguration.MarketplaceTokensPremium;
                        }
                        else
                        {
                            session.GetHabbo().MarketplaceTokens += ServerConfiguration.MarketplaceTokensNonPremium;
                        }

                        session.GetHabbo().Credits -= ServerConfiguration.MarketplaceTokensPrice;
                        session.GetHabbo().UpdateCredits(true);

                        using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                        {
                            dbClient.AddParamWithValue("userId", session.GetHabbo().ID);
                            dbClient.AddParamWithValue("tokens", session.GetHabbo().MarketplaceTokens);
                            dbClient.ExecuteQuery("UPDATE users SET marketplace_tokens = @tokens WHERE id = @userId LIMIT 1");
                        }

                        ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                        message_.Init(r63aOutgoing.MarketplaceCanMakeOffer);
                        message_.AppendInt32(1); //result, 1 = sell, 2 = no rights, 3 = no trade access, 4 = buy tickets
                        message_.AppendInt32(session.GetHabbo().MarketplaceTokens); //token count
                        session.SendMessage(message_);
                    }
                    else
                    {
                        ServerMessage noEnoughtCash = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                        noEnoughtCash.Init(r63aOutgoing.NoEnoughtCash);
                        noEnoughtCash.AppendBoolean(true);
                        noEnoughtCash.AppendBoolean(false);
                        session.SendMessage(noEnoughtCash);
                    }
                }
            }
        }
    }
}
