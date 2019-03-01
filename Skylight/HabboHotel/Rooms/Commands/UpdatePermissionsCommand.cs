using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class UpdatePermissionsCommand : Command
    {
        public override string CommandInfo()
        {
            return ":update_permissions - Update the permissions";
        }

        public override string RequiredPermission()
        {
            return "cmd_update_permissions";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (session.GetHabbo().HasPermission("cmd_update_permissions"))
            {
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    Skylight.GetGame().GetPermissionManager().LoadRanks(dbClient);
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
