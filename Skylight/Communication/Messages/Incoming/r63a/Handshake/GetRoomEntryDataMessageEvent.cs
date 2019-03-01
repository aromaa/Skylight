using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using SkylightEmulator.Messages.MultiRevision;
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
                Room room = Skylight.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().GetRoomSession().RequestedRoomID);
                session.GetHabbo().GetRoomSession().ResetRequestedRoom();
                if (room != null)
                {
                    if (room.RoomGamemapManager.Model != null)
                    {
                        session.SendData(room.RoomGamemapManager.Model.GetHeightmap(session.Revision, room.RoomGamemapManager.Tiles));
                        session.SendData(room.RoomGamemapManager.Model.GetRelativeHeightmap(session.Revision));
                        session.SendData(room.RoomGamemapManager.Model.GetPublicItems(session.Revision), true);

                        if (room.RoomData.Type == "private")
                        {
                            ServerMessage floorItems = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                            floorItems.Init(r63aOutgoing.FloorItems);
                            floorItems.AppendInt32(room.RoomItemManager.FloorItems.Count);
                            foreach(RoomItem item in room.RoomItemManager.FloorItems.Values)
                            {
                                item.Serialize(floorItems);
                            }
                            session.SendMessage(floorItems);

                            ServerMessage wallItems = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                            wallItems.Init(r63aOutgoing.WallItems);
                            wallItems.AppendInt32(room.RoomItemManager.WallItems.Count);
                            foreach(RoomItem item in room.RoomItemManager.WallItems.Values)
                            {
                                item.Serialize(wallItems);
                            }
                            session.SendMessage(wallItems);
                        }

                        room.RoomUserManager.EnterRoom(session);

                        ICollection<RoomUnit> users = room.RoomUserManager.GetRoomUsers();
                        ServerMessage setRoomUsers = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                        setRoomUsers.Init(r63aOutgoing.SetRoomUser);
                        setRoomUsers.AppendInt32(users.Count);
                        foreach(RoomUnit user in users)
                        {
                            user.Serialize(setRoomUsers);
                        }
                        session.SendMessage(setRoomUsers);

                        ServerMessage roomVIPsetting = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                        roomVIPsetting.Init(r63aOutgoing.RoomVIPSettings);
                        roomVIPsetting.AppendBoolean(room.RoomData.Hidewalls);
                        roomVIPsetting.AppendInt32(room.RoomData.Wallthick);
                        roomVIPsetting.AppendInt32(room.RoomData.Floorthick);
                        session.SendMessage(roomVIPsetting);

                        if (room.RoomData.Type == "private")
                        {
                            ServerMessage roomOwner = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                            roomOwner.Init(r63aOutgoing.RoomOwner);
                            roomOwner.AppendBoolean(true); //is private room
                            roomOwner.AppendUInt(room.ID);
                            roomOwner.AppendBoolean(room.HaveOwnerRights(session)); //is roomowner
                            session.SendMessage(roomOwner);

                            ServerMessage roomData = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                            roomData.Init(r63aOutgoing.RoomData);
                            roomData.AppendBoolean(false); //entered room
                            room.RoomData.Serialize(roomData, false);
                            roomData.AppendBoolean(false); //forward
                            roomData.AppendBoolean(room.RoomData.IsStaffPick); //is staff pick
                            session.SendMessage(roomData);
                        }
                        else
                        {
                            ServerMessage roomOwner = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                            roomOwner.Init(r63aOutgoing.RoomOwner);
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

                        foreach(RoomUnitUser user in room.RoomUserManager.GetRealUsers())
                        {
                            if (user.Dancing)
                            {
                                ServerMessage danceMessage = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                                danceMessage.Init(r63aOutgoing.Dance);
                                danceMessage.AppendInt32(user.VirtualID);
                                danceMessage.AppendInt32(user.DanceID);
                                session.SendMessage(danceMessage);
                            }

                            if (user.Sleeps)
                            {
                                ServerMessage sleeps = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                                sleeps.Init(r63aOutgoing.Sleeps);
                                sleeps.AppendInt32(user.VirtualID);
                                sleeps.AppendBoolean(true);
                                session.SendMessage(sleeps);
                            }

                            if (user.Handitem > 0 && user.HanditemTimer > 0)
                            {
                                ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                                message_.Init(r63aOutgoing.Handitem);
                                message_.AppendInt32(user.VirtualID);
                                message_.AppendInt32(user.Handitem);
                                session.SendMessage(message_);
                            }

                            if (user.ActiveEffect > 0)
                            {
                                session.SendMessage(BasicUtilies.GetRevisionPacketManager(Revision.PRODUCTION_201601012205_226667486).GetOutgoing(OutgoingPacketsEnum.Effect).Handle(new ValueHolder("VirtualID", user.VirtualID, "EffectID", user.ActiveEffect)));
                            }
                        }

                        room.RoomWiredManager.UserEnterRoom(session.GetHabbo().GetRoomSession().GetRoomUser());

                        if (session.GetHabbo().IsMuted())
                        {
                            ServerMessage flood = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                            flood.Init(r63aOutgoing.Flood);
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
