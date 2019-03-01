using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.HabboHotel.Users.Inventory;
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

namespace SkylightEmulator.HabboHotel.Users
{
    public class Habbo
    {
        public readonly uint ID;
        private GameClient Session;
        public string Username;
        public string RealName;
        public string Email;
        public string SSO;
        public int Rank;
        public int Credits;
        public Dictionary<int, int> ActivityPoints;
        public double ActivityPointsLastUpdate;
        public string Look;
        public string Gender;
        public string Motto;
        public double AccountCreated;
        public double LastOnline;
        public string IPLast;
        public uint HomeRoom;
        public double MuteExpires;
        public int DailyRespectPoints;
        public int DailyPetRespectPoints;


        private HabboMessenger HabboMessenger;
        private UserStats UserStats;
        private UserSettings UserSettings;
        private UserDataFactory UserDataFactory;
        private RoomSession RoomSession;
        private InventoryManager InventoryManager;

        public List<uint> FavouriteRooms;
        public List<uint> UserRooms;

        public Habbo(GameClient session, UserDataFactory dataFactory, uint id, string username, string realName, string email, string sso, int rank, int credits, string activityPoints, double activityPointsLastUpdate, string look, string gender, string motto, double accountCreated, double lastOnline, string ipLast, uint homeRoom, int dailyRespectPoints, int dailyPetRespectPoints, double muteExpires, bool blockNewFriends, bool hideOnline, bool hideInRoom, int volume, bool acceptTrading)
        {
            this.Session = session;
            if (this.Session != null)
            {
            }
            Skylight.GetGame().GetGameClientManager().UpdateCachedUsername(id, username);

            this.FavouriteRooms = new List<uint>();
            this.UserRooms = new List<uint>();

            this.UserDataFactory = dataFactory;
            this.ID = id;
            this.Username = username;
            this.RealName = realName;
            this.Email = email;
            this.SSO = sso;
            this.Rank = rank;
            this.Credits = credits;

            int pixels;
            this.ActivityPoints = new Dictionary<int,int>();
            if (!int.TryParse(activityPoints, out pixels))
            {
                foreach (string s in activityPoints.Split(';'))
                {
                    string[] activityPointsData = s.Split(',');

                    int activityPointId;
                    int activityPointAmount;
                    if (int.TryParse(activityPointsData[0], out activityPointId))
                    {
                        if (int.TryParse(activityPointsData[1], out activityPointAmount))
                        {
                            this.ActivityPoints.Add(activityPointId, activityPointAmount);
                        }
                    }
                }
            }
            else
            {
                this.ActivityPoints.Add(0, pixels);
            }

            this.ActivityPointsLastUpdate = activityPointsLastUpdate;
            this.Look = look;
            this.Gender = gender;
            this.Motto = motto;
            this.AccountCreated = accountCreated;
            this.LastOnline = lastOnline;
            this.IPLast = ipLast;
            this.HomeRoom = homeRoom;
            this.MuteExpires = muteExpires;
            this.DailyRespectPoints = dailyRespectPoints;
            this.DailyPetRespectPoints = dailyPetRespectPoints;

            this.UserSettings = new UserSettings(this.ID, blockNewFriends, hideOnline, hideInRoom, volume, acceptTrading);
            this.UserStats = new UserStats(this.ID);
            this.RoomSession = new RoomSession(this.ID, this);
            this.InventoryManager = new InventoryManager(this.GetSession(), this.GetUserDataFactory());

            this.LoadRooms();
        }

        public bool IsMuted()
        {
            return this.MuteExpires - TimeUtilies.GetUnixTimestamp() > 0;
        }

        public UserStats GetUserStats()
        {
            return this.UserStats;
        }

        public UserSettings GetUserSettings()
        {
            return this.UserSettings;
        }

        public GameClient GetSession()
        {
            return this.Session;
        }

        public HabboMessenger GetMessenger()
        {
            return this.HabboMessenger;
        }

        public RoomSession GetRoomSession()
        {
            return this.RoomSession;
        }

        public UserDataFactory GetUserDataFactory()
        {
            return this.UserDataFactory;
        }

        public InventoryManager GetInventoryManager()
        {
            return this.InventoryManager;
        }

        public void UpdateCredits(bool save)
        {
            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
            Message.Init(r63aOutgoing.UpdateCredits);
            Message.AppendStringWithBreak(this.Credits + ".0");
            this.GetSession().SendMessage(Message);

            if (save)
            {
                //sql query
            }
        }

        public void HandleDisconnection()
        {
            if (this.GetMessenger() != null)
            {
                this.HabboMessenger.UpdateAllFriends(true);
                this.RoomSession.HandleDisconnection();
            }
        }

        public void UpdateActivityPoints(int type, bool save)
        {
            if (type == -1)
            {
                foreach(int id in this.ActivityPoints.Keys.ToList())
                {
                    this.HandleActivityPointsUpdate(id, save);
                }
            }
            else
            {
                this.HandleActivityPointsUpdate(type, save);
            }
        }

        private void HandleActivityPointsUpdate(int id, bool save)
        {
            if (this.ActivityPoints.ContainsKey(id))
            {
                ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                Message.Init(r63aOutgoing.UpdateActivityPoints);
                Message.AppendInt32(this.ActivityPoints[id]);
                Message.AppendInt32(0); //unknown
                Message.AppendInt32(id);
                this.GetSession().SendMessage(Message);
            }
        }

        public void LoadRooms()
        {
            this.UserRooms.Clear();
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("ownerid", this.ID);
                DataTable rooms = dbClient.ReadDataTable("SELECT id FROM rooms WHERE ownerid = @ownerid LIMIT " + ServerConfiguration.MaxRoomsPerUser);
                if (rooms != null)
                {
                    foreach (DataRow room in rooms.Rows)
                    {
                        this.UserRooms.Add((uint)room["id"]);
                    }
                }
            }
        }

        public void InitMessenger()
        {
            if (this.GetMessenger() == null)
            {
                this.HabboMessenger = new HabboMessenger(this.ID, this);
                this.HabboMessenger.LoadFriends();
                this.HabboMessenger.LoadRequests();
                this.HabboMessenger.SendFriends();
                this.HabboMessenger.SendRequests();
                this.HabboMessenger.UpdateAllFriends(true);
            }
        }

        public void AddRoom(uint id)
        {
            this.UserRooms.Add(id);
        }
    }
}
