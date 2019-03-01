using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class UserInfoCommand : Command
    {
        public override string CommandInfo()
        {
            return ":userinfo [name] - Shows the details of user";
        }

        public override string RequiredPermission()
        {
            return "cmd_userinfo";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (args.Length >= 2)
            {
                if (session.GetHabbo().HasPermission("cmd_userinfo"))
                {
                    GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientByUsername(args[1]);
                    if (target != null)
                    {
                        string seasonalCurrnecy = "";
                        foreach(KeyValuePair<int, int> data in target.GetHabbo().ActivityPoints)
                        {
                            if (data.Key != 0)
                            {
                                seasonalCurrnecy += "\n" + data.Key + ": " + data.Value;
                            }
                        }

                        session.SendNotif("User information for user " + args[1] + "\nUser ID: " + target.GetHabbo().ID + "\nRank: " + target.GetHabbo().Rank + "\nOnline: Yes" + "\nMotto: " + target.GetHabbo().Motto + "\nCredits: " + target.GetHabbo().Credits + "\nPixels: " + target.GetHabbo().TryGetActivityPoints(0) + "\n\nSeasonal currency " + seasonalCurrnecy + "\n\nMuted: " + target.GetHabbo().IsMuted() + (session.GetHabbo().HasPermission("cmd_userinfo_viewip") ? "\nIP: " + target.GetIP() : "") + (target.GetHabbo().GetRoomSession().GetRoom() != null ? "\n\n - ROOM INFOMRATION -\nRoom ID: " + target.GetHabbo().GetRoomSession().GetRoom().ID + "\nRoom name: " + target.GetHabbo().GetRoomSession().GetRoom().RoomData.Name + "\nUsers in room: " + target.GetHabbo().GetRoomSession().GetRoom().RoomData.UsersNow : ""));
                    }
                    else
                    {
                        DataRow userData = null;
                        using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                        {
                            dbClient.AddParamWithValue("username", args[1]);
                            userData = dbClient.ReadDataRow("SELECT id, rank, motto, credits, activity_points, ip_last FROM users WHERE username = @username LIMIT 1");
                        }

                        if (userData != null)
                        {
                            string seasonalCurrnecy = "";
                            string activityPoints = (string)userData["activity_points"];
                            int pixels = 0;

                            if (!int.TryParse(activityPoints, out pixels))
                            {
                                foreach (string s in activityPoints.Split(';'))
                                {
                                    string[] activityPointsData = s.Split(',');

                                    int activityPointId;
                                    int activityPointAmount;
                                    if (int.TryParse(activityPointsData[0], out activityPointId))
                                    {
                                        if (activityPointId != 0)
                                        {
                                            if (int.TryParse(activityPointsData[1], out activityPointAmount))
                                            {
                                                seasonalCurrnecy += "\n" + activityPointId + ": " + activityPointAmount;
                                            }
                                        }
                                    }
                                }
                            }

                            session.SendNotif("User information for user " + args[1] + "\nUser ID: " + (uint)userData["id"] + "\nRank: " + (int)userData["rank"] + "\nOnline: No" + "\nMotto: " + (string)userData["motto"] + "\nCredits: " + (int)userData["credits"] + "\nPixels: " + pixels + "\n\nSeasonal currency " + seasonalCurrnecy + (session.GetHabbo().HasPermission("cmd_userinfo_viewip") ? "\n\nIP: " + (string)userData["ip_last"] : ""));
                        }
                        else
                        {
                            session.SendNotif("Unable to find user!");
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
