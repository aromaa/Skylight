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
    class PlacePostItMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            Room room = session.GetHabbo().GetRoomSession().GetRoom();
            if (room != null && room.GaveRoomRights(session))
            {
                uint itemId = message.PopWiredUInt();
                string wallData = message.PopFixedString();

                InventoryItem item = session.GetHabbo().GetInventoryManager().TryGetWallItem(itemId);
                if (item != null)
                {
                    if (wallData.StartsWith(":"))
                    {
                        RoomItem roomItem = RoomItem.GetRoomItem(itemId, room.ID, session.GetHabbo().ID, item.BaseItem, item.ExtraData, 0, 0, 0.0, 0, new WallCoordinate(wallData), room);
                        if (room.RoomItemManager.AddWallItemToRoom(session, roomItem))
                        {
                            session.GetHabbo().GetInventoryManager().RemoveItemFromHand(itemId, false);
                        }

                        using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                        {
                            dbClient.AddParamWithValue("roomid", room.ID);
                            dbClient.AddParamWithValue("itemid", itemId);
                            dbClient.ExecuteQuery("UPDATE items SET room_id = @roomid WHERE id = @itemid LIMIT 1");
                        }

                        if (session.GetHabbo().ID != room.RoomData.OwnerID)
                        {
                            session.GetHabbo().GetUserStats().NotesLeft++;
                            session.GetHabbo().GetUserAchievements().CheckAchievement("NotesLeft");

                            GameClient roomOwner = Skylight.GetGame().GetGameClientManager().GetGameClientById(room.RoomData.OwnerID);
                            if (roomOwner != null)
                            {
                                roomOwner.GetHabbo().GetUserStats().NotesReceived++;
                                roomOwner.GetHabbo().GetUserAchievements().CheckAchievement("NotesReceived");
                            }
                            else
                            {
                                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                                {
                                    dbClient.AddParamWithValue("ownerId", room.RoomData.OwnerID);
                                    dbClient.ExecuteQuery("UPDATE user_stats SET notes_received = notes_received + 1 WHERE user_id = @ownerId LIMIT 1");
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
