using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Users.Inventory;
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
    class MakeOfferMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (ServerConfiguration.EnableMarketplace)
            {
                if (session != null && session.GetHabbo() != null)
                {
                    if (session.GetHabbo().MarketplaceTokens > 0)
                    {
                        int price = message.PopWiredInt32();
                        if (price >= ServerConfiguration.MarketplaceMinPrice && price <= ServerConfiguration.MarketplaceMaxPrice)
                        {
                            int itemType = message.PopWiredInt32(); //1 = floor, 2 = wall
                            uint itemId = message.PopWiredUInt();

                            InventoryItem item = null;
                            if (itemType == 1)
                            {
                                item = session.GetHabbo().GetInventoryManager().TryGetFloorItem(itemId);
                            }
                            else if (itemType == 2)
                            {
                                item = session.GetHabbo().GetInventoryManager().TryGetWallItem(itemId);
                            }

                            if (item != null && item.GetItem() != null && item.GetItem().AllowTrade && item.GetItem().AllowMarketplaceSell)
                            {
                                Skylight.GetGame().GetCatalogManager().GetMarketplaceManager().MakeOffer(session, item, price);

                                session.GetHabbo().MarketplaceTokens--;
                                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                                {
                                    dbClient.AddParamWithValue("userId", session.GetHabbo().ID);
                                    dbClient.AddParamWithValue("tokens", session.GetHabbo().MarketplaceTokens);
                                    dbClient.ExecuteQuery("UPDATE users SET marketplace_tokens = @tokens WHERE id = @userId LIMIT 1");
                                }

                                ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                                message_.Init(r63aOutgoing.MakeOfferResult);
                                message_.AppendInt32(1); //result, 1 = success, 2 = technical error, 3 = marketplace disabled, 4 = item was just added to shop
                                session.SendMessage(message_);
                            }
                            else
                            {
                                ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                                message_.Init(r63aOutgoing.MakeOfferResult);
                                message_.AppendInt32(2); //result, 1 = success, 2 = technical error, 3 = marketplace disabled, 4 = item was just added to shop
                                session.SendMessage(message_);
                            }
                        }
                        else
                        {
                            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                            message_.Init(r63aOutgoing.MakeOfferResult);
                            message_.AppendInt32(2); //result, 1 = success, 2 = technical error, 3 = marketplace disabled, 4 = item was just added to shop
                            session.SendMessage(message_);
                        }
                    }
                    else
                    {
                        ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                        message_.Init(r63aOutgoing.MakeOfferResult);
                        message_.AppendInt32(2); //result, 1 = success, 2 = technical error, 3 = marketplace disabled, 4 = item was just added to shop
                        session.SendMessage(message_);
                    }
                }
            }
            else
            {
                ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                message_.Init(r63aOutgoing.MakeOfferResult);
                message_.AppendInt32(3); //result, 1 = success, 2 = technical error, 3 = marketplace disabled, 4 = item was just added to shop
                session.SendMessage(message_);
            }
        }
    }
}
