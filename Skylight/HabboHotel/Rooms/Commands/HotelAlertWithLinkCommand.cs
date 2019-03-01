using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Messages.MultiRevision;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class HotelAlertWithLinkCommand : Command
    {
        public override string CommandInfo()
        {
            return ":hal [link] [message] - Sends alert with link to all online users";
        }

        public override string RequiredPermission()
        {
            return "cmd_hal";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (session.GetHabbo().GetHALInterval() > 0)
            {
                if (session.GetHabbo().LastHALSended <= 0)
                {
                    using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                    {
                        session.GetHabbo().LastHALSended = dbClient.ReadDouble("SELECT LastHALSended FROM users WHERE Id = " + session.GetHabbo().ID + " LIMIT 1");
                    }
                }

                if (TimeUtilies.GetUnixTimestamp() - session.GetHabbo().LastHALSended >= session.GetHabbo().GetHALInterval())
                {
                    session.GetHabbo().LastHALSended = TimeUtilies.GetUnixTimestamp();

                    using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                    {
                        dbClient.AddParamWithValue("sessionid", session.GetHabbo().ID);
                        dbClient.ExecuteQuery("UPDATE users SET LastHALSended = UNIX_TIMESTAMP() WHERE id = @sessionid");
                    }
                }
                else
                {
                    TimeSpan timeLeft = new TimeSpan(0, 0, 0, (int)(session.GetHabbo().LastHALSended - TimeUtilies.GetUnixTimestamp() + session.GetHabbo().GetHALInterval()));
                    string alert = "You need to wait " + timeLeft.Seconds + " seconds";

                    if (timeLeft.TotalMinutes >= 1)
                    {
                        alert += ", " + timeLeft.Minutes + " minutes";
                    }

                    if (timeLeft.TotalHours >= 1)
                    {
                        alert += ", " + timeLeft.Hours + " hours";
                    }

                    if (timeLeft.TotalDays >= 1)
                    {
                        alert += ", " + timeLeft.Days + " days";
                    }

                    alert += " before you can use this command again!";
                    session.SendNotif(alert);
                    return true;
                }
            }

            if (args.Length >= 3)
            {
                if (session.GetHabbo().HasPermission("cmd_hal"))
                {
                    MultiRevisionServerMessage message = new MultiRevisionServerMessage(OutgoingPacketsEnum.NotifFromMod, new ValueHolder("Message", TextUtilies.MergeArrayToString(args, 2) + "\n\n" + session.GetHabbo().Username, "Link", args[1]));
                    
                    foreach (GameClient session_ in Skylight.GetGame().GetGameClientManager().GetClients())
                    {
                        try
                        {
                            session_.SendData(message.GetBytes(session_.Revision));
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
