using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Users;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class RoomSession
    {
        private readonly uint ID;
        private Habbo Habbo;

        public uint RequestedRoomID;
        public bool LoadingRoom;
        public bool WaitingForDoorbellAnswer;
        public uint CurrentRoomID;
        public RoomUser CurrentRoomRoomUser;

        public RoomSession(uint id, Habbo habbo)
        {
            this.ID = id;
            this.Habbo = habbo;
        }

        public void RequestPrivateRoom(uint id, string password)
        {
            this.ResetRequestedRoom();
            if (Skylight.GetGame().GetRoomManager().GetRoomData(id) != null)
            {
                if (this.IsInRoom)
                {
                    Room oldRoom = Skylight.GetGame().GetRoomManager().GetRoom(this.CurrentRoomID);
                    if (oldRoom != null)
                    {
                        oldRoom.RoomUserManager.LeaveRoom(this.Habbo.GetSession(), false);
                    }
                }

                Room room = Skylight.GetGame().GetRoomManager().GetAndLoadRoom(id);
                if (room != null)
                {
                    this.RequestedRoomID = id;
                    if (room.RoomData.Type == "public")
                    {
                        //later
                    }
                    else
                    {
                        ServerMessage message = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                        message.Init(r63aOutgoing.EnterPrivateRoom);
                        this.Habbo.GetSession().SendMessage(message);

                        if (room.RoomData.State == RoomStateType.OPEN)
                        {
                            this.LoadingRoom = true;
                        }
                        else if (room.RoomData.State == RoomStateType.LOCKED)
                        {
                            if (!room.IsOwner(this.Habbo.GetSession()))
                            {
                                if (room.RoomData.UsersNow == 0)
                                {
                                    ServerMessage emptyLockedRoom = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                                    emptyLockedRoom.Init(r63aOutgoing.DoorBellNoAnswer);
                                    this.Habbo.GetSession().SendMessage(emptyLockedRoom);
                                }
                                else
                                {
                                    this.WaitingForDoorbellAnswer = true;

                                    ServerMessage doorbellUser = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                                    doorbellUser.Init(r63aOutgoing.Doorbell);
                                    doorbellUser.AppendStringWithBreak("");
                                    this.Habbo.GetSession().SendMessage(doorbellUser);

                                    ServerMessage doorbellRoom = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                                    doorbellRoom.Init(r63aOutgoing.Doorbell);
                                    doorbellRoom.AppendStringWithBreak(this.Habbo.Username);
                                    room.SendToAllWhoHaveRights(doorbellRoom);
                                }
                            }
                            else
                            {
                                this.LoadingRoom = true;
                            }
                        }
                        else //its locked ofc now
                        {
                            if (!room.IsOwner(this.Habbo.GetSession()))
                            {
                                if (password.ToLower() == room.RoomData.Password.ToLower())
                                {
                                    this.LoadingRoom = true;
                                }
                                else
                                {
                                    ServerMessage roomError = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                                    roomError.Init(r63aOutgoing.RoomError);
                                    roomError.AppendInt32(-100002);
                                    this.Habbo.GetSession().SendMessage(roomError);

                                    ServerMessage LeaveRoom = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                                    LeaveRoom.Init(r63aOutgoing.LeaveRoom);
                                    this.Habbo.GetSession().SendMessage(LeaveRoom);
                                }
                            }
                            else
                            {
                                this.LoadingRoom = true;
                            }
                        }
                    }
                    this.EnterRoom();
                }
            }
        }

        public void EnterRoom()
        {
            Room room = Skylight.GetGame().GetRoomManager().GetRoom(this.RequestedRoomID);
            if (room != null && this.LoadingRoom)
            {
                ServerMessage message = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                message.Init(r63aOutgoing.LoadingRoomInfo);
                message.AppendStringWithBreak(room.RoomData.Model);
                message.AppendUInt(room.ID);
                this.Habbo.GetSession().SendMessage(message);

                if (room.RoomData.Type == "private")
                {
                    if (room.IsOwner(this.Habbo.GetSession()))
                    {
                        ServerMessage roomRights = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                        roomRights.Init(r63aOutgoing.RoomRights);
                        this.Habbo.GetSession().SendMessage(roomRights);

                        ServerMessage roomOwner = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                        roomOwner.Init(r63aOutgoing.IsRoomOwner);
                        this.Habbo.GetSession().SendMessage(roomOwner);
                    }
                    else
                    {

                    }

                    ServerMessage roomEvent = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                    roomEvent.Init(r63aOutgoing.RoomEvent);
                    roomEvent.AppendStringWithBreak("-1"); //no event
                    this.Habbo.GetSession().SendMessage(roomEvent);
                }
            }
        }

        public void EnteredRoom(uint id, RoomUser roomUser)
        {
            this.CurrentRoomID = id;
            this.CurrentRoomRoomUser = roomUser;
        }

        public void LeavedRoom()
        {
            this.CurrentRoomID = 0;
            this.CurrentRoomRoomUser = null;
        }

        public bool IsInRoom
        {
            get
            {
                return this.CurrentRoomID > 0;
            }
        }

        public void ResetRequestedRoom()
        {
            this.RequestedRoomID = 0;
            this.LoadingRoom = false;
            this.WaitingForDoorbellAnswer = false;
        }

        public void HandleDisconnection()
        {
            if (this.IsInRoom)
            {
                Skylight.GetGame().GetRoomManager().GetRoom(this.CurrentRoomID).RoomUserManager.LeaveRoom(this.Habbo.GetSession(), false);
            }
        }
    }
}
