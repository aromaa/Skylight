using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.HabboHotel.Support;
using SkylightEmulator.HabboHotel.Users.Messenger;
using SkylightEmulator.Messages;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Navigator
{
    public class NavigatorManager
    {
        private Dictionary<int, FlatCat> FlatCats;
        private Dictionary<int, PublicItem> PublicRooms;
        private ServerMessage RoomsWithHigestScoreCachedMessage;
        private DateTime RoomsWithHigestScoreCacheTime;

        private Dictionary<int, object> OldSchoolCategoryThingy;
        private Dictionary<KeyValuePair<int, bool>, int> OldSchoolCategoryThingy2;

        public NavigatorManager()
        {
            this.FlatCats = new Dictionary<int, FlatCat>();
            this.PublicRooms = new Dictionary<int,PublicItem>();
            this.OldSchoolCategoryThingy = new Dictionary<int, object>();
            this.OldSchoolCategoryThingy2 = new Dictionary<KeyValuePair<int, bool>, int>();
        }

        public void LoadPublicRooms(DatabaseClient dbClient)
        {
            Logging.Write("Loading navigator public rooms... ");
            Dictionary<int, PublicItem> newPublicRooms = new Dictionary<int, PublicItem>();

            foreach(KeyValuePair<int, object> item in this.OldSchoolCategoryThingy.ToList())
            {
                if (item.Value is PublicItem)
                {
                    this.OldSchoolCategoryThingy.Remove(item.Key);
                }
            }

            DataTable publicRooms = dbClient.ReadDataTable("SELECT id, bannertype, caption, image, image_type, type, room_id, category_parent_id FROM navigator_publics ORDER BY ordernum ASC");
            if (publicRooms != null)
            {
                foreach(DataRow dataRow in publicRooms.Rows)
                {
                    int id = (int)dataRow["id"];
                    PublicItem item = new PublicItem(id, int.Parse((string)dataRow["bannertype"]), (string)dataRow["caption"], (string)dataRow["image"], (string)dataRow["image_type"], (string)dataRow["type"], (uint)dataRow["room_id"], (int)dataRow["category_parent_id"]);
                    newPublicRooms.Add(id, item);
                    
                    for(int i = 5; ; i++)
                    {
                        object trash;
                        if (!this.OldSchoolCategoryThingy.TryGetValue(i, out trash))
                        {
                            this.OldSchoolCategoryThingy.Add(i, item);
                            this.OldSchoolCategoryThingy2.Add(new KeyValuePair<int, bool>(id, false), i);
                            break;
                        }
                    }
                }
            }

            this.PublicRooms = newPublicRooms;
            Logging.WriteLine("completed!", ConsoleColor.Green);
        }

        public void LoadFlatCats(DatabaseClient dbClient)
        {
            Logging.Write("Loading flat cats... ");
            Dictionary<int, FlatCat> newFlatCats = new Dictionary<int, FlatCat>();

            foreach (KeyValuePair<int, object> item in this.OldSchoolCategoryThingy.ToList())
            {
                if (item.Value is FlatCat)
                {
                    this.OldSchoolCategoryThingy.Remove(item.Key);
                }
            }

            DataTable flatCats = dbClient.ReadDataTable("SELECT id, caption, min_rank, can_trade FROM navigator_flatcats");
            if (flatCats != null)
            {
                foreach(DataRow dataRow in flatCats.Rows)
                {
                    int id = (int)dataRow["id"];
                    FlatCat item = new FlatCat(id, (string)dataRow["caption"], (int)dataRow["min_rank"], TextUtilies.StringToBool((string)dataRow["can_trade"]));
                    newFlatCats.Add(id, item);

                    for (int i = 5; ; i++)
                    {
                        object trash;
                        if (!this.OldSchoolCategoryThingy.TryGetValue(i, out trash))
                        {
                            this.OldSchoolCategoryThingy.Add(i, item);
                            this.OldSchoolCategoryThingy2.Add(new KeyValuePair<int, bool>(id, true), i);
                            break;
                        }
                    }
                }
            }

            this.FlatCats = newFlatCats;
            Logging.WriteLine("completed!", ConsoleColor.Green);
        }

        public FlatCat GetFlatCat(int id)
        {
            if (this.FlatCats.ContainsKey(id))
            {
                return this.FlatCats[id];
            }
            else
            {
                return new FlatCat(id, "Invalid flatcat", 0, false); //Fix invalid flatcats
            }
        }

        public ServerMessage GetPublicRooms()
        {
            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            Message.Init(r63aOutgoing.PublicRooms);
            Message.AppendInt32(this.PublicRooms.Count);
            foreach (PublicItem item in this.PublicRooms.Values.ToList())
            {
                if (item.Type == PublicItemType.CATEGORY)
                {
                    item.Serialize(Message);
                    foreach (PublicItem item_ in this.PublicRooms.Values.ToList())
                    {
                        if (item_.ParentCategoryID == item.ID)
                        {
                            item_.Serialize(Message);
                        }
                    }
                }
                else
                {
                    if (item.ParentCategoryID == 0 || item.ParentCategoryID == -1)
                    {
                        item.Serialize(Message);
                    }
                }
            }
            Message.AppendInt32(0);
            return Message;
        }

        public ServerMessage GetMyRooms(GameClient gameClient)
        {
            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            Message.Init(r63aOutgoing.MyRooms);
            Message.AppendInt32(3); //search type
            Message.AppendString(""); //unknown string
            Message.AppendInt32(gameClient.GetHabbo().UserRooms.Count);
            foreach(uint roomId in gameClient.GetHabbo().UserRooms)
            {
                Skylight.GetGame().GetRoomManager().TryGetAndLoadRoomData(roomId).Serialize(Message, false);
            }
            Message.AppendBoolean(false); //show featured room
            return Message;
        }

        public ServerMessage GetPopularRooms(GameClient session, int type)
        {
            if (type == -2)
            {
                TimeSpan span = (TimeSpan)(DateTime.Now - this.RoomsWithHigestScoreCacheTime);
                if (this.RoomsWithHigestScoreCachedMessage == null || span.TotalSeconds >= 60.0)
                {
                    this.RoomsWithHigestScoreCacheTime = DateTime.Now;
                    this.RoomsWithHigestScoreCachedMessage = this.GetPopularRoomsPost(null, type);
                }

                return this.RoomsWithHigestScoreCachedMessage;
            }
            else
            {
                return this.GetPopularRoomsPost(session, type);
            }
        }

        private ServerMessage GetPopularRoomsPost(GameClient session, int type)
        {
            ServerMessage popularRooms = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            popularRooms.Init(r63aOutgoing.Navigator);

            List<RoomData> rooms = new List<RoomData>();
            switch(type)
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
                            foreach(DataRow dataRow in dataTable.Rows)
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

                        foreach(Room room in Skylight.GetGame().GetRoomManager().GetLoadedRooms().Where(r => r.RoomData.UsersNow > 0).OrderByDescending(r =>  r.RoomData.UsersNow))
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

                        foreach(MessengerFriend friend in session.GetHabbo().GetMessenger().GetFriends())
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
                        if (type > -1)
                        {
                            popularRooms.AppendInt32(1);
                            popularRooms.AppendString(type.ToString());

                            rooms = Skylight.GetGame().GetRoomManager().GetLoadedRooms().Where(r => r.RoomData.Type == "private" && r.RoomData.UsersNow > 0 && r.RoomData.Category == type).OrderByDescending(r => r.RoomData.UsersNow).Take(50).Select(r => r.RoomData).ToList();
                        }
                        break;
                    }

            }

            popularRooms.AppendInt32(rooms.Count);
            foreach(RoomData room in rooms)
            {
                room.Serialize(popularRooms, false);
            }

            List<PublicItem> itemsThatsNotCategory = this.PublicRooms.Values.Where(i => i.Type != PublicItemType.CATEGORY).ToList();
            if (itemsThatsNotCategory.Count > 0)
            {
                popularRooms.AppendBoolean(true);
                itemsThatsNotCategory.ElementAt(new Random().Next(0, itemsThatsNotCategory.Count)).Serialize(popularRooms);
            }
            else
            {
                popularRooms.AppendBoolean(false);
            }

            return popularRooms;
        }

        public ServerMessage GetEvents(int type)
        {
            ServerMessage events = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            events.Init(r63aOutgoing.Navigator);
            events.AppendInt32(type);
            events.AppendString("");

            List<Room> rooms = Skylight.GetGame().GetRoomManager().GetLoadedRooms().Where(r => r != null && r.RoomData != null && r.RoomEvent != null && (type == -1 || r.RoomEvent.Category == type)).OrderByDescending(r => r.RoomData.UsersNow).Take(50).ToList();
            events.AppendInt32(rooms.Count);
            foreach (Room room in rooms)
            {
                room.RoomData.Serialize(events, true);
            }

            if (this.PublicRooms.Count > 0)
            {
                events.AppendBoolean(true);
                this.PublicRooms.Values.ElementAt(new Random().Next(0, this.PublicRooms.Count)).Serialize(events);
            }
            else
            {
                events.AppendBoolean(false);
            }
            return events;
        }

        public ServerMessage GetFlatCatsMessage(GameClient session)
        {
            return BasicUtilies.GetRevisionPacketManager(session.Revision).GetOutgoing(OutgoingPacketsEnum.FlatCats).Handle(new ValueHolder().AddValue("FlatCats", this.FlatCats.Values.ToList()).AddValue("Rank", session.GetHabbo().Rank));
        }

        public List<FlatCat> GetFlatCats()
        {
            return this.FlatCats.Values.ToList();
        }

        public ServerMessage GetPopularRoomTags()
        {
            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            Message.Init(r63aOutgoing.PopularRoomTags);

            List<RoomData> rooms = Skylight.GetGame().GetRoomManager().GetLoadedRooms().Where(r => r.RoomData.Type == "private" && r.RoomData.UsersNow > 0 && r.RoomData.Tags.Count > 0).OrderByDescending(r => r.RoomData.UsersNow).Take(50).Select(r => r.RoomData).ToList();
            Dictionary<string, int> tags = new Dictionary<string, int>();
            foreach(RoomData room in rooms)
            {
                foreach(string tag in room.Tags)
                {
                    if (tags.ContainsKey(tag))
                    {
                        tags[tag] += room.UsersNow;
                    }
                    else
                    {
                        tags[tag] = room.UsersNow;
                    }
                }
            }

            Message.AppendInt32(tags.Count);
            foreach(KeyValuePair<string, int> tag in tags)
            {
                Message.AppendString(tag.Key);
                Message.AppendInt32(tag.Value);
            }
            return Message;
        }

        public ServerMessage SearchRooms(string entry, Revision revision)
        {
            DataTable results = null;
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                if (entry.Length > 0)
                {
                    if (entry.StartsWith("owner:"))
                    {
                        dbClient.AddParamWithValue("owner", Skylight.GetGame().GetGameClientManager().GetIDByUsername(entry.Substring(6)));
                        results = dbClient.ReadDataTable("SELECT * FROM rooms WHERE type = 'private' AND ownerid = @owner ORDER BY users_now LIMIT 50");
                    }
                    else if (entry.StartsWith("tag:"))
                    {
                        dbClient.AddParamWithValue("query", "%" + entry.Substring(4).Replace("%", "\\%") + "%");
                        results = dbClient.ReadDataTable("SELECT * FROM rooms WHERE type = 'private' AND tags LIKE @query ORDER BY users_now LIMIT 50");
                    }
                    else
                    {
                        dbClient.AddParamWithValue("query", "%" + entry.Replace("%", "\\%") + "%");
                        results = dbClient.ReadDataTable("SELECT r.* FROM rooms r LEFT JOIN users u ON r.ownerid = u.id WHERE r.type = 'private' AND (r.name LIKE @query OR u.username LIKE @query) ORDER BY r.users_now LIMIT 50");
                    }
                }
            }

            List<RoomData> rooms = new List<RoomData>();
            if (results != null && results.Rows.Count > 0)
            {
                foreach(DataRow dataRow in results.Rows)
                {
                    RoomData room = Skylight.GetGame().GetRoomManager().TryGetAndLoadRoomData((uint)dataRow["id"], dataRow);
                    if (room != null)
                    {
                        rooms.Add(room);
                    }
                }
            }

            return BasicUtilies.GetRevisionPacketManager(revision).GetOutgoing(OutgoingPacketsEnum.RoomSearch).Handle(new ValueHolder("Entry", entry, "Rooms", rooms, "PublicRoom", this.PublicRooms.Count > 0 ? this.PublicRooms.Values.ElementAt(new Random().Next(0, this.PublicRooms.Count)) : null));
        }

        public ServerMessage GetMyRoomHistory(GameClient session)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            message.Init(r63aOutgoing.Navigator);
            message.AppendInt32(0);
            message.AppendString("");

            List<Roomvisit> rooms = Skylight.GetGame().GetRoomvisitManager().GetRoomvisits(session.GetHabbo().ID);
            message.AppendInt32(rooms.Count);
            foreach(Roomvisit roomvisit in rooms)
            {
                Skylight.GetGame().GetRoomManager().TryGetAndLoadRoomData(roomvisit.RoomID).Serialize(message, false);
            }
            return message;
        }

        public ServerMessage GetFavouriteRooms(GameClient session)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            message.Init(r63aOutgoing.Navigator);
            message.AppendInt32(0);
            message.AppendString("");
            message.AppendInt32(session.GetHabbo().FavouriteRooms.Count);
            foreach(uint roomId in session.GetHabbo().FavouriteRooms)
            {
                Skylight.GetGame().GetRoomManager().TryGetAndLoadRoomData(roomId).Serialize(message, false);
            }
            return message;
        }

        public void Shutdown()
        {
            if (this.FlatCats != null)
            {
                this.FlatCats.Clear();
            }
            this.FlatCats = null;

            if (this.PublicRooms != null)
            {
                this.PublicRooms.Clear();
            }
            this.PublicRooms = null;

            this.RoomsWithHigestScoreCachedMessage = null;
        }

        public PublicItem GetPublicItem(uint roomId, int category)
        {
            return this.PublicRooms.Values.FirstOrDefault(p => p.RoomID == roomId && p.ParentCategoryID == category);
        }

        public Dictionary<int, PublicItem>.ValueCollection GetPublicRoomItems()
        {
            return this.PublicRooms.Values;
        }

        public Dictionary<int, object> GetOldSchoolCategoryThingy()
        {
            return this.OldSchoolCategoryThingy;
        }

        public Dictionary<KeyValuePair<int, bool>, int> GetOldSchoolCategoryThingy2()
        {
            return this.OldSchoolCategoryThingy2;
        }
    }
}
