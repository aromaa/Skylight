using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class ConvertCreditsCommands : Command
    {
        public override string CommandInfo()
        {
            return ":convertcredits - Converts exhanges on your inventory to credits";
        }

        public override string RequiredPermission()
        {
            return "";
        }

        public override bool ShouldBeLogged()
        {
            return false;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            session.GetHabbo().GetInventoryManager().ConvertCredits();
            return true;
        }
    }
}
