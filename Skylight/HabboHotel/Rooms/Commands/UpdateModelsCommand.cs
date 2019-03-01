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
    class UpdateModelsCommand : Command
    {
        public override string CommandInfo()
        {
            return ":update_models - Update the models";
        }

        public override string RequiredPermission()
        {
            return "cmd_update_models";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (session.GetHabbo().HasPermission("cmd_update_models"))
            {
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    Skylight.GetGame().GetRoomManager().LoadRoomModels(dbClient);
                    Skylight.GetGame().GetRoomManager().LoadNewbieRooms(dbClient);
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
