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
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class NewNavigatorRoomsMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            string view = message.PopFixedString();
            string query = message.PopFixedString();

            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
            message_.Init(r63cOutgoing.NewNavigatorSearchResults);
            message_.AppendString(view);
            message_.AppendString(query);
            if (view == "official_view")
            {
                IEnumerable<PublicItem> items = Skylight.GetGame().GetNavigatorManager().GetPublicRoomItems().Where(i => i.Type != PublicItemType.CATEGORY && this.doFilter(query, i));
                IEnumerable<PublicItem> categorys = Skylight.GetGame().GetNavigatorManager().GetPublicRoomItems().Where(i => i.Type == PublicItemType.CATEGORY && items.Any(p => p.ParentCategoryID == i.ID));
                IEnumerable<PublicItem> publicRooms = items.Where(i => i.ParentCategoryID == 0);

                if (publicRooms.Count() > 0)
                {
                    message_.AppendInt32(categorys.Count() + 1); //count
                    message_.AppendString("official-root");
                    message_.AppendString(""); //text
                    message_.AppendInt32(0); //action allowed
                    message_.AppendBoolean(false); //closed
                    message_.AppendInt32(0); //display mode

                    message_.AppendInt32(publicRooms.Count());
                    foreach (PublicItem item in publicRooms)
                    {
                        item.RoomData.Serialize(message_, false);
                    }
                }
                else
                {
                    message_.AppendInt32(categorys.Count());
                }

                foreach (PublicItem item in categorys)
                {
                    message_.AppendString("");
                    message_.AppendString(item.Caption);
                    message_.AppendInt32(0);
                    message_.AppendBoolean(string.IsNullOrEmpty(query));
                    message_.AppendInt32(0);

                    IEnumerable<PublicItem> rooms = items.Where(i => i.ParentCategoryID == item.ID);
                    message_.AppendInt32(rooms.Count()); //count
                    foreach (PublicItem room in rooms)
                    {
                        room.RoomData.Serialize(message_, false);
                    }
                }
            }
            else if (view == "hotel_view")
            {
                IEnumerable<Room> rooms = Skylight.GetGame().GetRoomManager().LoadedRooms.Values.Where(i => this.doFilter(query, i.RoomData));
                IEnumerable<Room> popularRooms = rooms.OrderByDescending(r => r.RoomData.UsersNow).Take(50);

                HashSet<int> categorys = new HashSet<int>();
                foreach(int c in rooms.Select(r => r.RoomData.Category))
                {
                    categorys.Add(c);
                }

                if (!string.IsNullOrEmpty(query))
                {
                    DataTable results = null;
                    using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                    {
                        dbClient.AddParamWithValue("query", "%" + query.Replace("%", "\\%") + "%");
                        results = dbClient.ReadDataTable("SELECT r.* FROM rooms r LEFT JOIN users u ON r.ownerid = u.id WHERE r.type = 'private' AND (r.name LIKE @query OR u.username LIKE @query) ORDER BY r.users_now LIMIT 50");
                    }

                    if (results != null && results.Rows.Count > 0)
                    {
                        message_.AppendInt32(categorys.Count + (popularRooms.Count() > 0 ? 2 : 1));
                        message_.AppendString("");
                        message_.AppendString("Text Search"); //text
                        message_.AppendInt32(0); //action allowed
                        message_.AppendBoolean(false); //closed
                        message_.AppendInt32(0); //display mode

                        message_.AppendInt32(results.Rows.Count);
                        foreach (DataRow dataRow in results.Rows)
                        {
                            Skylight.GetGame().GetRoomManager().TryGetAndLoadRoomData((uint)dataRow["id"], dataRow).Serialize(message_, false);
                        }
                    }
                    else
                    {
                        message_.AppendInt32(categorys.Count + (popularRooms.Count() > 0 ? 1 : 0));
                    }
                }
                else
                {
                    message_.AppendInt32(categorys.Count + (popularRooms.Count() > 0 ? 1 : 0));
                }

                if (popularRooms.Count() > 0)
                {
                    message_.AppendString("popular");
                    message_.AppendString(""); //text
                    message_.AppendInt32(0); //action allowed
                    message_.AppendBoolean(false); //closed
                    message_.AppendInt32(0); //display mode

                    message_.AppendInt32(popularRooms.Count());
                    foreach (Room room in popularRooms)
                    {
                        room.RoomData.Serialize(message_, false);
                    }
                }

                foreach(int c in categorys)
                {
                    FlatCat flatCat = Skylight.GetGame().GetNavigatorManager().GetFlatCat(c);

                    message_.AppendString("");
                    message_.AppendString(flatCat.Caption);
                    message_.AppendInt32(0);
                    message_.AppendBoolean(string.IsNullOrEmpty(query));
                    message_.AppendInt32(0);

                    IEnumerable<Room> categoryRooms = rooms.Where(r => r.RoomData.Category == c).OrderByDescending(r => r.RoomData.UsersNow).Take(50);
                    message_.AppendInt32(categoryRooms.Count());
                    foreach(Room room in categoryRooms)
                    {
                        room.RoomData.Serialize(message_, false);
                    }
                }
            }
            else if (view == "myworld_view")
            {
                IEnumerable<RoomData> rooms = session.GetHabbo().UserRooms.Select(i => Skylight.GetGame().GetRoomManager().TryGetAndLoadRoomData(i)).Where(r => this.doFilter(query, r));

                message_.AppendInt32(1); //count
                message_.AppendString("my");
                message_.AppendString(""); //text
                message_.AppendInt32(0); //action allowed
                message_.AppendBoolean(false); //closed
                message_.AppendInt32(0); //display mode

                message_.AppendInt32(rooms.Count());
                foreach (RoomData room in rooms)
                {
                    room.Serialize(message_, false);
                }
            }
            else
            {
                message_.AppendInt32(0); //count
            }
            session.SendMessage(message_);
        }

        public bool doFilter(string query, PublicItem item)
        {
            return this.doFilter(query, item.RoomData);
        }

        public bool doFilter(string query, RoomData roomData)
        {
            if (!string.IsNullOrEmpty(query))
            {
                if (query.Contains(':'))
                {
                    string[] splitd = query.Split(':');
                    string filter = splitd[0];
                    string search = splitd[1];

                    if (!string.IsNullOrEmpty(search))
                    {
                        if (filter == "roomname")
                        {
                            if (roomData.Name.Contains(search))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else if (filter == "owner")
                        {
                            if (Skylight.GetGame().GetGameClientManager().GetUsernameByID(roomData.OwnerID).Contains(search))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else if (filter == "tag")
                        {
                            foreach (string tag in roomData.Tags)
                            {
                                if (tag.Contains(search))
                                {
                                    return true;
                                }
                            }

                            return false;
                        }
                        else if (filter == "group")
                        {
                            return false;
                        }
                        else
                        {
                            //??????
                            return true;
                        }
                    }
                    else
                    {
                        //??????
                        return true;
                    }
                }
                else
                {
                    if (roomData.Name.Contains(query))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return true;
            }
        }
    }
}
