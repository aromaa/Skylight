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
    class UpdateSettingsCommand : Command
    {
        public override string CommandInfo()
        {
            return ":update_settings - Updates the settings";
        }

        public override string RequiredPermission()
        {
            return "cmd_update_settings";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                ServerConfiguration.LoadConfigsFromDB(dbClient);
            }
            return true;
        }
    }
}
