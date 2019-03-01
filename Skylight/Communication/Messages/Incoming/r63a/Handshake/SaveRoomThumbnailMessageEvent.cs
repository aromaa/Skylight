using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
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
    class SaveRoomThumbnailMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null && session.GetHabbo().GetRoomSession() != null)
            {
                Room room = Skylight.GetGame().GetRoomManager().GetRoom(session.GetHabbo().GetRoomSession().CurrentRoomID);
                if (room != null && room.IsOwner(session))
                {
                    uint roomId = message.PopWiredUInt();

                    int backgroundImage = message.PopWiredInt32();
                    int foregroundImage = message.PopWiredInt32();

                    if (backgroundImage >= 1 && backgroundImage <= 24 && (foregroundImage >= 0 && foregroundImage <= 11)) //valid images
                    {
                        Dictionary<int, int> items = new Dictionary<int, int>();
                        int itemsCount = message.PopWiredInt32();

                        for (int i = 0; i < itemsCount; i++)
                        {
                            int key = message.PopWiredInt32();
                            int value = message.PopWiredInt32();
                            if (key < 0 || key > 10 || (value < 1 || value > 27) || items.ContainsKey(key))
                            {
                                continue;
                            }

                            items.Add(key, value);
                        }

                        RoomIcon roomIcon = new RoomIcon(backgroundImage, foregroundImage, items);
                        using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                        {
                            dbClient.AddParamWithValue("roomid", room.ID);
                            dbClient.AddParamWithValue("icon_bg", backgroundImage);
                            dbClient.AddParamWithValue("icon_fg", foregroundImage);
                            dbClient.AddParamWithValue("icon_items", roomIcon.ItemsToString());

                            dbClient.ExecuteQuery("UPDATE rooms SET icon_bg = @icon_bg, icon_fg = @icon_fg, icon_items = @icon_items WHERE id = @roomid LIMIT 1");
                        }

                        room.RoomData.RoomIcon = roomIcon;

                        ServerMessage roomThumbnailUpdated = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                        roomThumbnailUpdated.Init(r63aOutgoing.RoomThumbnailUpdated);
                        roomThumbnailUpdated.AppendUInt(room.ID);
                        roomThumbnailUpdated.AppendUInt(room.ID);
                        room.SendToAll(roomThumbnailUpdated, null);

                        ServerMessage roomUpdateOK = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                        roomUpdateOK.Init(r63aOutgoing.RoomUpdateOK);
                        roomUpdateOK.AppendUInt(room.ID);
                        room.SendToAll(roomUpdateOK, null);

                        ServerMessage roomData = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                        roomData.Init(r63aOutgoing.RoomData);
                        roomData.AppendBoolean(false); //entered room
                        room.RoomData.Serialize(roomData, false);
                        roomData.AppendBoolean(false); //forward
                        roomData.AppendBoolean(false); //is staff pick
                        room.SendToAll(roomData, null);
                    }
                }
            }
        }
    }
}
