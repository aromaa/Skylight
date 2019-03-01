using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
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
    class ApplyRoomEffectMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            Room room = session.GetHabbo().GetRoomSession().GetRoom();
            if (room != null && room.HaveOwnerRights(session))
            {
                uint itemId = message.PopWiredUInt();
                InventoryItem item = session.GetHabbo().GetInventoryManager().TryGetWallItem(itemId);
                if (item != null)
                {
                    if (item.GetItem().InteractionType.ToLower() == "roomeffect") //try avoid some cheaty things :D
                    {
                        string type = "";
                        if (item.GetItem().ItemName.Contains("floor"))
                        {
                            type = "floor";
                            room.RoomData.Floor = item.ExtraData;

                            session.GetHabbo().GetUserStats().FloorDesigner++;
                            session.GetHabbo().GetUserAchievements().CheckAchievement("FloorDesigner");
                        }
                        else if (item.GetItem().ItemName.Contains("wallpaper"))
                        {
                            type = "wallpaper";
                            room.RoomData.Wallpaper = item.ExtraData;

                            session.GetHabbo().GetUserStats().WallDesigner++;
                            session.GetHabbo().GetUserAchievements().CheckAchievement("WallDesigner");
                        }
                        else if (item.GetItem().ItemName.Contains("landscape"))
                        {
                            type = "landscape";
                            room.RoomData.Landscape = item.ExtraData;

                            session.GetHabbo().GetUserStats().LandscapeDesigner++;
                            session.GetHabbo().GetUserAchievements().CheckAchievement("LandscapeDesigner");
                        }

                        if (!string.IsNullOrEmpty(type))
                        {
                            session.GetHabbo().GetInventoryManager().RemoveItemFromHand(item.ID, true);

                            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                            {
                                dbClient.AddParamWithValue("id", room.ID);
                                dbClient.AddParamWithValue("extraData", item.ExtraData);
                                dbClient.ExecuteQuery("UPDATE rooms SET " + type + " = @extraData WHERE id = @id LIMIT 1");
                            }

                            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                            message_.Init(r63aOutgoing.ApplyRoomEffect);
                            message_.AppendString(type);
                            message_.AppendString(item.ExtraData);
                            room.SendToAll(message_);
                        }
                    }
                }
            }
        }
    }
}
