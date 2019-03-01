using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class UpdateFilterCommand : Command
    {
        public override string CommandInfo()
        {
            return ":update_filter - Updates the filter";
        }

        public override string RequiredPermission()
        {
            return "cmd_update_filter";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (session.GetHabbo().HasPermission("cmd_update_filter"))
            {
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    TextUtilies.LoadWordfilter(dbClient);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
