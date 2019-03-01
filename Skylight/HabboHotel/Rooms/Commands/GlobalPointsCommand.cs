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

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class GlobalPointsCommand : Command
    {
        public override string CommandInfo()
        {
            return ":globalpoints [type] [amount] - Gives x amount of points at selected type for everyone";
        }

        public override string RequiredPermission()
        {
            return "cmd_globalpoints";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (args.Length >= 3)
            {
                if (session.GetHabbo().HasPermission("cmd_globalpoints"))
                {
                    int type = int.Parse(args[1]);
                    int amount = int.Parse(args[2]);

                    using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                    {
                        //DataTable users = dbClient.ReadDataTable("SELECT id, activity_points FROM users WHERE online = '0'");
                        //if (users != null && users.Rows.Count > 0)
                        //{
                        //    StringBuilder query = new StringBuilder();

                        //    foreach (DataRow dataRow in users.Rows)
                        //    {
                        //        uint userId = (uint)dataRow["id"];
                        //        string activityPoints = (string)dataRow["activity_points"];

                        //        Dictionary<int, int> activityPoints_ = CurrenceUtilies.ActivityPointsToDictionary(activityPoints);
                        //        if (activityPoints_.ContainsKey(type))
                        //        {
                        //            activityPoints_[type] += amount;
                        //        }
                        //        else
                        //        {
                        //            activityPoints_.Add(type, amount);
                        //        }

                        //        dbClient.AddParamWithValue("activityPoints" + userId, CurrenceUtilies.ActivityPointsToString(activityPoints_));
                        //        query.Append("UPDATE users SET activity_points = @activityPoints" + userId + " WHERE id = " + userId + " LIMIT 1; ");
                        //    }

                        //    if (query.Length > 0)
                        //    {
                        //        dbClient.ExecuteQuery(query.ToString());
                        //    }
                        //}

                        dbClient.ExecuteQuery("CALL parse_activity_points(';', ','); INSERT INTO activity_points_parsed_data SELECT id," + type + "," + amount + " FROM users ON DUPLICATE KEY UPDATE value2 = value2 + " + amount + "; UPDATE users u JOIN(SELECT d.id, GROUP_CONCAT(CONCAT_WS(',', d.value, d.value2) SEPARATOR ';') AS ap FROM activity_points_parsed_data d GROUP BY id) s ON u.id = s.id SET u.activity_points = s.ap;");
                    }

                    foreach (GameClient session_ in Skylight.GetGame().GetGameClientManager().GetClients())
                    {
                        try
                        {
                            session_.GetHabbo().AddActivityPoints(type, amount);
                            session_.GetHabbo().UpdateActivityPoints(type, true);
                            session_.SendNotif("You just received " + amount + " points from staff!");
                        }
                        catch
                        {

                        }
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
