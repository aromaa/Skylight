using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Pathfinders;
using SkylightEmulator.Messages;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class Room
    {
        public readonly uint ID;
        public readonly RoomData RoomData;

        public RoomUserManager RoomUserManager;
        public RoomItemManager RoomItemManager;
        public RoomGamemapManager RoomGamemapManager;

        public Task RoomCycleTask;
        public bool RoomUnloaded;
        public int RoomEmptyTimer;

        public List<uint> UsersWithRights;

        public Room(RoomData roomData)
        {
            this.ID = roomData.ID;
            this.RoomData = roomData;

            this.RoomUserManager = new RoomUserManager(this);
            this.RoomItemManager = new RoomItemManager(this);
            this.RoomGamemapManager = new RoomGamemapManager(this);

            this.RoomItemManager.LoadItems(); //this FIRST, because we cant update tiles without we know what items there are
            this.RoomGamemapManager.UpdateTiles();

            this.UsersWithRights = new List<uint>();
        }

        public void OnCycle()
        {
            try
            {
                this.RoomUserManager.OnCycle();

                if (this.RoomData.UsersNow > 0)
                {
                    this.RoomEmptyTimer = 0;
                }
                else
                {
                    this.RoomEmptyTimer++;
                }

                if (!this.RoomUnloaded)
                {
                    if (this.RoomEmptyTimer >= 60) //60 = 30s
                    {
                        Skylight.GetGame().GetRoomManager().UnloadRoom(this);
                        return;
                    }

                    ServerMessage UserStatus = this.RoomUserManager.GetUserStatus(false);
                    if (UserStatus != null)
                    {
                        this.SendToAll(UserStatus, null);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void UnloadRoom()
        {
            if (!this.RoomUnloaded)
            {
                this.RoomUnloaded = true;

                ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                Message.Init(r63aOutgoing.LeaveRoom);
                this.SendToAll(Message, null);

                this.RoomItemManager.SaveAll();

                GC.SuppressFinalize(this);
            }
        }

        public void SendToAll(ServerMessage message, List<uint> ignoreList)
        {
            byte[] data = message.GetBytes();
            foreach (RoomUser user in this.RoomUserManager.GetRealUsers())
            {
                if (user != null && user.GetClient() != null && user.GetClient().GetHabbo() != null)
                {
                    if (ignoreList == null || !ignoreList.Contains(user.GetClient().GetHabbo().ID))
                    {
                        user.GetClient().SendData(data);
                    }
                }
            }
        }

        public void SendToAllWhoHaveRights(ServerMessage message)
        {
            byte[] data = message.GetBytes();
            foreach (RoomUser user in this.RoomUserManager.GetRealUsers())
            {
                if (user != null && user.GetClient() != null)
                {
                    if (this.HaveRights(user.GetClient()))
                    {
                        user.GetClient().SendData(data);
                    }
                }
            }
        }

        public void UpdateUsersCount()
        {
            this.RoomData.UsersNow = this.RoomUserManager.UsersInRoom;
        }

        public bool IsOwner(GameClient session)
        {
            return session.GetHabbo().ID == this.RoomData.OwnerID;
        }

        public bool HaveRights(GameClient session)
        {
            return this.IsOwner(session);
        }
    }
}
