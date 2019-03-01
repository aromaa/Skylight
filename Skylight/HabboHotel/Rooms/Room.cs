using Newtonsoft.Json;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Pathfinders;
using SkylightEmulator.HabboHotel.Pets;
using SkylightEmulator.HabboHotel.Rooms.Bots;
using SkylightEmulator.HabboHotel.Rooms.Exceptions;
using SkylightEmulator.HabboHotel.Users.Inventory;
using SkylightEmulator.Messages;
using SkylightEmulator.Messages.Cached;
using SkylightEmulator.Messages.MultiRevision;
using SkylightEmulator.Messages.Wrappers;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc;
using SkylightEmulator.Communication;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class Room
    {
        public readonly uint ID;
        public readonly RoomData RoomData;

        public RoomUserManager RoomUserManager;
        public RoomItemManager RoomItemManager;
        public RoomGamemapManager RoomGamemapManager;
        public RoomWiredManager RoomWiredManager;
        public RoomGameManager RoomGameManager;
        public RoomFreezeManager RoomFreezeManager;

        public Task RoomCycleTask;
        public volatile bool RoomUnloaded;
        public Stopwatch RoomEmptyTimer;
        public RoomEvent RoomEvent;

        public List<uint> UsersWithRights;
        public ConcurrentDictionary<uint, Trade> Trades; //User id, trade, both users are added

        public bool RoomMute;
        public int RollerSpeed = 4;
        public bool DisableDiagonal;

        public Stopwatch LastAutoSave;
        public Stopwatch LastGameCycle;
        public Stopwatch LastRoomCycle;

        public int RoomCycleInterval = 480;
        public int GameCycleInterval = 80;

        public CancellationTokenSource RoomCycleCancellationTokenSource;

        public Stopwatch RoomOverloadTimer;

        //achievement progress shit when owner was offline
        public int EquestrianTrackHost_Offline = 0;
        public int FootballGoalHost_Offline = 0;
        public double RoomHost_Offline = 0;

        private List<MultiRevisionServerMessage> BytesWaitingToBeSend;
        private bool QueueBytes;

        //returns true if the owner was online, false if the owner was offline
        public bool EquestrianTrackHost(int amount = 1)
        {
            GameClient owner = Skylight.GetGame().GetGameClientManager().GetGameClientById(this.RoomData.OwnerID);
            if (owner != null)
            {
                owner.GetHabbo().GetUserStats().EquestrianTrackHost += amount + this.EquestrianTrackHost_Offline;
                owner.GetHabbo().GetUserAchievements().CheckAchievement("EquestrianTrackHost");

                this.EquestrianTrackHost_Offline = 0;

                return true;
            }
            else
            {
                this.EquestrianTrackHost_Offline += amount;

                return false;
            }
        }

        //returns true if the owner was online, false if the owner was offline
        public bool FootballGoalHost(int amount = 1)
        {
            GameClient owner = Skylight.GetGame().GetGameClientManager().GetGameClientById(this.RoomData.OwnerID);
            if (owner != null)
            {
                owner.GetHabbo().GetUserStats().FootballGoalHost += amount + this.FootballGoalHost_Offline;
                owner.GetHabbo().GetUserAchievements().CheckAchievement("FootballGoalHost");

                this.FootballGoalHost_Offline = 0;

                return true;
            }
            else
            {
                this.FootballGoalHost_Offline += amount;

                return false;
            }
        }

        //returns true if the owner was online, false if the owner was offline
        public bool RoomHost(double amount = 0)
        {
            GameClient owner = Skylight.GetGame().GetGameClientManager().GetGameClientById(this.RoomData.OwnerID);
            if (owner != null)
            {
                owner.GetHabbo().GetUserStats().RoomHost += amount + this.RoomHost_Offline;
                owner.GetHabbo().GetUserAchievements().CheckAchievement("RoomHost");

                this.RoomHost_Offline = 0;

                return true;
            }
            else
            {
                this.RoomHost_Offline += amount;

                return false;
            }
        }
 
        public Room(RoomData roomData)
        {
            this.ID = roomData.ID;
            this.RoomData = roomData;
            this.RoomCycleCancellationTokenSource = new CancellationTokenSource();
            this.LastGameCycle = Stopwatch.StartNew();
            this.LastRoomCycle = Stopwatch.StartNew();
            this.LastAutoSave = Stopwatch.StartNew();

            this.RoomEvent = null;

            this.RoomFreezeManager = new RoomFreezeManager(this);
            this.RoomUserManager = new RoomUserManager(this);
            this.RoomItemManager = new RoomItemManager(this);
            this.RoomGamemapManager = new RoomGamemapManager(this);
            this.RoomGameManager = new RoomGameManager(this);
            this.RoomWiredManager = new RoomWiredManager(this);

            this.RoomItemManager.LoadItems(); //this FIRST, because we cant update tiles without we know what items there are
            this.RoomGamemapManager.UpdateTiles();

            this.UsersWithRights = new List<uint>();
            this.Trades = new ConcurrentDictionary<uint, Trade>();
            this.BytesWaitingToBeSend = new List<MultiRevisionServerMessage>();

            this.LoadUserRights();

            DataTable pets = null;
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("roomId", this.ID);
                pets = dbClient.ReadDataTable("SELECT * FROM user_pets WHERE room_id = @roomId");
            }

            if (pets != null && pets.Rows.Count > 0)
            {
                foreach(DataRow dataRow in pets.Rows)
                {
                    this.RoomUserManager.AddPet(new Pet((uint)dataRow["id"], (uint)dataRow["user_id"], (int)dataRow["type"], (string)dataRow["name"], (string)dataRow["race"], (string)dataRow["color"], (int)dataRow["expirience"], (int)dataRow["energy"], (int)dataRow["happiness"], (int)dataRow["respect"], (double)dataRow["create_timestamp"]), (int)dataRow["x"], (int)dataRow["y"], (double)dataRow["z"], 0);
                }
            }

            foreach(RoomBotData botData in Skylight.GetGame().GetBotManager().GetBotsByRoomId(this.RoomData.ID))
            {
                this.RoomUserManager.AddAIUser(botData, 0, botData.X, botData.Y, botData.Z, botData.Rotation);
            }
        }

        public void OnCycle()
        {
            try
            {
                if (!this.RoomUnloaded)
                {
                    this.SetQueueBytes(true);

                    if (this.ShouldGameCycle())
                    {
                        this.LastGameCycle.Restart();

                        this.RoomGameManager.OnCycle();
                    }

                    if (this.ShouldRoomCycle())
                    {
                        this.LastRoomCycle.Restart();

                        this.RoomWiredManager.OnCycle();
                        this.RoomItemManager.OnCycle();
                        this.RoomUserManager.OnCycle();

                        if (this.RoomData.UsersNow <= 0)
                        {
                            if (this.RoomEmptyTimer == null)
                            {
                                this.RoomEmptyTimer = Stopwatch.StartNew();
                            }
                            else
                            {
                                if (this.RoomEmptyTimer.Elapsed.TotalSeconds >= 30.0) //30s
                                {
                                    Skylight.GetGame().GetRoomManager().UnloadRoom(this);
                                }
                            }
                        }
                        else
                        {
                            MultiRevisionServerMessage UserStatus = this.RoomUserManager.GetUserStatus(false);
                            if (UserStatus != null)
                            {
                                this.SendToAll(UserStatus);
                            }
                        }
                    }

                    this.RoomItemManager.SyncPackets();
                    this.SetQueueBytes(false);
                }
            }
            catch (RoomOverloadException ex)
            {
                this.CrashRoom(ex);
            }
            catch (Exception ex)
            {
                Logging.LogRoomException(ex.ToString());

                foreach (RoomUnitUser user in this.RoomUserManager.GetRealUsers())
                {
                    try
                    {
                        user.Session.SendNotif("Room crashed! Something bad happend! :( This error is already reported to the staff team! :) Will be fixed soon");
                    }
                    catch
                    {

                    }
                }

                Skylight.GetGame().GetRoomManager().UnloadRoom(this);
            }
        }

        //10-2 times in second by default
        public bool ShouldCycle()
        {
            return this.ShouldGameCycle() || this.ShouldRoomCycle();
        }

        //10 times in secound by default
        public bool ShouldGameCycle()
        {
            return this.LastGameCycle.ElapsedMilliseconds >= this.GameCycleInterval;
        }

        //2 times in secound by default
        public bool ShouldRoomCycle()
        {
            return this.LastRoomCycle.ElapsedMilliseconds >= this.RoomCycleInterval;
        }

        public void ThrowIfRoomCycleCancalled(string reason, object because)
        {
            if (this.RoomCycleCancellationTokenSource.IsCancellationRequested)
            {
                throw new RoomOverloadException(this.ID, reason, because);
            }
        }

        public void CrashRoom(RoomOverloadException reason)
        {
            Logging.LogRoomOverload(reason.ToString());

            foreach (RoomUnitUser user in this.RoomUserManager.GetRealUsers())
            {
                try
                {
                    user.Session.SendNotif("Room overloading!!! This is not okay and room have unloaded! If you think this is an error please report it to staff!");
                }
                catch
                {

                }
            }

            Skylight.GetGame().GetRoomManager().UnloadRoom(this);
        }

        public void UnloadRoom()
        {
            if (!this.RoomUnloaded)
            {
                this.RoomUnloaded = true;

                ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                Message.Init(r63aOutgoing.LeaveRoom);
                byte[] bytes = Message.GetBytes();

                foreach(RoomUnitUser user in this.RoomUserManager.GetRealUsers())
                {
                    user.Session.GetHabbo().GetRoomSession().LeavedRoom();
                    user.Session.SendData(bytes);
                }

                this.RoomItemManager.SaveAll();
                this.RoomUserManager.SaveAll();

                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("roomId", this.ID);
                    dbClient.AddParamWithValue("data", JsonConvert.SerializeObject(this.RoomData.ExtraData));
                    dbClient.ExecuteQuery("UPDATE rooms SET data = @data WHERE id = @roomId LIMIT 1");
                }

                //achievement saving
                if (!this.EquestrianTrackHost(0) || !this.FootballGoalHost(0) ||!this.RoomHost(0))
                {
                    using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                    {
                        dbClient.AddParamWithValue("ownerId", this.RoomData.OwnerID);
                        dbClient.AddParamWithValue("equestrianTrackHost", this.EquestrianTrackHost_Offline);
                        dbClient.AddParamWithValue("footballGoalHost", this.FootballGoalHost_Offline);
                        dbClient.AddParamWithValue("roomHost", this.RoomHost_Offline);
                        dbClient.ExecuteQuery("UPDATE user_stats SET equestrian_track_host = equestrian_track_host + @equestrianTrackHost, football_goal_host = football_goal_host + @footballGoalHost, room_host = room_host + @roomHost WHERE user_id = @ownerId LIMIT 1");
                    }

                    this.EquestrianTrackHost_Offline = 0;
                    this.FootballGoalHost_Offline = 0;
                    this.RoomHost_Offline = 0;
                }
            }

            GC.SuppressFinalize(this);
        }

        public void SendToAll(ServerMessage message, List<uint> ignoreList = null)
        {
            CachedServerMessage data = new CachedServerMessage(message);
            foreach (RoomUnitUser user in this.RoomUserManager.GetRealUsers())
            {
                if (user.Session.GetHabbo() != null)
                {
                    if (ignoreList == null || !ignoreList.Contains(user.Session.GetHabbo().ID))
                    {
                        user.Session.SendMessage(data);
                    }
                }
            }
        }

        public void SendToAll(OutgoingPacketsEnum outgoing, ValueHolder valueHolder)
        {
            this.SendToAll(new MultiRevisionServerMessage(outgoing, valueHolder));
        }

        public void SendToAll(OutgoingHandler outgoing, List<uint> ignoreList = null)
        {
            if (true /*!this.QueueBytes || ignoreList != null*/) //TODO THE QUEUE
            {
                foreach (RoomUnitUser user in this.RoomUserManager.GetRealUsers())
                {
                    if (user.Session.GetHabbo() != null)
                    {
                        if (ignoreList == null || !ignoreList.Contains(user.Session.GetHabbo().ID))
                        {
                            user.Session.SendMessage(outgoing);
                        }
                    }
                }
            }
            else
            {
                //this.BytesWaitingToBeSend.Add(message);
            }
        }

        public void SendToAll(MultiRevisionServerMessage message, List<uint> ignoreList = null)
        {
            if (!this.QueueBytes || ignoreList != null)
            {
                foreach (RoomUnitUser user in this.RoomUserManager.GetRealUsers())
                {
                    if (user.Session.GetHabbo() != null)
                    {
                        if (ignoreList == null || !ignoreList.Contains(user.Session.GetHabbo().ID))
                        {
                            user.Session.SendData(message.GetBytes(user.Session.Revision), true);
                        }
                    }
                }
            }
            else
            {
                this.BytesWaitingToBeSend.Add(message);
            }
        }

        public void SendToAllRespectIgnores(ServerMessage message, uint senderId)
        {
            CachedServerMessage data = new CachedServerMessage(message);
            foreach (RoomUnitUser user in this.RoomUserManager.GetRealUsers())
            {
                if (user.Session.GetHabbo() != null)
                {
                    if (!user.Session.GetHabbo().IgnoredUsers.Contains(senderId))
                    {
                        user.Session.SendMessage(data);
                    }
                }
            }
        }
        public void SendToAllRespectIgnores(MultiRevisionServerMessage message, uint senderId)
        {
            foreach (RoomUnitUser user in this.RoomUserManager.GetRealUsers())
            {
                if (user.Session.GetHabbo() != null)
                {
                    if (!user.Session.GetHabbo().IgnoredUsers.Contains(senderId))
                    {
                        user.Session.SendData(message.GetBytes(user.Session.Revision), true);
                    }
                }
            }
        }

        public void SendToAllWhoHaveRights(ServerMessage message)
        {
            CachedServerMessage data = new CachedServerMessage(message);
            foreach (RoomUnitUser user in this.RoomUserManager.GetRealUsers())
            {
                if (user.Session != null)
                {
                    if (this.GaveRoomRights(user.Session))
                    {
                        user.Session.SendMessage(data);
                    }
                }
            }
        }

        public void SendToAllWhoHaveOwnerRights(ServerMessage message)
        {
            CachedServerMessage data = new CachedServerMessage(message);
            foreach (RoomUnitUser user in this.RoomUserManager.GetRealUsers())
            {
                if (user.Session != null)
                {
                    if (this.HaveOwnerRights(user.Session))
                    {
                        user.Session.SendMessage(data);
                    }
                }
            }
        }

        public void UpdateUsersCount()
        {
            this.RoomData.UsersNow = this.RoomUserManager.UsersInRoom;
        }

        public bool HaveOwnerRights(GameClient session)
        {
            return session.GetHabbo().ID == this.RoomData.OwnerID || session.GetHabbo().HasPermission("acc_anyroomowner");
        }

        public bool GaveRoomRights(GameClient session)
        {
            return this.HaveOwnerRights(session) || this.UsersWithRights.Contains(session.GetHabbo().ID) || session.GetHabbo().HasPermission("acc_anyroomrights");
        }

        public void LoadUserRights()
        {
            this.UsersWithRights.Clear();

            DataTable rights = null;
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("roomId", this.ID);
                rights = dbClient.ReadDataTable("SELECT user_id FROM room_rights WHERE room_id = @roomId");
            }

            if (rights != null && rights.Rows.Count > 0)
            {
                foreach(DataRow dataRow in rights.Rows)
                {
                    this.UsersWithRights.Add((uint)dataRow["user_id"]);
                }
            }
        }

        public void StartTrade(RoomUnitUser user, RoomUnitUser target)
        {
            Trade trade = new Trade(this, user, target);
            if (this.Trades.TryAdd(user.UserID, trade))
            {
                if (this.Trades.TryAdd(target.UserID, trade))
                {
                    trade.Start();
                }
                else
                {
                    this.Trades.TryRemove(user.UserID, out Trade trash); //If the other user JUST started to trade we can just remove the user, right?
                }
            }
        }

        public Trade GetTradeByUserId(uint userId)
        {
            this.Trades.TryGetValue(userId, out Trade trade);
            return trade;
        }

        public void SetQueueBytes(bool queue)
        {
            if (queue)
            {
                this.QueueBytes = true;
            }
            else
            {
                this.QueueBytes = false;

                if (this.BytesWaitingToBeSend.Count > 0)
                {
                    Dictionary<Revision, List<ArraySegment<byte>>> bytes = new Dictionary<Revision, List<ArraySegment<byte>>>();
                    foreach (RoomUnitUser user in this.RoomUserManager.GetRealUsers())
                    {
                        if (user?.Session != null)
                        {
                            if (!bytes.TryGetValue(user.Session.Revision, out List<ArraySegment<byte>> bytes_))
                            {
                                bytes_ = bytes[user.Session.Revision] = new List<ArraySegment<byte>>();

                                foreach (MultiRevisionServerMessage message in this.BytesWaitingToBeSend)
                                {
                                    bytes_.Add(new ArraySegment<byte>(message.GetBytes(user.Session.Revision)));
                                }
                            }

                            user.Session.SendData(bytes_);
                        }
                    }

                    this.BytesWaitingToBeSend.Clear();
                }
            }
        }

        public void AboutToWalkOn(RoomUnit unit)
        {
            this.RoomItemManager.AboutToWalkOn(unit);
        }

        public void UserWalkOn(RoomUnit unit)
        {
            this.RoomGamemapManager.UserWalkOn(unit);
            this.RoomItemManager.UserWalkOn(unit);
            this.RoomWiredManager.UserWalkOn(unit);
        }

        public void UserWalkOff(RoomUnit unit)
        {
            this.RoomGamemapManager.UserWalkOff(unit);
            this.RoomItemManager.UserWalkOff(unit);
            this.RoomWiredManager.UserWalkOff(unit);
        }
    }
}
