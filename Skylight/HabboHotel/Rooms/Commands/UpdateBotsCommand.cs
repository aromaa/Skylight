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
    class UpdateBotsCommand : Command
    {
        public override string CommandInfo()
        {
            return ":update_bots - Updates the bots";
        }

        public override string RequiredPermission()
        {
            return "cmd_update_bots";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (session.GetHabbo().HasPermission("cmd_update_bots"))
            {
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    Skylight.GetGame().GetBotManager().LoadBots(dbClient);
                    Skylight.GetGame().GetBotManager().LoadNewbieBotActions(dbClient);
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
