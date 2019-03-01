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
        public RoomUnitUser CurrentRoomRoomUser;
        public uint TargetTeleportID;

        public RoomSession(uint id, Habbo habbo)
        {
            this.ID = id;
            this.Habbo = habbo;
        }

        public Habbo GetHabbo()
        {
            return this.Habbo;
        }

        public RoomUnitUser GetRoomUser()
        {
            return this.CurrentRoomRoomUser;
        }

        public Room GetRoom()
        {
            if (this.GetRoomUser() != null)
            {
                return this.GetRoomUser().Room;
            }
            else
            {
                return null;
            }
        }

        public void RequestPrivateRoom(uint id, string password)
        {
            this.ResetRequestedRoom();
            if (Skylight.GetGame().GetRoomManager().TryGetAndLoadRoomData(id) != null)
            {
                if (this.IsInRoom)
                {
                    Room oldRoom = Skylight.GetGame().GetRoomManager().TryGetRoom(this.CurrentRoomID);
                    if (oldRoom != null)
                    {
                        oldRoom.RoomUserManager.LeaveRoom(this.Habbo.GetSession(), false);
                    }
                }

                Room room = Skylight.GetGame().GetRoomManager().TryGetAndLoadRoom(id);
                if (room != null)
                {
                    this.RequestedRoomID = id;

                    if (room.RoomUserManager.UserHaveBan(this.Habbo.ID))
                    {
                        this.Habbo.GetSession().SendMessage(BasicUtilies.GetRevisionPacketManager(this.Habbo.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.RoomErrorOnEnter).Handle(new ValueHolder().AddValue("ErrorCode", 4)));
                        this.Habbo.GetSession().SendMessage(BasicUtilies.GetRevisionPacketManager(this.Habbo.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.LeaveRoom).Handle());
                    }
                    else
                    {
                        if (room.RoomData.UsersNow >= room.RoomData.UsersMax && !this.GetHabbo().HasPermission("acc_enter_fullrooms"))
                        {
                            this.Habbo.GetSession().SendMessage(BasicUtilies.GetRevisionPacketManager(this.Habbo.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.RoomErrorOnEnter).Handle(new ValueHolder().AddValue("ErrorCode", 1)));
                            this.Habbo.GetSession().SendMessage(BasicUtilies.GetRevisionPacketManager(this.Habbo.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.LeaveRoom).Handle());
                        }
                        else
                        {
                            if (room.RoomData.Type == "public")
                            {
                                this.LoadingRoom = true;
                            }
                            else //private
                            {
                                this.Habbo.GetSession().SendMessage(BasicUtilies.GetRevisionPacketManager(this.Habbo.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.EnterPrivateRoom).Handle());

                                if (!this.GetHabbo().HasPermission("acc_enter_anyroom"))
                                {
                                    if (this.TargetTeleportID != 0)
                                    {
                                        RoomItem item = room.RoomItemManager.TryGetRoomItem(this.TargetTeleportID);
                                        if (item == null)
                                        {
                                            this.TargetTeleportID = 0;

                                            this.Habbo.GetSession().SendMessage(BasicUtilies.GetRevisionPacketManager(this.Habbo.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.DoorbellNoAnswer).Handle());
                                        }
                                        else
                                        {
                                            this.LoadingRoom = true;
                                        }
                                    }
                                    else
                                    {
                                        if (room.RoomData.State == RoomStateType.OPEN)
                                        {
                                            this.LoadingRoom = true;
                                        }
                                        else if (room.RoomData.State == RoomStateType.LOCKED)
                                        {
                                            if (!room.HaveOwnerRights(this.Habbo.GetSession()))
                                            {
                                                if (room.RoomData.UsersNow == 0)
                                                {
                                                    this.Habbo.GetSession().SendMessage(BasicUtilies.GetRevisionPacketManager(this.Habbo.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.DoorbellNoAnswer).Handle());
                                                }
                                                else
                                                {
                                                    this.WaitingForDoorbellAnswer = true;

                                                    this.Habbo.GetSession().SendMessage(BasicUtilies.GetRevisionPacketManager(this.Habbo.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.Doorbell).Handle());
                                                    room.SendToAllWhoHaveRights(BasicUtilies.GetRevisionPacketManager(this.Habbo.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.Doorbell).Handle(new ValueHolder().AddValue("Username", this.Habbo.Username)));
                                                }
                                            }
                                            else
                                            {
                                                this.LoadingRoom = true;
                                            }
                                        }
                                        else //its locked ofc now
                                        {
                                            if (!room.HaveOwnerRights(this.Habbo.GetSession()))
                                            {
                                                if (password.ToLower() == room.RoomData.Password.ToLower())
                                                {
                                                    this.LoadingRoom = true;
                                                }
                                                else
                                                {
                                                    this.Habbo.GetSession().SendMessage(BasicUtilies.GetRevisionPacketManager(this.Habbo.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.RoomError).Handle(new ValueHolder().AddValue("ErrorCode", -100002)));
                                                    this.Habbo.GetSession().SendMessage(BasicUtilies.GetRevisionPacketManager(this.Habbo.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.LeaveRoom).Handle());
                                                }
                                            }
                                            else
                                            {
                                                this.LoadingRoom = true;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    this.LoadingRoom = true;
                                }
                            }

                            this.EnterRoom();
                        }
                    }
                }
            }
        }

        public void GetRoomState(uint id) //r26
        {
            this.ResetRequestedRoom();
            if (Skylight.GetGame().GetRoomManager().TryGetAndLoadRoomData(id) != null)
            {
                if (this.IsInRoom)
                {
                    Room oldRoom = Skylight.GetGame().GetRoomManager().TryGetRoom(this.CurrentRoomID);
                    if (oldRoom != null)
                    {
                        oldRoom.RoomUserManager.LeaveRoom(this.Habbo.GetSession(), false);
                    }
                }

                Room room = Skylight.GetGame().GetRoomManager().TryGetAndLoadRoom(id);
                if (room != null)
                {
                    this.RequestedRoomID = id;

                    if (room.RoomData.UsersNow >= room.RoomData.UsersMax && !this.GetHabbo().HasPermission("acc_enter_fullrooms"))
                    {
                        this.Habbo.GetSession().SendMessage(BasicUtilies.GetRevisionPacketManager(this.Habbo.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.RoomErrorOnEnter).Handle(new ValueHolder().AddValue("ErrorCode", 1)));
                        this.Habbo.GetSession().SendMessage(BasicUtilies.GetRevisionPacketManager(this.Habbo.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.LeaveRoom).Handle());
                    }
                    else
                    {
                        if (room.RoomData.Type == "public")
                        {
                            this.LoadingRoom = true;

                            this.EnterRoom();
                        }
                        else //private
                        {
                            this.Habbo.GetSession().SendMessage(BasicUtilies.GetRevisionPacketManager(this.Habbo.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.EnterPrivateRoom).Handle());

                            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                            message.Init(r26Outgoing.Unknown);
                            message.AppendString("skylight", null);
                            this.Habbo.GetSession().SendMessage(message);

                            if (!this.GetHabbo().HasPermission("acc_enter_anyroom"))
                            {
                                if (this.TargetTeleportID != 0)
                                {
                                    RoomItem item = room.RoomItemManager.TryGetRoomItem(this.TargetTeleportID);
                                    if (item == null)
                                    {
                                        this.TargetTeleportID = 0;

                                        this.Habbo.GetSession().SendMessage(BasicUtilies.GetRevisionPacketManager(this.Habbo.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.DoorbellNoAnswer).Handle());
                                    }
                                    else
                                    {
                                        this.LoadingRoom = true;
                                    }
                                }
                                else
                                {
                                    if (room.RoomData.State == RoomStateType.OPEN)
                                    {
                                        this.LoadingRoom = true;
                                    }
                                }
                            }
                            else
                            {
                                this.LoadingRoom = true;
                            }
                        }
                    }
                }
            }
        }

        public void EnterCheckRoom(string password) //r26
        {
            Room room = Skylight.GetGame().GetRoomManager().TryGetRoom(this.RequestedRoomID);
            if (room != null)
            {
                if (room.RoomUserManager.UserHaveBan(this.Habbo.ID))
                {
                    this.Habbo.GetSession().SendMessage(BasicUtilies.GetRevisionPacketManager(this.Habbo.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.RoomErrorOnEnter).Handle(new ValueHolder().AddValue("ErrorCode", 4)));
                    this.Habbo.GetSession().SendMessage(BasicUtilies.GetRevisionPacketManager(this.Habbo.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.LeaveRoom).Handle());
                }
                else
                {
                    if (!this.LoadingRoom)
                    {
                        if (room.RoomData.State == RoomStateType.LOCKED)
                        {
                            if (!room.HaveOwnerRights(this.Habbo.GetSession()))
                            {
                                if (room.RoomData.UsersNow == 0)
                                {
                                    this.Habbo.GetSession().SendMessage(BasicUtilies.GetRevisionPacketManager(this.Habbo.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.DoorbellNoAnswer).Handle());
                                }
                                else
                                {
                                    this.WaitingForDoorbellAnswer = true;

                                    this.Habbo.GetSession().SendMessage(BasicUtilies.GetRevisionPacketManager(this.Habbo.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.Doorbell).Handle());
                                    room.SendToAllWhoHaveRights(BasicUtilies.GetRevisionPacketManager(this.Habbo.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.Doorbell).Handle(new ValueHolder().AddValue("Username", this.Habbo.Username)));
                                }
                            }
                            else
                            {
                                this.LoadingRoom = true;
                            }
                        }
                        else //its locked ofc now
                        {
                            if (!room.HaveOwnerRights(this.Habbo.GetSession()))
                            {
                                if (password.ToLower() == room.RoomData.Password.ToLower())
                                {
                                    this.LoadingRoom = true;
                                }
                                else
                                {
                                    this.Habbo.GetSession().SendMessage(BasicUtilies.GetRevisionPacketManager(this.Habbo.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.RoomError).Handle(new ValueHolder().AddValue("ErrorCode", -100002)));
                                    this.Habbo.GetSession().SendMessage(BasicUtilies.GetRevisionPacketManager(this.Habbo.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.LeaveRoom).Handle());
                                }
                            }
                            else
                            {
                                this.LoadingRoom = true;
                            }
                        }
                    }

                    if (this.LoadingRoom)
                    {
                        ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                        message.Init(r26Outgoing.EnterCheckRoom);
                        this.Habbo.GetSession().SendMessage(message);
                    }
                }
            }
        }

        public void HandleTeleport(Room room, uint teleportId)
        {
            this.TargetTeleportID = teleportId;
            this.RequestPrivateRoom(room.ID, "");
        }

        public void EnterRoom()
        {
            Room room = Skylight.GetGame().GetRoomManager().TryGetRoom(this.RequestedRoomID);
            if (room != null && this.LoadingRoom)
            {
                this.Habbo.GetSession().SendMessage(BasicUtilies.GetRevisionPacketManager(this.Habbo.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.LoadingRoomInfo).Handle(new ValueHolder().AddValue("RoomModel", room.RoomData.Model).AddValue("RoomID", room.ID)));

                if (room.RoomData.Type == "private")
                {
                    if (room.RoomData.Wallpaper != "0.0")
                    {
                        this.Habbo.GetSession().SendMessage(BasicUtilies.GetRevisionPacketManager(this.Habbo.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.ApplyRoomEffect).Handle(new ValueHolder().AddValue("Type", "wallpaper").AddValue("Data", room.RoomData.Wallpaper)));
                    }

                    if (room.RoomData.Floor != "0.0")
                    {
                        this.Habbo.GetSession().SendMessage(BasicUtilies.GetRevisionPacketManager(this.Habbo.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.ApplyRoomEffect).Handle(new ValueHolder().AddValue("Type", "floor").AddValue("Data", room.RoomData.Floor)));
                    }

                    if (room.RoomData.Landscape != "0.0")
                    {
                        this.Habbo.GetSession().SendMessage(BasicUtilies.GetRevisionPacketManager(this.Habbo.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.ApplyRoomEffect).Handle(new ValueHolder().AddValue("Type", "landscape").AddValue("Data", room.RoomData.Landscape)));
                    }

                    if (room.GaveRoomRights(this.Habbo.GetSession()))
                    {
                        if (room.HaveOwnerRights(this.Habbo.GetSession()))
                        {
                            this.Habbo.GetSession().SendMessage(BasicUtilies.GetRevisionPacketManager(this.Habbo.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.GiveRoomRights).Handle(new ValueHolder().AddValue("RightsLevel", 4)));
                            this.Habbo.GetSession().SendMessage(BasicUtilies.GetRevisionPacketManager(this.Habbo.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.IsRoomOwner).Handle());
                        }
                        else
                        {
                            this.Habbo.GetSession().SendMessage(BasicUtilies.GetRevisionPacketManager(this.Habbo.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.GiveRoomRights).Handle(new ValueHolder().AddValue("RightsLevel", 1)));
                        }
                    }
                    else
                    {
                        if (this.Habbo.GetSession().Revision >= Revision.RELEASE63_201211141113_913728051) //on r63a this is NOT send if the user DOSEN'T have rights
                        {
                            this.Habbo.GetSession().SendMessage(BasicUtilies.GetRevisionPacketManager(this.Habbo.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.GiveRoomRights).Handle(new ValueHolder().AddValue("RightsLevel", 0)));
                        }
                    }

                    if (this.Habbo.GetSession().Revision < Revision.RELEASE63_201211141113_913728051)
                    {
                        if (room.HaveOwnerRights(this.Habbo.GetSession()) || this.Habbo.RatedRooms.Contains(room.ID))
                        {
                            this.Habbo.GetSession().SendMessage(BasicUtilies.GetRevisionPacketManager(this.Habbo.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.RoomRating).Handle(new ValueHolder().AddValue("Score", room.RoomData.Score)));
                        }
                        else
                        {
                            this.Habbo.GetSession().SendMessage(BasicUtilies.GetRevisionPacketManager(this.Habbo.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.RoomRating).Handle(new ValueHolder().AddValue("Score", -1)));
                        }
                    }
                    else
                    {
                        this.Habbo.GetSession().SendMessage(BasicUtilies.GetRevisionPacketManager(this.Habbo.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.RoomRating).Handle(new ValueHolder().AddValue("Score", room.RoomData.Score).AddValue("CanVote", !room.HaveOwnerRights(this.Habbo.GetSession()) && !this.Habbo.RatedRooms.Contains(room.ID))));
                    }

                    if (this.Habbo.GetSession().Revision < Revision.RELEASE63_201211141113_913728051)
                    {
                        if (room.RoomEvent != null)
                        {
                            this.Habbo.GetSession().SendMessage(room.RoomEvent.Serialize());
                        }
                        else
                        {
                            if (this.Habbo.GetSession().Revision > Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169)
                            {
                                ServerMessage roomEvent = BasicUtilies.GetRevisionServerMessage(this.Habbo.GetSession().Revision);
                                roomEvent.Init(r63aOutgoing.RoomEvent);
                                roomEvent.AppendString("-1"); //no event
                                this.Habbo.GetSession().SendMessage(roomEvent);
                            }
                            else
                            {
                                ServerMessage roomEvent = BasicUtilies.GetRevisionServerMessage(this.Habbo.GetSession().Revision);
                                roomEvent.Init(r63aOutgoing.RoomEvent);
                                roomEvent.AppendString("-1", null); //no event
                                this.Habbo.GetSession().SendMessage(roomEvent);
                            }
                        }
                    }
                }
            }
        }

        public void EnteredRoom(uint id, RoomUnitUser roomUser)
        {
            this.CurrentRoomID = id;
            this.CurrentRoomRoomUser = roomUser;

            Skylight.GetGame().GetRoomvisitManager().LogRoomvisit(this.Habbo.GetSession());
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
                Skylight.GetGame().GetRoomManager().TryGetRoom(this.CurrentRoomID).RoomUserManager.LeaveRoom(this.Habbo.GetSession(), false);
            }
        }
    }
}
