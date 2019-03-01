using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class GetRoomEntryDataMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null && session.GetHabbo().GetRoomSession().RequestedRoomID > 0 && session.GetHabbo().GetRoomSession().LoadingRoom)
            {
                Room room = Skylight.GetGame().GetRoomManager().GetRoom(session.GetHabbo().GetRoomSession().RequestedRoomID);
                session.GetHabbo().GetRoomSession().ResetRequestedRoom();
                if (room != null)
                {
                    if (room.RoomGamemapManager.Model != null)
                    {
                        session.SendMessage(room.RoomGamemapManager.Model.GetHeightmap());
                        session.SendMessage(room.RoomGamemapManager.Model.GetRelativeHeightmap());

                        ServerMessage publicItems = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                        publicItems.Init(r63aOutgoing.PublicItems);
                        if (!string.IsNullOrEmpty(room.RoomGamemapManager.Model.PublicItems))
                        {
                            publicItems.AppendStringWithBreak(room.RoomGamemapManager.Model.PublicItems);
                        }
                        else
                        {
                            publicItems.AppendInt32(0);
                        }
                        session.SendMessage(publicItems);

                        if (room.RoomData.Type == "private")
                        {
                            ServerMessage floorItems = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                            floorItems.Init(r63aOutgoing.FloorItems);
                            floorItems.AppendInt32(room.RoomItemManager.FloorItems.Count);
                            foreach(RoomItem item in room.RoomItemManager.FloorItems.ToList())
                            {
                                item.Serialize(floorItems);
                            }
                            session.SendMessage(floorItems);

                            ServerMessage wallItems = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                            wallItems.Init(r63aOutgoing.WallItems);
                            wallItems.AppendInt32(room.RoomItemManager.WallItems.Count);
                            foreach(RoomItem item in room.RoomItemManager.WallItems.ToList())
                            {
                                item.Serialize(wallItems);
                            }
                            session.SendMessage(wallItems);
                        }

                        room.RoomUserManager.EnterRoom(session);

                        List<RoomUser> users = room.RoomUserManager.GetRealUsers();
                        ServerMessage setRoomUsers = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                        setRoomUsers.Init(r63aOutgoing.SetRoomUser);
                        setRoomUsers.AppendInt32(users.Count);
                        foreach(RoomUser user in users)
                        {
                            user.Serialize(setRoomUsers);
                        }
                        session.SendMessage(setRoomUsers);

                        ServerMessage roomVIPsetting = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                        roomVIPsetting.Init(r63aOutgoing.RoomVIPSettings);
                        roomVIPsetting.AppendBoolean(room.RoomData.Hidewalls);
                        roomVIPsetting.AppendInt32(room.RoomData.Wallthick);
                        roomVIPsetting.AppendInt32(room.RoomData.Floorthick);
                        session.SendMessage(roomVIPsetting);

                        if (room.RoomData.Type == "private")
                        {
                            ServerMessage roomOwner = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                            roomOwner.Init(r63aOutgoing.RoomOwner);
                            roomOwner.AppendBoolean(true); //is private room
                            roomOwner.AppendUInt(room.ID);
                            roomOwner.AppendBoolean(room.IsOwner(session)); //is roomowner
                            session.SendMessage(roomOwner);

                            ServerMessage roomData = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                            roomData.Init(r63aOutgoing.RoomData);
                            roomData.AppendBoolean(false); //entered room
                            room.RoomData.Serialize(roomData, false);
                            roomData.AppendBoolean(false); //forward
                            roomData.AppendBoolean(false); //is staff pick
                            session.SendMessage(roomData);
                        }
                        else
                        {
                            ServerMessage roomOwner = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                            roomOwner.Init(r63aOutgoing.RoomOwner);
                            roomOwner.AppendBoolean(false); //is private room
                            roomOwner.AppendStringWithBreak(room.RoomGamemapManager.Model.ID);
                            roomOwner.AppendInt32(0); //unknwown
                            session.SendMessage(roomOwner);
                        }

                        ServerMessage usersStatuses = room.RoomUserManager.GetUserStatus(true);
                        if (usersStatuses != null)
                        {
                            session.SendMessage(usersStatuses);
                        }

                        //show other users stuff
                    }
                    else
                    {
                        session.SendNotif("Failed load room model!");
                    }
                }
                else
                {
                    session.SendNotif("Room not found!");
                }
            }
        }
    }
}
