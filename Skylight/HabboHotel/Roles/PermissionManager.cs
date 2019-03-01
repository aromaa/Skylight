using SkylightEmulator.Core;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Roles
{
    public class PermissionManager
    {
        private Dictionary<int, PermissionRank> Ranks;

        public PermissionManager()
        {
            this.Ranks = new Dictionary<int, PermissionRank>();
        }

        public void LoadRanks(DatabaseClient dbClient)
        {
            Logging.Write("Loading ranks... ");
            Dictionary<int, PermissionRank> newRanks = new Dictionary<int, PermissionRank>();

            DataTable ranks = dbClient.ReadDataTable("SELECT * FROM ranks ORDER BY id ASC;");
            if (ranks != null && ranks.Rows.Count > 0)
            {
                foreach (DataRow dataRow in ranks.Rows)
                {
                    int id = (int)dataRow["id"];
                    newRanks.Add(id, new PermissionRank(id, (string)dataRow["name"], (string)dataRow["public_name"], (string)dataRow["badge_id"]));
                }

                //then we can load permissions
                DataTable permissionRanks = dbClient.ReadDataTable("SELECT * FROM permissions_ranks");
                if (permissionRanks != null && permissionRanks.Rows.Count > 0)
                {
                    foreach(DataRow dataRow in permissionRanks.Rows)
                    {
                        int rankId = (int)dataRow["rank_id"];

                        PermissionRank rank = null;
                        if (newRanks.TryGetValue(rankId, out rank))
                        {
                            List<string> permissions = new List<string>();
                            for (int i = 1; i < dataRow.ItemArray.Length; i++)
                            {
                                if (dataRow[i] is string) //enum (0, 1) aka bool
                                {
                                    if (TextUtilies.StringToBool((string)dataRow[i]))
                                    {
                                        permissions.Add(dataRow.Table.Columns[i].ColumnName);
                                    }
                                }
                                else if (dataRow[i] is int) //flood times, etc
                                {
                                    string columnName = dataRow.Table.Columns[i].ColumnName;
                                    if (columnName == "floodtime")
                                    {
                                        rank.Floodtime = (int)dataRow[i];
                                    }
                                    else if (columnName == "ha_interval")
                                    {
                                        rank.HaInterval = (int)dataRow[i];
                                    }
                                    else if (columnName == "hal_interval")
                                    {
                                        rank.HalInterval = (int)dataRow[i];
                                    }
                                    else if (columnName == "wired_trigger_limit")
                                    {
                                        rank.WiredTriggerLimit = (int)dataRow[i];
                                    }
                                    else if (columnName == "wired_action_limit")
                                    {
                                        rank.WiredActionLimit = (int)dataRow[i];
                                    }
                                    else if (columnName == "wired_condition_limit")
                                    {
                                        rank.WiredConditionLimit = (int)dataRow[i];
                                    }
                                }
                            }

                            rank.SetPermissions(permissions);
                        }
                    }
                }
            }

            this.Ranks = newRanks;
            Logging.WriteLine("completed!", ConsoleColor.Green);
        }

        public Dictionary<int, string> GetBadges()
        {
            return this.Ranks.ToDictionary(i => i.Key, i => i.Value.BadgeID);
        }

        public PermissionRank TryGetRank(int id)
        {
            PermissionRank rank;
            this.Ranks.TryGetValue(id, out rank);
            return rank;
        }

        public bool RankHasPermissions(int id, string permission)
        {
            PermissionRank rank = this.TryGetRank(id);
            if (rank != null)
            {
                return rank.HasPermissions(permission);
            }

            return false;
        }

        public void Shutdown()
        {
            if (this.Ranks != null)
            {
                this.Ranks.Clear();
            }
            this.Ranks = null;
        }
    }
}
