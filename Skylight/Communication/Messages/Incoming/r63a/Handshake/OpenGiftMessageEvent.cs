using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class OpenGiftMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            Room room = session.GetHabbo().GetRoomSession().GetRoom();
            if (room != null)
            {
                uint roomId = message.PopWiredUInt();
                RoomItem item = room.RoomItemManager.TryGetRoomItem(roomId);
                if (item != null)
                {
                    DataRow presentData = null;
                    using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                    {
                        dbClient.AddParamWithValue("itemId", item.ID);
                        presentData = dbClient.ReadDataRow("SELECT base_ids, amounts, extra_data FROM items_presents WHERE item_id = @itemId LIMIT 1");
                    }

                    if (presentData != null)
                    {
                        room.RoomItemManager.RemoveItem(session, item);

                        using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                        {
                            dbClient.AddParamWithValue("itemId", item.ID);
                            dbClient.ExecuteQuery("DELETE FROM items_presents WHERE item_id = @itemId LIMIT 1; DELETE FROM items WHERE id = @itemId LIMIT 1;");
                        }

                        session.GetHabbo().GetInventoryManager().SetQueueBytes(true);
                        string[] baseIds = ((string)presentData["base_ids"]).Split(',');
                        string[] amounts = ((string)presentData["amounts"]).Split(',');
                        for (int i = 0; i < baseIds.Length; i++)
                        {
                            Item baseItem = Skylight.GetGame().GetItemManager().TryGetItem(uint.Parse(baseIds[i]));
                            if (baseItem != null)
                            {
                                Skylight.GetGame().GetCatalogManager().AddItem(session, baseItem, int.Parse(amounts[i]), (string)presentData["extra_data"], true, false);

                                ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                                message_.Init(r63aOutgoing.OpenPresent);
                                message_.AppendString(baseItem.Type.ToString());
                                message_.AppendInt32(baseItem.SpriteID);
                                message_.AppendString(baseItem.PublicName);
                                session.SendMessage(message_);
                            }
                        }
                        session.GetHabbo().GetInventoryManager().SetQueueBytes(false);
                    }
                }
            }
        }
    }
}
