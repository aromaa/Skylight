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
        private DataTable MessengerFriends;
        private DataTable MessengerRequests;
        private DataTable InventoryItems;

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

        public DataTable GetMessengerFriends()
        {
            return this.MessengerFriends;
        }

        public DataTable GetMessengerRequests()
        {
            return this.MessengerRequests;
        }

        public DataTable GetInventoryItems()
        {
            return this.InventoryItems;
        }

        public UserDataFactory(string ip, string sso, bool cacheData)
        {
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("sso", sso);

                string str = "";
                if (ServerConfiguration.EnableSecureSession)
                {
                    str = "AND ip_last = '" + ip + "' ";
                }

                try
                {
                    if (Skylight.GetConfig()["debug"] == "1")
                    {
                        str = "";
                    }
                }
                catch
                {
                }

                this.UserData = dbClient.ReadDataRow("SELECT * FROM users WHERE auth_ticket = @sso " + str + " LIMIT 1;");
                if (this.UserData != null)
                {
                    uint id = (uint)this.UserData["Id"];
                    if (cacheData)
                    {
                        dbClient.AddParamWithValue("userid", id);

                        this.MessengerFriends = dbClient.ReadDataTable("SELECT IF(user_one_id != @userid, user_one_id, user_two_id) AS friend_id, u.look, u.motto, u.last_online FROM messenger_friends m LEFT JOIN users u ON u.id = IF(user_one_id != @userid, user_one_id, user_two_id)");
                        this.MessengerRequests = dbClient.ReadDataTable("SELECT id, from_id FROM messenger_requests WHERE to_id = @userid");
                        this.InventoryItems = dbClient.ReadDataTable("SELECT id, base_item, extra_data FROM items WHERE room_id = 0 AND user_id = @userid");
                    }
                }
            }
        }
    }
}
