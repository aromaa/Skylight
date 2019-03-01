using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using SkylightEmulator.Messages.MultiRevision;
using SkylightEmulator.Messages.NewCrypto;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63b
{
    class GetRoomEntryDataMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null && session.GetHabbo().GetRoomSession().RequestedRoomID > 0 && session.GetHabbo().GetRoomSession().LoadingRoom)
            {
                Room room = Skylight.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().GetRoomSession().RequestedRoomID);
                session.GetHabbo().GetRoomSession().ResetRequestedRoom();
                if (room != null)
                {
                    if (room.RoomGamemapManager.Model != null)
                    {
                        session.SendData(room.RoomGamemapManager.Model.GetHeightmap(session.Revision, room.RoomGamemapManager.Tiles));
                        session.SendData(room.RoomGamemapManager.Model.GetRelativeHeightmap(session.Revision));

                        //TODO: Fix public items
                        ServerMessage publicItems = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
                        publicItems.Init(r63bOutgoing.PublicItems);
                        publicItems.AppendInt32(0);
                        session.SendMessage(publicItems);

                        if (room.RoomData.Type == "private")
                        {
                            Dictionary<uint, string> floorItemOwners = new Dictionary<uint, string>();
                            List<byte> tempHolder = new List<byte>();
                            foreach (RoomItem item in room.RoomItemManager.FloorItems.Values)
                            {
                                if (!floorItemOwners.ContainsKey(item.UserID))
                                {
                                    floorItemOwners.Add(item.UserID, Skylight.GetGame().GetGameClientManager().GetUsernameByID(item.UserID));
                                }

                                ServerMessage temp = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
                                temp.Init();
                                item.Serialize(temp);
                                tempHolder.AddRange(temp.GetBytes());
                            }

                            ServerMessage floorItems = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
                            floorItems.Init(r63bOutgoing.FloorItems);
                            floorItems.AppendInt32(floorItemOwners.Count);
                            foreach(KeyValuePair<uint, string> floorItemOwner in floorItemOwners)
                            {
                                floorItems.AppendUInt(floorItemOwner.Key);
                                floorItems.AppendString(floorItemOwner.Value);
                            }
                            floorItems.AppendInt32(room.RoomItemManager.FloorItems.Values.Count);
                            floorItems.AppendBytes(tempHolder);
                            session.SendMessage(floorItems);

                            tempHolder = new List<byte>();
                            Dictionary<uint, string> wallItemOwners = new Dictionary<uint, string>();
                            foreach (RoomItem item in room.RoomItemManager.WallItems.Values)
                            {
                                if (!wallItemOwners.ContainsKey(item.UserID))
                                {
                                    wallItemOwners.Add(item.UserID, Skylight.GetGame().GetGameClientManager().GetUsernameByID(item.UserID));
                                }

                                ServerMessage temp = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
                                temp.Init();
                                item.Serialize(temp);
                                tempHolder.AddRange(temp.GetBytes());
                            }

                            ServerMessage wallItems = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
                            wallItems.Init(r63bOutgoing.WallItems);
                            wallItems.AppendInt32(wallItemOwners.Count);
                            foreach(KeyValuePair<uint, string> wallItemOwner in wallItemOwners)
                            {
                                wallItems.AppendUInt(wallItemOwner.Key);
                                wallItems.AppendString(wallItemOwner.Value);
                            }
                            wallItems.AppendInt32(room.RoomItemManager.WallItems.Values.Count);
                            wallItems.AppendBytes(tempHolder);
                            session.SendMessage(wallItems);
                        }

                        room.RoomUserManager.EnterRoom(session);

                        ICollection<RoomUnit> users = room.RoomUserManager.GetRoomUsers();
                        ServerMessage setRoomUsers = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
                        setRoomUsers.Init(r63bOutgoing.SetRoomUser);
                        setRoomUsers.AppendInt32(users.Count);
                        foreach (RoomUnit user in users)
                        {
                            user.Serialize(setRoomUsers);
                        }
                        session.SendMessage(setRoomUsers);

                        ServerMessage roomVIPsetting = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
                        roomVIPsetting.Init(r63bOutgoing.RoomVIPSettings);
                        roomVIPsetting.AppendBoolean(room.RoomData.Hidewalls);
                        roomVIPsetting.AppendInt32(room.RoomData.Wallthick);
                        roomVIPsetting.AppendInt32(room.RoomData.Floorthick);
                        session.SendMessage(roomVIPsetting);

                        if (room.RoomData.Type == "private")
                        {
                            ServerMessage roomOwner = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
                            roomOwner.Init(r63bOutgoing.RoomOwner);
                            roomOwner.AppendBoolean(true); //is private room
                            roomOwner.AppendUInt(room.ID);
                            roomOwner.AppendBoolean(room.HaveOwnerRights(session)); //is roomowner
                            session.SendMessage(roomOwner);

                            ServerMessage roomData = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
                            roomData.Init(r63bOutgoing.RoomData);
                            roomData.AppendBoolean(false); //entered room
                            room.RoomData.Serialize(roomData, false);
                            roomData.AppendBoolean(false); //forward
                            roomData.AppendBoolean(room.RoomData.IsStaffPick); //is staff pick
                            roomData.AppendBoolean(true);
                            session.SendMessage(roomData);
                        }
                        else
                        {
                            ServerMessage roomOwner = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
                            roomOwner.Init(r63bOutgoing.RoomOwner);
                            roomOwner.AppendBoolean(false); //is private room
                            roomOwner.AppendString(room.RoomGamemapManager.Model.ID);
                            roomOwner.AppendInt32(0); //unknwown
                            session.SendMessage(roomOwner);
                        }

                        MultiRevisionServerMessage usersStatuses = room.RoomUserManager.GetUserStatus(true);
                        if (usersStatuses != null)
                        {
                            session.SendData(usersStatuses.GetBytes(session.Revision));
                        }

                        foreach (RoomUnitUser user in room.RoomUserManager.GetRealUsers())
                        {
                            if (user.Dancing)
                            {
                                ServerMessage danceMessage = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
                                danceMessage.Init(r63bOutgoing.Dance);
                                danceMessage.AppendInt32(user.VirtualID);
                                danceMessage.AppendInt32(user.DanceID);
                                session.SendMessage(danceMessage);
                            }

                            if (user.Sleeps)
                            {
                                ServerMessage sleeps = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
                                sleeps.Init(r63bOutgoing.Sleeps);
                                sleeps.AppendInt32(user.VirtualID);
                                sleeps.AppendBoolean(true);
                                session.SendMessage(sleeps);
                            }

                            if (user.Handitem > 0 && user.HanditemTimer > 0)
                            {
                                ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
                                message_.Init(r63bOutgoing.Handitem);
                                message_.AppendInt32(user.VirtualID);
                                message_.AppendInt32(user.Handitem);
                                session.SendMessage(message_);
                            }

                            if (user.ActiveEffect > 0)
                            {
                                ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
                                Message.Init(r63bOutgoing.Effect);
                                Message.AppendInt32(user.VirtualID);
                                Message.AppendInt32(user.ActiveEffect);
                                session.SendMessage(Message);
                            }
                        }

                        room.RoomWiredManager.UserEnterRoom(session.GetHabbo().GetRoomSession().GetRoomUser());

                        if (session.GetHabbo().IsMuted())
                        {
                            ServerMessage flood = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
                            flood.Init(r63bOutgoing.Flood);
                            flood.AppendInt32(session.GetHabbo().MuteTimeLeft());
                            session.SendMessage(flood);
                        }
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
