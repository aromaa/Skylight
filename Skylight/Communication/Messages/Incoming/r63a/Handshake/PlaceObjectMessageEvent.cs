using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.HabboHotel.Users.Inventory;
using SkylightEmulator.Messages;
using SkylightEmulator.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class PlaceObjectMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null && session.GetHabbo().GetRoomSession() != null)
            {
                Room room = session.GetHabbo().GetRoomSession().GetRoom();
                if (room != null && room.GaveRoomRights(session))
                {
                    string data = message.PopFixedString();
                    string[] splittedData = data.Split(' ');

                    uint itemId = uint.Parse(splittedData[0].Replace("-", ""));
                    if (splittedData[1].StartsWith(":"))
                    {
                        InventoryItem item = session.GetHabbo().GetInventoryManager().TryGetWallItem(itemId);
                        if (item != null && item.GetItem() != null)
                        {
                            RoomItem roomItem = RoomItem.GetRoomItem(itemId, room.ID, session.GetHabbo().ID, item.BaseItem, item.ExtraData, 0, 0, 0.0, 0, new WallCoordinate(":" + data.Split(':')[1]), room);
                            if (room.RoomItemManager.AddWallItemToRoom(session, roomItem))
                            {
                                session.GetHabbo().GetInventoryManager().RemoveItemFromHand(itemId, false);

                                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                                {
                                    dbClient.AddParamWithValue("roomid", room.ID);
                                    dbClient.AddParamWithValue("itemid", itemId);
                                    dbClient.ExecuteQuery("UPDATE items SET room_id = @roomid WHERE id = @itemid LIMIT 1");
                                }
                            }
                        }
                    }
                    else
                    {
                        InventoryItem item = session.GetHabbo().GetInventoryManager().TryGetFloorItem(itemId);
                        if (item != null && item.GetItem() != null)
                        {
                            int x = int.Parse(splittedData[1]);
                            int y = int.Parse(splittedData[2]);
                            int rot = int.Parse(splittedData[3]);

                            RoomItem roomItem = RoomItem.GetRoomItem(itemId, room.ID, session.GetHabbo().ID, item.BaseItem, item.ExtraData, x, y, 0.0, rot, null, room);
                            if (room.RoomItemManager.AddFloorItemToRoom(session, roomItem, x, y, rot))
                            {
                                session.GetHabbo().GetInventoryManager().RemoveItemFromHand(itemId, false);

                                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                                {
                                    dbClient.AddParamWithValue("roomid", room.ID);
                                    dbClient.AddParamWithValue("itemid", itemId);
                                    dbClient.ExecuteQuery("UPDATE items SET room_id = @roomid WHERE id = @itemid LIMIT 1");
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
