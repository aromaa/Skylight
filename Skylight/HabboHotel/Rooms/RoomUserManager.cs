using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Pathfinders;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class RoomUserManager
    {
        private Room Room;
        private int NextVirtualID = 0;
        private Dictionary<int, RoomUser> RoomUsers;

        public RoomUserManager(Room room)
        {
            this.RoomUsers = new Dictionary<int, RoomUser>();
            this.Room = room;
        }

        public int UsersInRoom
        {
            get
            {
                return this.RoomUsers.Count;
            }
        }

        public int GetNextVirtualID()
        {
            return this.NextVirtualID++;
        }

        public void OnCycle()
        {
            foreach (RoomUser user in this.RoomUsers.Values.ToList())
            {
                if (user != null)
                {
                    if (user.NextStep)
                    {
                        user.NextStep = false;
                        user.SetLocation(user.NextStepX, user.NextStepY, 0); //we set height on EnterTile

                        this.UpdateUserStateOnTile(user);
                    }

                    if (user.Moving)
                    {
                        RoomTile tile = DreamPathfinder.GetNearestRoomTile(new Point(user.GetX, user.GetY), new Point(user.TargetX, user.TargetY), user.GetZ, this.Room, true);
                        if (tile.X != user.GetX || tile.Y != user.GetY)
                        {
                            this.Room.RoomGamemapManager.GetTile(user.GetX, user.GetY).UsersOnTile.Remove(user); //last tile

                            int x = tile.X;
                            int y = tile.Y;
                            double z = tile.GetZ(!tile.IsSeat && !tile.IsBed);

                            this.Room.RoomGamemapManager.GetTile(x, y).UsersOnTile.Add(user); //new tile

                            user.NextStep = true;
                            user.NextStepX = x;
                            user.NextStepY = y;

                            int rotation = WalkRotation.Walk(user.GetX, user.GetY, x, y);
                            user.SetRotation(rotation);

                            user.RemoveStatus("mv");
                            user.RemoveStatus("sit");
                            user.RemoveStatus("lay");
                            user.AddStatus("mv", string.Concat(new object[] { x, ",", y, ",", TextUtilies.DoubleWithDotDecimal(z) }));
                        }
                        else
                        {
                            user.Moving = false;

                            if (user.HasStatus("mv"))
                            {
                                user.RemoveStatus("mv");
                            }
                        }
                    }
                }
            }
        }

        public void UpdateUserStateOnTile(RoomUser user)
        {
            if (user != null)
            {
                RoomTile tile = this.Room.RoomGamemapManager.GetTile(user.GetX, user.GetY);
                if (tile != null)
                {
                    if (tile.IsSeat)
                    {
                        user.AddStatus("sit", TextUtilies.DoubleWithDotDecimal(tile.HigestRoomItem.BaseItem.Height));
                        user.SetLocation(user.GetX, user.GetY, tile.GetZ(false));
                        user.SetRotation(tile.HigestRoomItem.Rot);
                    }
                    else
                    {
                        user.RemoveStatus("sit");

                        user.SetLocation(user.GetX, user.GetY, tile.GetZ(true));
                    }
                }
            }
        }

        public void EnterRoom(GameClient gameClient)
        {
            RoomUser roomUser = new RoomUser(gameClient, this.GetNextVirtualID(), this.Room);
            if (roomUser != null && roomUser.GetClient() != null && roomUser.GetClient().GetHabbo() != null)
            {
                roomUser.SetLocation(this.Room.RoomGamemapManager.Model.DoorX, this.Room.RoomGamemapManager.Model.DoorY, this.Room.RoomGamemapManager.Model.DoorZ);
                roomUser.SetRotation(this.Room.RoomGamemapManager.Model.DoorDir);

                if (this.Room.IsOwner(gameClient))
                {
                    roomUser.AddStatus("flatctrl", "useradmin");
                }

                ServerMessage NewRoomUser = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                NewRoomUser.Init(r63aOutgoing.SetRoomUser);
                NewRoomUser.AppendInt32(1); //count
                roomUser.Serialize(NewRoomUser);
                this.Room.SendToAll(NewRoomUser, null);

                this.RoomUsers.Add(roomUser.VirtualID, roomUser);
                roomUser.GetClient().GetHabbo().GetRoomSession().EnteredRoom(this.Room.ID, roomUser);
                roomUser.GetClient().GetHabbo().GetMessenger().UpdateAllFriends(false);
                this.Room.UpdateUsersCount();
            }
        }

        public ServerMessage GetUserStatus(bool everyone)
        {
            List<RoomUser> users = this.RoomUsers.Values.Where(u => u.NeedUpdate == true || everyone).ToList();

            if (users.Count == 0)
            {
                return null;
            }
            else
            {
                ServerMessage statues = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                statues.Init(r63aOutgoing.UserStatues);
                statues.AppendInt32(users.Count);
                foreach (RoomUser user in users)
                {
                    user.NeedUpdate = false;

                    user.SerializeStatus(statues);
                }
                return statues;
            }
        }

        public void LeaveRoom(GameClient gameClient, bool sendLeavePacket)
        {
            if (gameClient != null && gameClient.GetHabbo() != null && gameClient.GetHabbo().GetRoomSession() != null)
            {
                RoomUser user = gameClient.GetHabbo().GetRoomSession().CurrentRoomRoomUser;
                if (user != null)
                {
                    if (sendLeavePacket)
                    {
                        ServerMessage LeaveRoom = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                        LeaveRoom.Init(r63aOutgoing.LeaveRoom);
                        gameClient.SendMessage(LeaveRoom);
                    }

                    ServerMessage UserLeaved = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                    UserLeaved.Init(r63aOutgoing.UserLeavedRoom);
                    UserLeaved.AppendInt32(user.VirtualID);
                    this.Room.SendToAll(UserLeaved, null);

                    this.RoomUsers.Remove(user.VirtualID);
                    this.Room.UpdateUsersCount();

                    gameClient.GetHabbo().GetRoomSession().LeavedRoom();
                }
            }
        }

        public List<RoomUser> GetRealUsers()
        {
            return this.RoomUsers.Values.ToList();
        }

        public void UpdateUserTiles()
        {

        }
    }
}
