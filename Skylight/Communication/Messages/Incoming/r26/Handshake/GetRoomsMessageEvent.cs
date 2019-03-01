using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Navigator;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r26.Handshake
{
    class GetRoomsMessageEvent : IncomingPacket
    {
        public const int PUBLIC = 0;
        public const int PRIVATE = 2;

        public const int NONE = 0;
        public const int PUBLIC_ROOMS = 3;
        public const int PRIVATE_ROOMS = 4;

        public void Handle(GameClient session, ClientMessage message)
        {
            bool hideFull = message.PopWiredBoolean();
            int caterogyId = message.PopWiredInt32();

            if (caterogyId == GetRoomsMessageEvent.PRIVATE_ROOMS) //private rooms
            {
                ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                Message.Init(r26Outgoing.SendRooms);
                Message.AppendBoolean(hideFull);
                Message.AppendInt32(caterogyId);
                Message.AppendInt32(GetRoomsMessageEvent.PRIVATE); //type
                Message.AppendString("Guestrooms"); //caption
                Message.AppendInt32(0); //unknown
                Message.AppendInt32(10000); //unknown
                Message.AppendInt32(GetRoomsMessageEvent.NONE); //parent id
                Message.AppendInt32(0); //rooms count

                foreach (FlatCat flatCat in Skylight.GetGame().GetNavigatorManager().GetFlatCats())
                {
                    int usersNow = 0;
                    int usersMax = 0;
                    foreach(RoomData room in Skylight.GetGame().GetRoomManager().GetLoadedRooms().Where(r => r.RoomData.Type == "private" && r.RoomData.UsersNow > 0 && r.RoomData.Category == flatCat.Id).OrderByDescending(r => r.RoomData.UsersNow).Take(50).Select(r => r.RoomData).ToList())
                    {
                        usersNow += room.UsersNow;
                        usersMax += room.UsersMax;
                    }

                    Message.AppendInt32(Skylight.GetGame().GetNavigatorManager().GetOldSchoolCategoryThingy2()[new KeyValuePair<int, bool>(flatCat.Id, true)]); //id
                    Message.AppendBoolean(false); //is room
                    Message.AppendString(flatCat.Caption);
                    Message.AppendInt32(usersNow); //users now
                    Message.AppendInt32(usersMax); //users max
                    Message.AppendInt32(GetRoomsMessageEvent.PRIVATE_ROOMS); //parent id
                }

                session.SendMessage(Message);
            }
            else if (caterogyId == GetRoomsMessageEvent.PUBLIC_ROOMS) //public rooms
            {
                ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                Message.Init(r26Outgoing.SendRooms);
                Message.AppendBoolean(hideFull);
                Message.AppendInt32(caterogyId);
                Message.AppendInt32(GetRoomsMessageEvent.PUBLIC); //type
                Message.AppendString("Publics"); //caption
                Message.AppendInt32(0); //unknown
                Message.AppendInt32(10000); //unknown
                Message.AppendInt32(GetRoomsMessageEvent.NONE); //parent id

                foreach (PublicItem category in Skylight.GetGame().GetNavigatorManager().GetPublicRoomItems().Where(i => i.Type == PublicItemType.CATEGORY && i.ParentCategoryID == 0))
                {
                    int usersNow = 0;
                    int usersMax = 0;
                    foreach (PublicItem item in Skylight.GetGame().GetNavigatorManager().GetPublicRoomItems().Where(i => i.ParentCategoryID == category.ID))
                    {
                        usersNow += item.RoomData.UsersNow;
                        usersMax += item.RoomData.UsersMax;
                    }

                    Message.AppendInt32(Skylight.GetGame().GetNavigatorManager().GetOldSchoolCategoryThingy2()[new KeyValuePair<int, bool>(category.ID, false)]); //id
                    Message.AppendBoolean(false); //is room
                    Message.AppendString(category.Caption); //name
                    Message.AppendInt32(usersNow); //users now
                    Message.AppendInt32(usersMax); //users max
                    Message.AppendInt32(GetRoomsMessageEvent.PUBLIC_ROOMS); //parent id
                }

                foreach (PublicItem item in Skylight.GetGame().GetNavigatorManager().GetPublicRoomItems().Where(i => i.Type != PublicItemType.CATEGORY && i.ParentCategoryID == 0))
                {
                    Message.AppendUInt(item.RoomData.ID); //id
                    Message.AppendBoolean(true); //is room
                    Message.AppendString(item.RoomData.Name); //name
                    Message.AppendInt32(item.RoomData.UsersNow); //visitors now
                    Message.AppendInt32(item.RoomData.UsersMax); //visitors max
                    Message.AppendInt32(caterogyId); //category inside
                    Message.AppendString(item.RoomData.Description); //desc
                    Message.AppendUInt(item.RoomData.ID); //id
                    Message.AppendBoolean(false); //unknwon
                    Message.AppendString(item.RoomData.PublicCCTs); //ccts
                    Message.AppendBoolean(false); //unknwon
                    Message.AppendBoolean(true); //unknwon
                }
                
                session.SendMessage(Message);
            }
            else //private room category
            {
                object category;
                Skylight.GetGame().GetNavigatorManager().GetOldSchoolCategoryThingy().TryGetValue(caterogyId, out category);

                if (category is FlatCat)
                {
                    FlatCat flatCat = (FlatCat)category;

                    ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                    Message.Init(r26Outgoing.SendRooms);
                    Message.AppendBoolean(hideFull);
                    Message.AppendInt32(caterogyId);
                    Message.AppendInt32(GetRoomsMessageEvent.PRIVATE); //type
                    Message.AppendString(flatCat.Caption);
                    Message.AppendInt32(0); //unknown
                    Message.AppendInt32(10000); //unknown
                    Message.AppendInt32(GetRoomsMessageEvent.PRIVATE_ROOMS); //parent id

                    List<Room> rooms = Skylight.GetGame().GetRoomManager().GetLoadedRooms().Where(r => !r.RoomData.IsPublicRoom && r.RoomData.Category == flatCat.Id).Take(50).ToList();

                    Message.AppendInt32(rooms.Count);
                    foreach (Room room in rooms)
                    {
                        Message.AppendUInt(room.RoomData.ID); //id
                        Message.AppendString(room.RoomData.Name); //name
                        Message.AppendString(Skylight.GetGame().GetGameClientManager().GetUsernameByID(room.RoomData.OwnerID)); //owner
                        Message.AppendString(room.RoomData.State == RoomStateType.OPEN ? "open" : room.RoomData.State == RoomStateType.LOCKED ? "closed" : "password");
                        Message.AppendInt32(room.RoomData.UsersNow); //visitors now
                        Message.AppendInt32(room.RoomData.UsersMax); //visitors max
                        Message.AppendString(room.RoomData.Description); //desc
                    }

                    session.SendMessage(Message);
                }
                else if (category is PublicItem)
                {
                    PublicItem item = (PublicItem)category;

                    if (item.Type == PublicItemType.CATEGORY)
                    {
                        ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                        Message.Init(r26Outgoing.SendRooms);
                        Message.AppendBoolean(hideFull);
                        Message.AppendInt32(caterogyId);
                        Message.AppendInt32(GetRoomsMessageEvent.PUBLIC); //type
                        Message.AppendString(item.Caption); //caption
                        Message.AppendInt32(0); //unknown
                        Message.AppendInt32(10000); //unknown
                        Message.AppendInt32(GetRoomsMessageEvent.PUBLIC_ROOMS); //parent id
                        
                        foreach (PublicItem item_ in Skylight.GetGame().GetNavigatorManager().GetPublicRoomItems().Where(i => i.Type != PublicItemType.CATEGORY && i.ParentCategoryID == item.ID))
                        {
                            Message.AppendUInt(item_.RoomData.ID); //id
                            Message.AppendBoolean(true); //is room
                            Message.AppendString(item_.RoomData.Name); //name
                            Message.AppendInt32(item_.RoomData.UsersNow); //visitors now
                            Message.AppendInt32(item_.RoomData.UsersMax); //visitors max
                            Message.AppendInt32(caterogyId); //category inside
                            Message.AppendString(item_.RoomData.Description); //desc
                            Message.AppendUInt(item_.RoomData.ID); //id
                            Message.AppendBoolean(false); //unknwon
                            Message.AppendString(item_.RoomData.PublicCCTs); //ccts
                            Message.AppendBoolean(false); //unknwon
                            Message.AppendBoolean(true); //unknwon
                        }

                        session.SendMessage(Message);
                    }
                }
            }
        }
    }
}
