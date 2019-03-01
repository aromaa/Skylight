using SkylightEmulator.Core;
using SkylightEmulator.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Users
{
    public class UserDataFactory
    {
        private DataRow UserData;
        private DataRow UserStats;
        private DataTable MessengerFriends;
        private DataTable MessengerRequests;
        private DataTable MessengerCategorys;
        private DataTable MessengerRequestsSend;
        private DataTable InventoryItems;
        private DataTable Subscriptions;
        private DataTable Wardrobe;
        private DataTable Badges;
        private DataTable RoomFavourites;
        private DataTable Pets;
        private DataTable IgnoredUsers;
        private DataTable Achievements;

        public bool IsUserLoaded
        {
            get
            {
                return this.UserData != null;
            }
        }

        public DataRow GetUserData()
        {
            return this.UserData;
        }

        public DataRow GetUserStats()
        {
            try
            {
                return this.UserStats;
            }
            finally
            {
                this.UserStats = null;
            }
        }

        public DataTable GetMessengerFriends()
        {
            try
            {
                return this.MessengerFriends;
            }
            finally
            {
                this.MessengerFriends = null;
            }
        }

        public DataTable GetMessengerFriendRequestsPending()
        {
            try
            {
                return this.MessengerRequests;
            }
            finally
            {
                this.MessengerRequests = null;
            }
        }

        public DataTable GetMessengerFriendCategorys()
        {
            try
            {
                return this.MessengerCategorys;
            }
            finally
            {
                this.MessengerCategorys = null;
            }
        }

        public DataTable GetMessengerFriendRequestsSend()
        {
            try
            {
                return this.MessengerRequestsSend;
            }
            finally
            {
                this.MessengerRequestsSend = null;
            }
        }

        public DataTable GetInventoryItems()
        {
            try
            {
                return this.InventoryItems;
            }
            finally
            {
                this.InventoryItems = null;
            }
        }

        public DataTable GetSubscriptions()
        {
            try
            {
                return this.Subscriptions;
            }
            finally
            {
                this.Subscriptions = null;
            }
        }

        public DataTable GetWardrobe()
        {
            try
            {
                return this.Wardrobe;
            }
            finally
            {
                this.Wardrobe = null;
            }
        }

        public DataTable GetBadges()
        {
            try
            {
                return this.Badges;
            }
            finally
            {
                this.Badges = null;
            }
        }

        public DataTable GetRoomFavourites()
        {
            try
            {
                return this.RoomFavourites;
            }
            finally
            {
                this.RoomFavourites = null;
            }
        }

        public DataTable GetPets()
        {
            return this.Pets;
        }

        public DataTable GetIgnoredUsers()
        {
            try
            {
                return this.IgnoredUsers;
            }
            finally
            {
                this.IgnoredUsers = null;
            }
        }

        public DataTable GetAchievements()
        {
            try
            {
                return this.Achievements;
            }
            finally
            {
                this.Achievements = null;
            }
        }

        public UserDataFactory(string ip, string machineId, string sso, bool cacheData, out int sessionId)
        {
            sessionId = -1; //default value

            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("sso", sso);
                dbClient.AddParamWithValue("ip", ip);

                string str = "";
                if (ServerConfiguration.EnableSecureSession)
                {
                    str = "AND ip_last = @ip ";
                }

                this.UserData = dbClient.ReadDataRow("SELECT * FROM users WHERE auth_ticket = @sso " + str + " LIMIT 1;");
                if (this.UserData != null)
                {
                    uint id = (uint)this.UserData["Id"];
                    if (cacheData)
                    {
                        dbClient.AddParamWithValue("userid", id);
                        dbClient.AddParamWithValue("machineId", machineId);
                        if (ServerConfiguration.UseIPLastForBans)
                        {
                            dbClient.AddParamWithValue("ipLast", (string)this.UserData["ip_last"]);
                            sessionId = (int)dbClient.ExecuteQuery("INSERT INTO user_logins(user_id, timestamp, ip, machine_id, auth_ticket) VALUES(@userid, UNIX_TIMESTAMP(), @ipLast, @machineId, @sso)");
                        }
                        else
                        {
                            sessionId = (int)dbClient.ExecuteQuery("INSERT INTO user_logins(user_id, timestamp, ip, machine_id, auth_ticket) VALUES(@userid, UNIX_TIMESTAMP(), @ip, @machineId, @sso)");
                        }
                        dbClient.ExecuteQuery("UPDATE users SET online = '1', auth_ticket = '', machine_id_last = @machineId WHERE id = @userId LIMIT 1"); //this first so we can store some data

                        this.UserStats = dbClient.ReadDataRow("SELECT * FROM user_stats WHERE user_id = @userid LIMIT 1");
                        this.MessengerFriends = dbClient.ReadDataTable("SELECT IF(user_one_id != @userid, user_one_id, user_two_id) AS friend_id, IF(user_one_id = @userid, user_one_category, user_two_category) AS category, IF(user_one_id = @userid, user_one_relation, user_two_relation) AS relation, u.look, u.motto, u.last_online FROM messenger_friends m LEFT JOIN users u ON u.id = IF(user_one_id != @userid, user_one_id, user_two_id) WHERE user_one_id = @userid OR user_two_id = @userid");
                        this.MessengerRequests = dbClient.ReadDataTable("SELECT m.from_id, u.username, u.look FROM messenger_requests m LEFT JOIN users u ON m.from_id = u.id WHERE to_id = @userid");
                        this.MessengerCategorys = dbClient.ReadDataTable("SELECT id, name FROM messenger_categorys WHERE user_id = @userid");
                        this.MessengerRequestsSend = dbClient.ReadDataTable("SELECT to_id FROM messenger_requests WHERE from_id = @userid");
                        this.InventoryItems = dbClient.ReadDataTable("SELECT id, base_item, extra_data FROM items WHERE room_id = 0 AND user_id = @userid");
                        this.Subscriptions = dbClient.ReadDataTable("SELECT id, subscription_name, subscription_started, subscription_expires FROM user_subscriptions WHERE user_id = @userid");
                        this.Wardrobe = dbClient.ReadDataTable("SELECT slot_id, gender, look FROM user_wardrobe WHERE user_id = @userid");
                        this.Badges = dbClient.ReadDataTable("SELECT badge_id, badge_slot FROM user_badges WHERE user_id = @userid");
                        this.RoomFavourites = dbClient.ReadDataTable("SELECT room_id FROM user_favorites WHERE user_id = @userid");
                        this.Pets = dbClient.ReadDataTable("SELECT * FROM user_pets WHERE user_id = @userid");
                        this.IgnoredUsers = dbClient.ReadDataTable("SELECT ignored_id FROM user_ignores WHERE user_id = @userid");
                        this.Achievements = dbClient.ReadDataTable("SELECT achievement_group, achievement_level FROM user_achievements WHERE user_id = @userid");
                    }
                }
            }
        }
    }
}
