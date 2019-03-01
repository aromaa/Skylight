using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Rooms;
using System.Data;
using SkylightEmulator.Storage;
using SkylightEmulator.HabboHotel.Users.Messenger;
using SkylightEmulator.HabboHotel.Navigator;

namespace SkylightEmulator.Communication.Messages.Incoming.r63b
{
    class GetRoomsMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            int category = message.PopFixedInt32();

            ServerMessage popularRooms = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
            popularRooms.Init(r63bOutgoing.Navigator);

            List<RoomData> rooms = new List<RoomData>();
            switch (category)
            {
                case -1:
                    {
                        popularRooms.AppendInt32(1);
                        popularRooms.AppendString("-1");

                        rooms = Skylight.GetGame().GetRoomManager().GetLoadedRooms().Where(r => r != null && r.RoomData != null && r.RoomData.Type == "private" && r.RoomData.UsersNow > 0).OrderByDescending(r => r.RoomData.UsersNow).Take(50).Select(r => r.RoomData).ToList();
                        break;
                    }
                case -2:
                    {
                        popularRooms.AppendInt32(2);
                        popularRooms.AppendString("");

                        DataTable dataTable = null;
                        using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                        {
                            dataTable = dbClient.ReadDataTable("SELECT * FROM rooms WHERE score > 0 AND type = 'private' ORDER BY score DESC LIMIT 50");
                        }

                        if (dataTable != null && dataTable.Rows.Count > 0)
                        {
                            foreach (DataRow dataRow in dataTable.Rows)
                            {
                                rooms.Add(Skylight.GetGame().GetRoomManager().TryGetAndLoadRoomData((uint)dataRow["id"], dataRow));
                            }
                        }
                        break;
                    }
                case -4:
                    {
                        popularRooms.AppendInt32(0);
                        popularRooms.AppendString("");

                        foreach (Room room in Skylight.GetGame().GetRoomManager().GetLoadedRooms().Where(r => r.RoomData.UsersNow > 0).OrderByDescending(r => r.RoomData.UsersNow))
                        {
                            if (session.GetHabbo().GetMessenger().GetFriends().Any(f => f.ID == room.RoomData.OwnerID))
                            {
                                rooms.Add(room.RoomData);

                                if (rooms.Count >= 50)
                                {
                                    break;
                                }
                            }
                        }
                        break;
                    }
                case -5:
                    {
                        popularRooms.AppendInt32(0);
                        popularRooms.AppendString("");

                        foreach (MessengerFriend friend in session.GetHabbo().GetMessenger().GetFriends())
                        {
                            if (friend.InRoom)
                            {
                                GameClient friend_ = Skylight.GetGame().GetGameClientManager().GetGameClientById(friend.ID);
                                rooms.Add(friend_.GetHabbo().GetRoomSession().GetRoom().RoomData);

                                if (rooms.Count >= 50)
                                {
                                    break;
                                }
                            }
                        }
                        break;
                    }
                default:
                    {
                        //category search
                        if (category > -1)
                        {
                            popularRooms.AppendInt32(1);
                            popularRooms.AppendString(category.ToString());

                            rooms = Skylight.GetGame().GetRoomManager().GetLoadedRooms().Where(r => r.RoomData.Type == "private" && r.RoomData.UsersNow > 0 && r.RoomData.Category == category).OrderByDescending(r => r.RoomData.UsersNow).Take(50).Select(r => r.RoomData).ToList();
                        }
                        break;
                    }

            }

            popularRooms.AppendInt32(rooms.Count);
            foreach (RoomData room in rooms)
            {
                room.Serialize(popularRooms, false);
            }

            List<PublicItem> itemsThatsNotCategory = Skylight.GetGame().GetNavigatorManager().GetPublicRoomItems().Where(i => i.Type != PublicItemType.CATEGORY).ToList();
            if (itemsThatsNotCategory.Count > 0)
            {
                popularRooms.AppendBoolean(true);
                itemsThatsNotCategory.ElementAt(new Random().Next(0, itemsThatsNotCategory.Count)).Serialize(popularRooms);
            }
            else
            {
                popularRooms.AppendBoolean(false);
            }

            session.SendMessage(popularRooms);
        }
    }
}
