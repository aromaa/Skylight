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
    public class BanManager
    {
        public Dictionary<int, Ban> Bans;

        public BanManager()
        {
            this.Bans = new Dictionary<int, Ban>();
        }

        public void LoadBans(DatabaseClient dbClient)
        {
            Logging.Write("Loading bans... ");
            Dictionary<int, Ban> newBans = new Dictionary<int, Ban>();

            DataTable bans = dbClient.ReadDataTable("SELECT * FROM bans;");
            if (bans != null && bans.Rows.Count > 0)
            {
                foreach(DataRow dataRow in bans.Rows)
                {
                    int id = (int)dataRow["id"];
                    BanType banType = (string)dataRow["type"] == "user" ? BanType.User : (string)dataRow["type"] == "ip" ? BanType.IP : BanType.Machine;

                    newBans.Add(id, new Ban(id, banType, (string)dataRow["value"], (string)dataRow["reason"], (double)dataRow["expire"], (uint)dataRow["added_by_id"], (double)dataRow["added_on"], TextUtilies.StringToBool((string)dataRow["active"])));
                }
            }

            this.Bans = newBans;
            Logging.WriteLine("completed!", ConsoleColor.Green);
        }

        public bool IsBanned(uint userId, string ip, string machineId)
        {
            string sUserId = userId.ToString();

            foreach(Ban ban in this.Bans.Values)
            {
                if (!ban.Expired && ban.Active)
                {
                    if (ban.BanType == BanType.User)
                    {
                        if (ban.Value == sUserId)
                        {
                            return true;
                        }
                    }
                    else if (ban.BanType == BanType.IP)
                    {
                        if (ban.Value == ip)
                        {
                            return true;
                        }
                    }
                    else if (ban.BanType == BanType.Machine)
                    {
                        if (ban.Value == machineId)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public Ban TryGetBan(uint userId, string ip, string machineId)
        {
            string sUserId = userId.ToString();

            foreach (Ban ban in this.Bans.Values)
            {
                if (!ban.Expired && ban.Active)
                {
                    if (ban.BanType == BanType.User)
                    {
                        if (ban.Value == sUserId)
                        {
                            return ban;
                        }
                    }
                    else if (ban.BanType == BanType.IP)
                    {
                        if (ban.Value == ip)
                        {
                            return ban;
                        }
                    }
                    else if (ban.BanType == BanType.Machine)
                    {
                        if (ban.Value == machineId)
                        {
                            return ban;
                        }
                    }
                }
            }

            return null;
        }

        public bool BanUser(GameClient banner, GameClient target, BanType type, string value, string reason, string lenght, bool disconnect = true)
        {
            if (!this.IsBanned(type == BanType.User ? uint.Parse(value) : 0, type == BanType.IP ? value : "", type == BanType.Machine ? value : ""))
            {
                char banLenghtChar = lenght.Substring(lenght.Length - 1)[0];
                int banLenght = 0;

                if (Char.IsNumber(banLenghtChar))
                {
                    banLenght = int.Parse(lenght);
                }
                else
                {
                    if (lenght != "P")
                    {
                        banLenght = int.Parse(lenght.Substring(0, lenght.Length - 1));

                        if (banLenghtChar == 'm')
                        {
                            banLenght *= 60;
                        }
                        else if (banLenghtChar == 'h')
                        {
                            banLenght *= 3600;
                        }
                        else if (banLenghtChar == 'd')
                        {
                            banLenght *= 86400;
                        }
                        else if (banLenghtChar == 'w')
                        {
                            banLenght *= 604800;
                        }
                        else if (banLenghtChar == 'M')
                        {
                            banLenght *= 2419200;
                        }
                        else if (banLenghtChar == 'y')
                        {
                            banLenght *= 30758400;
                        }
                    }
                }

                if (banLenght >= 0)
                {
                    double addedOn = TimeUtilies.GetUnixTimestamp();
                    double expires = addedOn + banLenght;

                    if (lenght == "P")
                    {
                        expires = -1;
                    }

                    int banId = 0;
                    using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                    {
                        dbClient.AddParamWithValue("type", type == BanType.User ? "user" : type == BanType.IP ? "ip" : "machine");
                        dbClient.AddParamWithValue("value", value);
                        dbClient.AddParamWithValue("reason", reason);
                        dbClient.AddParamWithValue("expires", expires);
                        dbClient.AddParamWithValue("addedbyid", banner.GetHabbo().ID);
                        dbClient.AddParamWithValue("addedon", addedOn);
                        banId = (int)dbClient.ExecuteQuery("INSERT INTO bans(type, value, reason, expire, added_by_id, added_on, active) VALUES(@type, @value, @reason, @expires, @addedbyid, @addedon, '1')");
                    }

                    if (banId > 0)
                    {
                        this.Bans.Add(banId, new Ban(banId, type, value, reason, expires, banner.GetHabbo().ID, addedOn, true));

                        if (lenght != "P")
                        {
                            banner.SendNotif("You have banned " + target.GetHabbo().Username + "!\n\nBan reason: " + reason + "\nBan lenght: " + lenght + " (" + banLenght + " secound)\nBan expires: " + TimeUtilies.UnixTimestampToDateTime(expires).ToString() + "\nBan type: " + (type == BanType.User ? "User" : type == BanType.IP ? "IP" : "Machine"));
                        }
                        else
                        {
                            expires = -1;

                            banner.SendNotif("You have banned " + target.GetHabbo().Username + "!\n\nBan reason: " + reason + "\nBan lenght: PERMAMENT\nBan expires: NEVER\nBan type: " + (type == BanType.User ? "User" : type == BanType.IP ? "IP" : "Machine"));
                        }

                        if (disconnect)
                        {
                            this.DisconnecBannedUsers();
                        }

                        return true;
                    }
                    else
                    {
                        banner.SendNotif("Database error!");

                        return false;
                    }
                }
                else
                {
                    banner.SendNotif("You can't enter negative ban lenght!");

                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public void DisconnecBannedUsers()
        {
            foreach(GameClient session in Skylight.GetGame().GetGameClientManager().GetClients())
            {
                try
                {
                    if (this.IsBanned(session.GetHabbo().ID, session.GetIP(), session.MachineID))
                    {
                        session.Stop("Banned!");
                    }
                }
                catch
                {

                }
            }
        }

        public int GetBanCountByUserID(uint userId)
        {
            string sUserId = userId.ToString();
            return this.Bans.Values.Where(b => b.BanType == BanType.User && b.Value == sUserId).Count();
        }

        public void Shutdown()
        {
            if (this.Bans != null)
            {
                this.Bans.Clear();
            }
            this.Bans = null;
        }
    }
}
