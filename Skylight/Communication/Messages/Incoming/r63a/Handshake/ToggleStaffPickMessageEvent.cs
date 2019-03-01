using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Navigator;
using SkylightEmulator.HabboHotel.Rooms;
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
    class ToggleStaffPickMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session.GetHabbo().HasPermission("acc_staffpick"))
            {
                uint roomId = message.PopWiredUInt();
                bool pick = message.PopWiredBoolean();

                Room room = Skylight.GetGame().GetRoomManager().TryGetAndLoadRoom(roomId);
                if (room != null)
                {
                    PublicItem item = Skylight.GetGame().GetNavigatorManager().GetPublicItem(roomId, ServerConfiguration.StaffPicksCategoryId);
                    if (item == null)
                    {
                        GameClient roomOwner = Skylight.GetGame().GetGameClientManager().GetGameClientById(room.RoomData.OwnerID);
                        using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                        {
                            dbClient.AddParamWithValue("roomId", room.ID);
                            dbClient.AddParamWithValue("caption", room.RoomData.Name);
                            dbClient.AddParamWithValue("categoryId", ServerConfiguration.StaffPicksCategoryId);
                            dbClient.ExecuteQuery("INSERT INTO navigator_publics(ordernum, bannertype, caption, image, type, room_id, category_parent_id, image_type) VALUES(0, '1', @caption, '', 'FLAT', @roomId, @categoryId, 'internal')");

                            Skylight.GetGame().GetNavigatorManager().LoadPublicRooms(dbClient);

                            if (roomOwner == null)
                            {
                                dbClient.AddParamWithValue("roomOwner", room.RoomData.OwnerID);
                                dbClient.ExecuteQuery("UPDATE user_stats SET staff_picks = staff_picks + 1 WHERE user_id = @roomOwner LIMIT 1");
                            }
                        }

                        if (roomOwner != null)
                        {
                            roomOwner.GetHabbo().GetUserStats().StaffPicks++;
                            roomOwner.GetHabbo().GetUserAchievements().CheckAchievement("StaffPick");
                        }
                    }
                    else
                    {
                        using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                        {
                            dbClient.AddParamWithValue("roomId", room.ID);
                            dbClient.AddParamWithValue("categoryId", ServerConfiguration.StaffPicksCategoryId);
                            dbClient.ExecuteQuery("DELETE FROM navigator_publics WHERE room_id = @roomId AND category_parent_id = @categoryId LIMIT 1");

                            Skylight.GetGame().GetNavigatorManager().LoadPublicRooms(dbClient);
                        }
                    }

                    ServerMessage roomData = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                    roomData.Init(r63aOutgoing.RoomData);
                    roomData.AppendBoolean(false); //entered room
                    room.RoomData.Serialize(roomData, false);
                    roomData.AppendBoolean(false); //forward
                    roomData.AppendBoolean(room.RoomData.IsStaffPick); //is staff pick
                    room.SendToAll(roomData);
                }
            }
        }
    }
}
