using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class HotelAlertCommand : Command
    {
        public override string CommandInfo()
        {
            return ":ha [message] - Sends alert to all online users";
        }

        public override string RequiredPermission()
        {
            return "cmd_ha";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (session.GetHabbo().GetHAInterval() > 0)
            {
                if (session.GetHabbo().LastHASended <= 0)
                {
                    using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                    {
                        session.GetHabbo().LastHASended = dbClient.ReadDouble("SELECT LastHASended FROM users WHERE Id = " + session.GetHabbo().ID + " LIMIT 1");
                    }
                }

                if (TimeUtilies.GetUnixTimestamp() - session.GetHabbo().LastHASended >= session.GetHabbo().GetHAInterval())
                {
                    session.GetHabbo().LastHASended = TimeUtilies.GetUnixTimestamp();

                    using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                    {
                        dbClient.AddParamWithValue("sessionid", session.GetHabbo().ID);
                        dbClient.ExecuteQuery("UPDATE users SET LastHASended = UNIX_TIMESTAMP() WHERE id = @sessionid");
                    }
                }
                else
                {
                    TimeSpan timeLeft = new TimeSpan(0, 0, 0, (int)(session.GetHabbo().LastHASended - TimeUtilies.GetUnixTimestamp() + session.GetHabbo().GetHAInterval()));
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

            if (args.Length >= 2)
            {
                if (session.GetHabbo().HasPermission("cmd_ha"))
                {
                    ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                    message.Init(r63aOutgoing.SendHotelAlert);
                    message.AppendString("Important Notice from Hotel Management"); //title
                    message.AppendString(TextUtilies.MergeArrayToString(args, 1) + "\n\n" + session.GetHabbo().Username); //message
                    message.AppendInt32(0); //parementers count
                    byte[] data = message.GetBytes();

                    foreach(GameClient session_ in Skylight.GetGame().GetGameClientManager().GetClients())
                    {
                        try
                        {
                            session_.SendData(data);
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
