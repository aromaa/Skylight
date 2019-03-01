using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Support
{
    public class CautionManager
    {
        public Dictionary<int, Caution> Cauctions;

        public CautionManager()
        {
            this.Cauctions = new Dictionary<int, Caution>();
        }

        public void LoadCauctions(DatabaseClient dbClient)
        {
            Logging.Write("Loading cauctions... ");

            DataTable cauctions = dbClient.ReadDataTable("SELECT * FROM moderation_cauctions");
            if (cauctions != null && cauctions.Rows.Count > 0)
            {
                foreach(DataRow dataRow in cauctions.Rows)
                {
                    int id = (int)dataRow["id"];
                    this.Cauctions.Add(id, new Caution(id, (uint)dataRow["user_id"], (string)dataRow["reason"], (uint)dataRow["added_by_id"], (double)dataRow["added_on"]));
                }
            }

            Logging.WriteLine("completed!", ConsoleColor.Green);
        }

        public int GetCauctionsByUserID(uint userId)
        {
            return this.Cauctions.Values.Where(c => c.UserID == userId).Count();
        }

        public void GiveCauction(GameClient gaver, GameClient target, string reason)
        {
            target.SendNotif(reason);

            double timestamp = TimeUtilies.GetUnixTimestamp();

            int id = 0;
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("userId", target.GetHabbo().ID);
                dbClient.AddParamWithValue("reason", reason);
                dbClient.AddParamWithValue("addedById", gaver.GetHabbo().ID);
                dbClient.AddParamWithValue("addedOn", timestamp);

                dbClient.ExecuteQuery("INSERT INTO moderation_cauctions(user_id, reason, added_by_id, added_on) VALUES(@userId, @reason, @addedById, @addedOn)");
            }

            if (id > 0)
            {
                this.Cauctions.Add(id, new Caution(id, target.GetHabbo().ID, reason, gaver.GetHabbo().ID, timestamp));
            }
        }

        public void Shutdown()
        {
            if (this.Cauctions != null)
            {
                this.Cauctions.Clear();
            }
            this.Cauctions = null;
        }
    }
}
