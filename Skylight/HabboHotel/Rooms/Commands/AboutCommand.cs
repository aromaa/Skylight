using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    public class AboutCommand : Command
    {
        public override string CommandInfo()
        {
            return ":about - Shows server information";
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            session.SendNotifWithLink("Skylight 1.0\n\nThanks/Credits;\nJonny [Skylight Lead Dev]\n\n" + Skylight.Version + "\n\nUptime: " + Skylight.Uptime.Days + " days, " + Skylight.Uptime.Hours + " hours, " + Skylight.Uptime.Minutes + " minutes, " + Skylight.Uptime.Seconds + " seconds\n\nLicenced for: " + Licence.LicenceHolder + "\n" + Licence.LicenceDetails, Licence.LicenceDetailsLink);
            return true;
        }

        public override string RequiredPermission()
        {
            return "";
        }

        public override bool ShouldBeLogged()
        {
            return false;
        }
    }
}
